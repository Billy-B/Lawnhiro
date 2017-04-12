using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class BinaryComparisonExpression : ConditionalExpression
    {
        public ScalarExpression Left { get; internal set; }
        public ScalarExpression Right { get; internal set; }
        internal ExpressionType Operation { get; set; }

        internal BinaryComparisonExpression() { }

        public override ExpressionType Type
        {
            get { return Operation; }
        }
        public override string ToString()
        {
            return Left + " " + Expression.GetStringExpression(Operation) + " " + Right;
        }

        internal override IEnumerable<Expression> EnumerateSubExpressions()
        {
            yield return Left;
            yield return Right;
        }
    }
}
