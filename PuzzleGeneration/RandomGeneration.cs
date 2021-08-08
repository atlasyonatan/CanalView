using CanalView;
using System;
using System.Collections.Generic;
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
            if (!unkowns.TryRandomItem(random, out var start))
                return;
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

        public static bool AddRandomNumber(Cell[,] board, Random random)
        {
            var vacantSpots = board.GetSpots()
                .Where(p => board[p.X, p.Y] == Cell.Unkown || board[p.X, p.Y] == Cell.Empty);
            if (vacantSpots.TryRandomItem(random, out var p))
            {
                FillNumber(board, p.X, p.Y);
                return true;
            }
            return false;
        }

        public static Cell[][,] AddRandomNumbers(Cell[,] board, Random random, int maxSolutions, Func<Cell[,], IEnumerable<Cell[,]>> solver)
        {
            var addCount = 0;
            Cell[][,] solutions = null;
            while (AddRandomNumber(board, random))
            {
                addCount++;
                var copy = board.Copy();
                Clean(copy);
                var count = 0;
                solutions = solver(copy).TakeWhile(_ => ++count <= maxSolutions).ToArray();
                if (count <= maxSolutions)
                    break;
            }
            return solutions;
        }

        public static void Clean(Cell[,] board)
        {
            foreach (var (x, y) in board.GetSpots())
                if (board[x, y] < 0)
                    board[x, y] = Cell.Unkown;
        }
    }
}

