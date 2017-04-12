using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class ObjectChangeTracker
    {
        public ObjectChangeTracker(object obj)
        {
            Object = obj;
        }
        public object Object;
        public Dictionary<PropertyManager, object> PropertyValues = new Dictionary<PropertyManager, object>();
    }
}
