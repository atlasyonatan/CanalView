using CanalView;
using PuzzleGeneration;
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
            var (width, height) = (5, 5);
            var weights = new (Cell, double)[] { (Cell.Full, 2), (Cell.Empty, 1) };
            var maxSolutions = 3;
            var r = new Random();
            var sw = new Stopwatch();
            var i = 0;
            while (true)
            {
                var board = Board.Blank(width, height);
                var start = board.RandomPosition(r);
                sw.Restart();
                Generation.AddValidPath(board, start, _ => Randomize.WeightedRandomItem(weights, r).item);
                var solutions = Generation.ApplyChangesUntilBelowMaxSolutions(board, maxSolutions, () => AddRandomNumber(board, r), PuzzleSolving.Solvers.InferSolver.Solve);
                sw.Stop();
                if (solutions.Length == 0)
                {
                    Console.WriteLine($"Puzzle #{++i} (n. of solutions = {solutions.Length} ,{sw.Elapsed:c}):");
                    Console.WriteLine("constructed:");
                    Console.WriteLine(board.Tostring());
                    Console.WriteLine();

                    //bug: solver is bad if it can't find solutions to constructed valid puzzle
                    Generation.Clean(board);
                    while(true)
                    {
                        var x = PuzzleSolving.Solvers.InferSolver.Solve(board).ToArray();//todo: debug this
                    }
                    
                }
                else
                {
                    Console.WriteLine($"Puzzle #{++i} (n. of solutions = {solutions.Length} ,{sw.Elapsed:c}):");
                    Generation.Clean(board);
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
