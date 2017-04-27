using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public abstract class Expression
    {
        public abstract ExpressionType Type { get; }

        internal abstract string ToCommandString();

        internal abstract IEnumerable<Expression> EnumerateSubExpressions();

        public static ScalarBinaryExpression Add(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new ScalarBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.Add
            };
        }

        public static ScalarBinaryExpression Subtract(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new ScalarBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.Subtract
            };
        }

        public static ScalarBinaryExpression Multiply(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new ScalarBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.Multiply
            };
        }

        public static ScalarBinaryExpression Divide(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new ScalarBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.Divide
            };
        }

        public static ScalarBinaryExpression Modulo(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new ScalarBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.Modulo
            };
        }

        public static ScalarBinaryExpression And(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new ScalarBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.And
            };
        }

        public static ScalarBinaryExpression Or(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new ScalarBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.Or
            };
        }

        public static ScalarBinaryExpression ExclusiveOr(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new ScalarBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.ExclusiveOr
            };
        }

        public static BinaryComparisonExpression Equal(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new BinaryComparisonExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.Equal
            };
        }

        public static BinaryComparisonExpression NotEqual(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new BinaryComparisonExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.NotEqual
            };
        }

        public static BinaryComparisonExpression LessThan(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new BinaryComparisonExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.LessThan
            };
        }

        public static BinaryComparisonExpression LessThanOrEqual(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new BinaryComparisonExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.LessThanOrEqualTo
            };
        }

        public static BinaryComparisonExpression GreaterThan(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new BinaryComparisonExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.GreaterThan
            };
        }

        public static BinaryComparisonExpression GreaterThanOrEqual(ScalarExpression left, ScalarExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new BinaryComparisonExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.GreaterThanOrEqualTo
            };
        }

        public static LogicalBinaryExpression AndAlso(ConditionalExpression left, ConditionalExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new LogicalBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.AndAlso
            };
        }

        public static LogicalBinaryExpression OrElse(ConditionalExpression left, ConditionalExpression right)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            return new LogicalBinaryExpression
            {
                Left = left,
                Right = right,
                Operation = ExpressionType.OrElse
            };
        }

        public static ScalarUnaryExpression Negate(ScalarExpression expression)
        {
            Utility.AssertNonNull(expression, "expression");
            return new ScalarUnaryExpression
            {
                Operand = expression,
                Operation = ExpressionType.Negate
            };
        }

        public static ScalarUnaryExpression OnesCompliment(ScalarExpression expression)
        {
            Utility.AssertNonNull(expression, "expression");
            return new ScalarUnaryExpression
            {
                Operand = expression,
                Operation = ExpressionType.OnesCompliment
            };
        }

        public static ScalarUnaryExpression UnaryPlus(ScalarExpression expression)
        {
            Utility.AssertNonNull(expression, "expression");
            return new ScalarUnaryExpression
            {
                Operand = expression,
                Operation = ExpressionType.UnaryPlus
            };
        }

        public static LogicalUnaryExpression Not(ConditionalExpression expression)
        {
            Utility.AssertNonNull(expression, "expression");
            return new LogicalUnaryExpression
            {
                Operand = expression,
                Operation = ExpressionType.Not
            };
        }

        public static JoinedTableExpression InnerJoin(TableValuedExpression left, TableValuedExpression right, ConditionalExpression joinCondition)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            Utility.AssertNonNull(joinCondition, "joinCondition");
            return new JoinedTableExpression
            {
                Left = left,
                Right = right,
                JoinType = ExpressionType.InnerJoin
            };
        }

        public static JoinedTableExpression LeftOuterJoin(TableValuedExpression left, TableValuedExpression right, ConditionalExpression joinCondition)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            Utility.AssertNonNull(joinCondition, "joinCondition");
            return new JoinedTableExpression
            {
                Left = left,
                Right = right,
                JoinType = ExpressionType.LeftOuterJoin
            };
        }

        public static JoinedTableExpression RightOuterJoin(TableValuedExpression left, TableValuedExpression right, ConditionalExpression joinCondition)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            Utility.AssertNonNull(joinCondition, "joinCondition");
            return new JoinedTableExpression
            {
                Left = left,
                Right = right,
                JoinType = ExpressionType.RightOuterJoin
            };
        }

        public static JoinedTableExpression FullOuterJoin(TableValuedExpression left, TableValuedExpression right, ConditionalExpression joinCondition)
        {
            Utility.AssertNonNull(left, "left");
            Utility.AssertNonNull(right, "right");
            Utility.AssertNonNull(joinCondition, "joinCondition");
            return new JoinedTableExpression
            {
                Left = left,
                Right = right,
                JoinType = ExpressionType.FullOuterJoin
            };
        }

        public static TableExpression Table(ITable table)
        {
            Utility.AssertNonNull(table, "table");
            return new TableExpression
            {
                Table = table
            };
        }

        public static TableExpression Table(ITable table, string alias)
        {
            Utility.AssertNonNull(table, "table");
            Utility.AssertNonNull(alias, "alias");
            return new TableExpression
            {
                Table = table,
                Alias = alias
            };
        }

        /*public static ParameterExpression Parameter(string parameterName, DbType parameterType)
        {
            Utility.AssertNonNull(parameterName, "parameterName");
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException("Cannot be blank", "parameterName");
            }
            return new ParameterExpression
            {
                ParameterName = parameterName,
                ParameterType = parameterType
            };
        }*/

        public static ConstantExpression Constant(object value)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }
            return new ConstantExpression(value);
        }

        public static ConstantExpression Constant(object value, DbType dbType)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }
            return new ConstantExpression(value, dbType);
        }

        public static ColumnAccessExpression Column(IColumn column)
        {
            Utility.AssertNonNull(column, "column");
            return new ColumnAccessExpression
            {
                Column = column
            };
        }

        public static ColumnAccessExpression Column(IColumn column, TableExpression tableExpression)
        {
            Utility.AssertNonNull(column, "column");
            Utility.AssertNonNull(tableExpression, "tableExpression");
            if (column.Table != tableExpression.Table)
            {
                throw new ArgumentException("Column must belong to the same table represented by the tableExpression parameter.", "column");
            }
            return new ColumnAccessExpression
            {
                Column = column,
                Table = tableExpression
            };
        }

        internal static string GetStringExpression(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Add:
                case ExpressionType.UnaryPlus:
                    return "+";
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.FullOuterJoin:
                    return "FULL OUTER JOIN";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqualTo:
                    return ">=";
                case ExpressionType.InnerJoin:
                    return "INNER JOIN";
                case ExpressionType.Is:
                    return "IS";
                case ExpressionType.IsNot:
                    return "IS NOT";
                case ExpressionType.LeftOuterJoin:
                    return "LEFT OUTER JOIN";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqualTo:
                    return "<=";
                case ExpressionType.Like:
                    return "LIKE";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.Not:
                    return "NOT";
                case ExpressionType.OnesCompliment:
                    return "~";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.RightOuterJoin:
                    return "RIGHT OUTER JOIN";
                case ExpressionType.Subtract:
                case ExpressionType.Negate:
                    return "-";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
