using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class LogicalUnaryExpression : ConditionalExpression
    {
        public ConditionalExpression Operand { get; internal set; }
        internal ExpressionType Operation { get; set; }

        internal LogicalUnaryExpression() { }

        internal override ConditionalExpression Dispatch(ExpressionVisitor visitor)
        {
            return visitor.VisitLogicalUnary(this);
        }

        public override ExpressionType Type
        {
            get { return Operation; }
        }
        public override string ToString()
        {
            return Expression.GetStringExpression(Operation) + "(" + Operand + ")";
        }
    }
}
