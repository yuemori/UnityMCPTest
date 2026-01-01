using System;
using NUnit.Framework;
using TicTacToe.Core.Domain;

namespace TicTacToe.Tests.EditMode
{
    [TestFixture]
    public class BoardPositionTests
    {
        #region Constructor Tests
        
        [TestCase(0)]
        [TestCase(4)]
        [TestCase(8)]
        public void Constructor_WithValidIndex_CreatesPosition(int index)
        {
            var position = new BoardPosition(index);
            Assert.AreEqual(index, position.Index);
        }
        
        [TestCase(-1)]
        [TestCase(9)]
        [TestCase(100)]
        public void Constructor_WithInvalidIndex_ThrowsException(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BoardPosition(index));
        }
        
        [TestCase(0, 0, 0)]
        [TestCase(0, 1, 1)]
        [TestCase(1, 0, 3)]
        [TestCase(1, 1, 4)]
        [TestCase(2, 2, 8)]
        public void Constructor_WithRowAndColumn_CalculatesCorrectIndex(int row, int column, int expectedIndex)
        {
            var position = new BoardPosition(row, column);
            Assert.AreEqual(expectedIndex, position.Index);
            Assert.AreEqual(row, position.Row);
            Assert.AreEqual(column, position.Column);
        }
        
        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(3, 0)]
        [TestCase(0, 3)]
        public void Constructor_WithInvalidRowOrColumn_ThrowsException(int row, int column)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BoardPosition(row, column));
        }
        
        #endregion
        
        #region Property Tests
        
        [TestCase(0, 0, 0)]
        [TestCase(4, 1, 1)]
        [TestCase(8, 2, 2)]
        public void RowAndColumn_ReturnsCorrectValues(int index, int expectedRow, int expectedColumn)
        {
            var position = new BoardPosition(index);
            Assert.AreEqual(expectedRow, position.Row);
            Assert.AreEqual(expectedColumn, position.Column);
        }
        
        #endregion
        
        #region Validation Tests
        
        [TestCase(0, true)]
        [TestCase(8, true)]
        [TestCase(-1, false)]
        [TestCase(9, false)]
        public void IsValidIndex_ReturnsCorrectResult(int index, bool expected)
        {
            Assert.AreEqual(expected, BoardPosition.IsValidIndex(index));
        }
        
        [TestCase(0, 0, true)]
        [TestCase(2, 2, true)]
        [TestCase(-1, 0, false)]
        [TestCase(3, 0, false)]
        public void IsValidPosition_ReturnsCorrectResult(int row, int column, bool expected)
        {
            Assert.AreEqual(expected, BoardPosition.IsValidPosition(row, column));
        }
        
        #endregion
        
        #region TryCreate Tests
        
        [Test]
        public void TryCreate_WithValidIndex_ReturnsPosition()
        {
            var result = BoardPosition.TryCreate(4);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Value.Index);
        }
        
        [Test]
        public void TryCreate_WithInvalidIndex_ReturnsNull()
        {
            var result = BoardPosition.TryCreate(-1);
            Assert.IsNull(result);
        }
        
        #endregion
        
        #region Equality Tests
        
        [Test]
        public void Equals_SameIndex_ReturnsTrue()
        {
            var pos1 = new BoardPosition(4);
            var pos2 = new BoardPosition(1, 1);
            Assert.IsTrue(pos1.Equals(pos2));
            Assert.IsTrue(pos1 == pos2);
        }
        
        [Test]
        public void Equals_DifferentIndex_ReturnsFalse()
        {
            var pos1 = new BoardPosition(4);
            var pos2 = new BoardPosition(5);
            Assert.IsFalse(pos1.Equals(pos2));
            Assert.IsTrue(pos1 != pos2);
        }
        
        #endregion
        
        #region Implicit Conversion Tests
        
        [Test]
        public void ImplicitConversion_FromInt_CreatesPosition()
        {
            BoardPosition position = 4;
            Assert.AreEqual(4, position.Index);
        }
        
        #endregion
        
        #region ToString Tests
        
        [TestCase(0, "(0, 0)")]
        [TestCase(4, "(1, 1)")]
        [TestCase(8, "(2, 2)")]
        public void ToString_ReturnsExpectedFormat(int index, string expected)
        {
            var position = new BoardPosition(index);
            Assert.AreEqual(expected, position.ToString());
        }
        
        #endregion
    }
}
