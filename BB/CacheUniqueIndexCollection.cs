using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class CacheUniqueIndexCollection<T> : ICollection<CacheUniqueIndex<T>>
    {
        private CacheUniqueIndex<T>[] _indices;
        private Dictionary<UniqueConstraint, CacheUniqueIndex<T>> _indexedByConstraint;

        internal CacheUniqueIndexCollection(IEnumerable<CacheUniqueIndex<T>> items)
        {
            _indices = items.ToArray();
            _indexedByConstraint = items.ToDictionary(i => i.Constraint);
        }

        public int Count
        {
            get { return _indices.Length; }
        }

        public CacheUniqueIndex<T> this[UniqueConstraint constraint]
        {
            get { return _indexedByConstraint[constraint]; }
        }

        void ICollection<CacheUniqueIndex<T>>.Add(CacheUniqueIndex<T> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<CacheUniqueIndex<T>>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<CacheUniqueIndex<T>>.Contains(CacheUniqueIndex<T> item)
        {
            return _indices.Contains(item);
        }

        void ICollection<CacheUniqueIndex<T>>.CopyTo(CacheUniqueIndex<T>[] array, int arrayIndex)
        {
            _indices.CopyTo(array, arrayIndex);
        }

        bool ICollection<CacheUniqueIndex<T>>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<CacheUniqueIndex<T>>.Remove(CacheUniqueIndex<T> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<CacheUniqueIndex<T>> IEnumerable<CacheUniqueIndex<T>>.GetEnumerator()
        {
            return ((IEnumerable<CacheUniqueIndex<T>>)_indices).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _indices.GetEnumerator();
        }
    }
}
