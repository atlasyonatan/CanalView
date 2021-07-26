using System;
using System.Collections.Generic;
using System.Linq;

namespace CanalView.Solvers
{
    public class BruteSolver : ISolver
    {
        private static readonly Cell[] FillOptions = new Cell[] { Cell.Full, Cell.Empty };
        public IEnumerable<Cell[,]> Solve(Cell[,] board)
        {
            board = board.Copy();
            var i = 0;
            var unknowns = board.GetSpots()
                    .Where(s => board[s.X, s.Y] == Cell.Unkown)
                    .ToArray();
            while (i >= 0)
            {
                var (x, y) = unknowns[i];
                var cell = board[x, y];
                int nextGuessIndex = cell == Cell.Unkown ? 0 : Array.IndexOf(FillOptions, cell) + 1;
                if (nextGuessIndex >= FillOptions.Length)
                {
                    board[x, y] = Cell.Unkown;
                    i--;
                    continue;
                }
                board[x, y] = FillOptions[nextGuessIndex];
                if (Legality.LegalSquare(board, x, y) && Legality.LegalNumbers(board, x, y))
                    if (i < unknowns.Length - 1)
                        i++;
                    else if ((board[x, y] == Cell.Full && Legality.LegalPath(board, x, y)) || (board[x, y] != Cell.Full && Legality.LegalPath(board)))
                        yield return board.Copy();
            }
        }
    }
}
