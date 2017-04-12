using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal abstract class IndexedCollection<T> : ICollection<T>
        where T : class
    {
        public abstract T GetByPrimaryKey(object primaryKey);

        public abstract int Count { get; }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public abstract void Add(T item);

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        public abstract bool Contains(T item);

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            foreach(T item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        public abstract IEnumerator<T> GetEnumerator();

        public abstract bool Remove(T item);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
