using System.Collections.Generic;

namespace CanalView
{
    public static class Solver
    {
        public static IEnumerable<Cell[,]> Solve(Cell[,] board)
        {
            List<Cell[,]> solutions = new List<Cell[,]>();
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            var size = width * height;
            void InnerSolve(Cell[,] currentBoard, int index)
            {
                if (!currentBoard.LegalSquare() || !currentBoard.LegalNumbers()) return;
                if (index >= size)
                {
                    if(currentBoard.LegalPath()) solutions.Add(currentBoard);
                    return;
                }
                var x = index % width;
                var y = index / width;
                if (currentBoard[x, y] == Cell.Unkown)
                    foreach (var fill in Rules.FillOptions)
                    {
                        var clone = (Cell[,])currentBoard.Clone();
                        clone[x, y] = fill;
                        InnerSolve(clone, index + 1);
                    }
                else InnerSolve(currentBoard, index + 1);
            }
            InnerSolve(board, 0);
            return solutions;
        }
    }
}
