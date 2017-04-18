using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BB
{
    internal abstract class EnumManager : TypeManager
    {
        public Type UnderlyingType { get; private set; }

        private object _valuesAndNames;

        private static Action<Type, object> _genericCacheSetter;

        private static ConstructorInfo _valuesAndNamesCtor;

        private object _rtTypeCache;

        static EnumManager()
        {
            Type rtType = typeof(EnumManager).GetType();
            PropertyInfo genericCacheProp = rtType.GetProperty("GenericCache", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo setter = genericCacheProp.GetSetMethod(true);
            DynamicMethod dynMethod = new DynamicMethod("setGenericCache", typeof(void), new[] { typeof(Type), typeof(object) }, rtType, true);
            ILGenerator ilGen = dynMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Castclass, rtType);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Call, setter);
            ilGen.Emit(OpCodes.Ret);
            _genericCacheSetter = (Action<Type, object>)dynMethod.CreateDelegate(typeof(Action<Type, object>));

            MethodInfo getCachedValuesAndNamesMethod = typeof(Enum).GetMethod("GetCachedValuesAndNames", BindingFlags.NonPublic | BindingFlags.Static);
            if (getCachedValuesAndNamesMethod == null)
            {
                throw new NotSupportedException("Missing ValuesAndNames implementation.");
            }
            _valuesAndNamesCtor = getCachedValuesAndNamesMethod.ReturnType.GetConstructor(new[] { typeof(ulong[]), typeof(string[]) });
        }

        internal override void Initialize()
        {
            throw new NotImplementedException();
            //FieldInfo field = ImplimentorType.GetField(AssemblyPreparer.ENUM_MANAGER_INSTANCE_FIELD_NAME);
            //field.SetValue(null, this);
            OnInitialize();
        }

        internal abstract void OnInitialize();

        public void Ensure()
        {
            _genericCacheSetter(Type, _valuesAndNames);
        }

        private static object buildValuesAndNames(ulong[] values, string[] names)
        {
            return _valuesAndNamesCtor.Invoke(new object[] { values, names });
        }

        protected void SetValuesAndNames(ulong[] values, string[] names)
        {
            if (names.Length != values.Length)
            {
                throw new InvalidOperationException("values and names arrays must have the same length.");
            }
            _valuesAndNames = buildValuesAndNames(values, names);
            _genericCacheSetter(Type, _valuesAndNames);
            Type rtType = typeof(EnumManager).GetType();
            PropertyInfo genericCacheProp = rtType.GetProperty("GenericCache", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            genericCacheProp.SetValue(Type, _valuesAndNames);
            PropertyInfo cacheProp = rtType.GetProperty("Cache", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            object rtTypeCache = cacheProp.GetValue(Type);
            Type cacheType = rtTypeCache.GetType();
            PropertyInfo cacheCacheProp = cacheType.GetProperty("GenericCache", BindingFlags.NonPublic | BindingFlags.Instance);
            cacheCacheProp.SetValue(rtTypeCache, _valuesAndNames);
            MethodInfo internalCompareExchange = typeof(GCHandle).GetMethod("InternalCompareExchange", BindingFlags.Static | BindingFlags.NonPublic);
            //internalCompareExchange.Invoke(null, new object[] { m_cache, rtTypeCache, null, true });
            GCHandle handle = GCHandle.Alloc(rtTypeCache, GCHandleType.WeakTrackResurrection);
            rtType.GetField("m_cache", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Type, (IntPtr)handle);
            
        }
    }
}
