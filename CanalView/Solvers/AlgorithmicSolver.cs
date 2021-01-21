﻿using System;
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
            private readonly Stack<((int X, int Y) Spot, int ValueIndex, int Color, int Index)> _guesses = new Stack<((int, int), int, int, int)>();
            private readonly Cell[] _fillOptions = new Cell[] { Cell.Full, Cell.Empty };
            //private int _currentColor = 0;
            private bool exit = false;

            public EnumeratorObject(Cell[,] board)
            {
                _board = board.Copy();
                _colors = new int[_board.GetLength(0), _board.GetLength(1)];
            }

            public bool MoveNext()
            {
                if (!_guesses.Any())
                {
                    if (exit) return false;
                    var legal = _board.FillMusts();
                    if (!legal) return false;

                    var hasUnknown = _board.GetSpots().TryFirst(s => _board[s.X, s.Y] == Cell.Unkown, out var unknownSpot);
                    if (!hasUnknown)
                    {
                        //board is completed
                        exit = true;
                        return _board.Legal();
                    }
                    else
                    {
                        //board is not compeleted

                        //push first guess
                        var top = _board.TopGuessSpots();
                        _guesses.Push((top[0].Spot, 0, 1, 0));
                    }
                }

                //check if has guesses
                while (_guesses.TryPop(out var guess))
                {
                    //already removed current guess from stack

                    //check if there's another value to guess in the same spot
                    var nextGuessValueIndex = guess.ValueIndex + 1;
                    if (nextGuessValueIndex < _fillOptions.Length)
                    {
                        //put the spot back into the stack but with next value
                        _guesses.Push((guess.Spot, nextGuessValueIndex, guess.Color, guess.Index));
                    }
                    
                    //current guess is unchecked

                    //clean board from prev guess
                    CleanColor(guess.Color);

                    //check if current guess is legal and successful musts
                    var clone = _board.Copy();
                    var legal = clone.Legal(guess.Spot.X + guess.Spot.Y * _board.GetLength(0)) && clone.FillMusts(guess.Spot.X, guess.Spot.Y);
                    if (legal)
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
                                _colors[X, Y] = guess.Color;
                        }

                        //current guess is legal

                        //populate stack with next guess spot
                        var top = _board.TopGuessSpots();
                        var nextGuessSpotIndex = guess.Index + 1;
                        if (nextGuessSpotIndex < top.Length)
                        {
                            //found next guess spot
                            _guesses.Push(guess);
                            _guesses.Push((top[nextGuessSpotIndex].Spot, 0, guess.Color, nextGuessSpotIndex));
                        }
                        else
                        {
                            //no more guess spots

                            //board is completed and legal -> will return true

                            //already handled: next guess spot is the same spot but next value

                            return true;
                        }
                    }
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
