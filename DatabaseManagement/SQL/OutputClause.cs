using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class OutputClause : Clause
    {
        public override string ToString()
        {
            return "output ";
        }
    }
}
