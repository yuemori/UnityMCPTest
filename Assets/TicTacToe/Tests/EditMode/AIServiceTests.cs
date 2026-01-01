using System;
using System.Linq;
using NUnit.Framework;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;
using TicTacToe.Core.Strategies;

namespace TicTacToe.Tests.EditMode
{
    [TestFixture]
    public class AIServiceTests
    {
        private AIService _aiService;
        private BoardRepository _boardRepository;
        private GameService _gameService;
        
        [SetUp]
        public void SetUp()
        {
            _aiService = new AIService();
            _boardRepository = new BoardRepository();
            _gameService = new GameService(_boardRepository);
        }
        
        [TearDown]
        public void TearDown()
        {
            _aiService?.Dispose();
            _gameService?.Dispose();
            _boardRepository?.Dispose();
        }
        
        #region Constructor Tests
        
        [Test]
        public void Constructor_Default_InitializesWithRandomStrategy()
        {
            Assert.IsNotNull(_aiService.CurrentStrategy);
            Assert.AreEqual("Random", _aiService.CurrentStrategy.Name);
        }
        
        [Test]
        public void Constructor_WithNullStrategy_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new AIService(null));
        }
        
        [Test]
        public void Constructor_WithCustomStrategy_UsesThatStrategy()
        {
            var mockStrategy = new MockAIStrategy("TestStrategy");
            var service = new AIService(mockStrategy);
            
            Assert.AreEqual("TestStrategy", service.CurrentStrategy.Name);
            
            service.Dispose();
        }
        
        #endregion
        
        #region RegisterStrategy Tests
        
        [Test]
        public void RegisterStrategy_WithValidStrategy_AddsToAvailable()
        {
            var newStrategy = new MockAIStrategy("NewStrategy");
            
            _aiService.RegisterStrategy(newStrategy);
            
            Assert.IsTrue(_aiService.AvailableStrategies.Any(s => s == "NewStrategy"));
        }
        
        [Test]
        public void RegisterStrategy_WithNullStrategy_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _aiService.RegisterStrategy(null));
        }
        
        [Test]
        public void RegisterStrategy_WithSameName_OverwritesPrevious()
        {
            var strategy1 = new MockAIStrategy("TestStrategy", 0);
            var strategy2 = new MockAIStrategy("TestStrategy", 4);
            
            _aiService.RegisterStrategy(strategy1);
            _aiService.RegisterStrategy(strategy2);
            _aiService.SelectStrategy("TestStrategy");
            
            var board = new CellState[9];
            var position = _aiService.DecideMove(board, CellState.X);
            
            Assert.AreEqual(4, position.Index); // strategy2が使われている
        }
        
        #endregion
        
        #region SelectStrategy Tests
        
        [Test]
        public void SelectStrategy_WithValidName_ReturnsTrue()
        {
            var result = _aiService.SelectStrategy("Random");
            
            Assert.IsTrue(result);
        }
        
        [Test]
        public void SelectStrategy_WithInvalidName_ReturnsFalse()
        {
            var result = _aiService.SelectStrategy("NonExistent");
            
            Assert.IsFalse(result);
        }
        
        [Test]
        public void SelectStrategy_WithNullName_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _aiService.SelectStrategy(null));
        }
        
        [Test]
        public void SelectStrategy_UpdatesCurrentStrategy()
        {
            var newStrategy = new MockAIStrategy("NewStrategy");
            _aiService.RegisterStrategy(newStrategy);
            
            _aiService.SelectStrategy("NewStrategy");
            
            Assert.AreEqual("NewStrategy", _aiService.CurrentStrategy.Name);
        }
        
        #endregion
        
        #region DecideMove Tests
        
        [Test]
        public void DecideMove_DelegatesToCurrentStrategy()
        {
            var mockStrategy = new MockAIStrategy("Mock", 5);
            _aiService.RegisterStrategy(mockStrategy);
            _aiService.SelectStrategy("Mock");
            
            var board = new CellState[9];
            var position = _aiService.DecideMove(board, CellState.X);
            
            Assert.AreEqual(5, position.Index);
        }
        
        [Test]
        public void DecideMove_WithEmptyBoard_ReturnsValidPosition()
        {
            var board = new CellState[9];
            
            var position = _aiService.DecideMove(board, CellState.X);
            
            Assert.IsTrue(BoardPosition.IsValidIndex(position.Index));
        }
        
        #endregion
        
        #region ExecuteAIMove Tests
        
        [Test]
        public void ExecuteAIMove_WithNullGameService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _aiService.ExecuteAIMove(null));
        }
        
        [Test]
        public void ExecuteAIMove_WhenGameOver_ReturnsFalse()
        {
            // ゲームを終了状態にする（Xが勝利）
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            _gameService.PlaceMark(new BoardPosition(0)); // X
            _gameService.PlaceMark(new BoardPosition(3)); // O
            _gameService.PlaceMark(new BoardPosition(1)); // X
            _gameService.PlaceMark(new BoardPosition(4)); // O
            _gameService.PlaceMark(new BoardPosition(2)); // X wins
            
            var result = _aiService.ExecuteAIMove(_gameService);
            
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ExecuteAIMove_WhenHumanTurn_ReturnsFalse()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            // Human (X) のターン
            
            var result = _aiService.ExecuteAIMove(_gameService);
            
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ExecuteAIMove_WhenAITurn_ReturnsTrue()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            _gameService.PlaceMark(new BoardPosition(0)); // Human plays
            // Now it's AI (O) turn
            
            var result = _aiService.ExecuteAIMove(_gameService);
            
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ExecuteAIMove_PlacesMarkOnBoard()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            _gameService.PlaceMark(new BoardPosition(0)); // Human plays X at 0
            
            // 配置前のマーク数を取得（MoveCountはReactivePropertyなのでCurrentValueを使用）
            var initialMoveCount = 2; // X at 0 placed, so 1 move
            
            _aiService.ExecuteAIMove(_gameService);
            
            // AIがマークを配置したか確認（盤面を直接チェック）
            var board = _boardRepository.GetBoardSnapshot();
            int markCount = 0;
            for (int i = 0; i < board.Count; i++)
            {
                if (board[i] != CellState.Empty) markCount++;
            }
            Assert.AreEqual(2, markCount); // Human's X + AI's O
        }
        
        [Test]
        public void ExecuteAIMove_AIAsFirstPlayer_Works()
        {
            _gameService.StartNewGame(PlayerType.AI, PlayerType.Human);
            // AI (X) のターン
            
            var result = _aiService.ExecuteAIMove(_gameService);
            
            Assert.IsTrue(result);
            
            // 盤面にマークが1つあることを確認
            var board = _boardRepository.GetBoardSnapshot();
            int markCount = 0;
            for (int i = 0; i < board.Count; i++)
            {
                if (board[i] != CellState.Empty) markCount++;
            }
            Assert.AreEqual(1, markCount);
        }
        
        [Test]
        public void ExecuteAIMove_PlacesCorrectMark()
        {
            _gameService.StartNewGame(PlayerType.Human, PlayerType.AI);
            _gameService.PlaceMark(new BoardPosition(0)); // Human plays X at 0
            
            // AIがプレイしたらOが置かれる
            _aiService.ExecuteAIMove(_gameService);
            
            // 盤面にOが1つあることを確認
            var board = _boardRepository.GetBoardSnapshot();
            int oCount = 0;
            for (int i = 0; i < board.Count; i++)
            {
                if (board[i] == CellState.O) oCount++;
            }
            Assert.AreEqual(1, oCount);
        }
        
        #endregion
        
        #region Dispose Tests
        
        [Test]
        public void AfterDispose_MethodsThrowException()
        {
            _aiService.Dispose();
            
            Assert.Throws<ObjectDisposedException>(() => 
                _aiService.RegisterStrategy(new MockAIStrategy("Test")));
            Assert.Throws<ObjectDisposedException>(() => 
                _aiService.SelectStrategy("Random"));
            Assert.Throws<ObjectDisposedException>(() => 
                _aiService.DecideMove(new CellState[9], CellState.X));
            Assert.Throws<ObjectDisposedException>(() => 
                _aiService.ExecuteAIMove(_gameService));
        }
        
        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            _aiService.Dispose();
            _aiService.Dispose(); // Should not throw
        }
        
        #endregion
        
        #region AvailableStrategies Tests
        
        [Test]
        public void AvailableStrategies_ContainsDefaultStrategy()
        {
            Assert.IsTrue(_aiService.AvailableStrategies.Any(s => s == "Random"));
        }
        
        [Test]
        public void AvailableStrategies_CountIncreases_WhenNewStrategyRegistered()
        {
            var initialCount = _aiService.AvailableStrategies.Count;
            
            _aiService.RegisterStrategy(new MockAIStrategy("New"));
            
            Assert.AreEqual(initialCount + 1, _aiService.AvailableStrategies.Count);
        }
        
        #endregion
        
        #region Mock Strategy
        
        /// <summary>
        /// テスト用のモック戦略
        /// </summary>
        private class MockAIStrategy : IAIStrategy
        {
            private readonly int _returnIndex;
            
            public string Name { get; }
            
            public MockAIStrategy(string name, int returnIndex = 0)
            {
                Name = name;
                _returnIndex = returnIndex;
            }
            
            public BoardPosition DecideMove(System.Collections.Generic.IReadOnlyList<CellState> board, CellState aiMark)
            {
                return new BoardPosition(_returnIndex);
            }
        }
        
        #endregion
    }
}
