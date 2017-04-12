using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class ObjectExtender
    {
        private static readonly ConditionalWeakTable<object, ObjectExtender> _extenders = new ConditionalWeakTable<object, ObjectExtender>();

        public static ObjectExtender GetExtender(object obj)
        {
            return _extenders.GetValue(obj, o => new ObjectExtender(o));
        }

        internal static ObjectExtender Create(object obj)
        {
            ObjectExtender ret = new ObjectExtender(obj);
            _extenders.Add(obj, ret);
            return ret;
        }

        internal object Object { get; private set; }
        internal ObjectState State { get; set; }
        internal HashSet<PropertyManager> InitializedProperties { get; private set; }
        internal IObjectDataSource DataSource { get; set; }

        private ObjectExtender(object obj)
        {
            Object = obj;
            InitializedProperties = new HashSet<PropertyManager>();
        }

        public object PrimaryKey { get; set; }
    }
}
