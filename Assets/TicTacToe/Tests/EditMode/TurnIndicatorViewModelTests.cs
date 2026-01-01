using System;
using NUnit.Framework;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;
using TicTacToe.Presentation.TurnIndicator;

namespace TicTacToe.Tests.EditMode.Presentation
{
    /// <summary>
    /// TurnIndicatorViewModelの単体テスト
    /// </summary>
    [TestFixture]
    public class TurnIndicatorViewModelTests
    {
        private BoardRepository _boardRepository;
        private GameService _gameService;
        private TurnIndicatorViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _boardRepository = new BoardRepository();
            _gameService = new GameService(_boardRepository);
            _viewModel = new TurnIndicatorViewModel(_gameService);
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
            Assert.Throws<ArgumentNullException>(() => new TurnIndicatorViewModel(null));
        }

        [Test]
        public void Constructor_InitializesWithEmptyTurnText()
        {
            // Assert
            Assert.AreEqual("", _viewModel.TurnText.CurrentValue);
        }

        [Test]
        public void Constructor_InitializesWithEmptyMark()
        {
            // Assert
            Assert.AreEqual(CellState.Empty, _viewModel.CurrentMark.CurrentValue);
        }

        [Test]
        public void Constructor_InitializesAsNotAIThinking()
        {
            // Assert
            Assert.IsFalse(_viewModel.IsAIThinking.CurrentValue);
        }

        [Test]
        public void Constructor_InitializesAsNotVisible()
        {
            // Assert
            Assert.IsFalse(_viewModel.IsVisible.CurrentValue);
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
        public void Initialize_WhenGameStartsWithHumanX_ShowsXTurn()
        {
            // Arrange
            _viewModel.Initialize();

            // Act
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);

            // Assert
            Assert.AreEqual("X's Turn", _viewModel.TurnText.CurrentValue);
            Assert.AreEqual(CellState.X, _viewModel.CurrentMark.CurrentValue);
        }

        [Test]
        public void Initialize_WhenGameStartsWithAIX_ShowsAIThinking()
        {
            // Arrange
            _viewModel.Initialize();

            // Act
            _gameService.StartNewGame(PlayerType.AI, PlayerType.Human);

            // Assert
            Assert.AreEqual("AI Thinking...", _viewModel.TurnText.CurrentValue);
            Assert.IsTrue(_viewModel.IsAIThinking.CurrentValue);
        }

        #endregion

        #region Turn Change Tests

        [Test]
        public void WhenTurnChangesToHuman_ShowsCorrectTurnText()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            
            // Act - Xが置いた後、Oのターンへ
            _gameService.PlaceMark(new BoardPosition(0));

            // Assert
            Assert.AreEqual("O's Turn", _viewModel.TurnText.CurrentValue);
            Assert.AreEqual(CellState.O, _viewModel.CurrentMark.CurrentValue);
        }

        [Test]
        public void WhenTurnChangesToAI_ShowsAIThinkingText()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            
            // Act - Humanが置いた後、AIのターンへ
            _gameService.PlaceMark(new BoardPosition(0));

            // Assert
            Assert.AreEqual("AI Thinking...", _viewModel.TurnText.CurrentValue);
            Assert.IsTrue(_viewModel.IsAIThinking.CurrentValue);
        }

        #endregion

        #region Visibility Tests

        [Test]
        public void WhenGameStarts_BecomesVisible()
        {
            // Arrange
            _viewModel.Initialize();

            // Act
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);

            // Assert - ゲーム進行中はIsVisibleがtrue
            Assert.IsTrue(_viewModel.IsVisible.CurrentValue);
        }

        [Test]
        public void WhenGameEnds_BecomesInvisible()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            
            // Act - Xを3連で並べて勝利
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(3)); // O
            _gameService.PlaceMark(new BoardPosition(1)); // X
            _gameService.PlaceMark(new BoardPosition(4)); // O
            _gameService.PlaceMark(new BoardPosition(2)); // X wins

            // Assert
            Assert.IsFalse(_viewModel.IsVisible.CurrentValue);
        }

        [Test]
        public void WhenGameEnds_AIThinkingIsFalse()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.Human);
            
            // Act - ゲーム終了
            _gameService.PlaceMark(new BoardPosition(0));
            _gameService.PlaceMark(new BoardPosition(3));
            _gameService.PlaceMark(new BoardPosition(1));
            _gameService.PlaceMark(new BoardPosition(4));
            _gameService.PlaceMark(new BoardPosition(2)); // X wins

            // Assert
            Assert.IsFalse(_viewModel.IsAIThinking.CurrentValue);
        }

        #endregion

        #region SetAIThinking Tests

        [Test]
        public void SetAIThinking_WhenTrue_UpdatesState()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);

            // Act
            _viewModel.SetAIThinking(true);

            // Assert
            Assert.IsTrue(_viewModel.IsAIThinking.CurrentValue);
            Assert.AreEqual("AI Thinking...", _viewModel.TurnText.CurrentValue);
        }

        [Test]
        public void SetAIThinking_WhenFalse_RestoresTurnText()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            _viewModel.SetAIThinking(true);

            // Act
            _viewModel.SetAIThinking(false);

            // Assert
            Assert.IsFalse(_viewModel.IsAIThinking.CurrentValue);
        }

        [Test]
        public void SetAIThinking_WhenDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            _viewModel.Initialize();
            _viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _viewModel.SetAIThinking(true));
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
        public void Reset_ClearsTurnText()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);

            // Act
            _viewModel.Reset();

            // Assert
            Assert.AreEqual("", _viewModel.TurnText.CurrentValue);
        }

        [Test]
        public void Reset_ClearsCurrentMark()
        {
            // Arrange
            _viewModel.Initialize();
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);

            // Act
            _viewModel.Reset();

            // Assert
            Assert.AreEqual(CellState.Empty, _viewModel.CurrentMark.CurrentValue);
        }

        [Test]
        public void Reset_ClearsAIThinking()
        {
            // Arrange
            _viewModel.Initialize();
            _viewModel.SetAIThinking(true);

            // Act
            _viewModel.Reset();

            // Assert
            Assert.IsFalse(_viewModel.IsAIThinking.CurrentValue);
        }

        [Test]
        public void Reset_SetsVisibleTrue()
        {
            // Arrange
            _viewModel.Initialize();
            _viewModel.SetVisible(false);

            // Act
            _viewModel.Reset();

            // Assert
            Assert.IsTrue(_viewModel.IsVisible.CurrentValue);
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
    }
}
