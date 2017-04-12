using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class ClusteredIndexedCollection<T> : IndexedCollection<T>
        where T : class
    {
        private T[] _items;
        private int _count;

        private Func<T, object> _keySelector;

        public override int Count
        {
            get { return _count; }
        }

        public override void Add(T item)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<T> GetEnumerator()
        {
            T[] items;
            int count;
            lock (this)
            {
                items = _items;
                count = _count;
            }
            for (int i = 0; i < count; i++)
            {
                yield return items[i];
            }
        }

        public override T GetByPrimaryKey(object primaryKey)
        {
            T[] items;
            int count;
            lock (this)
            {
                items = _items;
                count = _count;
            }
            int hashCode = primaryKey.GetHashCode();
            int low = 0;
            int high = count;
            int startIndex = -1, endIndex = -1;
            Func<T, object> keySelector = _keySelector;
            while (low <= high)
            {
                int mid = (high - low) / 2 + low;
                int current = keySelector(items[mid]).GetHashCode();
                if (current > hashCode)
                {
                    high = mid - 1;
                }
                else if (current == hashCode)
                {
                    startIndex = mid;
                    high = mid - 1;
                }
                else
                {
                    low = mid + 1;
                }
            }
            low = 0;
            high = count;
            while (low <= high)
            {
                int mid = (high - low) / 2 + low;
                int current = keySelector(items[mid]).GetHashCode();
                if (current > hashCode)
                {
                    high = mid - 1;
                }
                else if (current == hashCode)
                {
                    startIndex = mid;
                    high = mid + 1;
                }
                else
                {
                    low = mid + 1;
                }
            }
            if (startIndex == -1)
            {
                return null;
            }
            else if (startIndex == endIndex)
            {
                return items[startIndex];
            }
            else
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    T item = items[i];
                    if (primaryKey.Equals(keySelector(item)))
                    {
                        return item;
                    }
                }
                return null;
            }
        }
    }
}
