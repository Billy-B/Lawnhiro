using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class ScalarUnaryExpression : ScalarExpression
    {
        public ScalarExpression Operand { get; internal set; }
        internal ExpressionType Operation { get; set; }
        internal ScalarUnaryExpression() { }

        internal override ScalarExpression Dispatch(ExpressionVisitor visitor)
        {
            return visitor.VisitScalarUnary(this);
        }

        public override ExpressionType Type
        {
            get { return Operation; }
        }
        public override System.Data.DbType DbType
        {
            get { return Operand.DbType; }
        }
        public override string ToString()
        {
            return Expression.GetStringExpression(Operation) + "(" + Operand + ")";
        }
    }
}
