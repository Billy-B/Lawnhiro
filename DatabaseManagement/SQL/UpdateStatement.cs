using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class UpdateStatement : Statement
    {
        public ITable TableToUpdate { get; internal set; }

        public IList<KeyValuePair<IColumn, ScalarExpression>> ColumnsAndValues { get; internal set; }

        public ConditionalExpression WhereExpression { get; internal set; }

        public override string ToString()
        {
            string ret = "update " + TableToUpdate + " set " + string.Join(", ", ColumnsAndValues.Select(kvp => kvp.Key + "=" + kvp.Value));
            if (WhereExpression != null)
            {
                ret += " where " + WhereExpression;
            }
            return ret;
        }

        public override string ToCommandString()
        {
            string ret = "update " + TableToUpdate + " set " + string.Join(", ", ColumnsAndValues.Select(kvp => kvp.Key + "=" + kvp.Value.ToCommandString()));
            if (WhereExpression != null)
            {
                ret += " where " + WhereExpression.ToCommandString();
            }
            return ret;
        }

        internal override IEnumerable<Expression> EnumerateExpressions()
        {
            foreach (var kvp in ColumnsAndValues)
            {
                yield return kvp.Value;
            }
            if (WhereExpression != null)
            {
                yield return WhereExpression;
            }
        }
    }
}
