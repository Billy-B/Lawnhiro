using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using Mono.Reflection;
using System.Web;

namespace BB
{
    internal static class AssemblyPreparer
    {
        internal const string DYNAMIC_ASSEMBLY_NAME = "BB.Dynamic";
        internal const string ENUM_MANAGER_INSTANCE_FIELD_NAME = "Instance";

        private static MethodInfo _objectToString = typeof(object).GetMethod("ToString");
        private static MethodInfo _enumManagerEnsure = typeof(EnumManager).GetMethod("Ensure", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static MethodInfo _enumToString = typeof(Enum).GetMethods().Single(m => m.Name == "ToString" && m.GetParameters().Length == 0);

        internal static Dictionary<Type, PropertyInfo> _implTypeMapper;

        /*private class ManagedPropertyBuilder
        {
            private PropertyInfo _prop;
            private FieldBuilder _staticImplField;
            private TypeBuilder _implType;
            private MethodInfo _getter, _setter;
            private Type _managedType;
            private FieldInfo _backingField;

            public ManagedPropertyBuilder(PropertyInfo prop, TypeBuilder implType, FieldInfo backingField)
            {
                _prop = prop;
                _implType = implType;
                _managedType = prop.DeclaringType;
                _staticImplField = implType.DefineField(prop.Name, typeof(PropertyManager), FieldAttributes.Public | FieldAttributes.Static);
                _getter = prop.GetGetMethod(true);
                _setter = prop.GetSetMethod(true);
                _backingField = backingField;
            }

            public void Inject(Type bakedType)//, List<object> dynMethods)
            {
                FieldInfo bakedFld = bakedType.GetField(_staticImplField.Name, BindingFlags.Static | BindingFlags.Public);
                if (_prop.PropertyType.IsValueType && !_prop.PropertyType.IsPrimitive)
                {
                    //MethodInfo
                }
                if (_getter != null)
                {
                    DynamicMethod dynamicGetter = new DynamicMethod("get_" + _prop.Name, _prop.PropertyType, new[] { _managedType }, bakedType, true);
                    ILGenerator getterIL = dynamicGetter.GetILGenerator();
                    getterIL.Emit(OpCodes.Ldsfld, bakedFld);
                    getterIL.Emit(OpCodes.Ldarg_0);
                    getterIL.Emit(OpCodes.Callvirt, typeof(PropertyManager).GetMethod("GetValue"));
                    if (_prop.PropertyType.IsValueType)
                    {
                        getterIL.Emit(OpCodes.Unbox_Any, _prop.PropertyType);
                    }
                    else
                    {
                        getterIL.Emit(OpCodes.Castclass, _prop.PropertyType);
                    }
                    getterIL.Emit(OpCodes.Ret);
                    //dynMethods.Add(dynamicGetter);
                    Type delegateType = typeof(Func<,>).MakeGenericType(_managedType, _prop.PropertyType);
                    object del = dynamicGetter.CreateDelegate(delegateType);
                    //dynMethods.Add(del);
                    MethodReplacer.ReplaceMethod(dynamicGetter, _getter);
                }
                if (_setter != null)
                {
                    DynamicMethod dynamicSetter = new DynamicMethod("set_" + _prop.Name, typeof(void), new[] { _managedType, _prop.PropertyType }, bakedType, true);
                    ILGenerator setterIL = dynamicSetter.GetILGenerator();
                    setterIL.Emit(OpCodes.Ldsfld, bakedFld);
                    setterIL.Emit(OpCodes.Ldarg_0);
                    setterIL.Emit(OpCodes.Ldarg_1);
                    if (_prop.PropertyType.IsValueType)
                    {
                        setterIL.Emit(OpCodes.Box, _prop.PropertyType);
                    }
                    setterIL.Emit(OpCodes.Callvirt, typeof(PropertyManager).GetMethod("SetValue"));
                    setterIL.Emit(OpCodes.Ret);
                    //dynMethods.Add(dynamicSetter);
                    Type delegateType = typeof(Action<,>).MakeGenericType(_managedType, _prop.PropertyType);
                    object del = dynamicSetter.CreateDelegate(delegateType);
                    dynMethods.Add(del);
                    MethodReplacer.ReplaceMethod(dynamicSetter, _setter);
                }
            }
        }*/

        //private static List<object> _dynMethods; //store DynamicMethod objects here so that they are not garbage-collected.


        private static MethodInfo method_InitializeManagedType = ((Action<RuntimeTypeHandle>)TypeManager.Initialize).Method;

        public static void PrepareAssemblies()
        {
            bool isWebApp = HttpRuntime.AppDomainAppId != null;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            Assembly[] loadedAssemblies = currentDomain.GetAssemblies();
            string[] loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            string path = isWebApp ? currentDomain.SetupInformation.PrivateBinPath : currentDomain.BaseDirectory;

            string[] referencedPaths = Directory.GetFiles(path, "*.dll");
            string[] pathsToLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToArray();

            List<Assembly> assembliesToPrepare = new List<Assembly>();

            foreach (string referencedPath in referencedPaths)
            {
                Assembly loadedAssembly = loadedAssemblies.FirstOrDefault(a => a.Location == referencedPath);
                if (loadedAssembly == null)
                {
                    loadedAssembly = currentDomain.Load(AssemblyName.GetAssemblyName(referencedPath));
                }
                assembliesToPrepare.Add(loadedAssembly);
            }
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null && !assembliesToPrepare.Contains(entryAssembly))
            {
                assembliesToPrepare.Add(entryAssembly);
            }
            PrepareAssemblies(assembliesToPrepare);
        }

        public static void PrepareAssemblies(IEnumerable<Assembly> assembliesToPrepare)
        {
            Dictionary<Type, PropertyInfo> propImplMapper = new Dictionary<Type, PropertyInfo>();
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(DYNAMIC_ASSEMBLY_NAME), AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("BB.Dynamic");
            foreach (Assembly assembly in assembliesToPrepare)
            {
                List<Type> managedTypes = assembly.GetTypes().Where(t => t.GetCustomAttribute<ManagedTypeBaseAttribute>() != null).ToList();
                if (managedTypes.Any())
                {
                    AssemblyManager assemblyManager = AssemblyManager.Create(assembly);
                    foreach (Type type in managedTypes)
                    {
                        ManagedTypeBaseAttribute att = type.GetCustomAttribute<ManagedTypeBaseAttribute>();
                        if (type.IsClass)
                        {
                            PrepareManagedClass(type, dynamicModule, propImplMapper);
                            TypeManager.Create(type, assemblyManager, att);
                        }
                        else if (type.IsEnum)
                        {
                            throw new NotImplementedException();
                        }
                        else if (type.IsValueType)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                }
            }
            _implTypeMapper = propImplMapper;
            /*if (dynMethods.Any())
            {
                _dynMethods = dynMethods;
            }*/
        }

        public static void PrepareManagedClass(Type managedType, ModuleBuilder dynamicModule, IDictionary<Type, PropertyInfo> propImplMapper)//, List<object> dynMethods)
        {
            PropertyInfo[] allProperties = managedType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (PropertyInfo prop in allProperties)
            {
                ManagedPropertyBaseAttribute att = prop.GetCustomAttribute<ManagedPropertyBaseAttribute>();
                if (att != null)
                {
                    if (prop.GetBackingField() != null)
                    {
                        TypeBuilder propImplType = dynamicModule.DefineType("Impl_" + prop.MetadataToken);
                        Type bakedType = propImplType.CreateType();
                        MethodInfo getter = prop.GetGetMethod(true);
                        MethodInfo setter = prop.GetSetMethod(true);
                        if (getter != null)
                        {
                            MethodInfo newGetter = PropImpl.m_getPropValue.MakeGenericMethod(prop.PropertyType, bakedType);
                            MethodReplacer.ReplaceMethod(newGetter, getter);
                        }
                        if (setter != null)
                        {
                            MethodInfo newSetter = PropImpl.m_setPropValue.MakeGenericMethod(prop.PropertyType, bakedType);
                            MethodReplacer.ReplaceMethod(newSetter, setter);
                        }
                        propImplMapper.Add(propImplType, prop);
                    }
                }
            }
        }

        public static Type PrepareManagedEnum(Type enumType, ModuleBuilder dynamicModule, List<object> dynMethods)
        {
            TypeBuilder dynamicType = dynamicModule.DefineType("Impl_" + enumType.FullName);

            dynamicType.DefineField("Instance", typeof(EnumManager), FieldAttributes.Public | FieldAttributes.Static);

            ConstructorBuilder cctor = dynamicType.DefineTypeInitializer();
            ILGenerator cctorIL = cctor.GetILGenerator();
            cctorIL.Emit(OpCodes.Ldtoken, enumType);
            cctorIL.Emit(OpCodes.Call, method_InitializeManagedType);
            cctorIL.Emit(OpCodes.Ret);
            Type ret = dynamicType.CreateType();

            FieldInfo instanceField = ret.GetField(ENUM_MANAGER_INSTANCE_FIELD_NAME, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            DynamicMethod newToStringMethod = new DynamicMethod("ToString", typeof(string), new[] { typeof(object) });
            ILGenerator toStringIL = newToStringMethod.GetILGenerator();
            toStringIL.Emit(OpCodes.Ldsfld, instanceField);
            toStringIL.Emit(OpCodes.Callvirt, _enumManagerEnsure);
            toStringIL.Emit(OpCodes.Ldarg_0);
            toStringIL.Emit(OpCodes.Call, _enumToString);
            toStringIL.Emit(OpCodes.Ret);

            dynMethods.Add(newToStringMethod);
            //MethodReplacer.InjectVirtualMethod(newToStringMethod, _objectToString, enumType);

            return ret;
        }

        internal class PropImpl
        {
            public static MethodInfo m_getPropValue = typeof(PropImpl).GetMethod("GetPropValue");
            public static MethodInfo m_setPropValue = typeof(PropImpl).GetMethod("SetPropValue");

            public TProperty GetPropValue<TProperty, TPropImpl>()
            {
                return (TProperty)PropertyManagerImpl<TPropImpl>.Instance.GetValue(this);
            }
            public void SetPropValue<TProperty, TPropImpl>(TProperty value)
            {
                PropertyManagerImpl<TPropImpl>.Instance.SetValue(this, value);
            }
        }
    }
}
