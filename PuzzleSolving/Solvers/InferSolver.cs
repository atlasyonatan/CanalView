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
            public bool ToRemove = false;
        }
        public static IEnumerable<Cell[,]> Solve(Cell[,] board)
        {
            // setup
            board = board.Copy();
            var colors = new int[board.GetLength(0), board.GetLength(1)];
            var guesses = new Stack<GuessInfo>();
            IEnumerable<CellInfo> musts = null;
            GuessInfo currentGuess = null;
        #region start
        start:
            var copy = board.Copy();
            musts = guesses.TryPeek(out currentGuess)
                    ? ApplyMustsRecursively(copy, new CellInfo { Position = currentGuess.Guess.Spot })
                    : ApplyMustsRecursively(copy);
            if (musts == null)
                goto backwards;
            
            //find changes
            var changes = musts.Where(c => colors[c.Position.x, c.Position.y] == 0).ToArray();
            if (changes.Length == 0)
                goto forwards;

            // color changes
            foreach (var c in musts)
                colors[c.Position.x, c.Position.y] = guesses.Count;

            // apply changes
            board = copy;

            goto forwards;
        #endregion
        #region backwards
        backwards:
            // remove used guesses
            if (guesses.Count == 0)
                yield break;
            else
                while (currentGuess.ToRemove)
                {
                    guesses.Pop();
                    if (guesses.Count == 0)
                        yield break;
                    guesses.TryPeek(out currentGuess);
                }

            // Clean
            var color = guesses.Count;
            foreach (var (X, Y) in board.GetSpots().Where(s => colors[s.X, s.Y] >= color).ToArray())
            {
                board[X, Y] = Cell.Unkown;
                colors[X, Y] = 0;
            }

            currentGuess.ToRemove = true;

            // apply other value
            var otherValue = currentGuess.Guess.Value switch
            {
                Cell.Full => Cell.Empty,
                _ => Cell.Full
            };
            currentGuess.Guess = (currentGuess.Guess.Spot, otherValue);
            board[currentGuess.Guess.Spot.X, currentGuess.Guess.Spot.Y] = currentGuess.Guess.Value;
            colors[currentGuess.Guess.Spot.X, currentGuess.Guess.Spot.Y] = guesses.Count;
            goto start;
        #endregion
        #region forwards
        forwards:
            var completed = board.GetSpots().All(s => board[s.X, s.Y] != Cell.Unkown);
            if (completed)
            {
                yield return board.Copy();

                goto backwards;
            }

            // push and apply new guess
            var newGuessInfo = new GuessInfo { Guess = BestGuess(board) };
            guesses.Push(newGuessInfo);
            board[newGuessInfo.Guess.Spot.X, newGuessInfo.Guess.Spot.Y] = newGuessInfo.Guess.Value;
            colors[newGuessInfo.Guess.Spot.X, newGuessInfo.Guess.Spot.Y] = guesses.Count;
            goto start;
            #endregion
            /*
            ////////////////////////////
            //board = copy;

            //// board is completed
            //var completed = board.GetSpots().All(s => board[s.X, s.Y] != Cell.Unkown);
            //if (completed)
            //{
            //    foundPreviously = true;
            //    yield return board.Copy();
            //    //hasGuess = guesses.TryPeek(out guess);
            //    continue;
            //}

            //while (true)
            //{
            //    // try FillMusts
            //    var copy = board.Copy();
            //    var changes = guesses.TryPeek(out var guessInfo)
            //        ? ApplyMustsRecursively(copy, new CellInfo { Position = guessInfo.Guess.Spot })
            //        : ApplyMustsRecursively(copy);
            //    if (foundPreviously || changes == null)
            //    {
            //        foundPreviously = false;

            //        // remove used guesses
            //        if (guesses.Count == 0)
            //            yield break;
            //        else
            //            while (currentGuess.Remove)
            //            {
            //                guesses.Pop();
            //                if (guesses.Count == 0)
            //                    yield break;
            //                guesses.TryPeek(out currentGuess);
            //            }

            //        // Clean
            //        var color = guesses.Count;
            //        foreach (var (X, Y) in board.GetSpots().Where(s => colors[s.X, s.Y] >= color).ToArray())
            //        {
            //            board[X, Y] = Cell.Unkown;
            //            colors[X, Y] = 0;
            //        }

            //        guessInfo.Remove = true;

            //        // apply other value
            //        var otherValue = guessInfo.Guess.Value switch
            //        {
            //            Cell.Full => Cell.Empty,
            //            _ => Cell.Full
            //        };
            //        guessInfo.Guess = (guessInfo.Guess.Spot, otherValue);
            //        board[guessInfo.Guess.Spot.X, guessInfo.Guess.Spot.Y] = guessInfo.Guess.Value;
            //        colors[guessInfo.Guess.Spot.X, guessInfo.Guess.Spot.Y] = guesses.Count;

            //    }
            //    else
            //    {
            //        // apply changes
            //        foreach (var cell in changes)
            //        {
            //            if (colors[cell.Position.x, cell.Position.y] == 0)
            //                colors[cell.Position.x, cell.Position.y] = guesses.Count;
            //        }

            //        board = copy;

            //        // board is completed
            //        var completed = board.GetSpots().All(s => board[s.X, s.Y] != Cell.Unkown);
            //        if (completed)
            //        {
            //            foundPreviously = true;
            //            yield return board.Copy();
            //            //hasGuess = guesses.TryPeek(out guess);
            //            continue;
            //        }

            //        // push and apply new guess
            //        var newGuess = new GuessInfo { Guess = BestGuess(board) };
            //        guesses.Push(newGuess);
            //        board[newGuess.Guess.Spot.X, newGuess.Guess.Spot.Y] = newGuess.Guess.Value;
            //        colors[newGuess.Guess.Spot.X, newGuess.Guess.Spot.Y] = guesses.Count;
            //        //guessInfo = newGuess;
            //    }
            //}
            */
        }

        public static ((int X, int Y) Spot, Cell Value) BestGuess(Cell[,] board)
        {
            var first = board.GetSpots().First(s => board[s.X, s.Y] == Cell.Unkown);
            return (first, Cell.Full);
        }
    }
}
