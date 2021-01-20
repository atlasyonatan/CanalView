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
            public EnumerableObject(Cell[,] board) => _board = (Cell[,])board.Clone();
            public IEnumerator<Cell[,]> GetEnumerator() => new EnumeratorObject(_board);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        private class EnumeratorObject : IEnumerator<Cell[,]>
        {
            private readonly int _width;
            private readonly int _height;
            private readonly int _size;
            private readonly Cell[,] _board;
            private readonly int[,] _colors;
            private readonly Cell[] _fillOptions = Rules.FillOptions.ToArray();
            private int _index = 0;
            private int _currentColor = 0;
            private (int X, int Y) _currentGuess;

            public EnumeratorObject(Cell[,] board)
            {
                _width = _board.GetLength(0);
                _height = _board.GetLength(1);
                _size = _width * _height;
                _board = board.Copy();
                _colors = new int[_width, _height];
            }

            public bool MoveNext()
            {
                if (_currentColor == 0)
                {
                    if (!_board.FillMusts())
                        return false;
                    _currentColor++;
                }
                while (_currentColor > 0)
                {
                    if (!_board.GetSpots().TryFirst(s => _board[s.X, s.Y] == Cell.Unkown, out var unknownSpot))
                    {
                        Current = _board.Copy();

                        // remove current color
                        var clean = _board.GetSpots()
                            .Where(s => _colors[s.X, s.Y] == _currentColor && !s.Equals(_currentGuess));
                        foreach (var s in clean)
                        {
                            _board[s.X, s.Y] = Cell.Unkown;
                            _colors[s.X, s.Y] = 0;
                        }
                        return true;
                    }
                    var cell = _board[unknownSpot.X, unknownSpot.Y];
                    var nextGuessIndex = cell == Cell.Unkown ? 0 : Array.IndexOf(_fillOptions, cell) + 1;
                    if (nextGuessIndex >= _fillOptions.Length) // no more guesses
                    {
                        //clean current color


                    }
                    //    var nextGuess = _fillOptions
                    //    if (nextGuessIndex >= _fillOptions.Length)
                    //    {
                    //        _board[x, y] = Cell.Unkown;
                    //        _spotIndex--;
                    //        continue;
                    //    }
                    //    _board[unknownSpot.X,un]


                    //    var spot = _unknownSpots[_spotIndex];
                    //    var x = spot % _width;
                    //    var y = spot / _width;
                    //    var cell = _board[x, y];

                    //    if (nextGuessIndex >= _fillOptions.Length)
                    //    {
                    //        _board[x, y] = Cell.Unkown;
                    //        _spotIndex--;
                    //        continue;
                    //    }
                    //    _board[x, y] = _fillOptions[nextGuessIndex];
                    //    if (_board.LegalSquare(spot) && _board.LegalNumbers(spot))
                    //        if (_spotIndex < _unknownSpots.Length - 1)
                    //            _spotIndex++;
                    //        else if ((_board[x, y] == Cell.Full && _board.LegalPath(spot)) || (_board[x, y] != Cell.Full && _board.LegalPath()))
                    //        {
                    //            Current = (Cell[,])_board.Clone();
                    //            return true;
                    //        }
                    //}
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
