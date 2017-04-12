using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class ConcreteTypeManager
    {
        private static readonly Dictionary<Type, ConcreteTypeManager> _mapper = new Dictionary<Type, ConcreteTypeManager>();
        public static bool IsManaged(Type rtType)
        {
            return _mapper.ContainsKey(rtType);
        }
    }
}
