using System;
using System.Collections.Generic;

namespace Ariadna.Core
{
    public static class Extensions
    {
        public static ICollection<T> AddRange<T>(this ICollection<T> collection, ICollection<T> addedItems)
        {
            foreach (var item in addedItems)
                collection.Add(item);

            return collection;
        }

        public static ICollection<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> addedItems)
        {
            foreach (var item in addedItems)
                collection.Add(item);

            return collection;
        }

        public static ICollection<T> RemoveRange<T>(this ICollection<T> collection, ICollection<T> addedItems)
        {
            foreach (var item in addedItems)
                collection.Remove(item);

            return collection;
        }


        public static ICollection<T> Foreach<T>(this ICollection<T> collection, Action<T> action)
        {
            foreach (var item in collection) 
                action.Invoke(item);

            return collection;
        }

        public static T AddTo<T>(this T self, ICollection<T> c)
        {
            c.Add(self);
            return self;
        }

        public static T InsertTo<T>(this T self, IList<T> c, int index)
        {
            c.Insert(index, self);
            return self;
        }

        public static KeyValuePair<TKey, TValue> GetKeyValuePair<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            TKey key)
        {
            return new(key, dictionary[key]);
        }
    }
}
