using System;
using System.Diagnostics;
using CanalView.Solvers;

namespace CanalView
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var board = Boards.Medium_8x8;
            Console.WriteLine(board.Tostring());
            Console.WriteLine();
            var sw = new Stopwatch();
            sw.Start();
            var success = board.FillMusts();
            sw.Stop();
            Console.WriteLine($"{sw.Elapsed:c}");
            Console.WriteLine(success?board.Tostring():"unsuccessful :c");


            //var solutions = new GuessAndCheck().Solve(board);
            //int i = 0;
            //var sw = new Stopwatch();
            //TimeSpan total = TimeSpan.Zero;

            //sw.Start();
            //foreach (var solution in solutions)
            //{
            //    sw.Stop();
            //    total += sw.Elapsed;
            //    Console.WriteLine($"Solution #{++i} ({sw.Elapsed.ToString("c")}):");
            //    Console.WriteLine(Board.Tostring(solution));
            //    sw.Restart();
            //}
            //sw.Stop();
            //total += sw.Elapsed;

            //Console.WriteLine();
            //if(i == 0)
            //    Console.WriteLine($"No solutions! ({total:c})");
            //else
            //    Console.WriteLine($"Total: found {i} solution{(i > 1 ? "s" : "")} ({total:c})");
            Console.ReadLine();
        }
    }
}
