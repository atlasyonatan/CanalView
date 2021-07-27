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
            var data = new DataStructure();
            data.Cells = board.GetSpots()
                .Where(s => board[s.X, s.Y] != Cell.Unkown)
                .Select(s => (s.X, s.Y, (int)board[s.X,s.Y]))
                .ToArray();
            data.Size = (board.GetLength(0), board.GetLength(1));
            return JsonConvert.SerializeObject(data);
        }

        public static Cell[,] Deserialize(string s)
        {
            var data = JsonConvert.DeserializeObject<DataStructure>(s);
            return Board.Blank(data.Size.Width, data.Size.Height).Add(data.Cells);
        }
    }
}
