using System.Collections.Generic;

namespace CanalView
{
    public static class Rules
    {
        public static bool LegalSquare(this Cell[,] board)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            for (var y = 0; y < height - 1; y++)
                for (var x = 0; x < width - 1; x++)
                {
                    if (board[x, y] != Cell.Full ||
                        board[x + 1, y] != Cell.Full ||
                        board[x, y + 1] != Cell.Full ||
                        board[x + 1, y + 1] != Cell.Full) continue;
                    return false;
                }
            return true;
        }

        public static bool LegalNumbers(this Cell[,] board)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            var size = width * height;
            for (var i = 0; i < size; i++)
            {
                var x = i % width;
                var y = i / width;
                if (board[x, y] < 0) continue;

                var cellNumber = (int)board[x, y];
                var countUnknown = 0;
                var countFull = 0;
                foreach (var (dy, dx) in CardinalDirections)
                {
                    var scale = 1;
                    var foundUnknown = false;
                    while (true)
                    {
                        var newX = x + dx * scale;
                        var newY = y + dy * scale;
                        if (newX < 0 || newX >= width || newY < 0 || newY >= height) break;
                        if (board[newX, newY] == Cell.Empty) break;
                        if (foundUnknown || board[newX, newY] == Cell.Unkown)
                        {
                            foundUnknown = true;
                            countUnknown++;
                        }
                        else if (board[newX, newY] == Cell.Full) countFull++;
                        scale++;
                    }
                }
                if (countFull > cellNumber) return false;
                if (countUnknown + countFull < cellNumber) return false;
            }
            return true;
        }

        public static bool LegalPath(this Cell[,] board)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            var size = width * height;
            int x0 = -1;
            int y0 = -1;
            for (var i = 0; i < size; i++)
            {
                var x = i % width;
                var y = i / width;
                if (board[x, y] == Cell.Full)
                {
                    x0 = x;
                    y0 = y;
                    break;
                }
            }
            if (x0 == -1) return true;

            var painted = new bool[height, width];
            //flood fill
            void FloodFill(int x, int y)
            {
                if (painted[x, y]) return;
                painted[x, y] = true;
                foreach (var (dy, dx) in CardinalDirections)
                {
                    var newX = x + dx;
                    var newY = y + dy;
                    if (newX < 0 || newX >= width || newY < 0 || newY >= height) continue;
                    if (painted[newX, newY] || board[newX, newY] != Cell.Full) continue;
                    FloodFill(newX, newY);
                }
            }
            FloodFill(x0, y0);

            //check
            for (var i = 0; i < size; i++)
            {
                var x = i % width;
                var y = i / width;
                if (board[x, y] == Cell.Full && !painted[x, y]) return false;
            }

            return true;
        }

        public static readonly IEnumerable<(int dx, int dy)> CardinalDirections = new (int, int)[] { (0, -1), (-1, 0), (0, 1), (1, 0) };

        public static readonly IEnumerable<Cell> FillOptions = new Cell[] { Cell.Full, Cell.Empty };
    }
}
