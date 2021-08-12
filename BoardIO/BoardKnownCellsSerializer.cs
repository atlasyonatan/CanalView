using System.Linq;
using CanalView;
using Newtonsoft.Json;

namespace BoardIO
{
    public static class BoardKnownCellsSerializer
    {
        private class DataStructure
        {
            public (int Width, int Height) Size;
            public (int X, int Y, int Cell)[] Cells;
        }

        public static string Serialize(Cell[,] board)
        {
            var data = new DataStructure
            {
                Cells = board.Points()
                .Where(s => board[s.x, s.y] != Cell.Unkown)
                .Select(s => (s.x, s.y, (int)board[s.x, s.y]))
                .ToArray(),
                Size = (board.GetLength(0), board.GetLength(1))
            };
            return JsonConvert.SerializeObject(data);
        }

        public static Cell[,] Deserialize(string s)
        {
            var data = JsonConvert.DeserializeObject<DataStructure>(s);
            return Board.Blank(data.Size.Width, data.Size.Height).Add(data.Cells);
        }
    }
}
