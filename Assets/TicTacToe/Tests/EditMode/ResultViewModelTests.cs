using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;
using TicTacToe.Presentation.Result;

namespace TicTacToe.Tests.EditMode.Presentation
{
    /// <summary>
    /// ResultViewModelの単体テスト
    /// </summary>
    [TestFixture]
    public class ResultViewModelTests
    {
        private BoardRepository _boardRepository;
        private GameService _gameService;
        private ResultViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _boardRepository = new BoardRepository();
            _gameService = new GameService(_boardRepository);
            _viewModel = new ResultViewModel(_gameService);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
            _gameService?.Dispose();
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithNullGameService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ResultViewModel(null));
        }

        [Test]
        public void Constructor_InitializesWithEmptyResultText()
        {
            // Assert
            Assert.AreEqual("", _viewModel.ResultText.CurrentValue);
        }

        [Test]
        public void Constructor_InitializesWithEmptyWinnerMark()
        {
            // Assert
            Assert.AreEqual(CellState.Empty, _viewModel.WinnerMark.CurrentValue);
        }

        [Test]
        public void Constructor_InitializesAsNotVisible()
        {
            // Assert
            Assert.IsFalse(_viewModel.IsVisible.CurrentValue);
        }

        [Test]
        public void Constructor_InitializesAsNotWin()
        {
            // Assert
            Assert.IsFalse(_viewModel.IsWin.CurrentValue);
        }

        [Test]
        public void Constructor_InitializesAsNotDraw()
        {
            // Assert
            Assert.IsFalse(_viewModel.IsDraw.CurrentValue);
        }

        #endregion

        #region Initialize Tests

        [Test]
        public void Initialize_SetsInitializedTrue()
        {
            // Act
            _viewModel.Initialize();

            // Assert
            Assert.IsTrue(_viewModel.IsInitialized);
        }

        [Test]
        public void Initialize_WhenGameInProgress_StaysInvisible()
        {
            // Arrange
            _viewModel.Initialize();

            // Act
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);

            // Assert
            Assert.IsFalse(_viewModel.IsVisible.CurrentValue);
        }

        #endregion

        #region Win Result Tests

        [Test]
        public void WhenXWins_ShowsXWinResult()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            var showResultRaised = false;
            _viewModel.OnShowResult.Subscribe(_ => showResultRaised = true);

            // Act - X wins with top row
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(3)); // O
            _gameService.PlaceMark(new BoardPosition(1)); // X
            _gameService.PlaceMark(new BoardPosition(4)); // O
            _gameService.PlaceMark(new BoardPosition(2)); // X wins

            // Assert - Data is set correctly, visibility is controlled by Mediator
            Assert.IsTrue(showResultRaised); // OnShowResult event is raised
            Assert.AreEqual("X Wins!", _viewModel.ResultText.CurrentValue);
            Assert.AreEqual(CellState.X, _viewModel.WinnerMark.CurrentValue);
            Assert.IsTrue(_viewModel.IsWin.CurrentValue);
            Assert.IsFalse(_viewModel.IsDraw.CurrentValue);
        }

        [Test]
        public void WhenOWins_ShowsOWinResult()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            var showResultRaised = false;
            _viewModel.OnShowResult.Subscribe(_ => showResultRaised = true);

            // Act - O wins with left column
            _gameService.PlaceMark(new BoardPosition(1)); // X
            _gameService.PlaceMark(new BoardPosition(0)); // O
            _gameService.PlaceMark(new BoardPosition(2)); // X
            _gameService.PlaceMark(new BoardPosition(3)); // O
            _gameService.PlaceMark(new BoardPosition(4)); // X
            _gameService.PlaceMark(new BoardPosition(6)); // O wins

            // Assert - Data is set correctly, visibility is controlled by Mediator
            Assert.IsTrue(showResultRaised); // OnShowResult event is raised
            Assert.AreEqual("O Wins!", _viewModel.ResultText.CurrentValue);
            Assert.AreEqual(CellState.O, _viewModel.WinnerMark.CurrentValue);
            Assert.IsTrue(_viewModel.IsWin.CurrentValue);
            Assert.IsFalse(_viewModel.IsDraw.CurrentValue);
        }

        #endregion

        #region Draw Result Tests

        [Test]
        public void WhenDraw_ShowsDrawResult()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            var showResultRaised = false;
            _viewModel.OnShowResult.Subscribe(_ => showResultRaised = true);

            // Act - Create a draw:
            // X O X
            // X X O
            // O X O
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(1)); // O
            _gameService.PlaceMark(new BoardPosition(2)); // X
            _gameService.PlaceMark(new BoardPosition(5)); // O
            _gameService.PlaceMark(new BoardPosition(3)); // X
            _gameService.PlaceMark(new BoardPosition(6)); // O
            _gameService.PlaceMark(new BoardPosition(4)); // X
            _gameService.PlaceMark(new BoardPosition(8)); // O
            _gameService.PlaceMark(new BoardPosition(7)); // X - Draw

            // Assert - Data is set correctly, visibility is controlled by Mediator
            Assert.IsTrue(showResultRaised); // OnShowResult event is raised
            Assert.AreEqual("Draw", _viewModel.ResultText.CurrentValue);
            Assert.AreEqual(CellState.Empty, _viewModel.WinnerMark.CurrentValue);
            Assert.IsFalse(_viewModel.IsWin.CurrentValue);
            Assert.IsTrue(_viewModel.IsDraw.CurrentValue);
        }

        #endregion

        #region OnRestartClick Tests

        [Test]
        public void OnRestartClick_RaisesOnRestartRequestedEvent()
        {
            // Arrange
            _viewModel.Initialize();
            var eventRaised = false;
            _viewModel.OnRestartRequested
                .Subscribe(_ => eventRaised = true);

            // Act
            _viewModel.OnRestartClick();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void OnRestartClick_WhenDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            _viewModel.Initialize();
            _viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _viewModel.OnRestartClick());
        }

        #endregion

        #region SetVisible Tests

        [Test]
        public void SetVisible_UpdatesVisibility()
        {
            // Arrange
            _viewModel.Initialize();

            // Act
            _viewModel.SetVisible(true);

            // Assert
            Assert.IsTrue(_viewModel.IsVisible.CurrentValue);
        }

        [Test]
        public void SetVisible_WhenDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            _viewModel.Initialize();
            _viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _viewModel.SetVisible(true));
        }

        #endregion

        #region Reset Tests

        [Test]
        public void Reset_HidesPanel()
        {
            // Arrange
            _viewModel.Initialize();
            _viewModel.SetVisible(true);

            // Act
            _viewModel.Reset();

            // Assert
            Assert.IsFalse(_viewModel.IsVisible.CurrentValue);
        }

        [Test]
        public void Reset_ClearsResultText()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // Trigger a win to set result text
            _gameService.PlaceMark(new BoardPosition(0));
            _gameService.PlaceMark(new BoardPosition(3));
            _gameService.PlaceMark(new BoardPosition(1));
            _gameService.PlaceMark(new BoardPosition(4));
            _gameService.PlaceMark(new BoardPosition(2));

            // Act
            _viewModel.Reset();

            // Assert
            Assert.AreEqual("", _viewModel.ResultText.CurrentValue);
        }

        [Test]
        public void Reset_ClearsWinnerMark()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // Trigger a win
            _gameService.PlaceMark(new BoardPosition(0));
            _gameService.PlaceMark(new BoardPosition(3));
            _gameService.PlaceMark(new BoardPosition(1));
            _gameService.PlaceMark(new BoardPosition(4));
            _gameService.PlaceMark(new BoardPosition(2));

            // Act
            _viewModel.Reset();

            // Assert
            Assert.AreEqual(CellState.Empty, _viewModel.WinnerMark.CurrentValue);
        }

        [Test]
        public void Reset_ClearsIsWin()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // Trigger a win
            _gameService.PlaceMark(new BoardPosition(0));
            _gameService.PlaceMark(new BoardPosition(3));
            _gameService.PlaceMark(new BoardPosition(1));
            _gameService.PlaceMark(new BoardPosition(4));
            _gameService.PlaceMark(new BoardPosition(2));

            // Act
            _viewModel.Reset();

            // Assert
            Assert.IsFalse(_viewModel.IsWin.CurrentValue);
        }

        [Test]
        public void Reset_ClearsIsDraw()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            // Create a draw
            _gameService.PlaceMark(new BoardPosition(0));
            _gameService.PlaceMark(new BoardPosition(1));
            _gameService.PlaceMark(new BoardPosition(2));
            _gameService.PlaceMark(new BoardPosition(5));
            _gameService.PlaceMark(new BoardPosition(3));
            _gameService.PlaceMark(new BoardPosition(6));
            _gameService.PlaceMark(new BoardPosition(4));
            _gameService.PlaceMark(new BoardPosition(8));
            _gameService.PlaceMark(new BoardPosition(7));

            // Act
            _viewModel.Reset();

            // Assert
            Assert.IsFalse(_viewModel.IsDraw.CurrentValue);
        }

        [Test]
        public void Reset_WhenDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            _viewModel.Initialize();
            _viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _viewModel.Reset());
        }

        #endregion

        #region Dispose Tests

        [Test]
        public void Dispose_SetsIsDisposedTrue()
        {
            // Act
            _viewModel.Dispose();

            // Assert
            Assert.IsTrue(_viewModel.IsDisposed);
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Act & Assert - 例外が発生しないことを確認
            Assert.DoesNotThrow(() =>
            {
                _viewModel.Dispose();
                _viewModel.Dispose();
            });
        }

        #endregion

        #region Observable Subscription Tests

        [Test]
        public void OnRestartRequested_CanBeSubscribedMultipleTimes()
        {
            // Arrange
            _viewModel.Initialize();
            var count = 0;
            _viewModel.OnRestartRequested.Subscribe(_ => count++);
            _viewModel.OnRestartRequested.Subscribe(_ => count++);

            // Act
            _viewModel.OnRestartClick();

            // Assert
            Assert.AreEqual(2, count);
        }

        #endregion
    }
}
