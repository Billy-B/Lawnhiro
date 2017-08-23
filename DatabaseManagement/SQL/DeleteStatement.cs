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

        internal override Statement Parameterize(Parameterizer p)
        {
            return new DeleteStatement
            {
                Table = this.Table,
                WhereExpression = p.VisitConditional(this.WhereExpression),
                _parameterized = true
            };
        }
    }
}
