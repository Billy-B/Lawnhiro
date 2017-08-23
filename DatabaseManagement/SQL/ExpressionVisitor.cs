using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class ExpressionVisitor
    {
        public virtual Expression Visit(Expression node)
        {
            if (node == null)
            {
                return null;
            }
            return node.DispatchAny(this);
        }

        public virtual ConditionalExpression VisitConditional(ConditionalExpression node)
        {
            if (node == null)
            {
                return null;
            }
            return node.Dispatch(this);
        }

        public virtual ScalarExpression VisitScalar(ScalarExpression node)
        {
            if (node == null)
            {
                return null;
            }
            return node.Dispatch(this);
        }

        public virtual TableValuedExpression VisitTable(TableValuedExpression node)
        {
            if (node == null)
            {
                return null;
            }
            return node.Dispatch(this);
        }

        protected internal virtual ConditionalExpression VisitBinaryCompare(BinaryComparisonExpression node)
        {
            return new BinaryComparisonExpression
            {
                Left = VisitScalar(node.Left),
                Right = VisitScalar(node.Right),
                Operation = node.Operation
            };
        }

        protected internal virtual ScalarExpression VisitColumnAccess(ColumnAccessExpression node)
        {
            return new ColumnAccessExpression
            {
                Column = node.Column,
                Table = VisitTable(node.Table)
            };
        }

        protected internal virtual ScalarExpression VisitConstant(ConstantExpression node)
        {
            return node;
        }

        protected internal virtual ConditionalExpression VisitLogicalBinary(LogicalBinaryExpression node)
        {
            return new LogicalBinaryExpression
            {
                Left = VisitConditional(node.Left),
                Right = VisitConditional(node.Right),
                Operation = node.Operation
            };
        }

        protected internal virtual ConditionalExpression VisitLogicalUnary(LogicalUnaryExpression node)
        {
            return new LogicalUnaryExpression
            {
                Operand = VisitConditional(node.Operand),
                Operation = node.Operation
            };
        }

        protected internal virtual ScalarExpression VisitParameter(ParameterExpression node)
        {
            return node;
        }

        protected internal virtual ScalarExpression VisitScalarBinary(ScalarBinaryExpression node)
        {
            return new ScalarBinaryExpression
            {
                Left = VisitScalar(node.Left),
                Right = VisitScalar(node.Right),
                Operation = node.Operation
            };
        }

        protected internal virtual ScalarExpression VisitScalarUnary(ScalarUnaryExpression node)
        {
            return new ScalarUnaryExpression
            {
                Operand = VisitScalar(node.Operand),
                Operation = node.Operation
            };
        }
        protected internal virtual TableValuedExpression VisitTableJoin(JoinedTableExpression node)
        {
            return new JoinedTableExpression
            {
                Alias = node.Alias,
                JoinCondition = VisitConditional(node.JoinCondition),
                JoinType = node.JoinType,
                Left = VisitTable(node.Left),
                Right = VisitTable(node.Right)
            };
        }

        protected internal virtual TableValuedExpression VisitTableAccess(TableExpression node)
        {
            return node;
        }
    }
}
