using System;
using System.Collections.Generic;
using System.Linq;

namespace CanalView
{
    public static class Board
    {
        public static Cell[,] FromNumbers(int width, int height, IEnumerable<(int x, int y, Cell cell)> cells)
        {
            var numberArray = cells.ToArray();
            if (numberArray.Any(n => 
                n.x < 0 || 
                n.x >= width || 
                n.y < 0 || 
                n.y >= height)) throw new ArgumentOutOfRangeException();
            var board = new Cell[width, height];
            var size = width * height;
            for (var i = 0; i < size; i++)
            {
                var x = i % width;
                var y = i / width;
                board[x, y] = Cell.Unkown;
            }
            foreach (var (x, y, cell) in numberArray)
                board[x, y] = cell;
            return board;
        }

        public static Cell[,] Clone(Cell[,] board) => (Cell[,])board.Clone();

        public static string ToString(Cell[,] board) => string.Join('\n',
            Enumerable.Range(0, board.GetLength(1)).Select(y => string.Join(' ',
                Enumerable.Range(0, board.GetLength(0)).Select(x => ToString(board[x, y])))));

        public static string ToString(this Cell cell) => cell switch
        {
            Cell.Empty => "∙",
            Cell.Full => "■",
            Cell.Unkown => "_",
            _ => ((int)cell).ToString()
        };
    }
}
