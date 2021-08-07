using CanalView;
using System;
using static PuzzleGeneration.Generators;

namespace PuzzleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var dimensions = (8, 8);
            var r = new Random();
            var puzzles = PathGenerator(dimensions, r, 0.6);
            foreach (var board in puzzles)
            {
                Console.WriteLine(board.Tostring());
                Console.ReadLine();
            }
        }
    }
}
