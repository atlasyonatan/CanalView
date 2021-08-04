using CanalView;
using System;
using System.Collections.Generic;
using System.Linq;
using static CanalView.Array2DExtensions;

namespace PuzzleSolving
{
    //todo: omg check that it still works after rework
    public static class Musts
    {
        public class Must
        {
            public (int x, int y) Position;
            public Cell CellType;
            public bool Excludes = false;
            public (int x, int y) Cause;
        }

        #region ApplyMusts
        public static bool ApplyMustsRecursively(this Cell[,] board) => board.GetSpots()
            .Where(s => board[s.X, s.Y] != Cell.Unkown)
            .All(s => board.ApplyMustsRecursively(s.X, s.Y, null));
        public static bool ApplyMustsRecursively(this Cell[,] board, int x, int y, (int X, int Y)? cause)
        {
            var enumerable = board.GetMusts(x, y, cause);
            if (enumerable == null)
                return false;
            var musts = enumerable.ToArray();
            return musts.Length == 0 || (
                musts.All(must => board.TryApplyMust(must)) &&
                musts.All(must => board.ApplyMustsRecursively(must.Position.x, must.Position.y, must.Cause)));
            //if (musts.Length == 0)
            //    return true;
            //var a = musts.All(must => board.TryApplyMust(must));
            //Console.WriteLine(board.Tostring());
            //Console.WriteLine();
            //return a && musts.All(must => board.ApplyMustsRecursively(must.Position.x, must.Position.y, must.Cause));
        }
        public static bool TryApplyMust(this Cell[,] board, Must must)
        {
            var (x, y) = must.Position;
            if (must.Excludes)
                return board[x, y] != must.CellType;
            if (board[x, y] == must.CellType)
                return true;
            if (board[x, y] == Cell.Unkown)
            {
                board[x, y] = must.CellType;
                return true;
            }
            return false;
        }
        #endregion

        #region GetMusts
        public const Cell CHUNK_COLOR = (Cell)int.MinValue;
        public static IEnumerable<Must> GetMusts(this Cell[,] board, int x, int y, (int X, int Y)? cause) => board[x, y] switch
        {
            Cell.Empty => board.GetMusts_Empty(x, y, cause),
            Cell.Full => board.GetMusts_Full(x, y, cause),
            Cell.Unkown => Enumerable.Empty<Must>(),
            _ => board.GetMusts_Number(x, y)
        };

        #region Full
        public static IEnumerable<Must> GetMusts_Full(this Cell[,] board, int x, int y, (int X, int Y)? cause) => EnumerableExtensions.ContactIfNotNull(
            () => board.GetMusts_Full_LShape(x, y),
            () => board.GetMusts_Full_ConnectNumbers(x, y, cause),
            () => board.GetMusts_Full_Chunk(x, y));
        public static IEnumerable<Must> GetMusts_Full_LShape(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return Enumerable.Empty<Must>();
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
            var cause = (x, y);
            return Enumerable.Range(0, 8)
                .Where(i => (mustsArr & 1 << i) > 0)
                .Select(i => (X: x + ClockwiseDirections[i].X, Y: y + ClockwiseDirections[i].Y))
                .Select(p => new Must() { Cause = cause, CellType = Cell.Full, Position = p, Excludes = true });
        }
        public static IEnumerable<Must> GetMusts_Full_Chunk(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return Enumerable.Empty<Must>();
            var clone = board.Copy();
            clone.FloodFill(x, y, CHUNK_COLOR);
            if (clone.GetSpots().All(s => clone[s.X, s.Y] != Cell.Full))
                return Enumerable.Empty<Must>();

            var chunkExits = clone.GetSpots()
                .Where(s => clone[s.X, s.Y] == CHUNK_COLOR).SelectMany(s =>
                    Cardinals.Select(d => (X: s.X + d.X, Y: s.Y + d.Y)).Where(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Unkown))
                .Distinct()
                .ToArray();

            if (chunkExits.Length == 0)
                return null;
            var cause = (x, y);
            if (chunkExits.Length == 1)
                return chunkExits.Select(p => new Must() { Cause = cause, CellType = Cell.Full, Position = p });
            return Enumerable.Empty<Must>();
        }
        public static IEnumerable<Must> GetMusts_Full_ConnectNumbers(this Cell[,] board, int x, int y, (int X, int Y)? cause) =>
            board[x, y] != Cell.Full ? Enumerable.Empty<Must>() : board.GetMusts_ConnectNumbers(x, y, cause);
        #endregion

        #region Empty
        public static IEnumerable<Must> GetMusts_Empty(this Cell[,] board, int x, int y, (int X, int Y)? cause) => EnumerableExtensions.ContactIfNotNull(
            () => board.GetMusts_Empty_ConnectNumbers(x, y, cause),
            () => board.GetMusts_Empty_FullNeighbors(x, y, cause),
            () => board.GetMusts_Empty_UnknownNeighbors(x, y));
        public static IEnumerable<Must> GetMusts_Empty_ConnectNumbers(this Cell[,] board, int x, int y, (int X, int Y)? cause) =>
            board[x, y] != Cell.Empty ? Enumerable.Empty<Must>() : board.GetMusts_ConnectNumbers(x, y, cause);
        public static IEnumerable<Must> GetMusts_Empty_FullNeighbors(this Cell[,] board, int x, int y, (int X, int Y)? cause)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Empty)
                return Enumerable.Empty<Must>();

            var fullNeighbors = Cardinals.Select(d => (X: x + d.X, Y: y + d.Y)).Where(s =>
                board.Contains(s.X, s.Y) &&
                board[s.X, s.Y] == Cell.Full &&
                !s.Equals(cause));

            var clone = board.Copy();
            var chunkSpots = fullNeighbors.Where(n =>
            {
                if (clone[n.X, n.Y] == CHUNK_COLOR)
                    return false;
                clone.FloodFill(n.X, n.Y, CHUNK_COLOR);
                return true;
            });

            var newCause = (x, y);
            return chunkSpots.Select(p => new Must() { Cause = newCause, CellType = Cell.Full, Position = p });
        }
        public static IEnumerable<Must> GetMusts_Empty_UnknownNeighbors(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) ||
                board[x, y] != Cell.Empty ||
                board.GetSpots().All(s => board[s.X, s.Y] != Cell.Full))
                return Enumerable.Empty<Must>();

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
                    musts.AddRange(chunkSpots);
            }
            var cause = (x, y);
            return musts.Select(p => new Must() { Cause = cause, CellType = Cell.Empty, Position = p });
        }
        #endregion Empty

        //todo: check if this broke
        public static IEnumerable<Must> GetMusts_Number(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] < 0)
                return Enumerable.Empty<Must>();

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
                            return null;
                    }
                    scale++;
                }
            }
            #endregion

            var cause = (x, y);

            //Check for musts
            var musts = new List<Must>(); //new List<(int X, int Y)>() { };
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
                        return null;
                    if (mfs.Length > 0)
                    {
                        ////Update board
                        //foreach (var (X, Y) in mfs)
                        //    board[X, Y] = Cell.Full;

                        //Add to musts
                        musts.AddRange(mfs.Select(p => new Must() { Cause = cause, CellType = Cell.Full, Position = p }));

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

                    musts.AddRange(fillEmpties.Select(p => new Must { Cause = cause, CellType = Cell.Empty, Position = p }));
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
                            musts.Add(new Must { Cause = cause, CellType = Cell.Empty, Position = next });

                            //Update counts
                            unchangedCount = 0;
                            break;
                        }
                    }
                }
            }
            return musts;
        }

        public static IEnumerable<Must> GetMusts_ConnectNumbers(this Cell[,] board, int x, int y, (int X, int Y)? cause)
        {
            if (!board.Contains(x, y))
                return Enumerable.Empty<Must>();
            var musts = new List<Must>();
            for (var i = 0; i < Cardinals.Length; i++)
            {
                var scale = 1;
                while (true)
                {
                    var newX = x + Cardinals[i].X * scale;
                    var newY = y + Cardinals[i].Y * scale;
                    if (!board.Contains(newX, newY) || board[newX, newY] == Cell.Empty || (newX, newY).Equals(cause))
                        break;
                    if (board[newX, newY] >= 0)
                    {
                        var m = board.GetMusts_Number(newX, newY);
                        if (m == null)
                            return null;
                        musts.AddRange(m);
                        break;
                    }
                    scale++;
                }
            }
            return musts;
        }
        #endregion
    }
}
