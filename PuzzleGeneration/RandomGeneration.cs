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
                .Where(p => board[p.x, p.y] == Cell.Unknown || board[p.x, p.y] == Cell.Empty)
                .ToArray();
            if (vacantSpots.Length == 0)
                return false;
            var p = vacantSpots.RandomItem(random);
            Generation.FillNumber(board, p);
            return true;
        }
    }
}

