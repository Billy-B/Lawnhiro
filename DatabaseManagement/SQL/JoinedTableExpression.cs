using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class JoinedTableExpression : TableValuedExpression
    {
        public TableValuedExpression Left { get; internal set; }
        public TableValuedExpression Right { get; internal set; }

        internal ExpressionType JoinType { get; set; }

        public override ExpressionType Type
        {
            get { return JoinType; }
        }

        internal JoinedTableExpression() { }

        public override string ToString()
        {
            return Left + " " + Expression.GetStringExpression(JoinType) + " " + Right;
        }

        internal override string ToCommandString()
        {
            return Left.ToCommandString() + " " + Expression.GetStringExpression(JoinType) + " " + Right.ToCommandString();
        }

        internal override IEnumerable<Expression> EnumerateSubExpressions()
        {
            yield return Left;
            yield return Right;
        }
    }
}
