using System;
using System.Collections.Generic;
using System.Linq;

namespace PuzzleGeneration
{
    public static class Randomize
    {
        public static (int x, int y) RandomPosition<T>(this T[,] arr, Random random) =>
            (random.Next(0, arr.GetLength(0) - 1), random.Next(0, arr.GetLength(1) - 1));

        public static T RandomItem<T>(this IEnumerable<T> source, Random random)
        {
            var arr = source.ToArray();
            return arr[random.Next(arr.Length)];
        }

        public static (T item, double weight) WeightedRandomItem<T>(this IEnumerable<(T item, double weight)> source, Random random)
        {
            var arr = source.ToArray();
            var total = arr.Sum(x => x.weight);
            var r = random.NextDouble() * total;
            var i = 0;
            for (; i < arr.Length; i++)
            {
                if (r < arr[i].weight)
                    break;
                r -= arr[i].weight;
            }
            return arr[i];
        }
    }
}
