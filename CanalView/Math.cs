﻿using System;

namespace CanalView
{
    public static class Math
    {
        public static readonly (int X, int Y)[] Cardinals = new (int, int)[] { (0, -1), (-1, 0), (0, 1), (1, 0) };
        public static readonly (int X, int Y)[] Diagonals = new (int, int)[] { (-1, -1), (-1, 1), (1, -1), (1, 1) };
        public static readonly (int X, int Y)[] ClockwiseDirections = new (int, int)[] { (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1) };

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
