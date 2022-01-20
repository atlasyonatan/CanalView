using CanalView;
using System.Linq;
using static CanalView.Array2D;

namespace PuzzleSolving
{
    public static class Legality
    {
        public static bool Legal(this Cell[,] board) =>
            board.LegalSquare() &&
            board.LegalPath() &&
            board.LegalNumbers();

        public static bool Legal(this Cell[,] board, int x, int y) =>
            board.LegalSquare(x, y) &&
            board.LegalNumbers(x, y);

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

        public static bool LegalSquare(this Cell[,] board, int x, int y) =>
            !board.Contains(x, y) ||
            board[x, y] != Cell.Full ||
            !Diagonals.Select(d => (x: x + d.x, y: y + d.y)).Any(s =>
                board.Contains(s.x, s.y) &&
                board[x, s.y] == Cell.Full &&
                board[s.x, y] == Cell.Full &&
                board[s.x, s.y] == Cell.Full);

        public static bool LegalNumbers(this Cell[,] board) => !board.Points()
            .Any(s => board[s.x, s.y] >= 0 && !board.LegalNumbers(s.x, s.y));

        public static bool LegalNumbers(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y)) return true;
            if (board[x, y] >= 0)
            {
                var cellNumber = (int)board[x, y];
                var countUnknown = 0;
                var countFull = 0;
                foreach (var (dx, dy) in Cardinals)
                {
                    var scale = 1;
                    var foundUnknown = false;
                    while (true)
                    {
                        var newX = x + dx * scale;
                        var newY = y + dy * scale;
                        if (!board.Contains(newX, newY) || board[newX, newY] == Cell.Empty || board[newX, newY] >= 0)
                            break;
                        if (foundUnknown || board[newX, newY] == Cell.Unknown)
                        {
                            foundUnknown = true;
                            countUnknown++;
                        }
                        else if (board[newX, newY] == Cell.Full && ++countFull > cellNumber)
                            return false;
                        scale++;
                    }
                }
                return countUnknown + countFull >= cellNumber && (cellNumber != 0 || countFull <= 0);
            }
            foreach (var (dx, dy) in Cardinals)
            {
                var scale = 1;
                while (true)
                {
                    var newX = x + dx * scale;
                    var newY = y + dy * scale;
                    if (!board.Contains(newX, newY)) break;
                    if (board[newX, newY] >= 0)
                    {
                        if (!board.LegalNumbers(newX, newY))
                            return false;
                        break;
                    }
                    scale++;
                }
            }
            return true;
        }

        public static bool LegalPath(this Cell[,] board) =>
            !board.Points().TryFirst(s => board[s.x, s.y] == Cell.Full, out var spot) ||
            board.LegalPath(spot.x, spot.y);

        public static bool LegalPath(this Cell[,] board, int x, int y)
        {
            if (!board.Contains(x, y) || board[x, y] != Cell.Full)
                return true;
            var copy = board.Copy();
            FloodFill(copy, x, y, Cell.Full + 1);
            return !copy.Points().Any(s => copy[s.x, s.y] == Cell.Full);
        }
    }
}
