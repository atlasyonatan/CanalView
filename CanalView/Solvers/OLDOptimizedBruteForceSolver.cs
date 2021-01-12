using System.Collections;
using System.Collections.Generic;
using CanalView.Interfaces;

namespace CanalView
{
    public class OLDOptimizedBruteForceSolver : ISolver
    {
        public IEnumerable<Cell[,]> Solve(Cell[,] board)
        {
            board = (Cell[,])board.Clone();
            List<Cell[,]> solutions = new List<Cell[,]>();
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            var size = width * height;
            void InnerSolve(int index)
            {
                if (!board.LegalSquare() || !board.LegalNumbers()) return;
                if (index >= size)
                {
                    if (board.LegalPath()) solutions.Add((Cell[,])board.Clone());
                    return;
                }
                if (board[index % width, index / width] == Cell.Unkown)
                {
                    foreach (var fill in Rules.FillOptions)
                    {
                        board[index % width, index / width] = fill;
                        InnerSolve(index + 1);
                    }
                    board[index % width, index / width] = Cell.Unkown;
                }
                else InnerSolve(index + 1);
            }
            InnerSolve(0);
            return solutions;
        }

        
    }
}
