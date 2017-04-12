using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    public class ColumnAttribute : ManagedPropertyBaseAttribute
    {
        public string Name { get; set; }

        public ColumnAttribute() { }

        public ColumnAttribute(string name)
        {
            Name = name;
        }

        internal override PropertyManager CreateManager(PropertyInfo prop)
        {
            ColumnPropertyManager ret;
            Type propType = prop.PropertyType;
            if (propType == typeof(string))
            {
                ret = new StringColumnPropertyManager();
            }
            else
            {
                ret = new ColumnPropertyManager();
            }
            ret.ColumnAttribute = this;
            return ret;
        }
    }
}
