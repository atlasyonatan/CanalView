using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CanalView.Solvers
{
    public class GuessAndCheck : ISolver
    {
        public IEnumerable<Cell[,]> Solve(Cell[,] board) => new EnumerableObject(board);
        private class EnumerableObject : IEnumerable<Cell[,]>
        {
            private readonly Cell[,] _board;
            public EnumerableObject(Cell[,] board) => _board = (Cell[,])board.Clone();
            public IEnumerator<Cell[,]> GetEnumerator() => new EnumeratorObject(_board);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        private class EnumeratorObject : IEnumerator<Cell[,]>
        {
            private readonly Cell[,] _board;
            private readonly (int X, int Y)[] _unknowns;
            private readonly Cell[] _fillOptions = new Cell[] { Cell.Full, Cell.Empty };
            private int _index = 0;

            public EnumeratorObject(Cell[,] board)
            {
                _board = board.Copy();
                _unknowns = _board.GetSpots()
                    .Where(s => _board[s.X, s.Y] == Cell.Unkown)
                    .ToArray();
            }

            public bool MoveNext()
            {
                while (_index >= 0)
                {
                    var (X, Y) = _unknowns[_index];
                    var cell = _board[X, Y];
                    int nextGuessIndex = cell == Cell.Unkown ? 0 : Array.IndexOf(_fillOptions, cell) + 1;
                    if (nextGuessIndex >= _fillOptions.Length)
                    {
                        _board[X, Y] = Cell.Unkown;
                        _index--;
                        continue;
                    }
                    _board[X, Y] = _fillOptions[nextGuessIndex];
                    if (_board.LegalSquare(X, Y) && _board.LegalNumbers(X, Y))
                        if (_index < _unknowns.Length - 1)
                            _index++;
                        else if ((_board[X, Y] == Cell.Full && _board.LegalPath(X, Y)) || (_board[X, Y] != Cell.Full && _board.LegalPath()))
                        {
                            Current = _board.Copy();
                            return true;
                        }
                }
                return false;
            }

            public void Reset() => throw new NotSupportedException();
            public IEnumerator GetEnumerator() => this;
            public void Dispose() { }
            public Cell[,] Current { get; private set; } = null;
            object IEnumerator.Current => Current;
        }
    }
}
