using System;
using System.Collections.Generic;

namespace SM.DTOs.Utilities
{
    public static class LinqExtensions
    {
        public static HashSet<TKey> ToHashSet<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> map)
        {
            HashSet<TKey> returnVar = new HashSet<TKey>();
            foreach (var s in source)
            {
                returnVar.Add(map(s));
            }
            return returnVar;
        }
        public static HashSet<TKey> ToHashSet<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> map, out List<TKey> repeats)
        {
            HashSet<TKey> returnVar = new HashSet<TKey>();
            repeats = new List<TKey>();
            foreach(var s in source)
            {
                TKey k = map(s);
                if (!returnVar.Add(k)) {
                    repeats.Add(k);
                }
            }
            return returnVar;
        }

        public static bool EachDistinct<T>(this IEnumerable<T> source)
        {
            HashSet<T> hash = new HashSet<T>();
            foreach(T s in source)
            {
                if (hash.Add(s))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
