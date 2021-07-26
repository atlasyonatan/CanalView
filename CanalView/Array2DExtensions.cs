using System;
using System.Collections.Generic;
using System.Linq;

namespace CanalView
{
    public static class Array2DExtensions
    {
        public static readonly (int X, int Y)[] Cardinals = new (int, int)[] { (0, -1), (-1, 0), (0, 1), (1, 0) };
        public static readonly (int X, int Y)[] Diagonals = new (int, int)[] { (-1, -1), (-1, 1), (1, -1), (1, 1) };
        public static readonly (int X, int Y)[] ClockwiseDirections = new (int, int)[] { (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1) };

        public static bool Contains<T>(this T[,] arr, int x, int y) =>
            x >= 0 &&
            x < arr.GetLength(0) &&
            y >= 0 &&
            y < arr.GetLength(1);

        public static IEnumerable<(int X, int Y)> GetSpots<T>(this T[,] arr) => Enumerable.Range(0, arr.GetLength(0) * arr.GetLength(1))
            .Select(i => (X: i % arr.GetLength(0), Y: i / arr.GetLength(0)));

        public static T[,] Copy<T>(this T[,] arr) => (T[,])arr.Clone();

        public static T[,] Fill<T>(this T[,] board, T cell)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    board[x, y] = cell;
            return board;
        }

        public static T[,] Add<T>(this T[,] board, params (int x, int y, T cell)[] cells)
        {
            foreach (var (x, y, cell) in cells) board[x, y] = cell;
            return board;
        }

        public static T[,] FloodFill<T>(this T[,] board, int x, int y, T color) where T : IComparable
        {
            var match = board[x, y];
            if (match.CompareTo(color) == 0) return board;
            void InnerFloodFill(int x, int y)
            {
                board[x, y] = color;
                for (var i = 0; i < Cardinals.Length; i++)
                {
                    var newX = x + Cardinals[i].X;
                    var newY = y + Cardinals[i].Y;
                    if (board.Contains(newX, newY) && board[newX, newY].CompareTo(match) == 0)
                        InnerFloodFill(newX, newY);
                }
            }
            InnerFloodFill(x, y);
            return board;
        }
    }
}
