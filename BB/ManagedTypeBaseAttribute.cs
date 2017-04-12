using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct)]
    public abstract class ManagedTypeBaseAttribute : Attribute
    {
        internal abstract TypeManager CreateTypeManager(Type type);
    }
}
