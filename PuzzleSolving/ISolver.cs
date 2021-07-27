using CanalView;
using System.Collections.Generic;

namespace PuzzleSolving
{
    public interface ISolver
    {
        public IEnumerable<Cell[,]> Solve(Cell[,] board);
    }
}
