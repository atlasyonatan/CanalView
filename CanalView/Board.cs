using System;
using System.Collections.Generic;
using System.Linq;

namespace CanalView
{
    public static class Board
    {
        public static T[,] Fill<T>(this T[,] board, T cell)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    board[x, y] = cell;
            return board;
        }

        public static T[,] Add<T>(this T[,] board, params (int x, int y, T cell)[] cells)
        {
            foreach (var c in cells) board[c.x, c.y] = c.cell;
            return board;
        }

        public static Cell[,] Blank(int width, int height) => (new Cell[width, height]).Fill(Cell.Unkown);

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
