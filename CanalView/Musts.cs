using System.Collections.Generic;
using System.Linq;
using static CanalView.Math;

namespace CanalView
{
    public static class Musts
    {
        public const Cell CHUNK_COLOR = (Cell)int.MinValue;

        public static bool FillMusts(this Cell[,] board) => board.GetSpots()
            .Where(s => board[s.X, s.Y] != Cell.Unkown)
            .All(s => board.FillMusts(s.X, s.Y, null));

        public static bool FillMusts(this Cell[,] board, int x, int y, (int X, int Y)? cause = null) => board[x, y] switch
        {
            Cell.Full => board.FillMusts_Full(x, y, cause),
            Cell.Empty => board.FillMusts_Empty(x, y, cause),
            _ => board.FillMusts_Number(x, y)
        };

        #region Full

        public static bool FillMusts_Full(this Cell[,] board, int x, int y, (int X, int Y)? cause = null) =>
            board.FillMusts_Full_LShape(x, y) &&
            board.FillMusts_Full_ConnectNumbers(x, y, cause) &&
            board.FillMusts_Full_Chunk(x, y);

        public static bool FillMusts_Full_LShape(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return true;
            int fullSpots = 0;
            for (var i = 0; i < 8; i++)
            {
                var (dx, dy) = ClockwiseDirections[i];
                var newX = x + dx;
                var newY = y + dy;
                if (board.Contains(newX, newY) && board[newX, newY] == Cell.Full)
                    fullSpots |= 1 << i;
            }
            int mustsArr = 0;
            for (var i = 0; i < 8; i += 2)
            {
                var mask = 5 << i;
                if ((fullSpots & mask) == mask) mustsArr |= (2 << i) % 255;
                mask = 6 << i;
                if ((fullSpots & mask) == mask) mustsArr |= (1 << i) % 255;
                mask <<= 1;
                if ((fullSpots & mask) == mask) mustsArr |= (16 << i) % 255;
            }
            var musts = Enumerable.Range(0, 8)
                .Where(i => (mustsArr & 1 << i) > 0)
                .Select(i => (X: x + ClockwiseDirections[i].X, Y: y + ClockwiseDirections[i].Y))
                .ToArray();
            if (musts.Any(s => board[s.X, s.Y] == Cell.Full))
                return false;
            var avalible = musts.Where(s => board[s.X, s.Y] == Cell.Unkown)
                .ToArray();
            foreach (var (X, Y) in avalible) board[X, Y] = Cell.Empty;
            return avalible.All(s => board.FillMusts_Empty(s.X, s.Y));
        }

        public static bool FillMusts_Full_Chunk(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return true;
            var clone = board.Copy();
            clone.FloodFill(x, y, CHUNK_COLOR);
            if (clone.GetSpots().All(s => clone[s.X, s.Y] != Cell.Full))
                return true;

            var chunkExits = clone.GetSpots()
                .Where(s => clone[s.X, s.Y] == CHUNK_COLOR).SelectMany(s =>
                    Cardinals.Select(d => (X: s.X + d.X, Y: s.Y + d.Y)).Where(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Unkown))
                .Distinct()
                .ToArray();

            switch (chunkExits.Length)
            {
                case 0: return false;
                case 1:
                    board[chunkExits[0].X, chunkExits[0].Y] = Cell.Full;
                    return board.FillMusts_Full(chunkExits[0].X, chunkExits[0].Y);
                default: return true;
            }
        }

        public static bool FillMusts_Full_ConnectNumbers(this Cell[,] board, int x, int y, (int X, int Y)? cause = null) =>
            board[x, y] != Cell.Full || board.FillMusts_ConnectNumbers(x, y, cause);

        #endregion

        #region Empty

        public static bool FillMusts_Empty(this Cell[,] board, int x, int y, (int X, int Y)? cause = null) =>
            board.FillMusts_Empty_ConnectNumbers(x, y, cause) &&
            board.FillMusts_Empty_FullNeighbors(x, y, cause) &&
            board.FillMusts_Empty_UnknownNeighbors(x, y);

        public static bool FillMusts_Empty_ConnectNumbers(this Cell[,] board, int x, int y, (int X, int Y)? cause = null) =>
            board[x, y] != Cell.Empty || board.FillMusts_ConnectNumbers(x, y, cause);

        public static bool FillMusts_Empty_FullNeighbors(this Cell[,] board, int x, int y, (int X, int Y)? cause = null)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Empty)
                return true;

            var fullNeighbors = Cardinals.Select(d => (X: x + d.X, Y: y + d.Y)).Where(s =>
                board.Contains(s.X, s.Y) &&
                board[s.X, s.Y] == Cell.Full &&
                !s.Equals(cause));

            var clone = board.Copy();
            var chunkSpots = fullNeighbors.Where(n =>
            {
                if (clone[n.X, n.Y] == CHUNK_COLOR) return false;
                clone.FloodFill(n.X, n.Y, CHUNK_COLOR);
                return true;
            });

            var spot = (x, y);
            return chunkSpots.All(s => board.FillMusts_Full(s.X, s.Y, spot));
        }

        public static bool FillMusts_Empty_UnknownNeighbors(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) ||
                board[x, y] != Cell.Empty ||
                board.GetSpots().All(s => board[s.X, s.Y] != Cell.Full))
                return true;

            var unknownNeighbors = Cardinals.Select(d => (X: x + d.X, Y: y + d.Y)).Where(s =>
                board.Contains(s.X, s.Y) &&
                board[s.X, s.Y] == Cell.Unkown)
                .ToArray();

            var musts = new List<(int X, int Y)>() { };

            foreach (var (X, Y) in unknownNeighbors)
            {
                if (board[X, Y] != Cell.Unkown) continue;
                var clone = board.Copy();
                clone.FloodFill(X, Y, CHUNK_COLOR);
                var chunkSpots = clone.GetSpots()
                    .Where(s => clone[s.X, s.Y] == CHUNK_COLOR)
                    .ToArray();
                var touchingFull = chunkSpots.Any(s => Cardinals.Select(d => (X: s.X + d.X, Y: s.Y + d.Y))
                    .Any(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Full));
                if (!touchingFull)
                {
                    musts.AddRange(chunkSpots);
                    foreach (var s in chunkSpots)
                        board[s.X, s.Y] = Cell.Empty;
                }
            }

            return musts.All(s => board.FillMusts_Empty(s.X, s.Y, null));
        }

        #endregion Empty

        public static bool FillMusts_ConnectNumbers(this Cell[,] board, int x, int y, (int X, int Y)? cause = null)
        {
            if (!board.Contains(x, y)) return true;
            for (var i = 0; i < Cardinals.Length; i++)
            {
                var scale = 1;
                while (true)
                {
                    var newX = x + Cardinals[i].X * scale;
                    var newY = y + Cardinals[i].Y * scale;
                    if (!board.Contains(newX, newY) || board[newX, newY] == Cell.Empty || (newX, newY).Equals(cause))
                        break;
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

            #region Find counts
            // Find counts
            var cellNumber = (int)board[x, y];
            var fullCount = new int[4];
            var distances = new int[4];
            for (var j = 0; j < 4; j++)
            {
                var scale = 1;
                var foundUnknown = false;
                while (true)
                {
                    var newX = x + Cardinals[j].X * scale;
                    var newY = y + Cardinals[j].Y * scale;
                    if (!board.Contains(newX, newY) || board[newX, newY] == Cell.Empty || board[newX, newY] >= 0)
                    {
                        distances[j] = scale - 1;
                        break;
                    }
                    if (foundUnknown || board[newX, newY] == Cell.Unkown)
                        foundUnknown = true;
                    else if (board[newX, newY] == Cell.Full)
                    {
                        fullCount[j] += 1;
                        if (fullCount[j] > cellNumber)
                            return false;
                    }
                    scale++;
                }
            }
            #endregion

            //Check for musts
            var musts = new List<(int X, int Y)>() { };
            var totalFullCount = fullCount.Sum();
            var totalRemaining = distances.Sum() - totalFullCount;
            var unchangedCount = 0;
            var i = -1;
            while (unchangedCount < 4)
            {
                unchangedCount++;
                i = (i + 1) % 4;
                var remaining = cellNumber - totalFullCount - totalRemaining + distances[i] - fullCount[i];

                //check if found must fulls
                if (remaining > 0)
                {
                    var offset = fullCount[i] + 1;
                    var mfs = Enumerable.Range(offset, remaining)
                        .Select(scale => (
                            X: x + Cardinals[i].X * scale,
                            Y: y + Cardinals[i].Y * scale))
                        .Where(s => board.Contains(s.X, s.Y))
                        .ToArray();
                    if (mfs.Any(s => board[s.X, s.Y] != Cell.Unkown))
                        return false;
                    if (mfs.Length > 0)
                    {
                        //Update board
                        foreach (var (X, Y) in mfs)
                            board[X, Y] = Cell.Full;

                        //Add to musts
                        musts.AddRange(mfs);

                        //Update counts
                        unchangedCount = 0;
                        fullCount[i] += mfs.Length;
                        totalFullCount += mfs.Length;
                        totalRemaining -= mfs.Length;
                    }
                }

                //check if number is satisfied
                if (totalFullCount == cellNumber)
                {
                    //block cardinals
                    var fillEmpties = fullCount.Select((fc, i) => (
                        X: x + Cardinals[i].X * (fc + 1),
                        Y: y + Cardinals[i].Y * (fc + 1)))
                        .Where(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Unkown);
                    foreach (var s in fillEmpties)
                    {
                        board[s.X, s.Y] = Cell.Empty;
                        musts.Add(s);
                    }
                    break; //number satisfied so no more checks.
                }

                //Next must be empty if there are too many fulls after next
                var nextScale = fullCount[i] + 1;
                var next = (X: x + Cardinals[i].X * nextScale, Y: y + Cardinals[i].Y * nextScale);
                if (board.Contains(next.X, next.Y) && board[next.X, next.Y] == Cell.Unkown)
                {
                    var afterNextFullCount = 0;
                    while (true)
                    {
                        nextScale++;
                        var newX = x + Cardinals[i].X * nextScale;
                        var newY = y + Cardinals[i].Y * nextScale;
                        if (!board.Contains(newX, newY) || board[newX, newY] != Cell.Full) break;
                        afterNextFullCount++;
                        if (totalFullCount + afterNextFullCount + 1 > cellNumber)
                        {
                            //Next must be empty
                            board[next.X, next.Y] = Cell.Empty;
                            musts.Add(next);

                            //Update counts
                            unchangedCount = 0;
                            break;
                        }
                    }
                }
            }
            return musts.All(s => board.FillMusts(s.X, s.Y));
        }
    }
}
