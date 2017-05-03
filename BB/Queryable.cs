
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal static class QueryHelpers
    {
        private static readonly Func<Type, Type> method_GetElementType = (Func<Type, Type>)Delegate.CreateDelegate(typeof(Func<Type, Type>), Type.GetType("System.Data.Linq.SqlClient.TypeSystem, System.Data.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089").GetMethod("GetElementType", BindingFlags.NonPublic | BindingFlags.Static));

        public static Type GetElementType(Type type)
        {
            return method_GetElementType(type);
        }

        public static IQueryable CreateQuery(IQueryProvider provider, Expression expression)
        {
            Type elementType = GetElementType(expression.Type);
            return (IQueryable)Activator.CreateInstance(typeof(Queryable<>).MakeGenericType(elementType), new object[] { provider, expression });
        }

        public static IEnumerable GenericCast(IEnumerable nonGeneric, Type type)
        {
            return (IEnumerable)EnumerableMethods.Cast.MakeGenericMethod(type).Invoke(null, new object[] { nonGeneric });
        }
    }

    internal class Queryable<T> : IOrderedQueryable<T>
    {
        private Type _type = typeof(T);

        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        public Queryable()
        {
            Expression = Expression.Constant(this);
        }

        public Queryable(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType
        {
            get { return _type; }
        }

        public Expression Expression { get; internal set; }

        public IQueryProvider Provider { get; internal set; }
    }

    /*internal sealed class BBQuery<T> : IOrderedQueryable<T>, IQueryProvider, IEnumerable<T>, IOrderedQueryable, IEnumerable
    {
        System.Data.Linq.DataContext _context;
        Expression _queryExpression;
        private Type _type = typeof(T);

        public BBQuery(System.Data.Linq.DataContext context, Expression expression)
        {
            this._context = context;
            this._queryExpression = expression;
        }

        Expression IQueryable.Expression
        {
            get { return this._queryExpression; }
        }

        Type IQueryable.ElementType
        {
            get { return _type; }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException();
            }
            Type eType = QueryHelpers.GetElementType(expression.Type);
            Type qType = typeof(IQueryable<>).MakeGenericType(eType);
            if (!qType.IsAssignableFrom(expression.Type))
            {
                throw new ArgumentException("wrong type", "expression");
            }
            Type dqType = typeof(BBQuery<>).MakeGenericType(eType);
            return (IQueryable)Activator.CreateInstance(dqType, new object[] { this._context, expression });
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException();
            }
            if (!typeof(IQueryable<S>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentException("wrong type", "expression");
            }
            return new BBQuery<S>(this._context, expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return this._context.Provider.Execute(expression).ReturnValue;
        }

        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)this._context.Provider.Execute(expression).ReturnValue;
        }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return this;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this._context.Provider.Execute(this._queryExpression).ReturnValue).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)this._context.Provider.Execute(this._queryExpression).ReturnValue).GetEnumerator();
        }

        public override string ToString()
        {
            return this._context.Provider.GetQueryText(this._queryExpression);
        }
    }*/
}
