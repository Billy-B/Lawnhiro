using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class InsertClause : Clause
    {
        public ITable Table { get; private set; }
        public override string ToString()
        {
            return "insert into" + Table;
        }
    }
}
