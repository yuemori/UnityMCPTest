using System;
using NUnit.Framework;
using TicTacToe.Core.Domain;

namespace TicTacToe.Tests.EditMode
{
    [TestFixture]
    public class TurnInfoTests
    {
        #region Constructor Tests
        
        [Test]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            var turnInfo = new TurnInfo(CellState.X, 1, PlayerType.Human, PlayerType.AI);
            Assert.AreEqual(CellState.X, turnInfo.CurrentMark);
            Assert.AreEqual(1, turnInfo.TurnNumber);
            Assert.AreEqual(PlayerType.Human, turnInfo.XPlayerType);
            Assert.AreEqual(PlayerType.AI, turnInfo.OPlayerType);
            Assert.AreEqual(PlayerType.Human, turnInfo.CurrentPlayerType);
        }
        
        [Test]
        public void Constructor_WithEmptyMark_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => 
                new TurnInfo(CellState.Empty, 1, PlayerType.Human, PlayerType.AI));
        }
        
        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_WithInvalidTurnNumber_ThrowsException(int turnNumber)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new TurnInfo(CellState.X, turnNumber, PlayerType.Human, PlayerType.AI));
        }
        
        #endregion
        
        #region CreateInitial Tests
        
        [Test]
        public void CreateInitial_ReturnsCorrectTurn()
        {
            var turnInfo = TurnInfo.CreateInitial(PlayerType.Human, PlayerType.AI);
            Assert.AreEqual(CellState.X, turnInfo.CurrentMark);
            Assert.AreEqual(1, turnInfo.TurnNumber);
            Assert.AreEqual(PlayerType.Human, turnInfo.XPlayerType);
            Assert.AreEqual(PlayerType.AI, turnInfo.OPlayerType);
        }
        
        #endregion
        
        #region NextTurn Tests
        
        [Test]
        public void NextTurn_FromX_ReturnsOTurn()
        {
            var turnInfo = TurnInfo.CreateInitial(PlayerType.Human, PlayerType.AI);
            var nextTurn = turnInfo.NextTurn();
            Assert.AreEqual(CellState.O, nextTurn.CurrentMark);
            Assert.AreEqual(2, nextTurn.TurnNumber);
            Assert.AreEqual(PlayerType.AI, nextTurn.CurrentPlayerType);
        }
        
        [Test]
        public void NextTurn_FromO_ReturnsXTurn()
        {
            var turnInfo = new TurnInfo(CellState.O, 2, PlayerType.Human, PlayerType.AI);
            var nextTurn = turnInfo.NextTurn();
            Assert.AreEqual(CellState.X, nextTurn.CurrentMark);
            Assert.AreEqual(3, nextTurn.TurnNumber);
        }
        
        #endregion
        
        #region CurrentPlayerType Tests
        
        [Test]
        public void CurrentPlayerType_WhenX_ReturnsXPlayerType()
        {
            var turnInfo = new TurnInfo(CellState.X, 1, PlayerType.Human, PlayerType.AI);
            Assert.AreEqual(PlayerType.Human, turnInfo.CurrentPlayerType);
            Assert.IsTrue(turnInfo.IsCurrentPlayerHuman);
            Assert.IsFalse(turnInfo.IsCurrentPlayerAI);
        }
        
        [Test]
        public void CurrentPlayerType_WhenO_ReturnsOPlayerType()
        {
            var turnInfo = new TurnInfo(CellState.O, 2, PlayerType.Human, PlayerType.AI);
            Assert.AreEqual(PlayerType.AI, turnInfo.CurrentPlayerType);
            Assert.IsFalse(turnInfo.IsCurrentPlayerHuman);
            Assert.IsTrue(turnInfo.IsCurrentPlayerAI);
        }
        
        #endregion
        
        #region GetOpponentMark Tests
        
        [Test]
        public void GetOpponentMark_WhenX_ReturnsO()
        {
            var turnInfo = new TurnInfo(CellState.X, 1, PlayerType.Human, PlayerType.AI);
            Assert.AreEqual(CellState.O, turnInfo.GetOpponentMark());
        }
        
        [Test]
        public void GetOpponentMark_WhenO_ReturnsX()
        {
            var turnInfo = new TurnInfo(CellState.O, 2, PlayerType.Human, PlayerType.AI);
            Assert.AreEqual(CellState.X, turnInfo.GetOpponentMark());
        }
        
        #endregion
        
        #region ToString Tests
        
        [Test]
        public void ToString_ReturnsExpectedFormat()
        {
            var turnInfo = new TurnInfo(CellState.X, 1, PlayerType.Human, PlayerType.AI);
            Assert.AreEqual("Turn 1: X (Human)", turnInfo.ToString());
        }
        
        #endregion
    }
}
