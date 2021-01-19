using System.Linq;
using static CanalView.Math;

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

        public static bool FillMusts_Full(this Cell[,] board, int x, int y) =>
            !board.Contains(x, y) ||
            board[x, y] != Cell.Full ||
            board.FillMusts_Full_LShape(x, y) &&
            board.FillMusts_Full_Surrounded(x, y) &&
            board.FillMusts_Full_ConnectNumbers(x, y);

        public static bool FillMusts_Full_LShape(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return true;
            int fullSpots = 0;
            for (var i = 0; i < 8; i++)
            {
                var d = ClockwiseDirections[i];
                var newX = x + d.X;
                var newY = y + d.Y;
                if (board.Contains(newX, newY) && board[newX, newY] == Cell.Full)
                    fullSpots |= 1 << i;
            }
            int mustsArr = 0;
            for (var i = 0; i < 8; i += 2)
            {
                var mask = 5 << i;
                if ((fullSpots & mask) == mask) mustsArr |= 2 << i;
                mask = 6 << i;
                if ((fullSpots & mask) == mask) mustsArr |= 1 << i;
                mask <<= 1;
                if ((fullSpots & mask) == mask) mustsArr |= 16 << i;
            }
            var musts = Enumerable.Range(0, 8)
                .Where(i => (mustsArr & 1 << i) > 0)
                .Select(i => (X: x + ClockwiseDirections[i].X, Y: y + ClockwiseDirections[i].Y))
                .ToArray();
            if (musts.Any(s => board[s.X, s.Y] == Cell.Full))
                return false;
            var avalible = musts.Where(s => board[s.X, s.Y] == Cell.Unkown)
                .ToArray();
            foreach (var s in avalible) board[s.X, s.Y] = Cell.Empty;
            return avalible.All(s => board.FillMusts_Empty(s.X, s.Y));
        }

        public static bool FillMusts_Full_Surrounded(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return true;
            var spot = (x, y);
            if (board.GetSpots().All(s => board[s.X, s.Y] != Cell.Full || s.Equals(spot)))
                return true;
            var neighbors = Cardinals.Select(d => (X: x + d.X, Y: y + d.Y))
                .Where(s => board.Contains(s.X, s.Y))
                .ToArray();
            if (neighbors.Any(s => board[s.X, s.Y] == Cell.Full))
                return true;
            var avalible = neighbors.Where(s => board[s.X, s.Y] == Cell.Unkown).ToArray();
            switch (avalible.Length)
            {
                case 0: return false;
                case 1:
                    board[avalible[0].X, avalible[0].Y] = Cell.Full;
                    return board.FillMusts_Full(avalible[0].X, avalible[0].Y);
                default: return true;
            }
        }

        public static bool FillMusts_Full_ConnectNumbers(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return true;
            for (var i = 0; i < Cardinals.Length; i++)
            {
                var scale = 1;
                while (true)
                {
                    var newX = x + Cardinals[i].X * scale;
                    var newY = y + Cardinals[i].Y * scale;
                    if (!board.Contains(newX, newY) || board[newX, newY] == Cell.Empty) break;
                    if (board[newX, newY] >= 0 && !board.FillMusts_Number(newX, newY))
                        return false;
                    scale++;
                }
            }
            return true;
        }

        public static bool FillMusts_Empty(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Empty)
                return true;
            for (var i = 0; i < Cardinals.Length; i++)
            {
                var scale = 1;
                while (true)
                {
                    var newX = x + Cardinals[i].X * scale;
                    var newY = y + Cardinals[i].Y * scale;
                    if (!board.Contains(newX, newY) || board[newX, newY] == Cell.Empty) break;
                    if (board[newX, newY] >= 0 && !board.FillMusts_Number(newX, newY))
                        return false;
                    scale++;
                }
            }
            return true;
        }

        public static bool FillMusts_Number(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] < 0)
                return true;
            var cellNumber = (int)board[x, y];
            var fullCount = new int[4];
            var distances = new int[4];
            for (var i = 0; i < 4; i++)
            {
                var scale = 1;
                var foundUnknown = false;
                while (true)
                {
                    var newX = x + Cardinals[i].X * scale;
                    var newY = y + Cardinals[i].Y * scale;
                    if (!board.Contains(newX, newY) || board[newX, newY] == Cell.Empty || board[newX, newY] >= 0)
                    {
                        distances[0] = scale - 1;
                        break;
                    }
                    if (foundUnknown || board[newX, newY] == Cell.Unkown)
                        foundUnknown = true;
                    else if (board[newX, newY] == Cell.Full)
                    {
                        fullCount[i] += 1;
                        if (fullCount[i] > cellNumber)
                            return false;
                    }
                    scale++;
                }
            }
            var totalFullCount = fullCount.Sum();
            if (totalFullCount == cellNumber)
                return true;
            var totalRemaining = distances.Sum() - totalFullCount;
            for (var i = 0; i < 4; i++)
            {
                var remaining = 2 * (distances[i] - fullCount[i]) - totalRemaining;
                if (remaining <= 0) continue;
                
                var offset = fullCount[i] + 1;
                var mustFulls = Enumerable.Range(offset + 1, offset + remaining)
                    .Select(scale => (X: x + Cardinals[i].X * scale, Y: y + Cardinals[i].Y * scale))
                    .ToArray();
                if (mustFulls.Any(s => board[s.X, s.Y] != Cell.Unkown))
                    return false;
                var mustEmptys = Enumerable.Range(0, 4)
                    .Select(j => (Index: j, Scale: j == i ? offset + remaining + 1 : fullCount[j] + 1))
                    .Select(t => (X: x + Cardinals[t.Index].X * t.Scale, Y: y + Cardinals[t.Index].X * t.Scale))
                    .Where(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Unkown)
                    .ToArray();
                foreach (var s in mustFulls)
                    board[s.X, s.Y] = Cell.Full;
                foreach (var s in mustEmptys)
                    board[s.X, s.Y] = Cell.Empty;
                return mustEmptys.All(s => board.FillMusts_Empty(s.X, s.Y)) &&
                    mustFulls.All(s => board.FillMusts_Full(s.X, s.Y));
            }
            return true;
        }

        public static readonly (int X, int Y)[] ClockwiseDirections = new (int, int)[] { (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1) };
    }
}
