using CanalView;
using System;
using System.Diagnostics;
using System.Linq;
using static PuzzleGeneration.RandomGeneration;

namespace PuzzleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var (width, height) = (8, 8);
            var r = new Random();
            var sw = new Stopwatch();
            var i = 0;
            while (true)
            {
                var board = Board.Blank(width, height);
                sw.Restart();
                AddRandomValidPath(board, r, (Cell.Full, 3), (Cell.Empty, 1));
                var solutions = AddRandomNumbers(board, r, 1, PuzzleSolving.Solvers.InferSolver.Solve);
                sw.Stop();
                if (solutions.Length == 0)
                {
                    Console.WriteLine($"Puzzle #{++i} (n. of solutions = {solutions.Length} ,{sw.Elapsed:c}):");
                    Console.WriteLine("constructed:");
                    Console.WriteLine(board.Tostring());
                    Console.WriteLine();

                    //bug: solver is bad if it can't find solutions to constructed valid puzzle
                    Clean(board);
                    _ = PuzzleSolving.Solvers.InferSolver.Solve(board).ToArray();//todo: debug this
                }
                else
                {
                    Console.WriteLine($"Puzzle #{++i} (n. of solutions = {solutions.Length} ,{sw.Elapsed:c}):");
                    Clean(board);
                    Console.WriteLine(board.Tostring());
                    Console.WriteLine();
                    Console.WriteLine("Solutions:");
                    var j = 0;
                    foreach (var solution in solutions)
                    {
                        Console.WriteLine($"Solution #{++j}");
                        Console.WriteLine(solution.Tostring());
                        Console.WriteLine();
                    }
                }
                Console.ReadLine();
            }
        }
    }
}
