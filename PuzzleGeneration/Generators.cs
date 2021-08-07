using CanalView;
using System;
using System.Collections.Generic;
using static PuzzleGeneration.Generation;
using static PuzzleSolving.Musts;

namespace PuzzleGeneration
{
    public static class Generators
    {
        public static IEnumerable<Cell[,]> PathGenerator((int width, int height) dimensions, Random random, double pathChance)
        {
            while (true)
            {
                var n = random.Next(dimensions.width * dimensions.height);
                var start = (x: n % dimensions.width, y: n / dimensions.width);
                var board = Board.Blank(dimensions.width, dimensions.height);
                board[start.x, start.y] = Cell.Full;
                AddValidPath(board, start, position =>
                {
                    if (random.NextDouble() < pathChance)
                    {
                        board[position.x, position.y] = Cell.Full;
                        board.ApplyMusts_Full_LShape(new CellInfo { Position = position });
                    }
                });
                yield return board;
            }
        }

        public static IEnumerable<Cell[,]> PuzzleGenerator(Cell[,] board, Random random, double pathChance)
        {
            throw new NotImplementedException();
            //while (true)
            //{
            //    var b = board.Copy();
            //    //Generation.AddRandomFullPath(b, random, pathChance);
            //    yield return b;
            //}
        }


    }
}

