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

        public override ExpressionType Type
        {
            get { return Operation; }
        }
        public override string ToString()
        {
            return Expression.GetStringExpression(Operation) + "(" + Operand + ")";
        }

        internal override string ToCommandString()
        {
            return Expression.GetStringExpression(Operation) + "(" + Operand.ToCommandString() + ")";
        }

        internal override IEnumerable<Expression> EnumerateSubExpressions()
        {
            yield return Operand;
        }
    }
}
