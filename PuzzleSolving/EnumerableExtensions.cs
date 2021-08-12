using System;
using System.Collections.Generic;
using System.Linq;

namespace PuzzleSolving
{
    public static class EnumerableExtensions
    {
        public static bool TryFirst<T>(this IEnumerable<T> seq, out T result)
        {
            result = default;
            foreach (var item in seq)
            {
                result = item;
                return true;
            }
            return false;
        }

        public static bool TryFirst<T>(this IEnumerable<T> seq, Predicate<T> predicate, out T result)
        {
            result = default;
            foreach (var item in seq)
                if (predicate(item))
                {
                    result = item;
                    return true;
                }
            return false;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<IEnumerable<T>> sources)
        {
            var arr1 = sources.ToArray();
            foreach (var source in arr1)
            {
                var arr2 = source.ToArray();
                foreach (var item in arr2)
                    yield return item;
            }
                
            //return sources.SelectMany(s => s.Select(i => i));
        }

        public static IEnumerable<T> ContactIfNotNull<T>(params Func<IEnumerable<T>>[] selectors)
        {
            var results = new List<T>();
            foreach (var selector in selectors)
            {
                var result = selector();
                if (result == null)
                    return null;
                results.AddRange(result);
            }
            return results;
        }

        public static IEnumerable<TOut> ContactIfNotNull<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, IEnumerable<TOut>> selector)
        {
            if (source == null)
                return null;
            var results = new List<TOut>();
            foreach (var item in source)
            {
                var result = selector(item);
                if (result == null)
                    return null;
                results.AddRange(result);
            }
            return results;
        }
    }
}
