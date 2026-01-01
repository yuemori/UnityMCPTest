using System;
using System.Linq;
using NUnit.Framework;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Repositories;

namespace TicTacToe.Tests.EditMode
{
    [TestFixture]
    public class BoardRepositoryTests
    {
        private BoardRepository _repository;
        
        [SetUp]
        public void SetUp()
        {
            _repository = new BoardRepository();
        }
        
        [TearDown]
        public void TearDown()
        {
            _repository?.Dispose();
        }
        
        #region Initial State Tests
        
        [Test]
        public void NewRepository_IsEmpty()
        {
            Assert.IsTrue(_repository.IsEmpty);
            Assert.IsFalse(_repository.IsFull);
        }
        
        [Test]
        public void NewRepository_AllCellsAreEmpty()
        {
            for (int i = 0; i < BoardPosition.TotalCells; i++)
            {
                var cell = _repository.GetCell(new BoardPosition(i));
                Assert.AreEqual(CellState.Empty, cell);
            }
        }
        
        #endregion
        
        #region TryPlaceMark Tests
        
        [Test]
        public void TryPlaceMark_OnEmptyCell_ReturnsTrue()
        {
            var result = _repository.TryPlaceMark(new BoardPosition(0), CellState.X);
            
            Assert.IsTrue(result);
            Assert.AreEqual(CellState.X, _repository.GetCell(new BoardPosition(0)));
        }
        
        [Test]
        public void TryPlaceMark_OnOccupiedCell_ReturnsFalse()
        {
            _repository.TryPlaceMark(new BoardPosition(0), CellState.X);
            
            var result = _repository.TryPlaceMark(new BoardPosition(0), CellState.O);
            
            Assert.IsFalse(result);
            Assert.AreEqual(CellState.X, _repository.GetCell(new BoardPosition(0)));
        }
        
        [Test]
        public void TryPlaceMark_WithEmptyMark_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => 
                _repository.TryPlaceMark(new BoardPosition(0), CellState.Empty));
        }
        
        [Test]
        public void TryPlaceMark_MultipleMarks_BoardNotEmpty()
        {
            _repository.TryPlaceMark(new BoardPosition(0), CellState.X);
            _repository.TryPlaceMark(new BoardPosition(1), CellState.O);
            _repository.TryPlaceMark(new BoardPosition(2), CellState.X);
            
            Assert.IsFalse(_repository.IsEmpty);
        }
        
        #endregion
        
        #region IsFull Tests
        
        [Test]
        public void IsFull_WhenBoardFull_ReturnsTrue()
        {
            // 盤面を埋める
            var marks = new[] { CellState.X, CellState.O, CellState.X, 
                               CellState.O, CellState.X, CellState.O,
                               CellState.O, CellState.X, CellState.O };
            
            for (int i = 0; i < BoardPosition.TotalCells; i++)
            {
                _repository.TryPlaceMark(new BoardPosition(i), marks[i]);
            }
            
            Assert.IsTrue(_repository.IsFull);
            Assert.IsFalse(_repository.IsEmpty);
        }
        
        #endregion
        
        #region Reset Tests
        
        [Test]
        public void Reset_ClearsAllCells()
        {
            _repository.TryPlaceMark(new BoardPosition(0), CellState.X);
            _repository.TryPlaceMark(new BoardPosition(4), CellState.O);
            
            _repository.Reset();
            
            Assert.IsTrue(_repository.IsEmpty);
            
            for (int i = 0; i < BoardPosition.TotalCells; i++)
            {
                Assert.AreEqual(CellState.Empty, _repository.GetCell(new BoardPosition(i)));
            }
        }
        
        #endregion
        
        #region GetEmptyPositions Tests
        
        [Test]
        public void GetEmptyPositions_OnNewBoard_ReturnsAllPositions()
        {
            var emptyPositions = _repository.GetEmptyPositions();
            
            Assert.AreEqual(BoardPosition.TotalCells, emptyPositions.Count);
        }
        
        [Test]
        public void GetEmptyPositions_AfterPlacingMarks_ReturnsCorrectPositions()
        {
            _repository.TryPlaceMark(new BoardPosition(0), CellState.X);
            _repository.TryPlaceMark(new BoardPosition(4), CellState.O);
            _repository.TryPlaceMark(new BoardPosition(8), CellState.X);
            
            var emptyPositions = _repository.GetEmptyPositions();
            
            Assert.AreEqual(6, emptyPositions.Count);
            Assert.IsFalse(emptyPositions.Any(p => p.Index == 0));
            Assert.IsFalse(emptyPositions.Any(p => p.Index == 4));
            Assert.IsFalse(emptyPositions.Any(p => p.Index == 8));
        }
        
        [Test]
        public void GetEmptyPositions_OnFullBoard_ReturnsEmpty()
        {
            for (int i = 0; i < BoardPosition.TotalCells; i++)
            {
                _repository.TryPlaceMark(new BoardPosition(i), i % 2 == 0 ? CellState.X : CellState.O);
            }
            
            var emptyPositions = _repository.GetEmptyPositions();
            
            Assert.AreEqual(0, emptyPositions.Count);
        }
        
        #endregion
        
        #region GetBoardSnapshot Tests
        
        [Test]
        public void GetBoardSnapshot_ReturnsCorrectState()
        {
            _repository.TryPlaceMark(new BoardPosition(0), CellState.X);
            _repository.TryPlaceMark(new BoardPosition(4), CellState.O);
            
            var snapshot = _repository.GetBoardSnapshot();
            
            Assert.AreEqual(BoardPosition.TotalCells, snapshot.Count);
            Assert.AreEqual(CellState.X, snapshot[0]);
            Assert.AreEqual(CellState.O, snapshot[4]);
            Assert.AreEqual(CellState.Empty, snapshot[1]);
        }
        
        [Test]
        public void GetBoardSnapshot_ReturnsCopy_NotReference()
        {
            _repository.TryPlaceMark(new BoardPosition(0), CellState.X);
            
            var snapshot1 = _repository.GetBoardSnapshot();
            _repository.TryPlaceMark(new BoardPosition(1), CellState.O);
            var snapshot2 = _repository.GetBoardSnapshot();
            
            // snapshot1は変更されていない
            Assert.AreEqual(CellState.Empty, snapshot1[1]);
            Assert.AreEqual(CellState.O, snapshot2[1]);
        }
        
        #endregion
        
        #region IsPositionEmpty Tests
        
        [Test]
        public void IsPositionEmpty_OnEmptyCell_ReturnsTrue()
        {
            Assert.IsTrue(_repository.IsPositionEmpty(new BoardPosition(0)));
        }
        
        [Test]
        public void IsPositionEmpty_OnOccupiedCell_ReturnsFalse()
        {
            _repository.TryPlaceMark(new BoardPosition(0), CellState.X);
            
            Assert.IsFalse(_repository.IsPositionEmpty(new BoardPosition(0)));
        }
        
        #endregion
        
        #region Dispose Tests
        
        [Test]
        public void Dispose_CalledTwice_DoesNotThrow()
        {
            _repository.Dispose();
            Assert.DoesNotThrow(() => _repository.Dispose());
        }
        
        [Test]
        public void AfterDispose_MethodsThrowException()
        {
            _repository.Dispose();
            
            Assert.Throws<ObjectDisposedException>(() => _repository.GetCell(new BoardPosition(0)));
            Assert.Throws<ObjectDisposedException>(() => _repository.TryPlaceMark(new BoardPosition(0), CellState.X));
            Assert.Throws<ObjectDisposedException>(() => _repository.Reset());
        }
        
        #endregion
    }
}
