using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class SelectStatement : Statement
    {
        public IList<ScalarExpression> SelectedFields { get; internal set; }
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

        internal override Statement Parameterize(Parameterizer p)
        {
            return new SelectStatement
            {
                FromExpression = p.VisitTable(this.FromExpression),
                MaxRows = this.MaxRows,
                GroupByFields = this.GroupByFields?.Select(e => (FieldAccessExpression)p.VisitScalar(e)).ToList().AsReadOnly(),
                OrderByFields = this.OrderByFields?.Select(e => (FieldAccessExpression)p.VisitScalar(e)).ToList().AsReadOnly(),
                SelectedFields = this.SelectedFields?.Select(e => p.VisitScalar(e)).ToList().AsReadOnly(),
                WhereExpression = p.VisitConditional(this.WhereExpression),
                _parameterized = true
            };
        }
    }
}
