using CanalView;
using System;
using System.Linq;

namespace PuzzleGeneration
{
    public static class Generation
    {
        public static void AddValidPath(Cell[,] board, (int x, int y) start, Action<(int x, int y)> chooseCell)
        {
            void Fill((int x, int y) position)
            {
                chooseCell(position);
                if(board[position.x, position.y] == Cell.Full)
                {
                    var neighbors = Array2DExtensions.Cardinals
                            .Select(direction => (x: position.x + direction.X, y: position.y + direction.Y))
                            .Where(p => board.Contains(p.x, p.y) && board[p.x, p.y] == Cell.Unkown);
                    foreach (var neighbor in neighbors)
                        Fill(neighbor);
                }
            }
            Fill(start);
        }

        public static int FindNumber(this Cell[,] board, int x, int y)
        {
            var fullCount = 0;
            foreach (var (dx, dy) in Array2DExtensions.Cardinals)
            {
                var scale = 1;
                while (true)
                {
                    var (newX, newY) = (x + scale * dx, y + scale * dy);
                    if (!board.Contains(newX, newY) || board[newX, newY] != Cell.Full)
                        break;
                    fullCount++;
                    scale++;
                }
            }
            return fullCount;
        }
        public static void FillNumber(Cell[,] board, int x, int y) =>
            board[x, y] = (Cell)board.FindNumber(x, y);
        public static void AddRandomNumber(Cell[,] board, Random random)
        {
            var (x, y) = board.GetSpots()
                .Where(p => board[p.X, p.Y] == Cell.Unkown || board[p.X, p.Y] == Cell.Empty)
                .RandomItem(random);
            FillNumber(board, x, y);
        }
    }
}
