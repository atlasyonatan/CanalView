using System;
using System.Linq;
using static CanalView.Math;
using static System.Math;

namespace CanalView
{
    public static class Algorithms
    {
        public static bool FillMusts(this Cell[,] board) => board.GetSpots()
            .Where(s => board[s.X, s.Y] != Cell.Unkown && board[s.X, s.Y] >= Cell.Empty)
            .Any(s => board.FillMusts(s.X, s.Y));

        public static bool FillMusts(this Cell[,] board, int x, int y) => board.Contains(x, y) && (board[x, y]) switch
        {
            Cell.Full => board.FillMusts_Full(x, y),
            Cell.Empty => board.FillMusts_Empty(x, y),
            _ => board[x, y] < 0 || board.FillMusts_Number(x, y)
        };

        #region Full

        public static bool FillMusts_Full(this Cell[,] board, int x, int y) => 
            !board.Contains(x, y) || 
            board[x, y] != Cell.Full || 
            board.FillMusts_Full_LShape(x, y) &&
            board.FillMusts_Full_Surrounded(x, y) &&
            board.FillMusts_Full_ConnectNumbers(x, y);


        public static bool FillMusts_Full_LShape(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full) return true;
            var validDirections = Cardinals.Concat(Diagonals)
                .Where(d => board.Contains(x + d.X, y + d.Y))
                .ToArray();
            var cartesian =
                from d1 in validDirections
                from d2 in validDirections
                select (d1, d2);
            var neighbors = cartesian.Where(p => (Abs(p.d1.X - p.d2.X) == 1) ^ (Abs(p.d1.Y - p.d2.Y) == 1));
            var LshapePairs = neighbors.Where(p =>
                board[x + p.d1.X, y + p.d1.Y] == Cell.Full &&
                board[x + p.d2.X, y + p.d2.Y] == Cell.Full);
            var musts = LshapePairs.Select(p => (
                X: x == p.d1.X ? p.d2.X : p.d1.X,
                Y: y == p.d1.Y ? p.d2.Y : p.d1.Y))
                .ToArray();
            if (musts.Any(m => board[m.X, m.Y] == Cell.Full))
                return false;
            var unknownMusts = musts.Where(m => board[m.X, m.Y] == Cell.Unkown)
                .ToArray();
            foreach (var m in unknownMusts)
                board[m.X, m.Y] = Cell.Empty;
            return unknownMusts.All(m => board.FillMusts_Empty(m.X, m.Y));
        }

        public static bool FillMusts_Full_Surrounded(this Cell[,] board, int x, int y)
        {
            throw new NotImplementedException();
        }

        public static bool FillMusts_Full_ConnectNumbers(this Cell[,] board, int x, int y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Empty

        public static bool FillMusts_Empty(this Cell[,] board, int x, int y)
        {
            return board.FillMusts_Empty_BlockNumbers(x, y);
        }

        public static bool FillMusts_Empty_BlockNumbers(this Cell[,] board, int x, int y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Number

        public static bool FillMusts_Number(this Cell[,] board, int x, int y)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
