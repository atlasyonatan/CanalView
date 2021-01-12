using CanalView.Solvers;
using System;
using System.Diagnostics;
using System.Linq;

namespace CanalView
{
    class Program
    {
        static void Main(string[] args)
        {
            var board = Boards.Easy_5x5;
            Printer.PrintBoard(board);
            Console.WriteLine();
            var sw = new Stopwatch();
            sw.Start();
            var solutions = new OptimizedBruteForceSolver().Solve(board).ToArray();
            sw.Stop();
            Console.WriteLine(sw.Elapsed.ToString("c"));
            if (!solutions.Any()) Console.WriteLine("No solutions :c");
            Console.WriteLine($"Found {solutions.Length} solution{(solutions.Length > 1 ? "s": "")}:");
            foreach (var solution in solutions)
            {
                Printer.PrintBoard(solution);
                Console.WriteLine();
            }
        }
    }
}
