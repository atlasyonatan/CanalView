using CanalView;
using System.Collections.Generic;

namespace PuzzleGenerator
{
    public interface IBoardGenerator
    {
        IEnumerable<Cell[,]> Generate();
    }
}
