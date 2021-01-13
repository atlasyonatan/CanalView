using System.Collections.Generic;

namespace CanalView
{
    public interface ISolver
    {
        public IEnumerable<Cell[,]> Solve(Cell[,] board);
    }
}
