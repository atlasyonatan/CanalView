using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace CanalView.Solvers
{
    public class AlgorithmicSolver : ISolver
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
            private readonly Stack<((int X, int Y) Spot, int Color)> _guesses = new Stack<((int X, int Y) Spot, int Color)>();
            private readonly Cell[] _fillOptions = new Cell[] { Cell.Full, Cell.Empty };
            private int _currentColor = 0;

            public EnumeratorObject(Cell[,] board)
            {
                _board = board.Copy();
                _colors = new int[_board.GetLength(0), _board.GetLength(1)];
            }

            public bool MoveNext()
            {
                if (_currentColor == 0)
                {
                    if (!_board.FillMusts() || !_board.LegalSquare() || !_board.LegalNumbers())
                        return false;
                    (int X, int Y)? fullSpot = null;
                    var isCompleted = true;
                    foreach (var s in _board.GetSpots())
                    {
                        if (_board[s.X, s.Y] == Cell.Unkown)
                        {
                            isCompleted = false;
                            break;
                        }
                        if (_board[s.X, s.Y] == Cell.Full)
                        {
                            fullSpot = s;
                            break;
                        }
                    }
                    if (isCompleted)
                        return !fullSpot.HasValue || _board.LegalPath(fullSpot.Value.X + fullSpot.Value.Y * _board.GetLength(0));
                    _currentColor++;
                }

                //already filled with musts
                while (_currentColor >= 1)
                {
                    var legal = false;
                    if (_board.GetSpots().TryFirst(s => _board[s.X, s.Y] == Cell.Unkown, out var unknownSpot))
                        _guesses.Push((unknownSpot, _guesses.TryPeek(out var g) ? g.Color + 1 : 1));
                    else
                    {
                        //board is completed
                        legal = _board.Legal();
                        if (legal) Current = _board.Copy();
                    }

                    var (spot, color) = _guesses.Peek();
                    var cell = _board[spot.X, spot.Y];

                    //clean current+ color
                    CleanColor(color);

                    //find next guess index
                    var nextGuessValueIndex = cell == Cell.Unkown ? 0 : Array.IndexOf(_fillOptions, cell) + 1;

                    //check if on last guess
                    if (nextGuessValueIndex >= _fillOptions.Length)
                    {
                        //no more values for current guess
                        _guesses.Pop();
                        _currentColor--;
                        if (legal) return true;
                        continue;
                    }

                    //apply found guess
                    _board[spot.X, spot.Y] = _fillOptions[nextGuessValueIndex];
                    _colors[spot.X, spot.Y] = color;

                    //check if guess is legal and successful musts
                    var clone = _board.Copy();
                    if (clone.Legal(spot.X + spot.Y * _board.GetLength(0)) && clone.FillMusts(spot.X, spot.Y))
                    {
                        //changes are legal

                        //find changes
                        var changes = clone.GetSpots()
                            .Where(s => clone[s.X, s.Y] != _board[s.X, s.Y])
                            .ToArray();

                        if (changes.Any())
                        {
                            //record changes
                            _board = clone;

                            //mark changes with current color
                            foreach (var (X, Y) in changes)
                                _colors[X, Y] = color;
                        }
                    }
                    if (legal) return true;
                }
                return false;
            }

            private void CleanColor(int color)
            {
                foreach (var (X, Y) in _board.GetSpots().Where(s => _colors[s.X, s.Y] >= color).ToArray())
                {
                    _board[X, Y] = Cell.Unkown;
                    _colors[X, Y] = 0;
                }
            }

            public void Reset() => throw new NotSupportedException();
            public IEnumerator GetEnumerator() => this;
            public void Dispose() { }
            public Cell[,] Current { get; private set; } = null;
            object IEnumerator.Current => Current;
        }
    }
}
