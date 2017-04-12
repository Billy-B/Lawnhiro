using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class InsertStatement : Statement
    {
        public ITable Table { get; internal set; }

        public IList<IColumn> InsertColumns { get; internal set; }

        public IList<ScalarExpression> Values { get; internal set; }

        public override string ToString()
        {
            return "insert into " + Table + " (" + string.Join(",", InsertColumns) + ") values (" + string.Join(",", Values) + ")";
        }

        internal override IEnumerable<Expression> EnumerateExpressions()
        {
            return Values;
        }
    }
}
