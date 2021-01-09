using System;

namespace CanalView
{
    public static class Printer
    {
        public static void PrintBoard(Cell[,] board)
        {
            Console.WriteLine();
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cell = board[x, y];
                    Console.Write(cell switch
                    {
                        Cell.Empty => "O",
                        Cell.Full => "X",
                        Cell.Unkown => "■",
                        _ => (int)cell
                    });
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}
