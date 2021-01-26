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
            private readonly int[] _unknownSpots;
            private readonly int _width;
            private readonly int _height;
            private readonly Cell[] _fillOptions = new Cell[] { Cell.Full, Cell.Empty };
            private int _spotIndex = 0;

            public EnumeratorObject(Cell[,] board)
            {
                _board = board.Copy();
                _width = _board.GetLength(0);
                _height = _board.GetLength(1);
                _unknownSpots = Enumerable.Range(0, _width * _height)
                    .Where(spot => _board[spot % _width, spot / _width] == Cell.Unkown)
                    .ToArray();
            }

            public bool MoveNext()
            {
                while (_spotIndex >= 0)
                {
                    var spot = _unknownSpots[_spotIndex];
                    var x = spot % _width;
                    var y = spot / _width;
                    var cell = _board[x, y];
                    int nextGuessIndex = cell == Cell.Unkown ? 0 : Array.IndexOf(_fillOptions, cell) + 1;
                    if (nextGuessIndex >= _fillOptions.Length)
                    {
                        _board[x, y] = Cell.Unkown;
                        _spotIndex--;
                        continue;
                    }
                    _board[x, y] = _fillOptions[nextGuessIndex];
                    if (_board.LegalSquare(spot) && _board.LegalNumbers(spot))
                        if (_spotIndex < _unknownSpots.Length - 1)
                            _spotIndex++;
                        else if ((_board[x, y] == Cell.Full && _board.LegalPath(spot)) || (_board[x, y] != Cell.Full && _board.LegalPath()))
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
