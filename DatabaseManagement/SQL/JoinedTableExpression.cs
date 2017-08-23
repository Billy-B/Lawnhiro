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

        internal override TableValuedExpression Dispatch(ExpressionVisitor visitor)
        {
            return visitor.VisitTableJoin(this);
        }

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
    }
}
