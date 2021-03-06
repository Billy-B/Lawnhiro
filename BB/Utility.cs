﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal static class Utility
    {
        internal static InvalidOperationException CollectionReadOnlyException()
        {
            return new InvalidOperationException("The collection is read-only.");
        }
        internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue ret;
            dictionary.TryGetValue(key, out ret);
            return ret;
        }

        internal static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recursion)
        {
            return source.SelectMany(x => (recursion(x) != null && recursion(x).Any()) ? recursion(x).Flatten(recursion) : null).Where(x => x != null);
        }

        internal static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        internal static bool IsNullable(this Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                return type.GetGenericTypeDefinition() == typeof(Nullable<>);
            }
            else
            {
                return false;
            }
        }

        private static class ConditionalWeakTableFuncs<TKey, TValue>
            where TKey : class
            where TValue : class
        {
            internal static Func<ConditionalWeakTable<TKey, TValue>, ICollection<TKey>> KeysGetter;
            internal static Func<ConditionalWeakTable<TKey, TValue>, ICollection<TValue>> ValuesGetter;

            static ConditionalWeakTableFuncs()
            {
                KeysGetter = (Func<ConditionalWeakTable<TKey, TValue>, ICollection<TKey>>)Delegate.CreateDelegate(typeof(Func<ConditionalWeakTable<TKey, TValue>, ICollection<TKey>>), typeof(ConditionalWeakTable<TKey, TValue>).GetProperty("Keys", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true));
                ValuesGetter = (Func<ConditionalWeakTable<TKey, TValue>, ICollection<TValue>>)Delegate.CreateDelegate(typeof(Func<ConditionalWeakTable<TKey, TValue>, ICollection<TValue>>), typeof(ConditionalWeakTable<TKey, TValue>).GetProperty("Values", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true));
            }
        }

        internal static ICollection<TKey> GetKeys<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table)
            where TKey : class
            where TValue : class
        {
            return ConditionalWeakTableFuncs<TKey, TValue>.KeysGetter(table);
        }

        internal static ICollection<TValue> GetValues<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table)
            where TKey : class
            where TValue : class
        {
            return ConditionalWeakTableFuncs<TKey, TValue>.ValuesGetter(table);
        }
    }
}
