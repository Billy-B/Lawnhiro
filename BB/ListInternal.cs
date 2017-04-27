using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class ListInternal<T> : IList<T>
    {
        private List<T> _list;

        public ListInternal()
        {
            _list = new List<T>();
        }

        public ListInternal(IEnumerable<T> items)
        {
            _list = new List<T>(items);
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            internal set
            {
                _list[index] = value;
            }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            throw Utility.CollectionReadOnlyException();
        }

        int IList<T>.IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw Utility.CollectionReadOnlyException();
        }

        void ICollection<T>.Clear()
        {
            throw Utility.CollectionReadOnlyException();
        }

        T IList<T>.this[int index]
        {
            get { return this[index]; }
            set { throw Utility.CollectionReadOnlyException(); }
        }



        internal void Add(T item)
        {
            _list.Add(item);
        }
        internal bool Remove(T item)
        {
             return _list.Remove(item);
        }
        internal void Clear()
        {
            _list.Clear();
        }

        /*internal T GetItemOrDefaultOneIndexed(int index)
        {
            if ((uint)index > (uint)_list.Count)
            {
                return default(T);
            }
            return _list[index - 1];
        }*/


        void IList<T>.Insert(int index, T item)
        {
            throw Utility.CollectionReadOnlyException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw Utility.CollectionReadOnlyException();
        }


        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }
    }
}
