using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.ManagedCollections
{
    internal abstract class ChildObjectCollection<T> : ICollection<T>, IQueryable<T>
    {
        private List<T> _itemsCurrent;
        private List<T> _itemsInitial;

        protected abstract IQueryable<T> QueryItems();

        private List<T> List
        {
            get
            {
                if (_itemsCurrent == null)
                {
                    _itemsCurrent = QueryItems().ToList();
                }
                return _itemsCurrent;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_itemsCurrent == null)
            {
                return QueryItems().GetEnumerator();
            }
            else
            {
                return _itemsCurrent.GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        Type IQueryable.ElementType
        {
            get { return QueryItems().ElementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { return QueryItems().Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return QueryItems().Provider; }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            foreach (T item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        int ICollection<T>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
