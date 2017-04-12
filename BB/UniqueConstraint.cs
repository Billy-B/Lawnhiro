using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class UniqueConstraint
    {
        public Type DeclaringType { get; private set; }

        public IList<PropertyInfo> Properties { get; private set; }

        public UniqueConstraint(IEnumerable<PropertyInfo> properties)
        {
            Properties = properties.ToList();
        }
    }
}
