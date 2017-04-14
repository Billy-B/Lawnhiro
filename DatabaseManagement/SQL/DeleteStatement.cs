using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class DeleteStatement : Statement
    {
        public ITable Table { get; internal set; }
        public ConditionalExpression WhereExpression { get; internal set; }
        public override string ToString()
        {
            string ret = "delete from " + Table;
            if (WhereExpression != null)
            {
                ret += WhereExpression;
            }
            return ret;
        }

        public override string ToCommandString()
        {
            string ret = "delete from " + Table;
            if (WhereExpression != null)
            {
                ret += WhereExpression.ToCommandString();
            }
            return ret;
        }

        internal override IEnumerable<Expression> EnumerateExpressions()
        {
            if (WhereExpression != null)
            {
                yield return WhereExpression;
            }
        }
    }
}
