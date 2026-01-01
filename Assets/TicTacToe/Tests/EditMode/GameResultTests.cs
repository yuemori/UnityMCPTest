using System;
using NUnit.Framework;
using TicTacToe.Core.Domain;

namespace TicTacToe.Tests.EditMode
{
    [TestFixture]
    public class GameResultTests
    {
        #region Factory Method Tests
        
        [Test]
        public void InProgress_ReturnsCorrectState()
        {
            var result = GameResult.InProgress();
            Assert.AreEqual(GameState.InProgress, result.State);
            Assert.IsNull(result.Winner);
            Assert.IsNull(result.WinningLine);
            Assert.IsFalse(result.IsGameOver);
        }
        
        [Test]
        public void Draw_ReturnsCorrectState()
        {
            var result = GameResult.Draw();
            Assert.AreEqual(GameState.Draw, result.State);
            Assert.IsNull(result.Winner);
            Assert.IsNull(result.WinningLine);
            Assert.IsTrue(result.IsGameOver);
        }
        
        [TestCase(CellState.X, WinLine.Row0)]
        [TestCase(CellState.O, WinLine.Column1)]
        [TestCase(CellState.X, WinLine.DiagonalMain)]
        public void Win_ReturnsCorrectState(CellState winner, WinLine winLine)
        {
            var result = GameResult.Win(winner, winLine);
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(winner, result.Winner);
            Assert.AreEqual(winLine, result.WinningLine);
            Assert.IsTrue(result.IsGameOver);
        }
        
        [Test]
        public void Win_WithEmptyWinner_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => GameResult.Win(CellState.Empty, WinLine.Row0));
        }
        
        #endregion
        
        #region Equality Tests
        
        [Test]
        public void Equals_SameInProgress_ReturnsTrue()
        {
            var result1 = GameResult.InProgress();
            var result2 = GameResult.InProgress();
            Assert.IsTrue(result1.Equals(result2));
            Assert.IsTrue(result1 == result2);
        }
        
        [Test]
        public void Equals_DifferentWinners_ReturnsFalse()
        {
            var result1 = GameResult.Win(CellState.X, WinLine.Row0);
            var result2 = GameResult.Win(CellState.O, WinLine.Row0);
            Assert.IsFalse(result1.Equals(result2));
            Assert.IsTrue(result1 != result2);
        }
        
        #endregion
        
        #region ToString Tests
        
        [Test]
        public void ToString_InProgress_ReturnsExpected()
        {
            var result = GameResult.InProgress();
            Assert.AreEqual("InProgress", result.ToString());
        }
        
        [Test]
        public void ToString_Draw_ReturnsExpected()
        {
            var result = GameResult.Draw();
            Assert.AreEqual("Draw", result.ToString());
        }
        
        [Test]
        public void ToString_Win_ReturnsExpected()
        {
            var result = GameResult.Win(CellState.X, WinLine.DiagonalMain);
            Assert.AreEqual("Win: X", result.ToString());
        }
        
        #endregion
    }
}
