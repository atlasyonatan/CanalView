using CanalView;
using System;
using System.Linq;

namespace BoardSerialization
{
    public static class VisualSerializer
    {
        public static readonly char RowSeperator = '\n';
        public static readonly char CellSeperator = ',';
        public static string Serialize(this Cell[,] board) => string.Join(RowSeperator,
            Enumerable.Range(0, board.GetLength(1)).Select(y => string.Join(CellSeperator,
                Enumerable.Range(0, board.GetLength(0)).Select(x => SerializeCell(board[x, y])))));
        public static Cell[,] Deserialize(string s) => Array2D.ConvertTo2DArray(s.Split(RowSeperator)
            .Select(row => row.Split(CellSeperator).Select(DeserializeCell).ToArray())
            .ToArray());
        public static string SerializeCell(Cell cell) => cell switch
        {
            Cell.Empty => "e",
            Cell.Full => "f",
            Cell.Unkown => "_",
            _ => ((int)cell).ToString()
        };
        public static Cell DeserializeCell(string s) => s switch
        {
            "_" => Cell.Unkown,
            "e" => Cell.Empty,
            "f" => Cell.Full,
            _ => (Cell)(int.TryParse(s, out var n) ? n : throw new ArgumentException($"Indefined string {s}")),
        };
    }
}
