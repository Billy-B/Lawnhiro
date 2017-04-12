using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    public class ForeignKeyAttribute : ManagedPropertyBaseAttribute
    {
        public string Name { get; set; }
        public ForeignKeyAttribute() { }
        public ForeignKeyAttribute(string name)
        {
            Name = name;
        }

        internal override PropertyManager CreateManager(PropertyInfo prop)
        {
            return new ForeignKeyPropertyManager{ FkAttribute = this };
        }
    }
}
