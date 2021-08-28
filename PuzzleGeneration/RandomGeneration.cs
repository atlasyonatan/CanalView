using CanalView;
using System;
using System.Linq;

namespace PuzzleGeneration
{
    public static class RandomGeneration
    {
        public static bool AddRandomNumber(Cell[,] board, Random random)
        {
            var vacantSpots = board.Points()
                .Where(p => board[p.x, p.y] == Cell.Unkown || board[p.x, p.y] == Cell.Empty)
                .ToArray();
            if (vacantSpots.Length == 0)
                return false;
            var p = vacantSpots.RandomItem(random);
            Generation.FillNumber(board, p);
            return true;
        }

        public static bool RemoveRandomNumber(Cell[,] board, Random random)
        {
            var numbers = board.Points()
                .Where(p => board[p.x, p.y] > 0)
                .ToArray();
            if (numbers.Length == 0)
                return false;
            var p = numbers.RandomItem(random);
            board[p.x, p.y] = Cell.Empty;
            return true;
        }
    }
}

