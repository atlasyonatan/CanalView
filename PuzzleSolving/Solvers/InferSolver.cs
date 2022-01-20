using CanalView;
using System.Collections.Generic;
using System.Linq;
using static PuzzleSolving.Musts;

namespace PuzzleSolving.Solvers
{
    public static class InferSolver
    {
        private class GuessInfo
        {
            public ((int x, int y) Spot, Cell Value) Guess;
            public bool Remove = false;
        }
        public static IEnumerable<Cell[,]> Solve(Cell[,] board)
        {
            board = board.Copy();
            var colors = new int[board.GetLength(0), board.GetLength(1)];
            var foundPreviously = false;
            var guesses = new Stack<GuessInfo>();
            while (true)
            {
                // try FillMusts
                var copy = board.Copy();
                var changes = guesses.TryPeek(out var guessInfo)
                    ? ApplyMustsRecursively(copy, new CellInfo { Position = guessInfo.Guess.Spot })
                    : ApplyMustsRecursively(copy);
                if (foundPreviously || changes == null)
                {
                    foundPreviously = false;
                    
                    if (guesses.Count == 0)
                        yield break;

                    // remove used guesses
                    while (guessInfo.Remove)
                    {
                        guesses.Pop();
                        if (guesses.Count == 0)
                            yield break;
                        guesses.TryPeek(out guessInfo);
                    }

                    // Clean
                    var color = guesses.Count;
                    foreach (var (x, y) in board.Points().Where(s => colors[s.x, s.y] >= color).ToArray())
                    {
                        board[x, y] = Cell.Unknown;
                        colors[x, y] = 0;
                    }

                    guessInfo.Remove = true;

                    // apply other value
                    var otherValue = guessInfo.Guess.Value switch
                    {
                        Cell.Full => Cell.Empty,
                        _ => Cell.Full
                    };
                    guessInfo.Guess = (guessInfo.Guess.Spot, otherValue);
                    board[guessInfo.Guess.Spot.x, guessInfo.Guess.Spot.y] = guessInfo.Guess.Value;
                    colors[guessInfo.Guess.Spot.x, guessInfo.Guess.Spot.y] = guesses.Count;

                }
                else
                {
                    // apply changes
                    foreach (var cell in changes)
                        if (colors[cell.Position.x, cell.Position.y] == 0)
                            colors[cell.Position.x, cell.Position.y] = guesses.Count;

                    board = copy;

                    // board is completed
                    var completed = board.Points().All(s => board[s.x, s.y] != Cell.Unknown);
                    if (completed)
                    {
                        foundPreviously = true;
                        yield return board.Copy();
                        continue;
                    }

                    // push and apply new guess
                    var newGuess = new GuessInfo { Guess = BestGuess(board) };
                    guesses.Push(newGuess);
                    board[newGuess.Guess.Spot.x, newGuess.Guess.Spot.y] = newGuess.Guess.Value;
                    colors[newGuess.Guess.Spot.x, newGuess.Guess.Spot.y] = guesses.Count;
                }
            }
        }
        public static ((int x, int y) Spot, Cell Value) BestGuess(Cell[,] board)
        {
            var first = board.Points().First(s => board[s.x, s.y] == Cell.Unknown);
            return (first, Cell.Full);
        }
    }
}
