using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class ArrayIndexedCollection<T> : IndexedCollection<T>
        where T : class
    {
        private int _count;
        private T[] _items;

        private Func<T, int> _idSelector;

        public override int Count
        {
            get { return _count; }
        }

        public override void Add(T item)
        {
            lock (this)
            {
                T[] items = _items;
                int id = _idSelector(item);
                if (id < 0)
                {
                    throw new InvalidOperationException("Negative Id?");
                }
                if (id >= items.Length)
                {
                    int minimumRequiredLength = id + 1;
                    int newLength = items.Length;
                    while (newLength < minimumRequiredLength)
                    {
                        newLength *= 2;
                    }
                    Array.Resize(ref items, newLength);
                    items[id] = item;
                    _items = items;
                }
                else
                {
                    if (items[id] != null)
                    {
                        throw new InvalidOperationException("Array already contains item with id " + id);
                    }
                    items[id] = item;
                }
                _count++;
            }
        }

        public override bool Contains(T item)
        {
            T[] items = _items;
            int id = _idSelector(item);
            if ((uint)id > items.Length)
            {
                return false;
            }
            return items[id] != null;
        }

        public override T GetByPrimaryKey(object primaryKey)
        {
            int pkAsInt = Convert.ToInt32(primaryKey);
            T[] items = _items;
            if ((uint)pkAsInt >= items.Length)
            {
                return null;
            }
            return items[pkAsInt];
        }

        public override IEnumerator<T> GetEnumerator()
        {
            T[] items = _items;
            for (int i = 0; i < items.Length; i++)
            {
                T item = items[i];
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        public override bool Remove(T item)
        {
            int id = _idSelector(item);
            lock (this)
            {
                T[] items = _items;
                if ((uint)id >= items.Length)
                {
                    return false;
                }
                else if (items[id] != null)
                {
                    items[id] = null;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
