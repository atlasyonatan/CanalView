using System.Collections.Generic;

namespace CanalView
{
    public static class Solver
    {
        public static IEnumerable<Cell[,]> Solve(Cell[,] board)
        {
            board = (Cell[,])board.Clone();
            List<Cell[,]> solutions = new List<Cell[,]>();
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            var size = width * height;
            void InnerSolve(int index)
            {
                //Printer.PrintBoard(board);
                if (!board.LegalSquare() || !board.LegalNumbers()) return;
                if (index >= size)
                {
                    if(board.LegalPath()) solutions.Add((Cell[,])board.Clone());
                    return;
                }
                var x = index % width;
                var y = index / width;
                if (board[x, y] == Cell.Unkown)
                {
                    foreach (var fill in Rules.FillOptions)
                    {
                        board[x, y] = fill;
                        InnerSolve(index + 1);
                    }
                    board[x, y] = Cell.Unkown;
                }
                else InnerSolve(index + 1);
            }
            InnerSolve(0);
            return solutions;
        }
    }
}
