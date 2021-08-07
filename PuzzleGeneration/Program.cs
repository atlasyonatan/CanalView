using CanalView;
using System;
using System.Diagnostics;
using static PuzzleGeneration.RandomGeneration;

namespace PuzzleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var (width, height) = (10, 10);
            var r = new Random();
            var sw = new Stopwatch();
            var i = 0;
            while (true)
            {
                var board = Board.Blank(width, height);
                sw.Restart();
                AddRandomValidPath(board, r, (Cell.Full, 3), (Cell.Empty, 1));
                sw.Stop();
                Console.WriteLine($"Puzzle #{++i} ({sw.Elapsed:c}):");
                Console.WriteLine(board.Tostring());
                Console.ReadLine();
            }
        }
    }
}
