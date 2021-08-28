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
            var weights = new (Cell, double)[] { (Cell.Full, 7), (Cell.Empty, 4) };
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

                Generation.FillAllNumbers(board);
                var foundPuzzle = Generation.TryMutateUntilBeforeAboveMaxSolutions(board, maxSolutions, b => RemoveRandomNumber(b, r), PuzzleSolving.Solvers.InferSolver.Solve, out var puzzleInfo);
                //var foundPuzzle = Generation.TryMutateUntilBelowMaxSolutions(board, maxSolutions, b => AddRandomNumber(b, r), PuzzleSolving.Solvers.InferSolver.Solve, out var puzzleInfo);

                sw.Stop();
                if (!foundPuzzle)
                {
                    Console.WriteLine("couldn't mutate board");
                    Console.WriteLine(puzzleInfo?.Origin.Tostring());
                }
                if (puzzleInfo.Solutions.Length == 0)
                {
                    Console.WriteLine($"Puzzle #{++i} (n. of solutions = {puzzleInfo.Solutions.Length} ,{sw.Elapsed:c}):");
                    Console.WriteLine("constructed:");
                    Console.WriteLine(puzzleInfo.Origin.Tostring());
                    Console.WriteLine();

                    throw new Exception("solver is bad if it can't find solutions to constructed valid puzzle");
                }
                else
                {
                    Console.WriteLine($"Puzzle #{++i} (n. of solutions = {puzzleInfo.Solutions.Length} ,{sw.Elapsed:c}):");
                    Console.WriteLine(puzzleInfo.Puzzle.Tostring());
                    Console.WriteLine();
                    Console.WriteLine("Solutions:");
                    var j = 0;
                    foreach (var solution in puzzleInfo.Solutions)
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
