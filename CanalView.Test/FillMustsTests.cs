using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using static CanalView.Musts;


namespace CanalView.Test
{
    public class FillMustsTests
    {
        [Test]
        public void Full_LShape()
        {
            //var board = Board.Blank(5, 5).Add((2, 2, Cell.Full));
            //var clone = board.Copy();
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(2, 10));
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(-1, 0));
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(1, 1));
            //Assert.IsTrue(clone.FillMusts_Full_LShape(2, 2));
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(1, 2));
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(2, 1));
            //Assert.AreEqual(board, clone);
            
            //board.Add((1, 2, Cell.Full));
            //clone = board.Copy();
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(1, 1));
            //Assert.IsTrue(clone.FillMusts_Full_LShape(2, 2));
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(1, 2));
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(2, 1));
            //Assert.AreEqual(board, clone);

            //board.Add((1, 1, Cell.Full));
            //clone = board.Copy();
            //var expected = board.Copy().Add((2, 1, Cell.Empty));
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(1, 1));
            //Assert.IsTrue(clone.FillMusts_Full_LShape(2, 2));
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(1, 2));
            //Assert.IsTrue(board.Copy().FillMusts_Full_LShape(2, 1));
            //Assert.AreEqual(expected, clone);

            //board.Add((2, 1, Cell.Full));
            //clone = board.Copy();
            //Assert.IsFalse(clone.FillMusts_Full_LShape(2, 2));
            //Assert.AreEqual(expected, clone);


        }
    }
}
