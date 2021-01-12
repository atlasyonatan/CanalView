using CanalView.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CanalView.Solvers
{
    public class OptimizedBruteForceSolver : ISolver
    {
        public IEnumerable<Cell[,]> Solve(Cell[,] board) => new OptimizedBruteForceSolverEnumerable(board);
    }

    public class OptimizedBruteForceSolverEnumerable : IEnumerable<Cell[,]>
    {
        private readonly Cell[,] _board;
        public OptimizedBruteForceSolverEnumerable(Cell[,] board) => _board = Board.Clone(board);
        public IEnumerator<Cell[,]> GetEnumerator() => new OptimizedBruteForceSolverEnumerator(_board);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class OptimizedBruteForceSolverEnumerator : IEnumerator<Cell[,]>
    {
        private readonly Cell[,] _board;
        private readonly int[] _unknownSpots;
        private readonly int _width;
        private readonly int _height;
        private readonly int _size;
        private readonly Cell[] _fillOptions = Rules.FillOptions.ToArray();
        private int _spotIndex = 0;

        public OptimizedBruteForceSolverEnumerator(Cell[,] board)
        {
            _board = Board.Clone(board);
            _width = _board.GetLength(0);
            _height = _board.GetLength(1);
            _size = _width * _height;
            _unknownSpots = Enumerable.Range(0, _size)
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
                if (!_board.LegalSquare() || !_board.LegalNumbers()) continue;
                if (_spotIndex == _unknownSpots.Length - 1)
                {
                    if (_board.LegalPath()) Current = Board.Clone(_board);
                    return true;
                }
                _spotIndex++;
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
