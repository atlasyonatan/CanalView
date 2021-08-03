using CanalView;
using System;
using System.Linq;

namespace PuzzleGeneration
{
    public static class Generation
    {
        public static (int x, int y) RandomPosition<T>(this T[,] arr, Random random) =>
            (random.Next(0, arr.GetLength(0) - 1), random.Next(0, arr.GetLength(1) - 1));//todo: check which 2darr dimension corrisponds to width and height

        public static Cell[,] GenerateRandomPath(this Cell[,] board, (int x, int y) start, Random random, double fillChance)
        {
            void RandomNeighborFill(int x, int y)
            {
                if (board[x, y] != Cell.Empty)
                    return;
                if (random.NextDouble() < fillChance)
                {
                    board[x, y] = Cell.Full;

                    //todo: fillmusts L shape
                }
                var neighbors = Array2DExtensions.Cardinals
                    .Select(d => (x: x + d.X, y: y + d.Y))
                    .Where(p => board.Contains(p.x, p.y));
                foreach (var neighbor in neighbors)
                {

                }
            }
            throw new NotImplementedException();
        }
    }
}
