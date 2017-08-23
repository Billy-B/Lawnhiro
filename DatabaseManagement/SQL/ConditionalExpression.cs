using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public abstract class ConditionalExpression : Expression
    {
        public override Expression DispatchAny(ExpressionVisitor visitor)
        {
            return visitor.VisitConditional(this);
        }

        internal abstract ConditionalExpression Dispatch(ExpressionVisitor visitor);
    }
}
