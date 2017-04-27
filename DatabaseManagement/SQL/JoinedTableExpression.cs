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
        public ConditionalExpression JoinCondition { get; internal set; }

        internal ExpressionType JoinType { get; set; }

        public override ExpressionType Type
        {
            get { return JoinType; }
        }

        internal JoinedTableExpression() { }

        public override string ToString()
        {
            return Left + " " + Expression.GetStringExpression(JoinType) + " " + Right + " on " + JoinCondition;
        }

        internal override string ToCommandString()
        {
            return Left.ToCommandString() + " " + Expression.GetStringExpression(JoinType) + " " + Right.ToCommandString() + " on " + JoinCondition.ToCommandString();
        }

        internal override IEnumerable<Expression> EnumerateSubExpressions()
        {
            yield return Left;
            yield return Right;
            yield return JoinCondition;
        }
    }
}
