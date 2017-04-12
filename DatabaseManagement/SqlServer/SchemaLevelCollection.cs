using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    /*public class SchemaLevelCollection<T> : GenericCollection<T>
        where T : SchemaObject
    {
        public Schema Schema { get; private set; }

        internal SchemaLevelCollection(Schema s, IEnumerable<T> items) : base(items)
        {
            Schema = s;
        }

        public T this[string name]
        {
            get
            {
                T ret;
                if (!TryGetValue(name, out ret))
                {
                    throw new ArgumentException("No " + typeof(T).Name + "\"" + name + "\" exists in Schema " + this.Schema + ".");
                }
                return ret;
            }
        }

        public bool TryGetValue(string name, out T value)
        {
            SchemaObject ret;
            if (Schema._nameIndexed.TryGetValue(name.TrimBracketNotation(), out ret))
            {
                value = ret as T;
                return value != null;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }*/
}
