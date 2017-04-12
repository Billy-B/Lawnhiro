using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class ScalarFunction : Function
    {
        public DbType ReturnType { get; internal set; }

        internal ScalarFunction()
        {
        }
    }
}
