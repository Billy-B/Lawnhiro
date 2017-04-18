using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BB
{
    internal class ObjectContext
    {
        private static readonly ConditionalWeakTable<ExecutionContext, ObjectContext> _table = new ConditionalWeakTable<ExecutionContext, ObjectContext>();

        private readonly object _lock = new object();

        private readonly Dictionary<object, ObjectChangeTracker> _changeTrackers = new Dictionary<object, ObjectChangeTracker>();

        private readonly HashSet<object> _objectsToInsert = new HashSet<object>();

        private readonly HashSet<object> _objectsToDelete = new HashSet<object>();

        

        private ObjectContext() { }

        public static ObjectContext Current
        {
            get
            {
                return _table.GetValue(Thread.CurrentThread.ExecutionContext, ctx => new ObjectContext());
            }
        }

        public void Add(object obj)
        {
            lock (_objectsToInsert)
            {
                if (_objectsToInsert.Contains(obj))
                {
                    throw new ArgumentException("Already added?");
                }
                _objectsToInsert.Add(obj);
            }
        }

        public void Delete(Object obj)
        {
            _objectsToDelete.Add(obj);
        }

        private ObjectChangeTracker getChangeTracker(object obj)
        {
            lock (_changeTrackers)
            {
                ObjectChangeTracker ret;
                if (!_changeTrackers.TryGetValue(obj, out ret))
                {
                    ret = new ObjectChangeTracker(obj);
                    _changeTrackers[obj] = ret;
                }
                return ret;
            }
        }

        private class RepositoryCommit
        {
            public List<IGrouping<Type, object>> ObjectsToInsert, ObjectsToDelete;
            public IList<IGrouping<Type, ObjectChangeTracker>> ObjectsToUpdate;
        }

        public void CommitChanges()
        {
            IEnumerable<IGrouping<IObjectRepository, IGrouping<Type, object>>> objectsToInsertGroupedByTypeAndRepository = _objectsToInsert.GroupBy(o => o.GetType()).GroupBy(g => TypeManager.GetManager(g.Key).Repository);

            Dictionary<IObjectRepository, RepositoryCommit> repositoriesToCommit = new Dictionary<IObjectRepository, RepositoryCommit>();
            
            foreach (var grouping in objectsToInsertGroupedByTypeAndRepository)
            {
                repositoriesToCommit.Add(grouping.Key, new RepositoryCommit { ObjectsToInsert = grouping.ToList() });
            }

            IEnumerable<IGrouping<IObjectRepository, IGrouping<Type, object>>> objectsToDeleteGroupedByTypeAndRepository = _objectsToDelete.GroupBy(o => o.GetType()).GroupBy(g => TypeManager.GetManager(g.Key).Repository);

            foreach (var grouping in objectsToDeleteGroupedByTypeAndRepository)
            {
                RepositoryCommit existing;
                if (!repositoriesToCommit.TryGetValue(grouping.Key, out existing))
                {
                    existing = new RepositoryCommit();
                    repositoriesToCommit.Add(grouping.Key, existing);
                }
                existing.ObjectsToDelete = grouping.ToList();
            }

            ObjectChangeTracker[] changes;
            lock (_changeTrackers)
            {
                changes = _changeTrackers.Values.ToArray();
            }

            IEnumerable<IGrouping<IObjectRepository, IGrouping<Type, ObjectChangeTracker>>> objectsToUpdateGroupedByTypeAndRepository = changes.GroupBy(o => o.Object.GetType()).GroupBy(g => TypeManager.GetManager(g.Key).Repository);

            foreach (var grouping in objectsToUpdateGroupedByTypeAndRepository)
            {
                RepositoryCommit existing;
                if (!repositoriesToCommit.TryGetValue(grouping.Key, out existing))
                {
                    existing = new RepositoryCommit();
                    repositoriesToCommit.Add(grouping.Key, existing);
                }
                existing.ObjectsToUpdate = grouping.ToList();
            }

            foreach (var kvp in repositoriesToCommit)
            {
                RepositoryCommit commit = kvp.Value;
                kvp.Key.Commit(commit.ObjectsToInsert, commit.ObjectsToDelete, commit.ObjectsToUpdate);
            }
        }

        public void DisgardChanges()
        {
            lock (this)
            {
                _changeTrackers.Clear();
                _objectsToDelete.Clear();
                _objectsToInsert.Clear();
            }
        }

        public bool HasChangedPropertyValue(object obj, PropertyManager prop, out object value)
        {
            ObjectChangeTracker changeTracker;
            if (_changeTrackers.TryGetValue(obj, out changeTracker))
            {
                if (changeTracker.PropertyValues.TryGetValue(prop, out value))
                {
                    return true;
                }
            }
            
            value = null;
            return false;
        }

        public void SetPropertyValue(object obj, PropertyManager prop, object propValue)
        {
            ObjectChangeTracker changeTracker = getChangeTracker(obj);
            changeTracker.PropertyValues[prop] = propValue;
        }
    }
}
