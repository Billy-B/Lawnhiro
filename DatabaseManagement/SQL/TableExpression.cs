using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class TableExpression : TableValuedExpression
    {
        public new ITable Table { get; internal set; }

        internal override TableValuedExpression Dispatch(ExpressionVisitor node)
        {
            return node.VisitTableAccess(this);
        }

        internal TableExpression() { }

        public override ExpressionType Type
        {
            get { throw new NotImplementedException(); }
        }

        public override string ToString()
        {
            return Table.ToString();
        }
    }
}
