using System;
using System.Linq;

namespace CanalView
{
    public static class Algorithms
    {
        public static bool FillMusts(this Cell[,] board) => board.GetSpots()
            .Where(s => board[s.X, s.Y] != Cell.Unkown && board[s.X, s.Y] >= Cell.Empty)
            .All(s => board.FillMusts(s.X, s.Y));

        public static bool FillMusts(this Cell[,] board, int x, int y) => !board.Contains(x, y) || (board[x, y] switch
        {
            Cell.Full => board.FillMusts_Full(x, y),
            Cell.Empty => board.FillMusts_Empty(x, y),
            _ => board[x, y] < 0 || board.FillMusts_Number(x, y)
        });

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
            int spotsArr = 0;
            for (var i = 0; i < 8; i++)
            {
                var d = ClockwiseDirections[i];
                var newX = x + d.X;
                var newY = y + d.Y;
                if (board.Contains(newX, newY) && board[newX, newY] == Cell.Full)
                    spotsArr |= 1 << i;
            }
            int mustsArr = 0;
            for (var i = 0; i < 4; i++)
            {
                var mask = 5 << (i * 2);
                if ((spotsArr & mask) == mask) mustsArr |= 2 << (i * 2);
                mask = 6 << (i * 2);
                if ((spotsArr & mask) == mask) mustsArr |= 1 << (i * 2);
                mask >>= 1;
                if ((spotsArr & mask) == mask) mustsArr |= 16 << (i * 2);
            }
            var musts = Enumerable.Range(0, 8)
                .Where(i => (mustsArr & 1 << i) > 0)
                .Select(i => ClockwiseDirections[i])
                .ToArray();
            if (musts.Any(m => board[m.X, m.Y] == Cell.Full)) return false;
            var avalibleMusts = musts.Where(m => board[m.X, m.Y] == Cell.Unkown)
                .ToArray();
            foreach (var m in avalibleMusts) board[m.X, m.Y] = Cell.Empty;
            return avalibleMusts.All(m => board.FillMusts_Empty(m.X, m.Y));
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

        public static readonly (int X, int Y)[] ClockwiseDirections = new (int, int)[] { (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1) };
    }
}
