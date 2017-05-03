using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal static class EnumerableMethods
    {
        public static readonly MethodInfo All;
        public static readonly MethodInfo Any;
        public static readonly MethodInfo AnyMatchExpression;
        public static readonly MethodInfo Contains;
        public static readonly MethodInfo Count;
        public static readonly MethodInfo Cast;
        public static readonly MethodInfo CountMatchExpression;
        public static readonly MethodInfo First;
        public static readonly MethodInfo FirstMatchExpression;
        public static readonly MethodInfo FirstOrDefault;
        public static readonly MethodInfo FirstOrDefaultMatchExpression;
        public static readonly MethodInfo Last;
        public static readonly MethodInfo LastMatchExpression;
        public static readonly MethodInfo LastOrDefault;
        public static readonly MethodInfo LastOrDefaultMatchExpression;
        public static readonly MethodInfo Single;
        public static readonly MethodInfo SingleMatchExpression;
        public static readonly MethodInfo SingleOrDefault;
        public static readonly MethodInfo SingleOrDefaultMatchExpression;
        public static readonly MethodInfo Where;

        static EnumerableMethods()
        {
            MethodInfo[] allEnumerableMethods = typeof(Enumerable).GetMethods();
            All = allEnumerableMethods.Single(m => m.Name == "All");
            Any = allEnumerableMethods.Single(m => m.Name == "Any" && m.GetParameters().Length == 1);
            AnyMatchExpression = allEnumerableMethods.Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
            Cast = allEnumerableMethods.Single(m => m.Name == "Cast");
            Contains = allEnumerableMethods.Single(m => m.Name == "Contains" && m.GetParameters().Length == 2);
            Count = allEnumerableMethods.Single(m => m.Name == "Count" && m.GetParameters().Length == 1);
            CountMatchExpression = allEnumerableMethods.Single(m => m.Name == "Count" && m.GetParameters().Length == 2);
            First = allEnumerableMethods.Single(m => m.Name == "First" && m.GetParameters().Length == 1);
            FirstMatchExpression = allEnumerableMethods.Single(m => m.Name == "First" && m.GetParameters().Length == 2);
            FirstOrDefault = allEnumerableMethods.Single(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 1);
            FirstOrDefaultMatchExpression = allEnumerableMethods.Single(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 2);
            Last = allEnumerableMethods.Single(m => m.Name == "Last" && m.GetParameters().Length == 1);
            LastMatchExpression = allEnumerableMethods.Single(m => m.Name == "Last" && m.GetParameters().Length == 2);
            LastOrDefault = allEnumerableMethods.Single(m => m.Name == "LastOrDefault" && m.GetParameters().Length == 1);
            LastOrDefaultMatchExpression = allEnumerableMethods.Single(m => m.Name == "LastOrDefault" && m.GetParameters().Length == 2);
            Single = allEnumerableMethods.Single(m => m.Name == "Single" && m.GetParameters().Length == 1);
            SingleMatchExpression = allEnumerableMethods.Single(m => m.Name == "Single" && m.GetParameters().Length == 2);
            SingleOrDefault = allEnumerableMethods.Single(m => m.Name == "SingleOrDefault" && m.GetParameters().Length == 1);
            SingleOrDefaultMatchExpression = allEnumerableMethods.Single(m => m.Name == "SingleOrDefault" && m.GetParameters().Length == 2);
            Where = allEnumerableMethods.Single(m => m.Name == "Where" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2);
        }
    }
}
