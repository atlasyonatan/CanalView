using CanalView;
using System.Collections.Generic;
using System.Linq;
using static PuzzleSolving.Musts;

namespace PuzzleSolving.Solvers
{
    public static class InferSolver
    {
        public static IEnumerable<Cell[,]> Solve(Cell[,] board)
        {
            board = board.Copy();
            var colors = new int[board.GetLength(0), board.GetLength(1)];
            var foundPreviously = false;
            var guesses = new Stack<((int X, int Y) Spot, Cell Value)>();
            while (true)
            {
                // try FillMusts
                var copy = board.Copy();
                var changes = guesses.TryPeek(out var guess)
                    ? ApplyMustsRecursively(copy, new CellInfo { Position = guess.Spot })
                    : ApplyMustsRecursively(copy);
                if (foundPreviously || changes == null)
                {
                    foundPreviously = false;

                    // TryPop
                    if (!guesses.TryPop(out guess))
                        yield break;

                    // Clean
                    var color = guesses.Count + 1;
                    foreach (var (X, Y) in board.GetSpots().Where(s => colors[s.X, s.Y] >= color).ToArray())
                    {
                        board[X, Y] = Cell.Unkown;
                        colors[X, Y] = 0;
                    }

                    // apply other value
                    var otherValue = guess.Value switch
                    {
                        Cell.Full => Cell.Empty,
                        _ => Cell.Full
                    };
                    board[guess.Spot.X, guess.Spot.Y] = otherValue;
                    colors[guess.Spot.X, guess.Spot.Y] = guesses.Count;
                }
                else
                {
                    // apply changes
                    foreach (var cell in changes)
                        colors[cell.Position.x, cell.Position.y] = guesses.Count;
                    board = copy;

                    // board is completed
                    var completed = board.GetSpots().All(s => board[s.X, s.Y] != Cell.Unkown);
                    if (completed)
                    {
                        foundPreviously = true;
                        yield return board.Copy();
                        //hasGuess = guesses.TryPeek(out guess);
                        continue;
                    }

                    // push and apply new guess
                    var newGuess = BestGuess(board);
                    guesses.Push(newGuess);
                    board[newGuess.Spot.X, newGuess.Spot.Y] = newGuess.Value;
                    colors[newGuess.Spot.X, newGuess.Spot.Y] = guesses.Count;
                    guess = newGuess;
                }
            }
        }

        public static ((int X, int Y) Spot, Cell Value) BestGuess(Cell[,] board)
        {
            var first = board.GetSpots().First(s => board[s.X, s.Y] == Cell.Unkown);
            return (first, Cell.Full);
        }
    }
}
