using CanalView;
using System;
using System.Collections.Generic;
using System.Linq;
using static PuzzleGeneration.Generation;

namespace PuzzleGeneration
{
    public static class Generators
    {
        public static IEnumerable<Cell[,]> RandomPathGenerator(Cell[,] baseBoard, Random random, double fillChance)
        {
            var unknowns = baseBoard.GetSpots()
                .Where(s => baseBoard[s.X, s.Y] == Cell.Unkown)
                .ToList();
            while (true)
            {
                var board = baseBoard.Copy();
                var (x, y) = unknowns.RandomItem(random);
                board[x, y] = Cell.Full;
                AddRandomPath(board, x, y, random, fillChance);
                yield return board;
            }
        }

        public static IEnumerable<Cell[,]> RandomPathNumbersGenerator(Cell[,] baseBoard, Random random, double fillChance) =>
            RandomPathGenerator(baseBoard, random, fillChance).Select(board =>
            {
                var (x, y) = board.GetSpots()
                    .Where(p => board[p.X, p.Y] == Cell.Unkown || board[p.X, p.Y] == Cell.Empty)
                    .RandomItem(random);
                FillNumber(board, x, y);
                return board;
            });
        }
    }
}
