using System;
using System.Diagnostics;
using CanalView.Solvers;

namespace CanalView
{
    class Program
    {
        static void Main(string[] args)
        {
            var board = Boards.Easy_5x5;
            Console.WriteLine(Board.ToString(board));
            Console.WriteLine();
            var solutions = new GuessAndCheck().Solve(board);
            int i = 0;
            var sw = new Stopwatch();
            TimeSpan total = TimeSpan.Zero;
            sw.Start();
            foreach (var solution in solutions)
            {
                sw.Stop();
                total += sw.Elapsed;
                Console.WriteLine($"Solution #{++i} ({sw.Elapsed.ToString("c")}):");
                Console.WriteLine(Board.ToString(solution));
                sw.Restart();
            }
            sw.Stop();
            total += sw.Elapsed;
            
            Console.WriteLine();
            if(i == 0)
                Console.WriteLine($"No solutions! ({total.ToString("c")})");
            else
                Console.WriteLine($"Total: found {i} solution{(i > 1 ? "s" : "")} ({total.ToString("c")})");
        }
    }
}
