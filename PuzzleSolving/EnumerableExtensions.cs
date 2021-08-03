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

        public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] sources) =>
            sources.SelectMany(s => s.Select(i => i));

        public static IEnumerable<T> ContactIfNotNull<T>(params Func<IEnumerable<T>>[] functions)
        {
            var results = new List<IEnumerable<T>>();
            foreach (var func in functions)
            {
                var result = func();
                if (result == null)
                    return null;
                results.Add(result);
            }
            return Concat(results.ToArray());
        }
    }
}
