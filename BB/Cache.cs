using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal abstract class Cache
    {
        public abstract object QueryFirst();
        public abstract object QueryLast();
        public abstract object QueryFirstOrDefault();
        public abstract object QueryLastOrDefault();
        public abstract object QuerySingle();
        public abstract object QuerySingleOrDefault();
        public abstract object QueryFirst(Expression expression);
        public abstract object QueryFirstOrDefault(Expression expression);
        public abstract object QueryLast(Expression expression);
        public abstract object QueryLastOrDefault(Expression expression);
        public abstract object QuerySingle(Expression expression);
        public abstract object QuerySingleOrDefault(Expression expression);
        public abstract object QueryWhere(Expression expression);
        public abstract int QueryCount();
        public abstract int QueryCount(Expression expression);
        public abstract object GetObjectByPrimaryKey(object primaryKey);
        public abstract void Add(object obj);
        public abstract void Delete(object obj);

        public static Cache Create(Type type)
        {
            return (Cache)Activator.CreateInstance(typeof(Cache<>).MakeGenericType(type), true);
        }
    }

    internal class Cache<T> : Cache
        where T : class
    {
        internal Func<T, object> PrimaryKeyGetter;

        private Type _type = typeof(T);

        private IndexedCollection<T> _items;

        public CacheUniqueIndexCollection<T> UniqueIndices { get; private set; }

        public ICollection<ICacheIndex<T>> AllIndices { get; private set; }

        private List<CacheUniqueIndex<T>> _uniqueIndices = new List<CacheUniqueIndex<T>>();

        private Dictionary<UniqueConstraint, CacheUniqueIndex<T>> _indexedByConstraint = new Dictionary<UniqueConstraint, CacheUniqueIndex<T>>();

        public bool ExistsUniqueIndex(UniqueConstraint constraint, out CacheUniqueIndex<T> index)
        {
            if (constraint.DeclaringType != _type)
            {
                throw new ArgumentException("Constraint is not on type " + _type, "constraint");
            }
            throw new NotImplementedException();
        }

        public CacheUniqueIndex<T> GetOrCreateUniqueIndex(UniqueConstraint constraint)
        {
            throw new NotImplementedException();
        }

        public override object QueryFirst()
        {
            return _items.First();
        }

        public override object QueryLast()
        {
            return _items.Last();
        }

        public override object QueryFirstOrDefault()
        {
            return _items.FirstOrDefault();
        }

        public override object QueryLastOrDefault()
        {
            return _items.LastOrDefault();
        }

        public override object QuerySingle()
        {
            return _items.Single();
        }

        public override object QuerySingleOrDefault()
        {
            return _items.SingleOrDefault();
        }

        public override object QueryFirst(Expression expression)
        {
            return enumerateWhere(expression).First();
        }

        public override object QueryFirstOrDefault(Expression expression)
        {
            return enumerateWhere(expression).FirstOrDefault();
        }

        public override object QueryLast(Expression expression)
        {
            return enumerateWhere(expression).Last();
        }

        public override object QueryLastOrDefault(Expression expression)
        {
            return enumerateWhere(expression).LastOrDefault();
        }

        public override object QuerySingle(Expression expression)
        {
            return enumerateWhere(expression).Single();
        }

        public override object QuerySingleOrDefault(Expression expression)
        {
            return enumerateWhere(expression).SingleOrDefault();
        }

        public override object QueryWhere(Expression expression)
        {
            return enumerateWhere(expression);
        }

        public override int QueryCount()
        {
            return _items.Count;
        }

        public override int QueryCount(Expression expression)
        {
            return enumerateWhere(expression).Count();
        }

        public override void Add(object obj)
        {
            throw new NotImplementedException();
        }

        public override void Delete(object obj)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<T> enumerateWhere(Expression expression)
        {
            Func<T, bool> predicate = compileToGenericPredicate(expression);
            return _items.Where(predicate);
        }

        private static Func<T, bool> compileToGenericPredicate(Expression expression)
        {
            Expression<Func<T, bool>> asGeneric = expression as Expression<Func<T, bool>>;
            if (asGeneric == null)
            {
                throw new NotSupportedException();
            }
            return asGeneric.Compile();
        }

        public override object GetObjectByPrimaryKey(object primaryKey)
        {
            return _items.GetByPrimaryKey(primaryKey);
        }
    }
}
