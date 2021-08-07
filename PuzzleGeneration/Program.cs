using CanalView;
using System;
using static PuzzleGeneration.RandomGeneration;

namespace PuzzleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var (width, height) = (8, 8);
            var r = new Random();
            while (true)
            {
                var board = Board.Blank(width, height);
                AddRandomValidPath(board, r, (Cell.Full, 5), (Cell.Empty, 1));
                Console.WriteLine(board.Tostring());
                Console.ReadLine();
            }
        }
    }
}
