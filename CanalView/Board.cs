﻿using System.Linq;
using static CanalView.Array2D;

namespace CanalView
{
    public static class Board
    {
        public static Cell[,] Blank(int width, int height)
        {
            var board = new Cell[width, height];
            Fill(board, Cell.Unknown);
            return board;
        }

        public static Cell[,] Add(this Cell[,] board, params (int x, int y, int cell)[] cells)
        {
            foreach (var (x, y, cell) in cells) board[x, y] = (Cell)cell;
            return board;
        }

        public static string Tostring(this Cell[,] board) => string.Join('\n',
            Enumerable.Range(0, board.GetLength(1)).Select(y => string.Join(' ',
                Enumerable.Range(0, board.GetLength(0)).Select(x => board[x, y].Tostring()))));

        public static string Tostring(this Cell cell) => cell switch
        {
            Cell.Empty => "∙",
            Cell.Full => "■",
            Cell.Unknown => "_",
            _ => ((int)cell).ToString()
        };
    }
}
