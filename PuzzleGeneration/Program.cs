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
            var (width, height) = (8, 8);
            var weights = new (Cell, double)[] { (Cell.Full, 7), (Cell.Empty, 3) };
            var maxSolutions = 1;
            var r = new Random();
            var sw = new Stopwatch();
            var i = 0;
            while (true)
            {
                var board = Board.Blank(width, height);
                var start = board.RandomPosition(r);
                board[start.x, start.y] = Cell.Full;
                sw.Restart();
                Generation.AddValidPath(board, start, _ => Randomize.WeightedRandomItem(weights, r).item, allowLoop: false);
                var counts = board.Points().GroupBy(p => board[p.x, p.y]).ToDictionary(g => g.Key, g => g.Count());
                if (counts[Cell.Full] < counts[Cell.Empty])
                    continue;
                var solutions = Generation.ApplyChangesUntilBelowMaxSolutions(board, maxSolutions, () => AddRandomNumber(board, r), PuzzleSolving.Solvers.InferSolver.Solve);
                sw.Stop();
                if (solutions.Length == 0)
                {
                    Console.WriteLine($"Puzzle #{++i} (n. of solutions = {solutions.Length} ,{sw.Elapsed:c}):");
                    Console.WriteLine("constructed:");
                    Console.WriteLine(board.Tostring());
                    Console.WriteLine();

                    throw new Exception("solver is bad if it can't find solutions to constructed valid puzzle");
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
