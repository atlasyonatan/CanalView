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
            //private readonly Cell[] _fillOptions = new Cell[] { Cell.Full, Cell.Empty };
            private int _currentColor = 1;
            private readonly Stack<(int X, int Y)> _guesses = new Stack<(int, int)>();
            //private bool exit = false;

            public EnumeratorObject(Cell[,] board)
            {
                _board = board.Copy();
                _colors = new int[_board.GetLength(0), _board.GetLength(1)];
            }

            public bool MoveNext()
            {
                if (!_guesses.Any())
                {
                    //first time?

                    var success = _board.FillMusts();
                    if (!success)
                        return false;
                    var (guessSpot, guessValue) = _board.BestGuess();
                    _board[guessSpot.X, guessSpot.Y] = guessValue;
                    _colors[guessSpot.X, guessSpot.Y] = _currentColor;
                    _guesses.Push(guessSpot);
                }

                //first guess already inside board and stack
                while(_guesses.TryPeek(out var guessSpot))
                {
                    var copy = _board.Copy();
                    var success = copy.FillMusts(guessSpot.X, guessSpot.Y);
                    
                    if (success)
                    {
                        //debug for fillmusts logic:
                        var legal = copy.LegalNumbers() && copy.LegalSquare();
                        if (!legal)
                            throw new InvalidOperationException("board filled with musts but is illegal!");

                        //save changes to _board and mark them with _currentColor
                        foreach (var spot in copy.GetSpots())
                            if(copy[spot.X,spot.Y] != _board[spot.X, spot.Y])
                                _colors[spot.X, spot.Y] = _currentColor;
                        _board = copy;

                        //check if board is completed
                        var completed = _board.GetSpots().All(s => _board[s.X, s.Y] != Cell.Unkown);
                        if (completed)
                        {
                            //debug for fillmusts logic:
                            legal = _board.LegalPath();
                            if (!legal)
                                throw new InvalidOperationException("board is completed with musts but is no legal path!");


                            //check board is legal
                            if (_board.LegalPath())
                            {
                                Current = _board.Copy();
                                return true;
                            }
                                
                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        //must be other guessValue
                    }
                    
                    
                }
                return false;





                //if (!_guesses.Any())
                //{
                //    if (exit) return false;
                //    var legal = _board.FillMusts();
                //    if (!legal) return false;

                //    var hasUnknown = _board.GetSpots().TryFirst(s => _board[s.X, s.Y] == Cell.Unkown, out var unknownSpot);
                //    if (!hasUnknown)
                //    {
                //        //board is completed
                //        exit = true;
                //        return _board.Legal();
                //    }
                //    else
                //    {
                //        //board is not compeleted

                //        //push first guess
                //        var top = _board.TopGuessSpots();
                //        _guesses.Push((top[0].Spot, 0, 1, 0));
                //    }
                //}

                ////check if has guesses
                //while (_guesses.TryPop(out var guess))
                //{
                //    //already removed current guess from stack

                //    //check if there's another value to guess in the same spot
                //    var nextGuessValueIndex = guess.ValueIndex + 1;
                //    if (nextGuessValueIndex < _fillOptions.Length)
                //    {
                //        //put the spot back into the stack but with next value
                //        _guesses.Push((guess.Spot, nextGuessValueIndex, guess.Color, guess.Index));
                //    }
                    
                //    //current guess is unchecked

                //    //clean board from prev guess
                //    CleanColor(guess.Color);

                //    //check if current guess is legal and successful musts
                //    var clone = _board.Copy();
                //    var legal = clone.Legal(guess.Spot.X + guess.Spot.Y * _board.GetLength(0)) && clone.FillMusts(guess.Spot.X, guess.Spot.Y);
                //    if (legal)
                //    {
                //        //changes are legal

                //        //find changes
                //        var changes = clone.GetSpots()
                //            .Where(s => clone[s.X, s.Y] != _board[s.X, s.Y])
                //            .ToArray();

                //        if (changes.Any())
                //        {
                //            //record changes
                //            _board = clone;

                //            //mark changes with current color
                //            foreach (var (X, Y) in changes)
                //                _colors[X, Y] = guess.Color;
                //        }

                //        //current guess is legal

                //        //populate stack with next guess spot
                //        var top = _board.TopGuessSpots();
                //        var nextGuessSpotIndex = guess.Index + 1;
                //        if (nextGuessSpotIndex < top.Length)
                //        {
                //            //found next guess spot
                //            _guesses.Push(guess);
                //            _guesses.Push((top[nextGuessSpotIndex].Spot, 0, guess.Color, nextGuessSpotIndex));
                //        }
                //        else
                //        {
                //            //no more guess spots

                //            //board is completed and legal -> will return true

                //            //already handled: next guess spot is the same spot but next value

                //            return true;
                //        }
                //    }
                //}
                //return false;
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
