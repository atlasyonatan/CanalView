using CanalView;
using PuzzleGenerator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PuzzleGeneration.PuzzleGenerators
{
    public class PathGenerator : IBoardGenerator
    {
        private readonly Cell[,] _baseBoard;
        private readonly Random _random;
        public double FillChance { get; init; } = 0.5d;
        public PathGenerator(int height, int width, Random random) : this(Board.Blank(height, width), random) { }
        public PathGenerator(Cell[,] baseBoard, Random random)
        {
            _baseBoard = baseBoard;
            _random = random;
        }

        public IEnumerable<Cell[,]> Generate()
        {
            while (true)
            {
                var board = _baseBoard.Copy();
                var unknowns = board.GetSpots().Where(s => board[s.X, s.Y] == Cell.Unkown).ToArray();
                var (x, y) = Generation.RandomItem(unknowns, _random);
                board[x, y] = Cell.Full;
                board.GenerateRandomPath(x, y, _random, FillChance);
                yield return board;
            }
        }
    }
}
