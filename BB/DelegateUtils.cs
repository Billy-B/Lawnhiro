using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Mono.Reflection;

namespace BB
{
    internal static class DelegateUtils
    {
        private static readonly Dictionary<int, MethodInfo> _tupleCreateGenericMethods = typeof(Tuple).GetMethods().ToDictionary(m => m.GetParameters().Length);

        public static Func<T, object> BuildKeyGetter<T>(UniqueConstraint constraint)
        {
            if (constraint.DeclaringType != typeof(T))
            {
                throw new ArgumentException();
            }
            DynamicMethod dyn = new DynamicMethod("keyGetter_" + constraint, typeof(object), new[] { typeof(T) }, typeof(T));
            ILGenerator gen = dyn.GetILGenerator();
            FieldInfo[] backingFields = constraint.Properties.Select(p => p.GetBackingField()).ToArray();
            if (backingFields.Length == 1)
            {
                FieldInfo fld = backingFields[0];
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, fld);
                if (fld.FieldType.IsValueType)
                {
                    gen.Emit(OpCodes.Box, fld.FieldType);
                }
            }
            else
            {
                MethodInfo tupleCreateGenericMethod = _tupleCreateGenericMethods[backingFields.Length];
                Type[] fieldTypes = backingFields.Select(f => f.FieldType).ToArray();
                MethodInfo tupleCreateMethod = tupleCreateGenericMethod.MakeGenericMethod(fieldTypes);
                foreach (FieldInfo fld in backingFields)
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld);
                }
                gen.Emit(OpCodes.Call, tupleCreateMethod);
            }
            gen.Emit(OpCodes.Ret);
            return (Func<T, object>)dyn.CreateDelegate(typeof(Func<T, object>));
        }

        public static Func<object, object> CreateObjectGetterFromField(FieldInfo field)
        {
            DynamicMethod dyn = new DynamicMethod("get_" + field.Name, typeof(object), new[] { typeof(object) }, field.DeclaringType, true);
            ILGenerator il = dyn.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            Type declaringType = field.DeclaringType;
            if (declaringType.IsValueType)
            {
                il.Emit(declaringType.IsValueType? OpCodes.Unbox : OpCodes.Castclass,declaringType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, declaringType);
            }
            Type fieldType = field.FieldType;
            il.Emit(OpCodes.Ldfld);
            if (field.FieldType.IsValueType)
            {
                il.Emit(OpCodes.Box, field.FieldType);
            }
            il.Emit(OpCodes.Ret);
            return (Func<object, object>)dyn.CreateDelegate(typeof(Func<object, object>));
        }
    }
}
