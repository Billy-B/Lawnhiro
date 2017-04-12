using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ManagedAssemblyAttribute : Attribute
    {
        public string DefaultDatabase { get; set; }

        internal virtual AssemblyManager CreateManager()
        {
            return new AssemblyManager
            {
                DefaultDatabaseName = DefaultDatabase
            };
        }
    }
}
