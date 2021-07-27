using CanalView;

namespace PuzzleSolving
{
    public static class Puzzles
    {
        public static Cell[,] Easy_5x5 => Board.Blank(5, 5).Add(
            (0, 0, (Cell)5),
            (1, 2, (Cell)4),
            (3, 2, (Cell)3),
            (0, 4, (Cell)3),
            (2, 4, (Cell)1),
            (4, 4, (Cell)4));
        public static Cell[,] Easy2_5x5 => Board.Blank(5, 5).Add(
            (1, 1, (Cell)2),
            (4, 1, (Cell)5),
            (2, 2, (Cell)2),
            (3, 4, (Cell)4));
        public static Cell[,] Easy3_5x5 => Board.Blank(5, 5).Add(
            (0, 2, (Cell)2),
            (1, 0, (Cell)2),
            (1, 3, (Cell)2),
            (2, 2, (Cell)3),
            (3, 0, (Cell)5),
            (4, 2, (Cell)3),
            (4, 4, (Cell)2));
        public static Cell[,] Medium_8x8 => Board.Blank(8, 8).Add(
            (3, 0, (Cell)4),
            (6, 0, (Cell)3),
            (0, 2, (Cell)5),
            (2, 2, (Cell)2),
            (4, 2, (Cell)0),
            (6, 3, (Cell)2),
            (6, 5, (Cell)5),
            (1, 6, (Cell)0),
            (3, 6, (Cell)4),
            (6, 7, (Cell)4),
            (7, 7, (Cell)1));
        public static Cell[,] Medium_10x10 => Board.Blank(10, 10).Add(
            (1, 3, (Cell)6),
            (1, 8, (Cell)5),
            (2, 0, (Cell)3),
            (2, 1, (Cell)1),
            (2, 6, (Cell)3),
            (3, 7, (Cell)1),
            (4, 0, (Cell)4),
            (4, 1, (Cell)3),
            (4, 5, (Cell)4),
            (5, 4, (Cell)6),
            (5, 8, (Cell)3),
            (5, 9, (Cell)1),
            (6, 2, (Cell)7),
            (7, 3, (Cell)3),
            (7, 8, (Cell)2),
            (7, 9, (Cell)2),
            (8, 1, (Cell)1),
            (8, 3, (Cell)2),
            (8, 6, (Cell)6));
        public static Cell[,] Hard_12x12 => Board.Blank(12, 12).Add(
            (2, 0, (Cell)5),
            (10, 0, (Cell)5),
            (0, 1, (Cell)4),
            (6, 1, (Cell)1),
            (4, 2, (Cell)1),
            (6, 2, (Cell)3),
            (8, 2, (Cell)2),
            (3, 3, (Cell)1),
            (1, 4, (Cell)2),
            (10, 4, (Cell)2),
            (4, 5, (Cell)4),
            (10, 5, (Cell)3),
            (1, 6, (Cell)6),
            (7, 6, (Cell)3),
            (1, 7, (Cell)4),
            (10, 7, (Cell)1),
            (8, 8, (Cell)3),
            (3, 9, (Cell)4),
            (5, 9, (Cell)1),
            (7, 9, (Cell)2),
            (5, 10, (Cell)1),
            (11, 10, (Cell)2),
            (1, 11, (Cell)0),
            (9, 11, (Cell)3));
    }
}
