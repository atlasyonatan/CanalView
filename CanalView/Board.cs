using System.Linq;

namespace CanalView
{
    public static class Board
    {
        public static Cell[,] Blank(int width, int height) => new Cell[width, height].Fill(Cell.Unkown);

        public static string Tostring(this Cell[,] board) => string.Join('\n',
            Enumerable.Range(0, board.GetLength(1)).Select(y => string.Join(' ',
                Enumerable.Range(0, board.GetLength(0)).Select(x => board[x, y].Tostring()))));

        public static string Tostring(this Cell cell) => cell switch
        {
            Cell.Empty => "∙",
            Cell.Full => "■",
            Cell.Unkown => "_",
            _ => ((int)cell).ToString()
        };
    }
}
