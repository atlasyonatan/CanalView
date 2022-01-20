using CanalView;
using System.Linq;

namespace BoardData
{
    public class KnownCells
    {
        public (int Width, int Height) Size;
        public (int X, int Y, int Cell)[] Cells;
        public static KnownCells FromBoard(Cell[,] board) => new()
        {
            Cells = board.Points()
                .Where(s => board[s.x, s.y] != Cell.Unkown)
                .Select(s => (s.x, s.y, (int)board[s.x, s.y]))
                .ToArray(),
            Size = (board.GetLength(0), board.GetLength(1))
        };
        public static Cell[,] ToBoard(KnownCells knownCells) => 
            Board.Blank(knownCells.Size.Width, knownCells.Size.Height).Add(knownCells.Cells);
    }
}
