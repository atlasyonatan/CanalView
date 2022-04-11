using CanalView;
using PuzzleGeneration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static PuzzleGeneration.RandomGeneration;

namespace PuzzleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var savedPuzzlesFolderPath = @"C:\CanalView Saves";


            var (width, height) = (10, 10);
            var weights = new (Cell, double)[] { (Cell.Full, 9), (Cell.Empty, 5) };
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
                Console.WriteLine($"Puzzle #{++i} (n. of solutions = {solutions.Length} ,{sw.Elapsed:c}):");
                if (solutions.Length == 0)
                {
                    Console.WriteLine("constructed:");
                    Console.WriteLine(board.Tostring());
                    Console.WriteLine();

                    Console.WriteLine("solver is bad if it can't find solutions to constructed valid puzzle");
                }
                else
                {
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

                var input = Console.ReadLine();
                //save board
                if (input == "s")
                {
                    var puzzleName = $"CanalView Generated Puzzle {i} {solutions.Length} {DateTime.Now:yyyyMMddHHmmssffff}";
                    SaveBoardVisualText(board, puzzleName, savedPuzzlesFolderPath);
                    Console.ReadLine();
                }
            }
        }

        //static void SaveBoardJson(Cell[,] board, string name, string folderPath)
        //{
        //    var boardData = KnownCells.FromBoard(board);
        //    var data = JsonSerializer.Serialize(boardData, new JsonSerializerOptions { IncludeFields = true });
        //    var fileExtension = ".json";
        //    var filePath = Path.Combine(folderPath, name + fileExtension);
        //    File.WriteAllText(filePath, data);
        //    Console.WriteLine($"Saved puzzle at {filePath}");
        //    Console.WriteLine();
        //}

        static void SaveBoardVisualText(Cell[,] board, string name, string folderPath)
        {
            var data = BoardSerialization.VisualSerializer.Serialize(board);
            var fileExtension = ".txt";
            var filePath = Path.Combine(folderPath, name + fileExtension);
            File.WriteAllText(filePath, data);
            Console.WriteLine($"Saved puzzle at {filePath}");
            Console.WriteLine();
        }
    }
}
