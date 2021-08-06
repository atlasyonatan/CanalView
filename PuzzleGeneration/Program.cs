using CanalView;
using System;
using static PuzzleGeneration.Generators;

namespace PuzzleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = Board.Blank(5, 5);
            var r = new Random();
            var puzzles = RandomPathGenerator(b, r, 0.6);
            foreach (var board in puzzles)
            {
                Console.WriteLine(board.Tostring());
                Console.ReadLine();
            }
        }
    }
}
