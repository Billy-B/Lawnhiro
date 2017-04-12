using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    public class PropertyInitializationException : Exception
    {
        internal PropertyInitializationException(PropertyInfo prop, string message)
        {
            Property = prop;
        }

        private string _errorMessage;

        public override string Message
        {
            get
            {
                return "An error occurred initializing property " + Property + ". Additional Information:\n" + _errorMessage;
            }
        }

        public PropertyInfo Property { get; private set; }
    }
}
