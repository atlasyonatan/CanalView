using CanalView;
using System;
using System.Collections.Generic;
using System.Linq;
using static PuzzleSolving.Musts;

namespace PuzzleGeneration
{
    public static class Generation
    {
        public static void DepthFirstTransform<T>(T[,] arr, (int x, int y) position, Func<(int x, int y), bool> transform)
        {
            if (transform(position))
            {
                var neighbors = Array2D.Cardinals
                        .Select(direction => (x: position.x + direction.x, y: position.y + direction.y))
                        .Where(p => arr.Contains(p.x, p.y));
                foreach (var neighbor in neighbors)
                    DepthFirstTransform(arr, neighbor, transform);
            }
        }
        public static void AddValidPath(Cell[,] board, (int x, int y) start, Func<(int x, int y), Cell> chooseCell, bool allowLoop = true)
        {
            var done = new bool[board.GetLength(0), board.GetLength(1)];
            DepthFirstTransform(board, start, p =>
            {
                if (done[p.x, p.y])
                    return false;
                done[p.x, p.y] = true;

                if (board[p.x, p.y] == Cell.Full)
                    return true;
                if (board[p.x, p.y] != Cell.Unknown)
                    return false;

                Cell chosenCell;
                if (allowLoop)
                    chosenCell = chooseCell(p);
                else
                {
                    // oversight: this could be a problem with non-blank boards as input. could be fixed with floodfill
                    var count = Array2D.Cardinals.Select(d => (x: p.x + d.x, y: p.y + d.y))
                        .Count(s =>
                        done.Contains(s.x, s.y) &&
                        board[s.x, s.y] == Cell.Full &&
                        done[s.x, s.y]);
                    chosenCell = count > 1 ? Cell.Empty : chooseCell(p);
                }

                if (chosenCell != Cell.Full && chosenCell != Cell.Empty)
                    return false;
                board[p.x, p.y] = chosenCell;
                var changes = ApplyMustsRecursively(board, new CellInfo { Position = p });
                foreach (var cell in changes)
                    done[cell.Position.x, cell.Position.y] = true;
                return chosenCell == Cell.Full;
            });
        }
        public static int FindNumber(this Cell[,] board, (int x, int y) position)
        {
            var fullCount = 0;
            foreach (var (dx, dy) in Array2D.Cardinals)
            {
                var scale = 1;
                while (true)
                {
                    var (newX, newY) = (position.x + scale * dx, position.y + scale * dy);
                    if (!board.Contains(newX, newY) || board[newX, newY] != Cell.Full)
                        break;
                    fullCount++;
                    scale++;
                }
            }
            return fullCount;
        }
        public static void FillNumber(Cell[,] board, (int x, int y) position)
        {
            board[position.x, position.y] = (Cell)board.FindNumber(position);
            ApplyMustsRecursively(board, new CellInfo { Position = position });
        }
        public static Cell[][,] ApplyChangesUntilBelowMaxSolutions(Cell[,] board, int maxSolutions, Func<bool> applyChanges, Func<Cell[,], IEnumerable<Cell[,]>> solver)
        {
            Cell[][,] solutions = null;
            while (applyChanges())
            {
                var copy = board.Copy();
                Clean(copy);
                var count = 0;
                solutions = solver(copy).TakeWhile(_ => ++count <= maxSolutions).ToArray();
                if (count <= maxSolutions)
                    break;
            }
            return solutions;
        }
        public static void Clean(Cell[,] board)
        {
            foreach (var (x, y) in board.Points())
                if (board[x, y] < 0)
                    board[x, y] = Cell.Unknown;
        }
    }
}
