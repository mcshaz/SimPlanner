using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Dto.Utilities
{
    public static class LinqExtensions
    {
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
        {
            return new HashSet<TSource>(source);
        }
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
        /// <summary>
        /// no different to .GroupBy(x=>x).Select(x=>x.Key)
        /// </summary>
        /*
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source)
        {
            HashSet<T> hash = new HashSet<T>();
            foreach (T s in source)
            {
                if (hash.Add(s))
                {
                    yield return s;
                }
            }
        }
        */

        // no different to .GroupBy(x=>x).Select(x=>x.Key)
        public static IEnumerable<T> Repeats<T>(this IEnumerable<T> source)
        {
            HashSet<T> hash = new HashSet<T>();
            foreach (T s in source)
            {
                if (!hash.Add(s))
                {
                    yield return s;
                }
            }
        }


        public static TDest[] Map<TSource, TDest>(this IList<TSource> source, Func<TSource, TDest> predicate)
        {
            var returnVar = new TDest[source.Count];
            for (int i = 0; i < returnVar.Length; i++)
            {
                returnVar[i] = predicate(source[i]);
            }
            return returnVar;
        }
        /// <summary>
        /// Note for arrays - better to use Array.Copy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> AllButLast<T>(this IEnumerable<T> source) 
        {
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    T returnVar = e.Current;
                    while (e.MoveNext())
                    {
                        yield return returnVar;
                        returnVar = e.Current;
                    }
                }
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T find)
        {
            int i = 0;
            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (e.Current == null)
                    {
                        if (find == null)
                        {
                            return i;
                        }
                    }
                    else if (e.Current.Equals(find))
                    {
                        return i;
                    }
                    i++;
                }
            }
            return -1;
        }

        //if performance critical and > ~3 elements, use unsafe method - starting point around here: http://www.techmikael.com/2009/01/fast-byte-array-comparison-in-c.html

        public static bool StartsWith<T>(this IList<T> source, IList<T> startList)
        {
            if (startList.Count > source.Count) { return false; }
            for (int i=0; i < startList.Count; i++)
            {
                if (!source[i].Equals(startList[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static int CorrespondingStartElements<T>(this IList<T> source, IList<T> comparisonList)
        {
            int i = 0;
            while (i<source.Count && i<comparisonList.Count && source[i].Equals(comparisonList[i]))
            {
                i++;
            }
            return i;
        }

        public static int IndexWhere<T>(this IList<T> source, Func<T, bool> predicate)
        {
            for (int i=0; i<source.Count; i++)
            {
                if (predicate(source[i]))
                {
                    return i;
                }
            }
            return - 1;
        }

        public static void AddOrCreate<TKey, TCollection, TValue>(
            this Dictionary<TKey, TCollection> dictionary, TKey key, TValue value)
            where TCollection : ICollection<TValue>, new()
        {
            TCollection collection;
            if (!dictionary.TryGetValue(key, out collection))
            {
                collection = new TCollection();
                dictionary.Add(key, collection);
            }
            collection.Add(value);
        }

        public static void AddRangeOrCreate<TKey, TCollection, TValue>(
            this Dictionary<TKey, TCollection> dictionary, TKey key, IEnumerable<TValue> values)
            where TCollection : ICollection<TValue>, new()
        {
            TCollection collection;
            if (!dictionary.TryGetValue(key, out collection))
            {
                collection = new TCollection();
                dictionary.Add(key, collection);
            }
            foreach (var v in values)
            {
                collection.Add(v);
            }
        }


        public static List<KeyValuePair<TKey,TValue>> ToKeyValuePairList<TSource, TKey, TValue>(
            this IEnumerable<TSource> source, Func<TSource,TKey> keySelector, Func<TSource,TValue> valueSelector)
        {
            return new List<KeyValuePair<TKey, TValue>>(source.Select(s=>new KeyValuePair<TKey, TValue>(keySelector(s), valueSelector(s))));
        }

        public static IEnumerable<TValue> TryGetValues<TKey, TValue>(this IDictionary<TKey, TValue> dict, params TKey[] keys)
        {
            foreach(var k in keys)
            {
                TValue val;
                if (dict.TryGetValue(k, out val))
                {
                    yield return val;
                }
            }
        }
    }
}
