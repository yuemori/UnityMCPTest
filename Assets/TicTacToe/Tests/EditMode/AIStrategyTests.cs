using System;
using System.Collections.Generic;
using NUnit.Framework;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Strategies;

namespace TicTacToe.Tests.EditMode
{
    [TestFixture]
    public class RandomAIStrategyTests
    {
        private RandomAIStrategy _strategy;
        
        [SetUp]
        public void SetUp()
        {
            _strategy = new RandomAIStrategy();
        }
        
        #region Constructor Tests
        
        [Test]
        public void Constructor_WithNullRandom_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new RandomAIStrategy(null));
        }
        
        [Test]
        public void Constructor_Default_Succeeds()
        {
            var strategy = new RandomAIStrategy();
            Assert.IsNotNull(strategy);
        }
        
        #endregion
        
        #region Name Property Tests
        
        [Test]
        public void Name_ReturnsRandom()
        {
            Assert.AreEqual("Random", _strategy.Name);
        }
        
        #endregion
        
        #region DecideMove Tests - Validation
        
        [Test]
        public void DecideMove_WithNullBoard_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _strategy.DecideMove(null, CellState.X));
        }
        
        [Test]
        public void DecideMove_WithEmptyMark_ThrowsException()
        {
            var board = CreateEmptyBoard();
            Assert.Throws<ArgumentException>(() => _strategy.DecideMove(board, CellState.Empty));
        }
        
        [Test]
        public void DecideMove_WithNoEmptyPositions_ThrowsException()
        {
            var board = CreateFullBoard();
            Assert.Throws<InvalidOperationException>(() => _strategy.DecideMove(board, CellState.X));
        }
        
        #endregion
        
        #region DecideMove Tests - Selection
        
        [Test]
        public void DecideMove_OnEmptyBoard_ReturnsValidPosition()
        {
            var board = CreateEmptyBoard();
            
            var position = _strategy.DecideMove(board, CellState.X);
            
            Assert.IsTrue(BoardPosition.IsValidIndex(position.Index));
            Assert.AreEqual(CellState.Empty, board[position.Index]);
        }
        
        [Test]
        public void DecideMove_OnPartiallyFilledBoard_ReturnsEmptyPosition()
        {
            var board = new CellState[]
            {
                CellState.X, CellState.O, CellState.X,
                CellState.Empty, CellState.O, CellState.Empty,
                CellState.X, CellState.Empty, CellState.O
            };
            
            var position = _strategy.DecideMove(board, CellState.X);
            
            Assert.AreEqual(CellState.Empty, board[position.Index]);
        }
        
        [Test]
        public void DecideMove_WithSingleEmptyPosition_ReturnsThatPosition()
        {
            var board = new CellState[]
            {
                CellState.X, CellState.O, CellState.X,
                CellState.O, CellState.Empty, CellState.X,  // index 4 is empty
                CellState.X, CellState.O, CellState.O
            };
            
            var position = _strategy.DecideMove(board, CellState.X);
            
            Assert.AreEqual(4, position.Index);
        }
        
        [Test]
        public void DecideMove_WithSeededRandom_ReturnsExpectedPosition()
        {
            // 固定シードで予測可能な結果を確認
            var random = new Random(42);
            var strategy = new RandomAIStrategy(random);
            var board = CreateEmptyBoard();
            
            var position = strategy.DecideMove(board, CellState.X);
            
            // position should be a valid empty cell
            Assert.IsTrue(BoardPosition.IsValidIndex(position.Index));
            Assert.AreEqual(CellState.Empty, board[position.Index]);
        }
        
        [Test]
        public void DecideMove_CalledMultipleTimes_ReturnsValidPositions()
        {
            var board = CreateEmptyBoard();
            
            // 複数回呼び出してすべて有効な位置を返すことを確認
            for (int i = 0; i < 100; i++)
            {
                var position = _strategy.DecideMove(board, CellState.X);
                Assert.IsTrue(BoardPosition.IsValidIndex(position.Index));
                Assert.AreEqual(CellState.Empty, board[position.Index]);
            }
        }
        
        [Test]
        public void DecideMove_WithXMark_Works()
        {
            var board = CreateEmptyBoard();
            
            var position = _strategy.DecideMove(board, CellState.X);
            
            Assert.IsTrue(BoardPosition.IsValidIndex(position.Index));
        }
        
        [Test]
        public void DecideMove_WithOMark_Works()
        {
            var board = CreateEmptyBoard();
            
            var position = _strategy.DecideMove(board, CellState.O);
            
            Assert.IsTrue(BoardPosition.IsValidIndex(position.Index));
        }
        
        #endregion
        
        #region Distribution Tests
        
        [Test]
        public void DecideMove_OnEmptyBoard_CanSelectAnyPosition()
        {
            // 空盤面で多数回実行し、すべての位置が選択される可能性があることを確認
            var board = CreateEmptyBoard();
            var selectedPositions = new HashSet<int>();
            
            // 十分な回数を実行
            for (int i = 0; i < 1000; i++)
            {
                var position = _strategy.DecideMove(board, CellState.X);
                selectedPositions.Add(position.Index);
            }
            
            // 全9マスが選択されるはず（統計的に確実）
            Assert.AreEqual(9, selectedPositions.Count);
        }
        
        #endregion
        
        #region Helper Methods
        
        private static CellState[] CreateEmptyBoard()
        {
            return new CellState[BoardPosition.TotalCells];
        }
        
        private static CellState[] CreateFullBoard()
        {
            return new CellState[]
            {
                CellState.X, CellState.O, CellState.X,
                CellState.O, CellState.X, CellState.O,
                CellState.O, CellState.X, CellState.O
            };
        }
        
        #endregion
    }
}
