using System;
using System.Collections.Generic;

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
    }
}
