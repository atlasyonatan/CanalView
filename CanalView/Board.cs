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

        public static bool Contains<T>(this T[,] arr, int x, int y) =>
            x >= 0 &&
            x < arr.GetLength(0) &&
            y >= 0 &&
            y < arr.GetLength(1);

        public static IEnumerable<(int X, int Y)> GetSpots<T>(this T[,] arr) => Enumerable.Range(0, arr.GetLength(0) * arr.GetLength(1))
            .Select(i => (X: i % arr.GetLength(0), Y: i / arr.GetLength(0)));
    }
}
