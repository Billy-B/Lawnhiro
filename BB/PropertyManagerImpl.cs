using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal static class PropertyManagerImpl<T>
    {
        public static PropertyManager Instance;

        static PropertyManagerImpl()
        {
            Instance = ClassManager.GetPropertyManagerInstance(typeof(T));
        }
    }
}
