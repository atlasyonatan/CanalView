using CanalView;
using NUnit.Framework;
using System;

namespace BoardSerialization.Test
{
    public class VisualSerilizerTests
    {
        [Test]
        public void ValidSerializeDeserializeBoard()
        {
            var board = Puzzles.BuiltIn.Hard_12x12;
            var serialized = VisualSerializer.Serialize(board);
            var deserialized = VisualSerializer.Deserialize(serialized);
            Assert.IsTrue(board.SequenceEquals(deserialized));
        }
        [Test]
        public void InvalidSerializeDeserializeBoard()
        {
            var board = Puzzles.BuiltIn.Hard_12x12;
            var serialized = VisualSerializer.Serialize(board);
            var badCellCount = "_," + serialized;
            Assert.Throws<InvalidOperationException>(() => VisualSerializer.Deserialize(badCellCount));
            var badCellChar = serialized.Replace("_", "invalidCellString");
            Assert.Throws<ArgumentException>(() => VisualSerializer.Deserialize(badCellChar));
        }
    }
}