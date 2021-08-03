using CanalView;
using PuzzleGenerator;
using System;
using System.Collections.Generic;

namespace PuzzleGeneration.PuzzleGenerators
{
    public class PathGenerator : IBoardGenerator
    {
        private readonly Random _random;
        public int Height { get; }
        public int Width { get; }
        public double FillChance { get; set; } = 0.5d;
        public PathGenerator(int height, int width, Random random)
        {
            Height = height;
            Width = width;
            _random = random;
        }

        public IEnumerable<Cell[,]> Generate()
        {
            var board = Board.Blank(Height, Width);
            var (x, y) = board.RandomPosition(_random);
            board[x, y] = Cell.Full;

            throw new NotImplementedException();

        }
    }
}
