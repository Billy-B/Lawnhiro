using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false)]
    public class TableAttribute : ManagedTypeBaseAttribute
    {
        public string Name { get; set; }

        public string IdColumn { get; set; }

        public string NameColumn { get; set; }

        public TableAttribute() { }

        public TableAttribute(string name)
        {
            Name = name;
        }

        internal override TypeManager CreateTypeManager(Type type)
        {
            if (type.IsEnum)
            {
                return new TableEnumManager(this);
            }
            else
            {
                return new TableBoundClassManager(this);
            }
        }
    }
}
