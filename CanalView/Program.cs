using System;
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
            var solutions = Solver.Solve(board).ToArray();
            if (!solutions.Any()) Console.WriteLine("No solutions :c");
            foreach (var solution in solutions)
                Printer.PrintBoard(solution);
        }
    }
}
