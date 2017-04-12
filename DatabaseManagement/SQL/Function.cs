using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public abstract class Function
    {
        public string Name { get; internal set; }

        public IList<FunctionParameter> Parameters { get; internal set; }
    }
}
