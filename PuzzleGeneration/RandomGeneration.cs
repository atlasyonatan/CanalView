using CanalView;
using System;
using System.Linq;
using static PuzzleGeneration.Generation;
using static PuzzleSolving.Musts;

namespace PuzzleGeneration
{
    public static class RandomGeneration
    {
        public static void AddRandomValidPath(Cell[,] board, Random random, params (Cell type, double weight)[] weights)
        {
            var unkowns = board.GetSpots().Where(p => board[p.X, p.Y] == Cell.Unkown).ToArray();
            if (unkowns.Length == 0)
                return;
            var start = unkowns.RandomItem(random);
            board[start.X, start.Y] = Cell.Full;
            var done = new bool[board.GetLength(0), board.GetLength(1)];
            DepthFirstTransform(board, start, p =>
            {
                if (done[p.x, p.y])
                    return false;
                done[p.x, p.y] = true;

                if (board[p.x, p.y] == Cell.Full)
                    return true;
                if (board[p.x, p.y] != Cell.Unkown)
                    return false;

                var chosenCell = Randomize.WeightedRandomItem(weights, random).item;
                if (chosenCell != Cell.Full && chosenCell != Cell.Empty)
                    return false;
                board[p.x, p.y] = chosenCell;
                var changes = ApplyMustsRecursively(board, new CellInfo { Position = p });
                foreach (var cell in changes)
                    done[cell.Position.x, cell.Position.y] = true;
                return chosenCell == Cell.Full;
            });
        }

        public static void AddRandomNumber(Cell[,] board, Random random)
        {
            var (x, y) = board.GetSpots()
                .Where(p => board[p.X, p.Y] == Cell.Unkown || board[p.X, p.Y] == Cell.Empty)
                .RandomItem(random);
            FillNumber(board, x, y);
        }

        //public static void AddRandomNumbers(Cell[,] board, Random random, double pathChance)
        //{
        //    AddRandomNumber(board, random);
        //}
    }
}

