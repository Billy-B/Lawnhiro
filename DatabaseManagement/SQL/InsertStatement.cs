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

        public IList<IColumn> OutputColumns { get; internal set; }

        public override string ToString()
        {
            string ret = "insert into " + Table + " (" + string.Join(",", InsertColumns) + ")";
            if (OutputColumns != null)
            {
                ret += " output " + string.Join(",", OutputColumns.Select(c => "inserted." + c.Name));
            }
            ret += " values(" + string.Join(", ", Values) + ")";
            return ret;
        }

        internal override IEnumerable<Expression> EnumerateExpressions()
        {
            return Values;
        }
    }
}
