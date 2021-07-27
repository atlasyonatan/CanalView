using CanalView;
using System.Collections.Generic;

namespace PuzzleGenerator
{
    public interface IPuzzleGenerator
    {
        IEnumerable<Cell[,]> Generate();
    }
}
