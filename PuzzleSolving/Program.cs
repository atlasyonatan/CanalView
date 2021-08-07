using System;
using System.Diagnostics;
using CanalView;
using PuzzleSolving.Solvers;

namespace PuzzleSolving
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var board = Puzzles.Medium_8x8; //todo: fix bug: this finds multiple wrong solutions on Medium10x10 and Hard12x12
            Console.WriteLine(board.Tostring());
            Console.WriteLine();

            //var sw = new Stopwatch();
            //sw.Start();
            //var success = board.FillMusts();
            //sw.Stop();
            //Console.WriteLine($"{sw.Elapsed:c}");
            //Console.WriteLine(success ? board.Tostring() : "unsuccessful :c");

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
