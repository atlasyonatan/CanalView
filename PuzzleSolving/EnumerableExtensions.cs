using System;
using System.Collections.Generic;

namespace PuzzleSolving
{
    public static class EnumerableExtensions
    {
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
    }
}
