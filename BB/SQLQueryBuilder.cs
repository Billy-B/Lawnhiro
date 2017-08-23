using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using DbConditionalExpression = DatabaseManagement.SQL.ConditionalExpression;
using DbExpression = DatabaseManagement.SQL.Expression;
using ScalarExpression = DatabaseManagement.SQL.ScalarExpression;
using System.Reflection;

namespace BB
{
    internal static class SQLQueryBuilder
    {
        public static DbConditionalExpression GenerateConditional(Expression expr)
        {
            BinaryExpression binExp;
            switch (expr.NodeType)
            {
                case ExpressionType.AndAlso:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.AndAlso(GenerateConditional(binExp.Left), GenerateConditional(binExp.Right));
                case ExpressionType.OrElse:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.OrElse(GenerateConditional(binExp.Left), GenerateConditional(binExp.Right));
                case ExpressionType.Equal:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Equal(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.NotEqual:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.NotEqual(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.LessThan:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.LessThan(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.LessThanOrEqual:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.LessThanOrEqual(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.GreaterThan:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.GreaterThan(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.GreaterThanOrEqual:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.GreaterThanOrEqual(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.MemberAccess:
                    MemberExpression membExpr = (MemberExpression)expr;
                    PropertyInfo prop = membExpr.Member as PropertyInfo;
                    if (prop == null)
                    {
                        throw new InvalidOperationException("Member " + membExpr.Member + " is not a queryable property.");
                    }
                    PropertyManager propManager = PropertyManager.GetManager(prop);
                    if (propManager == null)
                    {
                        throw new InvalidOperationException("Property " + prop + " is not a managed property.");
                    }
                    ColumnPropertyManager colMgr = propManager as ColumnPropertyManager;
                    if (colMgr != null)
                    {
                        return DbExpression.Equal(DbExpression.Column(colMgr.Column), DbExpression.Constant(true));
                    }
                    throw new NotImplementedException();
                case ExpressionType.Constant:
                    ConstantExpression constExpr = (ConstantExpression)expr;
                    if ((bool)constExpr.Value)
                    {
                        return DbExpression.Equal(DbExpression.Constant(1), DbExpression.Constant(1)); // 1 = 1 (true)
                    }
                    else
                    {
                        return DbExpression.Equal(DbExpression.Constant(1), DbExpression.Constant(0)); // 1 = 0 (false)
                    }
                case ExpressionType.Not:
                    UnaryExpression unaryExp = (UnaryExpression)expr;
                    return DbExpression.Not(GenerateConditional(unaryExp.Operand));
                default:
                    throw new NotImplementedException();
            }
        }

        public static ScalarExpression GenerateScalar(Expression expr)
        {
            BinaryExpression binExp;
            UnaryExpression unaryExp;
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Add(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Subtract:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Subtract(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Multiply:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Multiply(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Divide:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Divide(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Modulo:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Modulo(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.And:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.And(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Or:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Or(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.ExclusiveOr:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.ExclusiveOr(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Constant:
                    ConstantExpression constExp = (ConstantExpression)expr;
                    return DbExpression.Constant(constExp.Value);
                case ExpressionType.MemberAccess:
                    MemberExpression membExp = (MemberExpression)expr;
                    PropertyInfo prop = membExp.Member as PropertyInfo;
                    if (prop != null)
                    {
                        if (prop.GetMethod.IsStatic)
                        {
                            return DbExpression.Constant(prop.GetMethod.Invoke(null, null));
                        }
                        else
                        {
                            PropertyManager manager = PropertyManager.GetManager(prop);
                            if (manager == null)
                            {
                                throw new InvalidOperationException("Property " + prop + " is not a managed property.");
                            }
                            ColumnPropertyManager colMgr = manager as ColumnPropertyManager;
                            if (colMgr != null)
                            {
                                return DbExpression.Column(colMgr.Column);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                    }
                    else
                    {
                        FieldInfo field = (FieldInfo)membExp.Member;
                        if (field.IsStatic)
                        {
                            return DbExpression.Constant(field.GetValue(null));
                        }
                        else
                        {
                            throw new InvalidOperationException("Non - static field?");
                        }
                    }
                case ExpressionType.Negate:
                    unaryExp = (UnaryExpression)expr;
                    return DbExpression.Negate(GenerateScalar(unaryExp.Operand));
                case ExpressionType.UnaryPlus:
                    unaryExp = (UnaryExpression)expr;
                    return GenerateScalar(unaryExp.Operand);
                case ExpressionType.OnesComplement:
                    unaryExp = (UnaryExpression)expr;
                    return DbExpression.OnesCompliment(GenerateScalar(unaryExp.Operand));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
