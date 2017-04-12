using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class DatabaseAttribute : Attribute
    {
        public string Name { get; private set; }

        public DatabaseAttribute(string name)
        {
            Name = name;
        }
    }
}
