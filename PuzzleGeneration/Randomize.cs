using System;
using System.Collections.Generic;
using System.Linq;

namespace PuzzleGeneration
{
    public static class Randomize
    {
        public static (int x, int y) RandomPosition<T>(this T[,] arr, Random random) =>
            (random.Next(0, arr.GetLength(0) - 1), random.Next(0, arr.GetLength(1) - 1));//todo: check which 2darr dimension corrisponds to width and height

        public static T RandomItem<T>(this IEnumerable<T> source, Random random)
        {
            var arr = source.ToArray();
            return arr[random.Next(arr.Length)];
        }
    }
}
