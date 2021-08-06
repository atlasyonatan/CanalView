using CanalView;
using System;
using System.Linq;
using static PuzzleSolving.Musts;

namespace PuzzleGeneration
{
    public static class Generation
    {
        public static void AddRandomPath(Cell[,] board, int x, int y, Random random, double fillChance)
        {
            void RandomNeighborFill(int X, int Y)
            {
                if (random.NextDouble() < fillChance)
                {
                    board[X, Y] = Cell.Full;
                    board.ApplyMusts_Full_LShape(new CellInfo { Position = (X, Y) });
                    var neighbors = Array2DExtensions.Cardinals.Select(d => (x: X + d.X, y: Y + d.Y))
                        .Where(p => board.Contains(p.x, p.y) && board[p.x, p.y] == Cell.Unkown);
                    foreach (var neighbor in neighbors)
                        RandomNeighborFill(neighbor.x, neighbor.y);
                }
            }
            RandomNeighborFill(x, y);
        }
        public static int FindNumber(this Cell[,] board, int x, int y)
        {
            throw new NotImplementedException();
        }
        public static void FillNumber(Cell[,] board, int x, int y) => 
            board[x, y] = (Cell)board.FindNumber(x, y);
    }
}
