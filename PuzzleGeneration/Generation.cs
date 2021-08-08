using CanalView;
using System;
using System.Linq;
using static PuzzleSolving.Musts;

namespace PuzzleGeneration
{
    public static class Generation
    {
        public static void DepthFirstTransform<T>(T[,] arr, (int x, int y) position, Func<(int x, int y), bool> transform)
        {
            if (transform(position))
            {
                var neighbors = Array2DExtensions.Cardinals
                        .Select(direction => (x: position.x + direction.X, y: position.y + direction.Y))
                        .Where(p => arr.Contains(p.x, p.y));
                foreach (var neighbor in neighbors)
                    DepthFirstTransform(arr, neighbor, transform);
            }
        }

        //public static void AddValidFullPath(Cell[,] board, (int x, int y) position, Action<(int x, int y)> chooseCell)
        //{
        //    DepthFirstTransform(board, position, (b, p) =>
        //    {
        //        var before = b[p.x, p.y];

        //    })
        //    chooseCell(position);
        //    if (board[position.x, position.y] == Cell.Full)
        //    {
        //        board.ApplyMusts_Full(new CellInfo { Position = position });
        //        var neighbors = Array2DExtensions.Cardinals
        //                .Select(direction => (x: position.x + direction.X, y: position.y + direction.Y))
        //                .Where(p => board.Contains(p.x, p.y) && board[p.x, p.y] == Cell.Unkown);
        //        foreach (var neighbor in neighbors)
        //            AddValidFullPath(board, neighbor, chooseCell);
        //    }
        //}

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
        public static void FillNumber(Cell[,] board, int x, int y)
        {
            board[x, y] = (Cell)board.FindNumber(x, y);
            ApplyMustsRecursively(board, new CellInfo { Position = (x, y) });
        }

        public static void Unknownify(Cell[,] board)
        {
            foreach (var (x,y) in board.GetSpots())
            {
                if (board[x, y] == Cell.Empty)
                    board[x, y] = Cell.Unkown;
            }
        }
    }
}
