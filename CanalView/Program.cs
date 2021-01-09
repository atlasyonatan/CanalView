using System;
using System.Diagnostics;
using System.Linq;

namespace CanalView
{
    class Program
    {
        static void Main(string[] args)
        {
            var board = Boards.Easy;
            Printer.PrintBoard(board);
            Console.WriteLine();
            var sw = new Stopwatch();
            sw.Start();
            var solutions = Solver.Solve(board).ToArray();
            sw.Stop();
            Console.WriteLine(sw.Elapsed.ToString("c"));
            if (!solutions.Any()) Console.WriteLine("No solutions :c");
            foreach (var solution in solutions)
                Printer.PrintBoard(solution);
        }
    }
}
