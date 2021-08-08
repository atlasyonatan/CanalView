using CanalView;
using System;
using System.Collections.Generic;
using System.Linq;
using static CanalView.Array2DExtensions;
using static PuzzleSolving.BitWiseOperators;
using static PuzzleSolving.EnumerableExtensions;

namespace PuzzleSolving
{
    //todo: omg check that it still works after rework
    public static class Musts
    {
        private static readonly CellInfo[] None = Array.Empty<CellInfo>();
        private const Cell CHUNK_COLOR = (Cell)int.MinValue;

        public class CellInfo
        {
            public (int x, int y) Position;
            public (int x, int y)? Cause = null;
        }

        public static IEnumerable<CellInfo> ApplyMustsRecursively(Cell[,] board)
        {
            var placesOfInterest = board.GetSpots()
                .Where(s => board[s.X, s.Y] != Cell.Unkown)
                .Select(s => new CellInfo { Position = s })
                .ToArray();
            return placesOfInterest.ContactIfNotNull(c => ApplyMustsRecursively(board, c));
        }
        public static IEnumerable<CellInfo> ApplyMustsRecursively(Cell[,] board, CellInfo cell) =>
            ApplyMustsRecursively(board, ApplyMusts(board, cell));
        public static IEnumerable<CellInfo> ApplyMustsRecursively(Cell[,] board, IEnumerable<CellInfo> musts) =>
            musts.ContactIfNotNull(c => ApplyMustsRecursively(board, c));
        public static IEnumerable<CellInfo> ApplyMusts(Cell[,] board, CellInfo cell) => board[cell.Position.x, cell.Position.y] switch
        {
            Cell.Empty => ApplyMusts_Empty(board, cell),
            Cell.Full => ApplyMusts_Full(board, cell),
            Cell.Unkown => None,
            _ => ApplyMusts_Number(board, cell)
        };

        #region Full
        public static IEnumerable<CellInfo> ApplyMusts_Full(Cell[,] board, CellInfo cell) => board[cell.Position.x, cell.Position.y] == Cell.Full
                ? ContactIfNotNull(
                    () => ApplyMusts_Full_LShape(board, cell),
                    () => GetMusts_ConnectNumbers(board, cell),
                    () => ApplyMusts_Full_Chunk(board, cell))
                : None;
        public static IEnumerable<CellInfo> ApplyMusts_Full_LShape(Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return None;
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
                if ((fullSpots & mask) == mask) mustsArr |= RotateLeft(1u, i, 8);
                mask = RotateLeft(mask, 1, 8);
                if ((fullSpots & mask) == mask) mustsArr |= RotateLeft(16u, i, 8);
            }
            var musts = Enumerable.Range(0, 8)
                .Where(i => (mustsArr & 1 << i) > 0)
                .Select(i => (X: x + ClockwiseDirections[i].X, Y: y + ClockwiseDirections[i].Y))
                .ToArray();

            //apply direct found musts
            var changes = new List<CellInfo>();
            foreach (var p in musts)
                switch (board[p.X, p.Y])
                {
                    case Cell.Full:
                        return null;
                    case Cell.Unkown:
                        //Update board
                        board[p.X, p.Y] = Cell.Empty;
                        changes.Add(new CellInfo { Position = p, Cause = cell.Position });
                        break;
                    default:
                        break;
                }
            return changes;
        }
        public static IEnumerable<CellInfo> ApplyMusts_Full_Chunk(Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return None;
            var clone = board.Copy();
            clone.FloodFill(x, y, CHUNK_COLOR);
            if (clone.GetSpots().All(s => clone[s.X, s.Y] != Cell.Full))
                return None;

            var chunkExits = clone.GetSpots()
                .Where(s => clone[s.X, s.Y] == CHUNK_COLOR).SelectMany(s =>
                    Cardinals.Select(d => (X: s.X + d.X, Y: s.Y + d.Y)).Where(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Unkown))
                .Distinct()
                .ToArray();

            switch (chunkExits.Length)
            {
                case 0:
                    return null;
                case 1:
                    {
                        //apply direct found musts
                        var exit = chunkExits[0];
                        board[exit.X, exit.Y] = Cell.Full;
                        return new CellInfo[] { new CellInfo() { Cause = cell.Position, Position = exit } };
                    }
                default:
                    return None;
            }
        }
        #endregion

        #region Empty
        public static IEnumerable<CellInfo> ApplyMusts_Empty(Cell[,] board, CellInfo cell) => board[cell.Position.x, cell.Position.y] == Cell.Empty
                ? ContactIfNotNull(
                    () => GetMusts_ConnectNumbers(board, cell),
                    () => GetMusts_Empty_FullNeighbors(board, cell),
                    () => ApplyMusts_Empty_UnknownNeighbors(board, cell))
                : None;
        public static IEnumerable<CellInfo> GetMusts_Empty_FullNeighbors(Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) || board[x, y] != Cell.Empty)
                return None;

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
            return chunkSpots.Select(p => new CellInfo() { Cause = cell.Position, Position = p });
        }
        public static IEnumerable<CellInfo> ApplyMusts_Empty_UnknownNeighbors(Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) ||
                board[x, y] != Cell.Empty ||
                board.GetSpots().All(s => board[s.X, s.Y] != Cell.Full))
                return None; ;

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
            return musts.Select(p => new CellInfo() { Cause = cell.Position, Position = p });
        }
        #endregion Empty

        #region Number
        public static IEnumerable<CellInfo> ApplyMusts_Number(Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y) || board[x, y] < 0)
                return None;

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

            #region Check for musts
            var changes = new List<CellInfo>();
            var totalFullCount = fullCount.Sum();
            var totalDistances = distances.Sum();
            var totalRemaining = totalDistances - totalFullCount;
            if (totalDistances < cellNumber)
                return null;
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
                    if (mfs.Any(s => board[s.X, s.Y] != Cell.Unkown && board[s.X, s.Y] != Cell.Full))
                        return null;
                    if (mfs.Length > 0)
                    {
                        //Update board
                        foreach (var p in mfs)
                        {
                            board[p.X, p.Y] = Cell.Full;
                            changes.Add(new CellInfo { Cause = cell.Position, Position = p });
                        }

                        //Update counts
                        unchangedCount = 0;
                        fullCount[i] += mfs.Length;
                        totalFullCount += mfs.Length;
                        totalRemaining -= mfs.Length;
                    }
                }

                //check if number is satisfied
                if (totalFullCount > cellNumber)
                    return null;
                if (totalFullCount == cellNumber)
                {
                    //block cardinals
                    var fillEmpties = fullCount.Select((fc, i) => (
                        X: x + Cardinals[i].X * (fc + 1),
                        Y: y + Cardinals[i].Y * (fc + 1)))
                        .Where(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Unkown);

                    //Update board
                    foreach (var p in fillEmpties)
                    {
                        board[p.X, p.Y] = Cell.Empty;
                        changes.Add(new CellInfo { Position = p, Cause = cell.Position });
                    }
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

                                //Update board
                                board[next.X, next.Y] = Cell.Empty;
                                changes.Add(new CellInfo { Cause = cell.Position, Position = next });

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
            #endregion

            return changes;
        }
        public static IEnumerable<CellInfo> GetMusts_ConnectNumbers(Cell[,] board, CellInfo cell)
        {
            var (x, y) = cell.Position;
            if (!board.Contains(x, y))
                return None;
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
                        var sub = ApplyMusts_Number(board, new CellInfo { Position = (newX, newY), Cause = cell.Position });
                        if (sub == null)
                            return null;
                        musts.AddRange(sub);
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
