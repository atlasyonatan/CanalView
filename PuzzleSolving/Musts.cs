using CanalView;
using System;
using System.Collections.Generic;
using System.Linq;
using static CanalView.Array2DExtensions;
using static PuzzleSolving.BitWiseOperators;

namespace PuzzleSolving
{
    //todo: omg check that it still works after rework
    public static class Musts
    {
        public class CellInfo
        {
            public (int x, int y) Position;
            public Cell CellType;
            public bool RequiredToBe = true;
            public (int x, int y)? Cause = null;
        }

        public static CellInfo NeutralCellInfo(this Cell[,] board, int x, int y) => new()
        {
            Position = (x, y),
            CellType = board[x, y]
        };

        #region ApplyMusts
        public static bool ApplyMustsRecursively(this Cell[,] board) => board.GetSpots()
            .Where(s => board[s.X, s.Y] != Cell.Unkown)
            .ToArray()
            .Select(s => board.NeutralCellInfo(s.X, s.Y))
            .All(c => board.ApplyMustsRecursively(c));

        public static bool ApplyMustsRecursively(this Cell[,] board, CellInfo cell)
        {
            var enumerable = board.GetMusts(cell);
            if (enumerable == null)
                return false;
            var musts = enumerable.Where(c => board[c.Position.x,c.Position.y] == Cell.Unkown || board.IsntAlready(c))
                .ToArray();
            //return musts.Length == 0 || (
            //    musts.All(c => board.TryApplyMust(c)) &&
            //    musts.All(c => board.ApplyMustsRecursively(c)));
            if (musts.Length == 0)
                return true;
            var a = musts.All(c => board.TryApplyMust(c));
            //Console.WriteLine(board.Tostring());
            //Console.WriteLine();
            return a && musts.All(c => board.ApplyMustsRecursively(c));
        }

        public static bool IsntAlready(this Cell[,] board, CellInfo cell) =>
            cell.RequiredToBe ^ board[cell.Position.x, cell.Position.y] == cell.CellType;

        public static bool TryApplyMust(this Cell[,] board, CellInfo cell)
        {
            var current = board[cell.Position.x, cell.Position.y];
            if (cell.RequiredToBe)
            {
                if(current == Cell.Unkown)
                {
                    board[cell.Position.x, cell.Position.y] = cell.CellType;
                    return true;
                }
                return current == cell.CellType;
            }
            else if (cell.CellType == Cell.Full)
                board[cell.Position.x, cell.Position.y] = Cell.Empty;
            return true;
        }
        #endregion

        #region GetMusts
        public const Cell CHUNK_COLOR = (Cell)int.MinValue;
        public static IEnumerable<CellInfo> GetMusts(this Cell[,] board, CellInfo cell) => board[cell.Position.x, cell.Position.y] switch
        {
            Cell.Empty => board.GetMusts_Empty(cell),
            Cell.Full => board.GetMusts_Full(cell),
            Cell.Unkown => Enumerable.Empty<CellInfo>(),
            _ => board.GetMusts_Number(cell)
        };

        #region Full
        public static IEnumerable<CellInfo> GetMusts_Full(this Cell[,] board, CellInfo cell) => EnumerableExtensions.ContactIfNotNull(
            () => board.GetMusts_Full_LShape(cell),
            () => board.GetMusts_Full_ConnectNumbers(cell),
            () => board.GetMusts_Full_Chunk(cell));
        public static IEnumerable<CellInfo> GetMusts_Full_LShape(this Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return Enumerable.Empty<CellInfo>();
            uint fullSpots = 0;
            for (var i = 0; i < 8; i++)
            {
                var (dx, dy) = ClockwiseDirections[i];
                var newX = x + dx;
                var newY = y + dy;
                if (board.Contains(newX, newY) && board[newX, newY] == Cell.Full)
                    fullSpots |= 1u << i;
            }
            uint mustsArr = 0;
            for (var i = 0; i < 8; i += 2)
            {
                var mask = RotateLeft(5u, i, 8);
                if ((fullSpots & mask) == mask) mustsArr |= RotateLeft(2u, i, 8);
                mask = RotateLeft(6u, i, 8);
                if ((fullSpots & mask) == mask) mustsArr |= RotateLeft(1u,i,8);
                mask = RotateLeft(mask,1,8);
                if ((fullSpots & mask) == mask) mustsArr |= RotateLeft(16u, i, 8);
            }
            var cause = (x, y);
            return Enumerable.Range(0, 8)
                .Where(i => (mustsArr & 1 << i) > 0)
                .Select(i => (X: x + ClockwiseDirections[i].X, Y: y + ClockwiseDirections[i].Y))
                .Select(p => new CellInfo()
                {
                    Cause = cause,
                    CellType = Cell.Full,
                    Position = p,
                    RequiredToBe = false
                });
        }
        public static IEnumerable<CellInfo> GetMusts_Full_Chunk(this Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return Enumerable.Empty<CellInfo>();
            var clone = board.Copy();
            clone.FloodFill(x, y, CHUNK_COLOR);
            if (clone.GetSpots().All(s => clone[s.X, s.Y] != Cell.Full))
                return Enumerable.Empty<CellInfo>();

            var chunkExits = clone.GetSpots()
                .Where(s => clone[s.X, s.Y] == CHUNK_COLOR).SelectMany(s =>
                    Cardinals.Select(d => (X: s.X + d.X, Y: s.Y + d.Y)).Where(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Unkown))
                .Distinct()
                .ToArray();

            if (chunkExits.Length == 0)
                return null;
            var cause = (x, y);
            if (chunkExits.Length == 1)
                return chunkExits.Select(p => new CellInfo() { Cause = cause, CellType = Cell.Full, Position = p });
            return Enumerable.Empty<CellInfo>();
        }
        public static IEnumerable<CellInfo> GetMusts_Full_ConnectNumbers(this Cell[,] board, CellInfo cell) =>
            board[cell.Position.x, cell.Position.y] != Cell.Full ? Enumerable.Empty<CellInfo>() : board.GetMusts_ConnectNumbers(cell);
        #endregion

        #region Empty
        public static IEnumerable<CellInfo> GetMusts_Empty(this Cell[,] board, CellInfo cell) => EnumerableExtensions.ContactIfNotNull(
            () => board.GetMusts_Empty_ConnectNumbers(cell),
            () => board.GetMusts_Empty_FullNeighbors(cell),
            () => board.GetMusts_Empty_UnknownNeighbors(cell));
        public static IEnumerable<CellInfo> GetMusts_Empty_ConnectNumbers(this Cell[,] board, CellInfo cell) =>
            board[cell.Position.x, cell.Position.y] != Cell.Empty ? Enumerable.Empty<CellInfo>() : board.GetMusts_ConnectNumbers(cell);
        public static IEnumerable<CellInfo> GetMusts_Empty_FullNeighbors(this Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) || board[x, y] != Cell.Empty)
                return Enumerable.Empty<CellInfo>();

            var fullNeighbors = Cardinals.Select(d => (X: x + d.X, Y: y + d.Y)).Where(s =>
                board.Contains(s.X, s.Y) &&
                board[s.X, s.Y] == Cell.Full &&
                !s.Equals(cell.Cause));

            var clone = board.Copy();
            var chunkSpots = fullNeighbors.Where(n =>
            {
                if (clone[n.X, n.Y] == CHUNK_COLOR)
                    return false;
                clone.FloodFill(n.X, n.Y, CHUNK_COLOR);
                return true;
            });

            var newCause = (x, y);
            return chunkSpots.Select(p => new CellInfo() { Cause = newCause, CellType = Cell.Full, Position = p });
        }
        public static IEnumerable<CellInfo> GetMusts_Empty_UnknownNeighbors(this Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) ||
                board[x, y] != Cell.Empty ||
                board.GetSpots().All(s => board[s.X, s.Y] != Cell.Full))
                return Enumerable.Empty<CellInfo>();

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
            return musts.Select(p => new CellInfo() { Cause = cause, CellType = Cell.Empty, Position = p });
        }
        #endregion Empty

        public static IEnumerable<CellInfo> GetMusts_Number(this Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) || board[x, y] < 0)
                return Enumerable.Empty<CellInfo>();

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
            var musts = new List<CellInfo>(); //new List<(int X, int Y)>() { };
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
                        musts.AddRange(mfs.Select(p => new CellInfo() { Cause = cause, CellType = Cell.Full, Position = p }));

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

                    musts.AddRange(fillEmpties.Select(p => new CellInfo { Cause = cause, CellType = Cell.Empty, Position = p }));
                    break; //number satisfied so no more checks.
                }

                //Next must be empty if there are too many fulls after next
                if (fullCount[i] < distances[i])
                {
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
                                musts.Add(new CellInfo { Cause = cause, CellType = Cell.Empty, Position = next });

                                //Update counts
                                unchangedCount = 0;
                                totalRemaining -= distances[i] - fullCount[i];
                                distances[i] = fullCount[i];
                                break;
                            }
                        }
                    }
                }
            }
            return musts;
        }
        public static IEnumerable<CellInfo> GetMusts_ConnectNumbers(this Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y))
                return Enumerable.Empty<CellInfo>();
            var musts = new List<CellInfo>();
            for (var i = 0; i < Cardinals.Length; i++)
            {
                var scale = 1;
                while (true)
                {
                    var newX = x + Cardinals[i].X * scale;
                    var newY = y + Cardinals[i].Y * scale;
                    if (!board.Contains(newX, newY) || board[newX, newY] == Cell.Empty || (newX, newY).Equals(cell.Cause))
                        break;
                    if (board[newX, newY] >= 0)
                    {
                        var newCell = board.NeutralCellInfo(newX, newY);
                        newCell.Cause = cell.Position;
                        var m = board.GetMusts_Number(newCell);
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
