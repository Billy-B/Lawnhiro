using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            IQueryable asQueryable;
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    ConstantExpression constExpr = (ConstantExpression)expression;
                    asQueryable = constExpr.Value as IQueryable;
                    if (asQueryable != null && asQueryable.Provider == this)
                    {
                        return (TResult)QueryHelpers.GenericCast(_typeManager.EnumerateValues(), _typeManager.Type);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                case ExpressionType.Call:
                    MethodCallExpression reduced = reduceQueryMethod((MethodCallExpression)expression);
                    ConstantExpression firstArg = reduced.Arguments[0] as ConstantExpression;
                    if (firstArg == null)
                    {
                        throw new InvalidOperationException();
                    }
                    asQueryable = firstArg.Value as IQueryable;
                    if (asQueryable != null && asQueryable.Provider == this)
                    {
                        LambdaExpression lambda = getPredicateLambda(reduced);
                        MethodInfo queryableMethod = reduced.Method.GetGenericMethodDefinition();
                        if (queryableMethod == QueryableMethods.All)
                        {
                            return (TResult)(object)_typeManager.QueryAll(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.Any)
                        {
                            return (TResult)(object)_typeManager.QueryAny();
                        }
                        else if (queryableMethod == QueryableMethods.AnyMatchExpression)
                        {
                            return (TResult)(object)_typeManager.QueryAny(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.Count)
                        {
                            return (TResult)(object)_typeManager.QueryCount();
                        }
                        else if (queryableMethod == QueryableMethods.CountMatchExpression)
                        {
                            return (TResult)(object)_typeManager.QueryCount(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.First)
                        {
                            return (TResult)_typeManager.QueryFirst();
                        }
                        else if (queryableMethod == QueryableMethods.FirstMatchExpression)
                        {
                            return (TResult)_typeManager.QueryFirst(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.FirstOrDefault)
                        {
                            return (TResult)_typeManager.QueryFirstOrDefault();
                        }
                        else if (queryableMethod == QueryableMethods.FirstOrDefaultMatchExpression)
                        {
                            return (TResult)_typeManager.QueryFirstOrDefault(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.Last)
                        {
                            return (TResult)_typeManager.QueryLast();
                        }
                        else if (queryableMethod == QueryableMethods.LastMatchExpression)
                        {
                            return (TResult)_typeManager.QueryLast(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.LastOrDefault)
                        {
                            return (TResult)_typeManager.QueryLastOrDefault();
                        }
                        else if (queryableMethod == QueryableMethods.LastOrDefaultMatchExpression)
                        {
                            return (TResult)_typeManager.QueryLastOrDefault(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.Single)
                        {
                            return (TResult)_typeManager.QuerySingle();
                        }
                        else if (queryableMethod == QueryableMethods.SingleMatchExpression)
                        {
                            return (TResult)_typeManager.QuerySingle(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.SingleOrDefault)
                        {
                            return (TResult)_typeManager.QuerySingleOrDefault();
                        }
                        else if (queryableMethod == QueryableMethods.SingleOrDefaultMatchExpression)
                        {
                            return (TResult)_typeManager.QuerySingleOrDefault(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.Where)
                        {
                            return (TResult)_typeManager.QueryWhere(lambda);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                default:
                    throw new InvalidOperationException();

            }
            //return _typeManager.Execute<TResult>(expression);
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

        private static MethodCallExpression reduceQueryMethod(MethodCallExpression expression)
        {
            MethodInfo method = expression.Method;
            Debug.Assert(method.DeclaringType == typeof(Queryable), "Non-queryable method?");
            MethodInfo genericDef = method.GetGenericMethodDefinition();
            Type genericType = method.GetGenericArguments()[0];
            var arguments = expression.Arguments;
            Expression firstArg = arguments[0];
            MethodCallExpression firstArgAsMethodCall = firstArg as MethodCallExpression;
            if (firstArgAsMethodCall == null)
            {
                return expression;
            }
            else
            {
                firstArgAsMethodCall = reduceQueryMethod(firstArgAsMethodCall);
                if (firstArgAsMethodCall.Method.GetGenericMethodDefinition() == QueryableMethods.Where)
                {
                    LambdaExpression whereExpression = getPredicateLambda(firstArgAsMethodCall);
                    if (genericDef == QueryableMethods.Where 
                        || genericDef == QueryableMethods.AnyMatchExpression 
                        || genericDef == QueryableMethods.All 
                        || genericDef == QueryableMethods.CountMatchExpression
                        || genericDef == QueryableMethods.FirstMatchExpression
                        || genericDef == QueryableMethods.FirstOrDefaultMatchExpression
                        || genericDef == QueryableMethods.LastMatchExpression
                        || genericDef == QueryableMethods.LastOrDefaultMatchExpression
                        || genericDef == QueryableMethods.SingleMatchExpression
                        || genericDef == QueryableMethods.SingleOrDefaultMatchExpression)
                    {
                        LambdaExpression baseWhereExpression = getPredicateLambda(expression);
                        Expression newBody = Expression.AndAlso(whereExpression.Body, baseWhereExpression.Body);
                        LambdaExpression newLambda = andAlso(baseWhereExpression, whereExpression);// Expression.Lambda(baseWhereExpression.GetType().GetGenericArguments()[0], newBody, parameters);
                        return Expression.Call(method, firstArgAsMethodCall.Arguments[0], Expression.Quote(newLambda));
                    }
                    else if (genericDef == QueryableMethods.Any)
                    {
                        return Expression.Call(QueryableMethods.AnyMatchExpression.MakeGenericMethod(genericType),  firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.Count)
                    {
                        return Expression.Call(QueryableMethods.CountMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.First)
                    {
                        return Expression.Call(QueryableMethods.FirstMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.FirstOrDefault)
                    {
                        return Expression.Call(QueryableMethods.FirstOrDefaultMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.Last)
                    {
                        return Expression.Call(QueryableMethods.LastMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.LastOrDefault)
                    {
                        return Expression.Call(QueryableMethods.LastOrDefaultMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.Single)
                    {
                        return Expression.Call(QueryableMethods.SingleMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.SingleOrDefault)
                    {
                        return Expression.Call(QueryableMethods.SingleOrDefaultMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                }
            }
            /*if (genericDef == QueryableMethods.Where)
            {
                Expression left = arguments[0];
                UnaryExpression quoteExpression = (UnaryExpression)arguments[1];
                LambdaExpression quoteOperand = (LambdaExpression)quoteExpression.Operand;
                switch (left.NodeType)
                {
                    case ExpressionType.Constant:
                        return expression;
                    case ExpressionType.Call:
                        MethodCallExpression leftMethodExpression = reduceQueryMethod((MethodCallExpression)left);
                        MethodInfo leftMethod = leftMethodExpression.Method.GetGenericMethodDefinition();
                        Expression leftArg = leftMethodExpression.Arguments[0];
                        if (leftMethod == QueryableMethods.Where) // two Where methods called consecutively, can be reduced to one Where call with And-Also.
                        {
                            UnaryExpression leftQuoteExpression = (UnaryExpression)leftMethodExpression.Arguments[1];
                            LambdaExpression leftQuoteOperand = (LambdaExpression)leftQuoteExpression.Operand;
                            Expression newBody = Expression.AndAlso(leftQuoteOperand.Body, quoteOperand.Body);
                            LambdaExpression newLambda = Expression.Lambda(quoteOperand.GetType().GetGenericArguments()[0], newBody, quoteOperand.Parameters[0]);
                            MethodCallExpression leftArgAsMethodCall = leftArg as MethodCallExpression;
                            return Expression.Call(method, leftArg, Expression.Quote(newLambda));
                        }
                        else
                        {
                            return expression;
                        }
                }
            }
            else if (genericDef == QueryableMethods.Any)
            {
                Expression arg = arguments[0];
                MethodCallExpression asMethodCall = arg as MethodCallExpression;
                if (asMethodCall == null)
                {
                    return expression;
                }
                else
                {
                    asMethodCall = reduceQueryMethod(asMethodCall);
                    MethodInfo argMethod = asMethodCall.Method.GetGenericMethodDefinition();
                    if (argMethod == QueryableMethods.Where) // Where(*predicate*).Any() can be reduced to Any(*predicate*)
                    {
                        Type genericType = asMethodCall.Method.GetGenericArguments()[0];
                        LambdaExpression lambda = (LambdaExpression)((UnaryExpression)asMethodCall.Arguments[1]).Operand;
                        return Expression.Call(QueryableMethods.AnyMatchExpression.MakeGenericMethod(genericType), asMethodCall.Arguments[0], Expression.Quote(lambda));
                    }
                }
            }
            else if (genericDef == QueryableMethods.AnyMatchExpression)
            {
                Expression arg = arguments[0];
                MethodCallExpression asMethodCall = arg as MethodCallExpression;
                if (asMethodCall == null)
                {
                    return expression;
                }
                else
                {
                    asMethodCall = reduceQueryMethod(asMethodCall);
                    MethodInfo argMethod = asMethodCall.Method.GetGenericMethodDefinition();
                    if (argMethod == QueryableMethods.Where) // Where(*predicate1*).Any(*predicate2*) can be reduced to Any(*predicate1* AndAlso *predicate2*)
                    {
                        Type genericType = asMethodCall.Method.GetGenericArguments()[0];
                        LambdaExpression lambda = (LambdaExpression)((UnaryExpression)asMethodCall.Arguments[1]).Operand;
                        return Expression.Call(QueryableMethods.AnyMatchExpression.MakeGenericMethod(genericType), asMethodCall.Arguments[0], Expression.Quote(lambda));
                    }
                }
            }
            else if (genericDef == QueryableMethods.Count)
            {
                Expression arg = arguments[0];
                MethodCallExpression asMethodCall = arg as MethodCallExpression;
                if (asMethodCall == null)
                {
                    return expression;
                }
                else
                {
                    asMethodCall = reduceQueryMethod(asMethodCall);
                    MethodInfo argMethod = asMethodCall.Method.GetGenericMethodDefinition();
                    if (argMethod == QueryableMethods.Where) // Where(*predicate*).Count() can be reduced to Count(*predicate*)
                    {
                        Type genericType = asMethodCall.Method.GetGenericArguments()[0];
                        LambdaExpression lambda = (LambdaExpression)((UnaryExpression)asMethodCall.Arguments[1]).Operand;
                        return Expression.Call(QueryableMethods.CountMatchExpression.MakeGenericMethod(genericType), asMethodCall.Arguments[0], Expression.Quote(lambda));
                    }
                }
            }*/
            throw new NotImplementedException();
        }

        static LambdaExpression andAlso(LambdaExpression left, LambdaExpression right)
        {
            // need to detect whether they use the same
            // parameter instance; if not, they need fixing
            ParameterExpression param = left.Parameters[0];
            Type leftDelegateType = left.GetType().GetGenericArguments()[0];
            Type rightDelegateType = right.GetType().GetGenericArguments()[0];
            if (!leftDelegateType.Equals(rightDelegateType))
            {
                throw new ArgumentException("Delegate types do not match");
            }

            var visitor = new ParameterUpdateVisitor(right.Parameters[0], left.Parameters[0]);
            var body = visitor.Visit(right.Body);

            return Expression.Lambda(leftDelegateType, Expression.AndAlso(left.Body, body), left.Parameters[0]);
        }

        class ParameterUpdateVisitor : ExpressionVisitor
        {
            private ParameterExpression _oldParameter;
            private ParameterExpression _newParameter;

            public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (object.ReferenceEquals(node, _oldParameter))
                    return _newParameter;

                return base.VisitParameter(node);
            }
        }

        private static LambdaExpression getPredicateLambda(MethodCallExpression expression)
        {
            foreach (Expression arg in expression.Arguments)
            {
                if (arg.NodeType == ExpressionType.Quote)
                {
                    return (LambdaExpression)((UnaryExpression)arg).Operand;
                }
            }
            return null;
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
