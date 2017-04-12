using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BB
{
    internal abstract class ClassManager : TypeManager
    {
        static ClassManager()
        {
            var timer = new System.Threading.Timer(o => cleanUpReferences(), null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        }

        private static List<ClassManager> _allClassManagers = new List<ClassManager>();

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

        internal sealed override void Initialize()
        {
            lock (_allClassManagers)
            {
                _allClassManagers.Add(this);
            }
            List<PropertyManager> managedProperties = new List<PropertyManager>();
            foreach (FieldInfo implField in ImplimentorType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                PropertyInfo prop = Type.GetProperty(implField.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                PropertyManager propMgr = PropertyManager.Create(prop);
                propMgr.TypeManager = this;
                managedProperties.Add(propMgr);
                implField.SetValue(null, propMgr);
            }
            ManagedProperties = managedProperties;
            OnInitialize();
            foreach (PropertyManager manager in managedProperties)
            {
                manager.Initialize();
            }
            ValidManagedProperties = managedProperties.Where(p => p.IsValid).ToList();
            OnPropertiesInitialized();
            _uninitializedObj = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(Type);
        }
    }
}
