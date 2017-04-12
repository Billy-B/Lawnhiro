using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    internal class GenericCollection<T> : IReadOnlyList<T>
    {
        private T[] _items;

        internal GenericCollection(IEnumerable<T> items)
        {
            _items = items.ToArray();
        }

        public T this[int index]
        {
            get
            {
                if ((uint)index > _items.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                return _items[index];
            }
        }

        public int Count
        {
            get { return _items.Length; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
