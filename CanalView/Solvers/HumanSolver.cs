using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace CanalView.Solvers
{
    public class HumanSolver : ISolver
    {
        public IEnumerable<Cell[,]> Solve(Cell[,] board) => new EnumerableObject(board);

        private class EnumerableObject : IEnumerable<Cell[,]>
        {
            private readonly Cell[,] _board;
            public EnumerableObject(Cell[,] board) => _board = board.Copy();
            public IEnumerator<Cell[,]> GetEnumerator() => new EnumeratorObject(_board);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        private class EnumeratorObject : IEnumerator<Cell[,]>
        {
            private Cell[,] _board;
            private readonly int[,] _colors;
            private readonly Stack<((int X, int Y)Spot, Cell Value)> _guesses = new Stack<((int X, int Y) Spot, Cell Value)>();
            private bool _foundPreviously = false;

            public EnumeratorObject(Cell[,] board)
            {
                _board = board.Copy();
                _colors = new int[_board.GetLength(0), _board.GetLength(1)];
            }

            public bool MoveNext()
            {
                var hasGuess = _guesses.TryPeek(out var guess);
                while (true)
                {
                    // try FillMusts
                    var copy = _board.Copy();
                    var success = hasGuess ? copy.FillMusts(guess.Spot.X, guess.Spot.Y) : copy.FillMusts();
                    //var success = copy.FillMusts();
                    if (!_foundPreviously && success)
                    {
                        // apply changes
                        foreach (var spot in copy.GetSpots().Where(s=>copy[s.X,s.Y] != _board[s.X,s.Y]))
                            _colors[spot.X, spot.Y] = _guesses.Count();
                        _board = copy;

                        // board is completed
                        var completed = _board.GetSpots().All(s => _board[s.X, s.Y] != Cell.Unkown);
                        if (completed)
                        {
                            _foundPreviously = true;
                            Current = _board.Copy();
                            return true;
                        }

                        // push and apply new guess
                        var newGuess = BestGuess();
                        _guesses.Push(newGuess);
                        _board[newGuess.Spot.X, newGuess.Spot.Y] = newGuess.Value;
                        _colors[newGuess.Spot.X, newGuess.Spot.Y] = _guesses.Count();
                        guess = newGuess;
                    }
                    else
                    {
                        _foundPreviously = false;
                        
                        // TryPop
                        hasGuess = _guesses.TryPop(out guess);
                        if (!hasGuess) return false;
                        
                        // Clean
                        CleanColor(_guesses.Count() + 1);

                        // apply other value
                        var otherValue = guess.Value switch
                        {
                            Cell.Full => Cell.Empty,
                            _ => Cell.Full
                        };
                        _board[guess.Spot.X, guess.Spot.Y] = otherValue;
                        _colors[guess.Spot.X, guess.Spot.Y] = _guesses.Count();
                    }
                }
            }

            private void CleanColor(int color)
            {
                foreach (var (X, Y) in _board.GetSpots().Where(s => _colors[s.X, s.Y] >= color).ToArray())
                {
                    _board[X, Y] = Cell.Unkown;
                    _colors[X, Y] = 0;
                }
            }

            private ((int X, int Y) Spot, Cell Value) BestGuess()
            {
                var first = _board.GetSpots().First(s => _board[s.X, s.Y] == Cell.Unkown);
                return (first, Cell.Full);
            }

            public void Reset() => throw new NotSupportedException();
            public IEnumerator GetEnumerator() => this;
            public void Dispose() { }
            public Cell[,] Current { get; private set; } = null;
            object IEnumerator.Current => Current;
        }
    }
}
