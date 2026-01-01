using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;
using TicTacToe.Core.Strategies;
using TicTacToe.Presentation.Board;
using TicTacToe.Presentation.Mediators;
using TicTacToe.Presentation.Result;
using TicTacToe.Presentation.TurnIndicator;

namespace TicTacToe.Tests.EditMode.Presentation
{
    /// <summary>
    /// GameMediatorの単体テスト
    /// </summary>
    [TestFixture]
    public class GameMediatorTests
    {
        private BoardRepository _boardRepository;
        private GameService _gameService;
        private AIService _aiService;
        private BoardViewModel _boardViewModel;
        private TurnIndicatorViewModel _turnIndicatorViewModel;
        private ResultViewModel _resultViewModel;
        private GameMediator _mediator;

        [SetUp]
        public void SetUp()
        {
            _boardRepository = new BoardRepository();
            _gameService = new GameService(_boardRepository);
            _aiService = new AIService(new RandomAIStrategy());
            _boardViewModel = new BoardViewModel(_gameService);
            _turnIndicatorViewModel = new TurnIndicatorViewModel(_gameService);
            _resultViewModel = new ResultViewModel(_gameService);
            _mediator = new GameMediator(
                _gameService, 
                _aiService, 
                _boardViewModel, 
                _turnIndicatorViewModel, 
                _resultViewModel);
            
            // テスト用にAI思考ディレイを0に設定
            _mediator.AIThinkingDelayMs = 0;
        }

        [TearDown]
        public void TearDown()
        {
            _mediator?.Dispose();
            _resultViewModel?.Dispose();
            _turnIndicatorViewModel?.Dispose();
            _boardViewModel?.Dispose();
            _aiService?.Dispose();
            _gameService?.Dispose();
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithNullGameService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GameMediator(
                null, _aiService, _boardViewModel, _turnIndicatorViewModel, _resultViewModel));
        }

        [Test]
        public void Constructor_WithNullAIService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GameMediator(
                _gameService, null, _boardViewModel, _turnIndicatorViewModel, _resultViewModel));
        }

        [Test]
        public void Constructor_WithNullBoardViewModel_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GameMediator(
                _gameService, _aiService, null, _turnIndicatorViewModel, _resultViewModel));
        }

        [Test]
        public void Constructor_WithNullTurnIndicatorViewModel_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GameMediator(
                _gameService, _aiService, _boardViewModel, null, _resultViewModel));
        }

        [Test]
        public void Constructor_WithNullResultViewModel_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GameMediator(
                _gameService, _aiService, _boardViewModel, _turnIndicatorViewModel, null));
        }

        [Test]
        public void Constructor_InitializesIsGameStartedAsFalse()
        {
            // Assert
            Assert.IsFalse(_mediator.IsGameStarted.CurrentValue);
        }

        #endregion

        #region Initialize Tests

        [Test]
        public void Initialize_SetsInitializedTrue()
        {
            // Act
            _mediator.Initialize();

            // Assert
            Assert.IsTrue(_mediator.IsInitialized);
        }

        [Test]
        public void Initialize_InitializesChildViewModels()
        {
            // Act
            _mediator.Initialize();

            // Assert
            Assert.IsTrue(_boardViewModel.IsInitialized);
            Assert.IsTrue(_turnIndicatorViewModel.IsInitialized);
            Assert.IsTrue(_resultViewModel.IsInitialized);
        }

        #endregion

        #region StartNewGame Tests

        [Test]
        public void StartNewGame_SetsIsGameStartedTrue()
        {
            // Arrange
            _mediator.Initialize();

            // Act
            _mediator.StartNewGame(humanFirst: true);

            // Assert
            Assert.IsTrue(_mediator.IsGameStarted.CurrentValue);
        }

        [Test]
        public void StartNewGame_WithHumanFirst_SetsHumanAsX()
        {
            // Arrange
            _mediator.Initialize();

            // Act
            _mediator.StartNewGame(humanFirst: true);

            // Assert
            var turn = _gameService.CurrentTurn.CurrentValue;
            Assert.AreEqual(PlayerType.Human, turn.XPlayerType);
            Assert.AreEqual(PlayerType.AI, turn.OPlayerType);
        }

        [Test]
        public void StartNewGame_WithAIFirst_SetsAIAsX()
        {
            // Arrange
            _mediator.Initialize();

            // Act
            _mediator.StartNewGame(humanFirst: false);

            // Assert
            var turn = _gameService.CurrentTurn.CurrentValue;
            Assert.AreEqual(PlayerType.AI, turn.XPlayerType);
            Assert.AreEqual(PlayerType.Human, turn.OPlayerType);
        }

        [Test]
        public void StartNewGame_RaisesOnGameStartedEvent()
        {
            // Arrange
            _mediator.Initialize();
            var eventRaised = false;
            _mediator.OnGameStarted.Subscribe(_ => eventRaised = true);

            // Act
            _mediator.StartNewGame(humanFirst: true);

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void StartNewGame_ResetsBoardViewModel()
        {
            // Arrange
            _mediator.Initialize();
            _mediator.StartNewGame(humanFirst: true);
            // Place some marks
            _gameService.PlaceMark(new BoardPosition(0));

            // Act
            _mediator.StartNewGame(humanFirst: true);

            // Assert - Board should be reset
            Assert.AreEqual(CellState.Empty, _boardViewModel.Cells[0].CellState.CurrentValue);
        }

        [Test]
        public void StartNewGame_ResetsTurnIndicatorViewModel()
        {
            // Arrange
            _mediator.Initialize();
            _turnIndicatorViewModel.SetVisible(false);

            // Act
            _mediator.StartNewGame(humanFirst: true);

            // Assert
            Assert.IsTrue(_turnIndicatorViewModel.IsVisible.CurrentValue);
        }

        [Test]
        public void StartNewGame_ResetsResultViewModel()
        {
            // Arrange
            _mediator.Initialize();
            _resultViewModel.SetVisible(true);

            // Act
            _mediator.StartNewGame(humanFirst: true);

            // Assert
            Assert.IsFalse(_resultViewModel.IsVisible.CurrentValue);
        }

        [Test]
        public void StartNewGame_WhenDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            _mediator.Initialize();
            _mediator.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _mediator.StartNewGame());
        }

        #endregion

        #region AI Turn Tests

        [Test]
        public void StartNewGame_WithAIFirst_AIPlacesMark()
        {
            // Arrange
            _mediator.Initialize();

            // Act
            _mediator.StartNewGame(humanFirst: false);

            // Wait a bit for async AI move (since delay is 0, it should be immediate)
            System.Threading.Thread.Sleep(50);

            // Assert - AI should have placed a mark
            var board = _boardRepository.GetBoardSnapshot();
            var xCount = 0;
            foreach (var cell in board)
            {
                if (cell == CellState.X) xCount++;
            }
            Assert.AreEqual(1, xCount);
        }

        #endregion

        #region Restart Request Tests

        [Test]
        public void WhenResultViewModelRequestsRestart_StartsNewGame()
        {
            // Arrange
            _mediator.Initialize();
            _mediator.StartNewGame(humanFirst: true);
            
            // Complete a game
            _gameService.PlaceMark(new BoardPosition(0));
            _gameService.PlaceMark(new BoardPosition(3));
            _gameService.PlaceMark(new BoardPosition(1));
            _gameService.PlaceMark(new BoardPosition(4));
            _gameService.PlaceMark(new BoardPosition(2)); // X wins

            // Act - Request restart
            _resultViewModel.OnRestartClick();

            // Assert - Game should restart
            Assert.IsTrue(_mediator.IsGameStarted.CurrentValue);
            Assert.IsFalse(_gameService.IsGameOver);
        }

        #endregion

        #region Game End Tests

        [Test]
        public void WhenGameEnds_RaisesOnGameEndedEvent()
        {
            // Arrange
            _mediator.Initialize();
            var eventRaised = false;
            _mediator.OnGameEnded.Subscribe(_ => eventRaised = true);
            _mediator.StartNewGame(humanFirst: true);

            // Act - Complete a game
            _gameService.PlaceMark(new BoardPosition(0));
            _gameService.PlaceMark(new BoardPosition(3));
            _gameService.PlaceMark(new BoardPosition(1));
            _gameService.PlaceMark(new BoardPosition(4));
            _gameService.PlaceMark(new BoardPosition(2)); // X wins

            // Wait for async operations
            System.Threading.Thread.Sleep(50);

            // Assert
            Assert.IsTrue(eventRaised);
        }

        #endregion

        #region StopGame Tests

        [Test]
        public void StopGame_SetsIsGameStartedFalse()
        {
            // Arrange
            _mediator.Initialize();
            _mediator.StartNewGame(humanFirst: true);

            // Act
            _mediator.StopGame();

            // Assert
            Assert.IsFalse(_mediator.IsGameStarted.CurrentValue);
        }

        [Test]
        public void StopGame_WhenDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            _mediator.Initialize();
            _mediator.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _mediator.StopGame());
        }

        #endregion

        #region AIThinkingDelayMs Tests

        [Test]
        public void AIThinkingDelayMs_DefaultValue_Is500()
        {
            // Arrange
            using var mediator = new GameMediator(
                _gameService, _aiService, _boardViewModel, 
                _turnIndicatorViewModel, _resultViewModel);

            // Assert
            Assert.AreEqual(500, mediator.AIThinkingDelayMs);
        }

        [Test]
        public void AIThinkingDelayMs_CanBeModified()
        {
            // Act
            _mediator.AIThinkingDelayMs = 1000;

            // Assert
            Assert.AreEqual(1000, _mediator.AIThinkingDelayMs);
        }

        #endregion

        #region Dispose Tests

        [Test]
        public void Dispose_SetsIsDisposedTrue()
        {
            // Act
            _mediator.Dispose();

            // Assert
            Assert.IsTrue(_mediator.IsDisposed);
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Act & Assert - 例外が発生しないことを確認
            Assert.DoesNotThrow(() =>
            {
                _mediator.Dispose();
                _mediator.Dispose();
            });
        }

        #endregion

        #region Integration Tests

        [Test]
        public void FullGameFlow_HumanVsAI_WorksCorrectly()
        {
            // Arrange
            _mediator.Initialize();
            _mediator.ResultShowDelayMs = 0; // Disable delay for test

            // Act - Human first (X: Human, O: AI)
            _mediator.StartNewGame(humanFirst: true);

            // Verify initial state
            Assert.IsTrue(_mediator.IsGameStarted.CurrentValue);
            Assert.IsTrue(_turnIndicatorViewModel.IsVisible.CurrentValue);
            Assert.IsFalse(_resultViewModel.IsVisible.CurrentValue);

            // Human places marks - AI will automatically respond
            _gameService.PlaceMark(new BoardPosition(0)); // X (Human)
            // AI places O automatically
            System.Threading.Thread.Sleep(50);

            _gameService.PlaceMark(new BoardPosition(1)); // X (Human)
            System.Threading.Thread.Sleep(50);

            _gameService.PlaceMark(new BoardPosition(2)); // X wins (Human)

            // Wait for async result display
            System.Threading.Thread.Sleep(50);

            // Assert end state
            Assert.IsTrue(_gameService.IsGameOver);
            Assert.IsTrue(_resultViewModel.IsVisible.CurrentValue);
            Assert.IsFalse(_turnIndicatorViewModel.IsVisible.CurrentValue);
        }

        [Test]
        public void FullGameFlow_RestartAfterWin_WorksCorrectly()
        {
            // Arrange
            _mediator.Initialize();
            _mediator.StartNewGame(humanFirst: true);

            // Play until X wins
            _gameService.PlaceMark(new BoardPosition(0));
            _gameService.PlaceMark(new BoardPosition(3));
            _gameService.PlaceMark(new BoardPosition(1));
            _gameService.PlaceMark(new BoardPosition(4));
            _gameService.PlaceMark(new BoardPosition(2));

            // Act - Restart
            _resultViewModel.OnRestartClick();

            // Assert
            Assert.IsFalse(_gameService.IsGameOver);
            Assert.IsFalse(_resultViewModel.IsVisible.CurrentValue);
            Assert.IsTrue(_turnIndicatorViewModel.IsVisible.CurrentValue);
        }

        #endregion
    }
}
