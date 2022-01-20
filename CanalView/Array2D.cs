using System;
using System.Collections.Generic;
using System.Linq;

namespace CanalView
{
    public static class Array2D
    {
        public static readonly (int x, int y)[] Cardinals = new (int, int)[] { (0, -1), (-1, 0), (0, 1), (1, 0) };
        public static readonly (int x, int y)[] Diagonals = new (int, int)[] { (-1, -1), (-1, 1), (1, -1), (1, 1) };
        public static readonly (int x, int y)[] ClockwiseDirections = new (int, int)[] { (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1) };

        public static bool Contains<T>(this T[,] arr, int x, int y) =>
            x >= 0 &&
            x < arr.GetLength(0) &&
            y >= 0 &&
            y < arr.GetLength(1);

        public static IEnumerable<(int x, int y)> Points<T>(this T[,] arr) => Enumerable.Range(0, arr.GetLength(0) * arr.GetLength(1))
            .Select(i => (X: i % arr.GetLength(0), Y: i / arr.GetLength(0)));

        public static T[,] Copy<T>(this T[,] arr) => (T[,])arr.Clone();

        public static void Fill<T>(T[,] arr, T cell)
        {
            var width = arr.GetLength(0);
            var height = arr.GetLength(1);
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    arr[x, y] = cell;
        }

        public static void FloodFill<T>(T[,] arr, int x, int y, T color) where T : IComparable
        {
            var match = arr[x, y];
            if (match.CompareTo(color) == 0)
                return;
            void InnerFloodFill(int x, int y)
            {
                arr[x, y] = color;
                for (var i = 0; i < Cardinals.Length; i++)
                {
                    var newX = x + Cardinals[i].x;
                    var newY = y + Cardinals[i].y;
                    if (arr.Contains(newX, newY) && arr[newX, newY].CompareTo(match) == 0)
                        InnerFloodFill(newX, newY);
                }
            }
            InnerFloodFill(x, y);
        }

        public static T[,] ConvertTo2DArray<T>(T[][] jaggedArray)
        {
            var columnLength = jaggedArray.Length;
            if (columnLength == 0)
                return new T[0, 0];
            var rowLength = jaggedArray[0].Length;
            if (jaggedArray.Any(row => row.Length != rowLength))
                throw new InvalidOperationException("Different row lengths in jagged array.");
            var array2D = new T[rowLength, columnLength];
            for (int y = 0; y < columnLength; y++)
                for (int x = 0; x < rowLength; x++)
                    array2D[x, y] = jaggedArray[y][x];
            return array2D;
        }

        public static bool TryConvertTo2DArray<T>(T[][] jaggedArray, out T[,] array2D)
        {
            array2D = new T[0, 0];
            var columnLength = jaggedArray.Length;
            if (columnLength == 0)
                return true;
            var rowLength = jaggedArray[0].Length;
            if (jaggedArray.Any(row => row.Length != rowLength))
                return false;
            array2D = new T[rowLength, columnLength];
            for (int y = 0; y < columnLength; y++)
                for (int x = 0; x < rowLength; x++)
                    array2D[x, y] = jaggedArray[y][x];
            return true;
        }

        public static T[][] ToJaggedArray<T>(T[,] array2D)
        {
            var jagged = new T[array2D.GetLength(1)][];
            for (int y = 0; y < array2D.GetLength(1); y++)
            {
                jagged[y] = new T[array2D.GetLength(0)];
                for (int x = 0; x < array2D.GetLength(0); x++)
                    jagged[y][x] = array2D[x, y];
            }
            return jagged;
        }

        public static bool SequenceEquals<T>(this T[,] a, T[,] b) => a.Rank == b.Rank
            && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
            && a.Cast<T>().SequenceEqual(b.Cast<T>());
    }
}
