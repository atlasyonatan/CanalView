using System;
using System.Diagnostics;
using CanalView;
using Puzzles;
using PuzzleSolving.Solvers;


namespace PuzzleSolving
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var board = BuiltIn.Hard_12x12;
            Console.WriteLine(board.Tostring());
            Console.WriteLine();

            var solutions = InferSolver.Solve(board);
            int i = 0;
            var sw = new Stopwatch();
            TimeSpan total = TimeSpan.Zero;

            sw.Start();
            foreach (var solution in solutions)
            {
                sw.Stop();
                total += sw.Elapsed;
                Console.WriteLine($"Solution #{++i} ({sw.Elapsed:c}):");
                Console.WriteLine(Board.Tostring(solution));
                sw.Restart();
            }
            sw.Stop();
            total += sw.Elapsed;

            Console.WriteLine();
            if (i == 0)
                Console.WriteLine($"No solutions! ({total:c})");
            else
                Console.WriteLine($"Total: found {i} solution{(i > 1 ? "s" : "")} ({total:c})");
            Console.ReadLine();
        }
    }
}
