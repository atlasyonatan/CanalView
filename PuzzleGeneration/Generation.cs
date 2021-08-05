using CanalView;
using System;
using System.Linq;
using static PuzzleSolving.Musts;
using static CanalView.Array2DExtensions;
using System.Collections.Generic;

namespace PuzzleGeneration
{
    public static class Generation
    {
        public static (int x, int y) RandomPosition<T>(this T[,] arr, Random random) =>
            (random.Next(0, arr.GetLength(0) - 1), random.Next(0, arr.GetLength(1) - 1));//todo: check which 2darr dimension corrisponds to width and height

        public static Cell[,] GenerateRandomPath(this Cell[,] board, int x, int y, Random random, double fillChance)
        {
            void RandomNeighborFill(int x, int y)
            {
                if (random.NextDouble() < fillChance)
                {
                    board[x, y] = Cell.Full;
                    board.ApplyMusts_Full_LShape(new CellInfo { Position = (x, y) });
                    var neighbors = Cardinals.Select(d => (x: x + d.X, y: y + d.Y))
                        .Where(p => board.Contains(p.x, p.y) && board[p.x, p.y] == Cell.Unkown);
                    foreach (var neighbor in neighbors)
                        RandomNeighborFill(neighbor.x, neighbor.y);
                }
            }
            RandomNeighborFill(x, y);
            return board;
        }

        public static T RandomItem<T>(this IEnumerable<T> source, Random random)
        {
            var arr = source.ToArray();
            return arr[random.Next(arr.Length)];
        }
    }
}
