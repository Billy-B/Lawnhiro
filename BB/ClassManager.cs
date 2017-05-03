using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mono.Reflection;
using System.Collections;
using System.Linq.Expressions;

namespace BB
{
    internal abstract class ClassManager : TypeManager
    {
        static ClassManager()
        {
            var timer = new Timer(o => cleanUpReferences(), null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        }

        private static List<ClassManager> _allClassManagers = new List<ClassManager>();

        private Dictionary<PropertyInfo, PropertyManager> _indexedByProperty;

        internal static PropertyManager GetPropertyManagerInstance(Type propImplType)
        {
            PropertyInfo prop = AssemblyPreparer._implTypeMapper[propImplType];

            ClassManager manager = (ClassManager)TypeManager.GetManager(prop.DeclaringType);
            manager.EnsureInitialized();
            return manager.GetManager(prop);
        }

        public PropertyManager GetManager(PropertyInfo prop)
        {
            return _indexedByProperty[prop];
        }

        private static void cleanUpReferences()
        {
            ClassManager[] managers;
            lock (_allClassManagers)
            {
                managers = _allClassManagers.ToArray();
            }
            foreach (ClassManager manager in managers)
            {
                manager.RemoveDeadReferences();
            }
        }
        private long _pkAccessCounter;

        public override IEnumerable EnumerateValues()
        {
            return EnumerateData().Select(ds => GetAttachedObject(ds));
        }

        /*public IEnumerable EnumerateValues(System.Linq.Expressions.Expression expression)
        {
            return EnumerateData(expression).Select(ds => GetAttachedObject(ds));
        }*/

        internal abstract IEnumerable<IObjectDataSource> EnumerateData();

        public object GetByPrimaryKey(object primaryKey)
        {
            Interlocked.Increment(ref _pkAccessCounter);
            Cache cache = Cache;
            if (cache != null)
            {
                return cache.GetObjectByPrimaryKey(primaryKey);
            }
            else
            {
                object ret = getExistingReference(primaryKey);
                if (ret == null)
                {
                    IObjectDataSource dataSource = FetchByPrimaryKey(primaryKey);
                    if (dataSource != null)
                    {
                        ret = getExistingOrCreateAttachedObject(dataSource);
                    }
                }
                return ret;
            }
        }

        protected abstract IObjectDataSource FetchByPrimaryKey(object primaryKey);

        private static readonly Func<object, object> method_MemberwiseClone = (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>), typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance));

        public IList<PropertyManager> ManagedProperties { get; private set; }

        public IList<PropertyManager> ValidManagedProperties { get; private set; }

        public Cache Cache { get; private set; }

        public bool HasIdentity { get; set; }

        public bool Cached
        {
            get { return Cache != null; }
        }

        internal abstract void OnInitialize();
        internal abstract void OnPropertiesInitialized();

        private Dictionary<object, WeakReference> _referenceCache = new Dictionary<object, WeakReference>();

        private object _uninitializedObj;

        internal void RemoveDeadReferences()
        {
            var kvps = _referenceCache.ToArray();
            object[] keysToRemove = kvps.Where(kvp => !kvp.Value.IsAlive).Select(kvp => kvp.Key).ToArray();
            lock (_referenceCache)
            {
                for (int i = 0; i < keysToRemove.Length; i++)
                {
                    _referenceCache.Remove(keysToRemove[i]);
                }
            }
        }

        private object getExistingReference(object primaryKey)
        {
            WeakReference weakRef;
            bool found;
            lock (_referenceCache)
            {
                found = _referenceCache.TryGetValue(primaryKey, out weakRef);
            }
            if (found)
            {
                return weakRef.Target;
            }
            else
            {
                return null;
            }
        }

        public object GetAttachedObject(IObjectDataSource dataSource)
        {
            return getExistingOrCreateAttachedObject(dataSource);
        }

        private object getExistingOrCreateAttachedObject(IObjectDataSource dataSource)
        {
            WeakReference existing;
            object primaryKey = CreatePrimaryKey(dataSource);
            lock (_referenceCache)
            {
                if (_referenceCache.TryGetValue(primaryKey, out existing))
                {
                    object ret = existing.Target;
                    if (ret == null)
                    {
                        ret = createNewAttachedObject(dataSource);
                        existing.Target = ret;
                    }
                    return ret;
                }
                else
                {
                    object ret = createNewAttachedObject(dataSource);
                    _referenceCache.Add(primaryKey, new WeakReference(ret));
                    return ret;
                }
            }
        }

        private object createNewAttachedObject(IObjectDataSource dataSource)
        {
            object primaryKey = CreatePrimaryKey(dataSource);
            object ret = method_MemberwiseClone(_uninitializedObj);
            ObjectExtender newExtender = ObjectExtender.Create(ret);
            newExtender.DataSource = dataSource;
            newExtender.State = ObjectState.Attached;
            newExtender.PrimaryKey = primaryKey;
            return ret;
        }

        public abstract object CreatePrimaryKey(IObjectDataSource source);

        public static object GetPrimaryKey(object obj)
        {
            return ObjectExtender.GetExtender(obj).PrimaryKey;
        }

        internal static PropertyInfo[] GetManagedProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => p.GetBackingField() != null && p.GetCustomAttribute<ManagedPropertyBaseAttribute>() != null).ToArray();
        }

        internal sealed override void Initialize()
        {
            lock (_allClassManagers)
            {
                _allClassManagers.Add(this);
            }
            PropertyInfo[] managedProps = GetManagedProperties(this.Type);

            List<PropertyManager> managerList = new List<PropertyManager>();
            foreach (PropertyInfo prop in managedProps)
            {
                PropertyManager propMgr = PropertyManager.Create(prop);
                propMgr.TypeManager = this;
                managerList.Add(propMgr);
            }
            ManagedProperties = managerList;
            _indexedByProperty = managerList.ToDictionary(p => p.Property);
            OnInitialize();
            foreach (PropertyManager manager in managerList)
            {
                manager.Initialize();
            }
            ValidManagedProperties = managerList.Where(p => p.IsValid).ToList();
            OnPropertiesInitialized();
            _uninitializedObj = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(Type);
        }
    }
}
