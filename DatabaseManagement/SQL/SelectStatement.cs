using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class SelectStatement : Statement
    {
        public IList<Expression> SelectedFields { get; internal set; }
        public int? MaxRows { get; internal set; }
        public TableValuedExpression FromExpression { get; internal set; }
        public ConditionalExpression WhereExpression { get; internal set; }
        public IList<FieldAccessExpression> OrderByFields { get; internal set; }
        public IList<FieldAccessExpression> GroupByFields { get; internal set; }
        public override string ToString()
        {
            string ret = "select ";
            if (MaxRows != null)
            {
                ret += "top " + MaxRows + " ";
            }
            if (SelectedFields == null)
            {
                ret += "*";
            }
            else
            {
                ret += string.Join(",", SelectedFields);
            }
            if (FromExpression != null)
            {
                ret += " from " + FromExpression;
            }
            if (WhereExpression != null)
            {
                ret += " where " + WhereExpression;
            }
            if (GroupByFields != null)
            {
                ret += " group by " + String.Join(",", GroupByFields);
            }
            if (OrderByFields != null)
            {
                ret += " order by " + String.Join(",", OrderByFields);
            }
            return ret;
        }

        internal SelectStatement() { }

        internal override IEnumerable<Expression> EnumerateExpressions()
        {
            if (SelectedFields != null)
            {
                foreach (Expression expr in SelectedFields)
                {
                    yield return expr;
                }
            }
            if (FromExpression != null)
            {
                yield return FromExpression;
            }
            if (WhereExpression != null)
            {
                yield return WhereExpression;
            }
            if (OrderByFields != null)
            {
                foreach (FieldAccessExpression expr in OrderByFields)
                {
                    yield return expr;
                }
            }
            if (GroupByFields != null)
            {
                foreach (FieldAccessExpression expr in GroupByFields)
                {
                    yield return expr;
                }
            }
        }
    }
}
