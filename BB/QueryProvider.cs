using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class QueryProvider : IQueryProvider
    {
        //internal static readonly QueryProvider Instance = new QueryProvider();

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Queryable<TElement> { Expression = expression, Provider = this };
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return QueryHelpers.CreateQuery(this, expression);
        }

        private TypeManager _typeManager;

        public QueryProvider(TypeManager typeManager)
        {
            _typeManager = typeManager;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _typeManager.Execute<TResult>(expression);
            /*switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    queriedType = QueryHelpers.GetElementType(expression.Type);
                    break;
                case ExpressionType.Call:
                    MethodCallExpression methodCallExpr = (MethodCallExpression)expression;
                    ConstantExpression firstArgument = (ConstantExpression)methodCallExpr.Arguments[0];
                    queriedType = QueryHelpers.GetElementType(methodCallExpr.Arguments.First().Type);
                    break;
                    /*MethodInfo method = methodCallExpr.Method;
                    if (method.IsGenericMethod)
                    {
                        MethodInfo genericDef = method.GetGenericMethodDefinition();
                        if (genericDef == QueryableMethods.Where)
                        {
                            //Expression predicateExpression = methodCallExpr.
                        }
                        else
                        {
                            throw new NotSupportedException("Unqueryable method");
                        }
                    }
                    throw new NotSupportedException();
                default:
                    throw new NotSupportedException();
            }
            TypeManager manager = TypeManager.GetManager(queriedType);
            return manager.Execute<TResult>(expression);*/
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
