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
            public ((int X, int Y) Spot, Cell Value) Guess;
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

                    // remove used guesses
                    if (guesses.Count == 0)
                        yield break;
                    else
                        while (guessInfo.Remove)
                        {
                            guesses.Pop();
                            if (guesses.Count == 0)
                                yield break;
                            guesses.TryPeek(out guessInfo);
                        }

                    // Clean
                    var color = guesses.Count;
                    foreach (var (X, Y) in board.GetSpots().Where(s => colors[s.X, s.Y] >= color).ToArray())
                    {
                        board[X, Y] = Cell.Unkown;
                        colors[X, Y] = 0;
                    }

                    guessInfo.Remove = true;

                    // apply other value
                    var otherValue = guessInfo.Guess.Value switch
                    {
                        Cell.Full => Cell.Empty,
                        _ => Cell.Full
                    };
                    guessInfo.Guess = (guessInfo.Guess.Spot, otherValue);
                    board[guessInfo.Guess.Spot.X, guessInfo.Guess.Spot.Y] = guessInfo.Guess.Value;
                    colors[guessInfo.Guess.Spot.X, guessInfo.Guess.Spot.Y] = guesses.Count;

                }
                else
                {
                    // apply changes
                    var changesArr = changes.ToArray();
                    //System.Console.WriteLine(copy.Tostring());
                    //System.Console.WriteLine();
                    foreach (var cell in changesArr)
                    {
                        if (colors[cell.Position.x, cell.Position.y] == 0)
                            colors[cell.Position.x, cell.Position.y] = guesses.Count;
                    }

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
                    var newGuess = new GuessInfo { Guess = BestGuess(board) };
                    guesses.Push(newGuess);
                    board[newGuess.Guess.Spot.X, newGuess.Guess.Spot.Y] = newGuess.Guess.Value;
                    colors[newGuess.Guess.Spot.X, newGuess.Guess.Spot.Y] = guesses.Count;
                    //guessInfo = newGuess;
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
