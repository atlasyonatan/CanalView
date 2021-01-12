using System;
using System.Collections.Generic;
using System.Text;

namespace CanalView.Interfaces
{
    public interface ISolver
    {
        public IEnumerable<Cell[,]> Solve(Cell[,] board);
    }
}
