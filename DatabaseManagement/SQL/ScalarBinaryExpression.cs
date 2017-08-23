using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class ScalarBinaryExpression : ScalarExpression
    {
        public ScalarExpression Left { get; internal set; }
        public ScalarExpression Right { get; internal set; }

        internal ExpressionType Operation { get; set; }

        internal DbType ResultantDbType { get; set; }

        internal override ScalarExpression Dispatch(ExpressionVisitor visitor)
        {
            return visitor.VisitScalarBinary(this);
        }

        public override ExpressionType Type
        {
            get { return Operation; }
        }

        public override DbType DbType
        {
            get { return ResultantDbType; }
        }

        public override string ToString()
        {
            return Left + " " + Expression.GetStringExpression(Operation) + " " + Right;
        }
    }
}
