using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal static class QueryableMethods
    {
        public static readonly MethodInfo All;
        public static readonly MethodInfo Any;
        public static readonly MethodInfo AnyMatchExpression;
        public static readonly MethodInfo Contains;
        public static readonly MethodInfo Count;
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

        static QueryableMethods()
        {
            MethodInfo[] allQueryableMethods = typeof(Queryable).GetMethods();
            All = allQueryableMethods.Single(m => m.Name == "All");
            Any = allQueryableMethods.Single(m => m.Name == "Any" && m.GetParameters().Length == 1);
            AnyMatchExpression = allQueryableMethods.Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
            Contains = allQueryableMethods.Single(m => m.Name == "Contains" && m.GetParameters().Length == 2);
            Count = allQueryableMethods.Single(m => m.Name == "Count" && m.GetParameters().Length == 1);
            CountMatchExpression = allQueryableMethods.Single(m => m.Name == "Count" && m.GetParameters().Length == 2);
            First = allQueryableMethods.Single(m => m.Name == "First" && m.GetParameters().Length == 1);
            FirstMatchExpression = allQueryableMethods.Single(m => m.Name == "First" && m.GetParameters().Length == 2);
            FirstOrDefault = allQueryableMethods.Single(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 1);
            FirstOrDefaultMatchExpression = allQueryableMethods.Single(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 2);
            Last = allQueryableMethods.Single(m => m.Name == "Last" && m.GetParameters().Length == 1);
            LastMatchExpression = allQueryableMethods.Single(m => m.Name == "Last" && m.GetParameters().Length == 2);
            LastOrDefault = allQueryableMethods.Single(m => m.Name == "LastOrDefault" && m.GetParameters().Length == 1);
            LastOrDefaultMatchExpression = allQueryableMethods.Single(m => m.Name == "LastOrDefault" && m.GetParameters().Length == 2);
            Single = allQueryableMethods.Single(m => m.Name == "Single" && m.GetParameters().Length == 1);
            SingleMatchExpression = allQueryableMethods.Single(m => m.Name == "Single" && m.GetParameters().Length == 2);
            SingleOrDefault = allQueryableMethods.Single(m => m.Name == "SingleOrDefault" && m.GetParameters().Length == 1);
            SingleOrDefaultMatchExpression = allQueryableMethods.Single(m => m.Name == "SingleOrDefault" && m.GetParameters().Length == 2);
            Where = allQueryableMethods.Single(m => m.Name == "Where" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == 2);
        }
    }
}
