namespace CanalView
{
    public static class Boards
    {
        public static readonly Cell[,] Easy = Board.FromNumbers(5, 5, new (int, int, Cell)[]
            {
                (0,0,(Cell)5),
                (1,2,(Cell)4),
                (3,2,(Cell)3),
                (0,4,(Cell)3),
                (2,4,(Cell)1),
                (4,4,(Cell)4),
            });

        //public static readonly Cell[,] Medium = Board.FromNumbers(8, 8, new (int, int, Cell)[]
        //    {
        //        (0,0,(Cell)5),
        //        (1,2,(Cell)4),
        //        (3,2,(Cell)3),
        //        (0,4,(Cell)3),
        //        (2,4,(Cell)1),
        //        (4,4,(Cell)4),
        //    });
    }
}
