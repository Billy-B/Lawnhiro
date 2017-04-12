using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class CacheUniqueIndex<T> : ICacheIndex<T>
    {
        public UniqueConstraint Constraint { get; private set; }

        private readonly Dictionary<object, T> _indexedByKey;

        private readonly Func<T, object> _keyGetter;

        public void Add(T value)
        {
            _indexedByKey.Add(_keyGetter(value), value);
        }

        public void Remove(T value)
        {
            _indexedByKey.Remove(_keyGetter(value));
        }

        public T GetValue(object key)
        {
            return _indexedByKey[key];
        }

        public bool ContainsObjectWithKey(T value)
        {
            return _indexedByKey.ContainsKey(_keyGetter(value));
        }

        public CacheUniqueIndex(UniqueConstraint constraint, IEnumerable<T> items)
        {
            Constraint = constraint;
            _keyGetter = DelegateUtils.BuildKeyGetter<T>(constraint);
            _indexedByKey = items.ToDictionary(_keyGetter);
        }
    }
}
