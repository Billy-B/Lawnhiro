using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal static class KeyConverter
    {
        public static object[] GetArray(object systemKey)
        {
            Type type = systemKey.GetType();
            if (type.IsGenericType)
            {
                int size = type.GetGenericParameterConstraints().Length;
                switch (size)
                {
                    case 2:
                        {
                            Tuple<object, object> asTuple = (Tuple<object, object>)systemKey;
                            return new object[] { asTuple.Item1, asTuple.Item2 };
                        }
                    case 3:
                        {
                            Tuple<object, object, object> asTuple = (Tuple<object, object, object>)systemKey;
                            return new object[] { asTuple.Item1, asTuple.Item2, asTuple.Item3 };
                        }
                    case 4:
                        {
                            Tuple<object, object, object, object> asTuple = (Tuple<object, object, object, object>)systemKey;
                            return new object[] { asTuple.Item1, asTuple.Item2, asTuple.Item3, asTuple.Item4 };
                        }
                    case 5:
                        {
                            Tuple<object, object, object, object, object> asTuple = (Tuple<object, object, object, object, object>)systemKey;
                            return new object[] { asTuple.Item1, asTuple.Item2, asTuple.Item3, asTuple.Item4, asTuple.Item5 };
                        }
                    case 6:
                        {
                            Tuple<object, object, object, object, object, object> asTuple = (Tuple<object, object, object, object, object, object>)systemKey;
                            return new object[] { asTuple.Item1, asTuple.Item2, asTuple.Item3, asTuple.Item4, asTuple.Item5, asTuple.Item6 };
                        }
                    case 7:
                        {
                            Tuple<object, object, object, object, object, object, object> asTuple = (Tuple<object, object, object, object, object, object, object>)systemKey;
                            return new object[] { asTuple.Item1, asTuple.Item2, asTuple.Item3, asTuple.Item4, asTuple.Item5, asTuple.Item6, asTuple.Item7 };
                        }
                    default:
                        throw new InvalidOperationException("More than 7 items in primary key?");
                }
            }
            else
            {
                return new object[] { systemKey };
            }
        }

        public static object GetSystemKey(object[] array)
        {
            switch (array.Length)
            {
                case 1:
                    return array[0];
                case 2:
                    return Tuple.Create(array[0], array[1]);
                case 3:
                    return Tuple.Create(array[0], array[1], array[2]);
                case 4:
                    return Tuple.Create(array[0], array[1], array[2], array[3]);
                case 5:
                    return Tuple.Create(array[0], array[1], array[2], array[3], array[4]);
                case 6:
                    return Tuple.Create(array[0], array[1], array[2], array[3], array[4], array[5]);
                case 7:
                    return Tuple.Create(array[0], array[1], array[2], array[3], array[4], array[5], array[6]);
                default:
                    throw new InvalidOperationException("More than 7 items in primary key?");
            }
        }
    }
}
