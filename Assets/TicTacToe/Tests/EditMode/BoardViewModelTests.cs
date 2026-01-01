using System;
using NUnit.Framework;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;
using TicTacToe.Presentation.Board;

namespace TicTacToe.Tests.EditMode.Presentation
{
    /// <summary>
    /// BoardViewModelの単体テスト
    /// </summary>
    [TestFixture]
    public class BoardViewModelTests
    {
        private BoardRepository _boardRepository;
        private GameService _gameService;
        private BoardViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _boardRepository = new BoardRepository();
            _gameService = new GameService(_boardRepository);
            _viewModel = new BoardViewModel(_gameService);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
            _gameService?.Dispose();
            _boardRepository?.Dispose();
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithNullGameService_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BoardViewModel(null));
        }

        [Test]
        public void Constructor_Creates9Cells()
        {
            // Assert
            Assert.AreEqual(9, _viewModel.Cells.Count);
        }

        [Test]
        public void Constructor_AllCellsHaveCorrectPosition()
        {
            // Assert
            for (int i = 0; i < 9; i++)
            {
                Assert.AreEqual(i, _viewModel.Cells[i].Position.Index);
            }
        }

        [Test]
        public void Constructor_InitializesAsBoardInteractable()
        {
            // Assert
            Assert.IsTrue(_viewModel.IsBoardInteractable.CurrentValue);
        }

        #endregion

        #region Initialize Tests

        [Test]
        public void Initialize_SetsIsInitialized()
        {
            // Act
            _viewModel.Initialize();

            // Assert
            Assert.IsTrue(_viewModel.IsInitialized);
        }

        [Test]
        public void Initialize_CalledMultipleTimes_OnlyInitializesOnce()
        {
            // Act
            _viewModel.Initialize();
            _viewModel.Initialize();

            // Assert - no exception thrown
            Assert.IsTrue(_viewModel.IsInitialized);
        }

        #endregion

        #region Cell Click Integration Tests

        [Test]
        public void CellClick_WhenGameStarted_PlacesMark()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            _viewModel.Initialize();

            // Act - クリックをシミュレート
            _viewModel.Cells[0].OnClick();

            // Assert
            Assert.AreEqual(CellState.X, _viewModel.Cells[0].CellState.CurrentValue);
        }

        [Test]
        public void CellClick_OnOccupiedCell_DoesNothing()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _viewModel.Initialize();
            _viewModel.Cells[0].OnClick(); // X places

            // Act - 同じセルに再度クリック
            var stateBefore = _viewModel.Cells[0].CellState.CurrentValue;
            _viewModel.Cells[0].OnClick();

            // Assert
            Assert.AreEqual(stateBefore, _viewModel.Cells[0].CellState.CurrentValue);
        }

        [Test]
        public void CellClick_AlternatesPlayers()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _viewModel.Initialize();

            // Act
            _viewModel.Cells[0].OnClick(); // X
            _viewModel.Cells[1].OnClick(); // O

            // Assert
            Assert.AreEqual(CellState.X, _viewModel.Cells[0].CellState.CurrentValue);
            Assert.AreEqual(CellState.O, _viewModel.Cells[1].CellState.CurrentValue);
        }

        #endregion

        #region Board State Sync Tests

        [Test]
        public void Initialize_SyncsWithExistingBoardState()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(4)); // O

            // Act
            _viewModel.Initialize();

            // Assert
            Assert.AreEqual(CellState.X, _viewModel.Cells[0].CellState.CurrentValue);
            Assert.AreEqual(CellState.O, _viewModel.Cells[4].CellState.CurrentValue);
        }

        [Test]
        public void OnMarkPlaced_UpdatesCellState()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);

            // Act
            _gameService.PlaceMark(new BoardPosition(0));

            // Assert
            Assert.AreEqual(CellState.X, _viewModel.Cells[0].CellState.CurrentValue);
        }

        #endregion

        #region Clickable State Tests

        [Test]
        public void CellClickable_AfterMarkPlaced_IsFalse()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _viewModel.Initialize();

            // Act
            _viewModel.Cells[0].OnClick();

            // Assert
            Assert.IsFalse(_viewModel.Cells[0].IsClickable.CurrentValue);
        }

        [Test]
        public void CellClickable_EmptyCells_StayClickable()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _viewModel.Initialize();

            // Act
            _viewModel.Cells[0].OnClick();

            // Assert - 他のセルはクリック可能
            Assert.IsTrue(_viewModel.Cells[1].IsClickable.CurrentValue);
            Assert.IsTrue(_viewModel.Cells[4].IsClickable.CurrentValue);
        }

        [Test]
        public void BoardInteractable_WhenGameOver_IsFalse()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _viewModel.Initialize();

            // Act - X wins with top row
            _viewModel.Cells[0].OnClick(); // X
            _viewModel.Cells[3].OnClick(); // O
            _viewModel.Cells[1].OnClick(); // X
            _viewModel.Cells[4].OnClick(); // O
            _viewModel.Cells[2].OnClick(); // X wins

            // Assert
            Assert.IsFalse(_viewModel.IsBoardInteractable.CurrentValue);
        }

        [Test]
        public void AllCells_WhenGameOver_AreNotClickable()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _viewModel.Initialize();

            // Act - X wins with top row
            _viewModel.Cells[0].OnClick(); // X
            _viewModel.Cells[3].OnClick(); // O
            _viewModel.Cells[1].OnClick(); // X
            _viewModel.Cells[4].OnClick(); // O
            _viewModel.Cells[2].OnClick(); // X wins

            // Assert - 全セルがクリック不可
            foreach (var cell in _viewModel.Cells)
            {
                Assert.IsFalse(cell.IsClickable.CurrentValue);
            }
        }

        #endregion

        #region AI Turn Tests

        [Test]
        public void CellClickable_DuringAITurn_IsFalse()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.AI, PlayerType.Human); // AI first
            _viewModel.Initialize();

            // Assert - AIのターン中は全セルがクリック不可
            foreach (var cell in _viewModel.Cells)
            {
                Assert.IsFalse(cell.IsClickable.CurrentValue);
            }
        }

        [Test]
        public void CellClickable_AfterAITurn_IsTrue()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            _viewModel.Initialize();

            // Assert - Humanのターンなのでクリック可能
            foreach (var cell in _viewModel.Cells)
            {
                Assert.IsTrue(cell.IsClickable.CurrentValue);
            }
        }

        #endregion

        #region Reset Tests

        [Test]
        public void Reset_ClearsAllCells()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _viewModel.Initialize();
            _viewModel.Cells[0].OnClick();
            _viewModel.Cells[1].OnClick();

            // Act
            _viewModel.Reset();

            // Assert
            foreach (var cell in _viewModel.Cells)
            {
                Assert.AreEqual(CellState.Empty, cell.CellState.CurrentValue);
            }
        }

        [Test]
        public void Reset_RestoresClickability()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _viewModel.Initialize();
            _viewModel.Cells[0].OnClick();

            // Act
            _viewModel.Reset();

            // Assert
            foreach (var cell in _viewModel.Cells)
            {
                Assert.IsTrue(cell.IsClickable.CurrentValue);
            }
        }

        [Test]
        public void Reset_RestoresBoardInteractable()
        {
            // Arrange
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            _viewModel.Initialize();
            
            // Xが勝利
            _viewModel.Cells[0].OnClick();
            _viewModel.Cells[3].OnClick();
            _viewModel.Cells[1].OnClick();
            _viewModel.Cells[4].OnClick();
            _viewModel.Cells[2].OnClick();

            // Act
            _viewModel.Reset();

            // Assert
            Assert.IsTrue(_viewModel.IsBoardInteractable.CurrentValue);
        }

        #endregion

        #region Dispose Tests

        [Test]
        public void Dispose_SetsIsDisposed()
        {
            // Act
            _viewModel.Dispose();

            // Assert
            Assert.IsTrue(_viewModel.IsDisposed);
        }

        [Test]
        public void Dispose_DisposesAllCells()
        {
            // Arrange
            _viewModel.Initialize();

            // Act
            _viewModel.Dispose();

            // Assert
            foreach (var cell in _viewModel.Cells)
            {
                Assert.IsTrue(cell.IsDisposed);
            }
        }

        [Test]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _viewModel.Dispose();
                _viewModel.Dispose();
            });
        }

        [Test]
        public void Reset_AfterDispose_ThrowsException()
        {
            // Arrange
            _viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _viewModel.Reset());
        }

        #endregion
    }
}
