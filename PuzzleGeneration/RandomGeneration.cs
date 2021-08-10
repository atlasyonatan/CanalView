using CanalView;
using System;
using System.Linq;

namespace PuzzleGeneration
{
    public static class RandomGeneration
    {
        public static bool AddRandomNumber(Cell[,] board, Random random)
        {
            var vacantSpots = board.GetSpots()
                .Where(p => board[p.X, p.Y] == Cell.Unkown || board[p.X, p.Y] == Cell.Empty);
            if (vacantSpots.TryRandomItem(random, out var p))
            {
                Generation.FillNumber(board, p);
                return true;
            }
            return false;
        }
    }
}

