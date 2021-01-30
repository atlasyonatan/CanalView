using NUnit.Framework;
using System.Linq;

namespace CanalView.Test
{
    public class RulesTests
    {
        [Test]
        public void LegalSquareTest()
        {
            var board = Board.Blank(3, 3);
            Assert.IsTrue(board.LegalSquare());
            Assert.IsTrue(board.LegalSquare(0));
            Assert.IsTrue(board.LegalSquare(8));
            board[1, 1] = Cell.Full;
            Assert.IsTrue(board.LegalSquare());
            Assert.IsTrue(board.LegalSquare(0));
            Assert.IsTrue(board.LegalSquare(8));
            board[0, 1] = Cell.Full;
            Assert.IsTrue(board.LegalSquare());
            Assert.IsTrue(board.LegalSquare(0));
            Assert.IsTrue(board.LegalSquare(8));
            board[1, 0] = Cell.Full;
            Assert.IsTrue(board.LegalSquare());
            Assert.IsTrue(board.LegalSquare(0));
            Assert.IsTrue(board.LegalSquare(8));
            board[0, 0] = Cell.Full;
            Assert.IsFalse(board.LegalSquare());
            Assert.IsFalse(board.LegalSquare(0));
            Assert.IsTrue(board.LegalSquare(8));
        }

        [Test]
        public void LegalNumbersTest()
        {
            var board = Board.Blank(3, 3);
            board[1, 1] = (Cell)4;
            Assert.AreEqual(0, Enumerable.Range(0, 9).Where(i => !board.LegalNumbers(i % board.GetLength(0), i / board.GetLength(0))).Count());
            Assert.IsTrue(board.LegalNumbers());
            board[1, 1] = (Cell)0;
            board[1, 0] = Cell.Full;
            Assert.AreEqual(5, Enumerable.Range(0, 9).Where(i => !board.LegalNumbers(i % board.GetLength(0), i / board.GetLength(0))).Count());
            Assert.IsFalse(board.LegalNumbers());
        }

        [Test]
        public void LegalPathTest()
        {
            var board = Board.Blank(3, 3);
            Assert.IsTrue(board.LegalPath());
            board[0, 0] = Cell.Full;
            Assert.IsTrue(board.LegalPath());
            board[1, 1] = Cell.Full;
            Assert.IsFalse(board.LegalPath());
            board[0, 1] = Cell.Full;
            Assert.IsTrue(board.LegalPath());
        }
    }
}