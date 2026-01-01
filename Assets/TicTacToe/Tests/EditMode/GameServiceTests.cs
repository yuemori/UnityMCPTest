using System;
using NUnit.Framework;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;

namespace TicTacToe.Tests.EditMode
{
    [TestFixture]
    public class GameServiceTests
    {
        private BoardRepository _boardRepository;
        private GameService _gameService;
        
        [SetUp]
        public void SetUp()
        {
            _boardRepository = new BoardRepository();
            _gameService = new GameService(_boardRepository);
        }
        
        [TearDown]
        public void TearDown()
        {
            _gameService?.Dispose();
            _boardRepository?.Dispose();
        }
        
        #region Constructor Tests
        
        [Test]
        public void Constructor_WithNullRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new GameService(null));
        }
        
        #endregion
        
        #region StartNewGame Tests
        
        [Test]
        public void StartNewGame_InitializesCorrectly()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            
            Assert.IsFalse(_gameService.IsGameOver);
        }
        
        [Test]
        public void StartNewGame_ResetsBoard()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _gameService.PlaceMark(new BoardPosition(0));
            
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            
            Assert.IsTrue(_boardRepository.IsEmpty);
        }
        
        #endregion
        
        #region PlaceMark Tests
        
        [Test]
        public void PlaceMark_OnValidPosition_ReturnsTrue()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            
            var result = _gameService.PlaceMark(new BoardPosition(4));
            
            Assert.IsTrue(result);
            Assert.AreEqual(CellState.X, _boardRepository.GetCell(new BoardPosition(4)));
        }
        
        [Test]
        public void PlaceMark_OnOccupiedPosition_ReturnsFalse()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _gameService.PlaceMark(new BoardPosition(4));
            
            var result = _gameService.PlaceMark(new BoardPosition(4));
            
            Assert.IsFalse(result);
        }
        
        [Test]
        public void PlaceMark_AfterGameOver_ReturnsFalse()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // Xが勝利（横一列）
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(3)); // O
            _gameService.PlaceMark(new BoardPosition(1)); // X
            _gameService.PlaceMark(new BoardPosition(4)); // O
            _gameService.PlaceMark(new BoardPosition(2)); // X wins
            
            Assert.IsTrue(_gameService.IsGameOver);
            
            var result = _gameService.PlaceMark(new BoardPosition(5));
            Assert.IsFalse(result);
        }
        
        [Test]
        public void PlaceMark_AlternatesTurns()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            
            _gameService.PlaceMark(new BoardPosition(0)); // X at 0
            _gameService.PlaceMark(new BoardPosition(1)); // O at 1
            
            Assert.AreEqual(CellState.X, _boardRepository.GetCell(new BoardPosition(0)));
            Assert.AreEqual(CellState.O, _boardRepository.GetCell(new BoardPosition(1)));
        }
        
        #endregion
        
        #region Win Detection Tests - Rows
        
        [Test]
        public void CheckGameResult_Row0Win_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // X: 0, 1, 2 (top row)
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(3)); // O
            _gameService.PlaceMark(new BoardPosition(1)); // X
            _gameService.PlaceMark(new BoardPosition(4)); // O
            _gameService.PlaceMark(new BoardPosition(2)); // X wins
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(CellState.X, result.Winner);
            Assert.AreEqual(WinLine.Row0, result.WinningLine);
        }
        
        [Test]
        public void CheckGameResult_Row1Win_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // X: 3, 4, 5 (middle row)
            _gameService.PlaceMark(new BoardPosition(3)); // X
            _gameService.PlaceMark(new BoardPosition(0)); // O
            _gameService.PlaceMark(new BoardPosition(4)); // X
            _gameService.PlaceMark(new BoardPosition(1)); // O
            _gameService.PlaceMark(new BoardPosition(5)); // X wins
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(CellState.X, result.Winner);
            Assert.AreEqual(WinLine.Row1, result.WinningLine);
        }
        
        [Test]
        public void CheckGameResult_Row2Win_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // X: 6, 7, 8 (bottom row)
            _gameService.PlaceMark(new BoardPosition(6)); // X
            _gameService.PlaceMark(new BoardPosition(0)); // O
            _gameService.PlaceMark(new BoardPosition(7)); // X
            _gameService.PlaceMark(new BoardPosition(1)); // O
            _gameService.PlaceMark(new BoardPosition(8)); // X wins
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(CellState.X, result.Winner);
            Assert.AreEqual(WinLine.Row2, result.WinningLine);
        }
        
        #endregion
        
        #region Win Detection Tests - Columns
        
        [Test]
        public void CheckGameResult_Column0Win_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // X: 0, 3, 6 (left column)
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(1)); // O
            _gameService.PlaceMark(new BoardPosition(3)); // X
            _gameService.PlaceMark(new BoardPosition(2)); // O
            _gameService.PlaceMark(new BoardPosition(6)); // X wins
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(CellState.X, result.Winner);
            Assert.AreEqual(WinLine.Column0, result.WinningLine);
        }
        
        [Test]
        public void CheckGameResult_Column1Win_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // X: 1, 4, 7 (middle column)
            _gameService.PlaceMark(new BoardPosition(1)); // X
            _gameService.PlaceMark(new BoardPosition(0)); // O
            _gameService.PlaceMark(new BoardPosition(4)); // X
            _gameService.PlaceMark(new BoardPosition(2)); // O
            _gameService.PlaceMark(new BoardPosition(7)); // X wins
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(CellState.X, result.Winner);
            Assert.AreEqual(WinLine.Column1, result.WinningLine);
        }
        
        [Test]
        public void CheckGameResult_Column2Win_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // X: 2, 5, 8 (right column)
            _gameService.PlaceMark(new BoardPosition(2)); // X
            _gameService.PlaceMark(new BoardPosition(0)); // O
            _gameService.PlaceMark(new BoardPosition(5)); // X
            _gameService.PlaceMark(new BoardPosition(1)); // O
            _gameService.PlaceMark(new BoardPosition(8)); // X wins
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(CellState.X, result.Winner);
            Assert.AreEqual(WinLine.Column2, result.WinningLine);
        }
        
        #endregion
        
        #region Win Detection Tests - Diagonals
        
        [Test]
        public void CheckGameResult_DiagonalMainWin_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // X: 0, 4, 8 (main diagonal)
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(1)); // O
            _gameService.PlaceMark(new BoardPosition(4)); // X
            _gameService.PlaceMark(new BoardPosition(2)); // O
            _gameService.PlaceMark(new BoardPosition(8)); // X wins
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(CellState.X, result.Winner);
            Assert.AreEqual(WinLine.DiagonalMain, result.WinningLine);
        }
        
        [Test]
        public void CheckGameResult_DiagonalAntiWin_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // X: 2, 4, 6 (anti diagonal)
            _gameService.PlaceMark(new BoardPosition(2)); // X
            _gameService.PlaceMark(new BoardPosition(0)); // O
            _gameService.PlaceMark(new BoardPosition(4)); // X
            _gameService.PlaceMark(new BoardPosition(1)); // O
            _gameService.PlaceMark(new BoardPosition(6)); // X wins
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(CellState.X, result.Winner);
            Assert.AreEqual(WinLine.DiagonalAnti, result.WinningLine);
        }
        
        #endregion
        
        #region Win Detection Tests - O Player Win
        
        [Test]
        public void CheckGameResult_OPlayerWins_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // O wins with row 0
            _gameService.PlaceMark(new BoardPosition(3)); // X
            _gameService.PlaceMark(new BoardPosition(0)); // O
            _gameService.PlaceMark(new BoardPosition(4)); // X
            _gameService.PlaceMark(new BoardPosition(1)); // O
            _gameService.PlaceMark(new BoardPosition(6)); // X
            _gameService.PlaceMark(new BoardPosition(2)); // O wins
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Win, result.State);
            Assert.AreEqual(CellState.O, result.Winner);
            Assert.AreEqual(WinLine.Row0, result.WinningLine);
        }
        
        #endregion
        
        #region Draw Detection Tests
        
        [Test]
        public void CheckGameResult_Draw_ReturnsCorrectResult()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // 引き分けパターン:
            // X | O | X
            // X | X | O
            // O | X | O
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(1)); // O
            _gameService.PlaceMark(new BoardPosition(2)); // X
            _gameService.PlaceMark(new BoardPosition(5)); // O
            _gameService.PlaceMark(new BoardPosition(3)); // X
            _gameService.PlaceMark(new BoardPosition(6)); // O
            _gameService.PlaceMark(new BoardPosition(4)); // X
            _gameService.PlaceMark(new BoardPosition(8)); // O
            _gameService.PlaceMark(new BoardPosition(7)); // X - Draw
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.Draw, result.State);
            Assert.IsNull(result.Winner);
            Assert.IsNull(result.WinningLine);
        }
        
        #endregion
        
        #region InProgress Tests
        
        [Test]
        public void CheckGameResult_GameInProgress_ReturnsInProgress()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(4)); // O
            
            var result = _gameService.CheckGameResult();
            Assert.AreEqual(GameState.InProgress, result.State);
            Assert.IsFalse(result.IsGameOver);
        }
        
        #endregion
        
        #region FindWinningMove Tests
        
        [Test]
        public void FindWinningMove_WhenWinPossible_ReturnsPosition()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // X has 0, 1 - needs 2 to win
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(3)); // O
            _gameService.PlaceMark(new BoardPosition(1)); // X
            _gameService.PlaceMark(new BoardPosition(4)); // O
            
            var winningMove = _gameService.FindWinningMove(CellState.X);
            
            Assert.IsNotNull(winningMove);
            Assert.AreEqual(2, winningMove.Value.Index);
        }
        
        [Test]
        public void FindWinningMove_WhenNoWinPossible_ReturnsNull()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _gameService.PlaceMark(new BoardPosition(0)); // X
            
            var winningMove = _gameService.FindWinningMove(CellState.X);
            
            Assert.IsNull(winningMove);
        }
        
        [Test]
        public void FindWinningMove_WithEmptyMark_ThrowsException()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            
            Assert.Throws<ArgumentException>(() => _gameService.FindWinningMove(CellState.Empty));
        }
        
        #endregion
        
        #region GetAvailableMoves Tests
        
        [Test]
        public void GetAvailableMoves_ReturnsCorrectPositions()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(4)); // O
            
            var availableMoves = _gameService.GetAvailableMoves();
            
            Assert.AreEqual(7, availableMoves.Count);
        }
        
        #endregion
        
        #region Dispose Tests
        
        [Test]
        public void AfterDispose_MethodsThrowException()
        {
            _gameService.Dispose();
            
            Assert.Throws<ObjectDisposedException>(() => 
                _gameService.StartNewGame(PlayerType.Human, PlayerType.Human));
            Assert.Throws<ObjectDisposedException>(() => 
                _gameService.PlaceMark(new BoardPosition(0)));
            Assert.Throws<ObjectDisposedException>(() => 
                _gameService.CheckGameResult());
        }
        
        #endregion
    }
}
