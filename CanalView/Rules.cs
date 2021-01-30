﻿using System.Linq;
using static CanalView.Math;

namespace CanalView
{
    public static class Rules
    {
        public static bool Legal(this Cell[,] board) =>
            board.LegalSquare() &&
            board.LegalPath() &&
            board.LegalNumbers();

        public static bool Legal(this Cell[,] board, int index) =>
            board.LegalSquare(index) &&
            board.LegalNumbers(index % board.GetLength(0), index / board.GetLength(0));

        public static bool LegalSquare(this Cell[,] board)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            for (var y = 0; y < height - 1; y++)
                for (var x = 0; x < width - 1; x++)
                {
                    if (board[x, y] != Cell.Full ||
                        board[x + 1, y] != Cell.Full ||
                        board[x, y + 1] != Cell.Full ||
                        board[x + 1, y + 1] != Cell.Full) continue;
                    return false;
                }
            return true;
        }

        public static bool LegalSquare(this Cell[,] board, int index)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            var size = width * height;
            if (index >= size) return true;
            var x = index % width;
            var y = index / width;
            if (board[x, y] == Cell.Full)
            {
                foreach (var (dx, dy) in Diagonals)
                {
                    var newX = x + dx;
                    var newY = y + dy;
                    if (newX < 0 || newX >= width || newY < 0 || newY >= height) continue;
                    if (board[x, newY] == Cell.Full &&
                        board[newX, y] == Cell.Full &&
                        board[newX, newY] == Cell.Full) return false;
                }
            }
            return true;
        }

        public static bool LegalNumbers(this Cell[,] board)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            var size = width * height;
            for (var i = 0; i < size; i++)
                if (board[i % width, i / width] >= 0 && !board.LegalNumbers(i % board.GetLength(0), i / board.GetLength(0)))
                    return false;
            return true;
        }

        public static bool LegalNumbers(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y)) return true;
            if (board[x, y] >= 0)
            {
                var cellNumber = (int)board[x, y];
                var countUnknown = 0;
                var countFull = 0;
                foreach (var (dx, dy) in Cardinals)
                {
                    var scale = 1;
                    var foundUnknown = false;
                    while (true)
                    {
                        var newX = x + dx * scale;
                        var newY = y + dy * scale;
                        if (!board.Contains(newX, newY)) break;
                        if (board[newX, newY] == Cell.Empty || board[newX, newY] >= 0) break;
                        if (foundUnknown || board[newX, newY] == Cell.Unkown)
                        {
                            foundUnknown = true;
                            countUnknown++;
                        }
                        else if (board[newX, newY] == Cell.Full && ++countFull > cellNumber) return false;
                        scale++;
                    }
                }
                return (countUnknown + countFull >= cellNumber) && (cellNumber != 0 || countFull <= 0);
            }
            foreach (var (dx, dy) in Cardinals)
            {
                var scale = 1;
                while (true)
                {
                    var newX = x + dx * scale;
                    var newY = y + dy * scale;
                    if (!board.Contains(newX, newY)) break;
                    if (board[newX, newY] >= 0)
                    {
                        if (!board.LegalNumbers(newX, newY))
                            return false;
                        break;
                    }
                    scale++;
                }
            }
            return true;
        }

        public static bool LegalPath(this Cell[,] board) =>
            !board.GetSpots().TryFirst(s => board[s.X, s.Y] == Cell.Full, out var spot) ||
            board.LegalPath(spot.X, spot.Y);

        public static bool LegalPath(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return true;
            var flooded = board.Copy().FloodFill(x, y, Cell.Full + 1);
            return !flooded.GetSpots().Any(s => flooded[s.X, s.Y] == Cell.Full);
        }
    }
}
