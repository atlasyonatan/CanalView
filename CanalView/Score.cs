using System;
using System.Collections.Generic;
using System.Text;

namespace CanalView
{
    public static class Score
    {
        public static (float Full, float Empty)[,] Score_ByNumber(this Cell[,] board)
        {
            throw new NotImplementedException();
        }

        public static (float Full, float Empty)[,] Score_ByNeighborhood(this Cell[,] board)
        {
            var score = new (float Full, float Empty)[] { };
            foreach (var s in board.GetSpots())
            {

            }




            //board.GetSpots()
            //.Where(s => board[s.X, s.Y] == Cell.Unkown)
            //.Select(s => (Spot: s,
            //    CardinalQuality: 4-Math.Cardinals
            //        .Select(d => (X: s.X + d.X, Y: s.Y + d.Y))
            //        .Count(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Unkown),
            //    DiagonalQuality: 4-Math.Diagonals
            //        .Select(d => (X: s.X + d.X, Y: s.Y + d.Y))
            //        .Count(s => board.Contains(s.X, s.Y) && board[s.X, s.Y] == Cell.Unkown)))
            //.OrderByDescending(s => s.CardinalQuality)
            //.ThenByDescending(s => s.DiagonalQuality)
            //.ThenBy(s => s.Spot.X)
            //.ThenBy(s => s.Spot.Y)
            //.ToArray();
        }
    }
}
