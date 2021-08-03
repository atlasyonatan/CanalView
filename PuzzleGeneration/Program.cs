using CanalView;
using System;
using PuzzleGeneration.PuzzleGenerators;

namespace PuzzleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = Board.Blank(20, 20);
            var r = new Random();
            IBoardGenerator generator = new PathGenerator(b, r) { FillChance = 0.6d};
            foreach (var board in generator.Generate())
            {
                Console.WriteLine(board.Tostring());
                Console.ReadLine();
            }
        }
    }
}
