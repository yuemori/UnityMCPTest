using System;
using System.Collections.Generic;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Strategies;

namespace TicTacToe.Core.Services
{
    /// <summary>
    /// AI戦略の管理と実行を担当するサービス
    /// </summary>
    public class AIService : IDisposable
    {
        private readonly Dictionary<string, IAIStrategy> _strategies;
        private IAIStrategy _currentStrategy;
        private bool _disposed;
        
        /// <summary>
        /// 現在選択されている戦略
        /// </summary>
        public IAIStrategy CurrentStrategy => _currentStrategy;
        
        /// <summary>
        /// 利用可能な戦略名のリスト
        /// </summary>
        public IReadOnlyCollection<string> AvailableStrategies => _strategies.Keys;
        
        /// <summary>
        /// デフォルトのRandomAIStrategyで初期化
        /// </summary>
        public AIService() : this(new RandomAIStrategy())
        {
        }
        
        /// <summary>
        /// 指定した戦略で初期化
        /// </summary>
        /// <param name="defaultStrategy">初期戦略</param>
        /// <exception cref="ArgumentNullException">strategyがnullの場合</exception>
        public AIService(IAIStrategy defaultStrategy)
        {
            if (defaultStrategy == null)
            {
                throw new ArgumentNullException(nameof(defaultStrategy));
            }
            
            _strategies = new Dictionary<string, IAIStrategy>();
            _currentStrategy = defaultStrategy;
            
            RegisterStrategy(defaultStrategy);
        }
        
        /// <summary>
        /// 戦略を登録
        /// </summary>
        /// <param name="strategy">登録する戦略</param>
        /// <exception cref="ArgumentNullException">strategyがnullの場合</exception>
        public void RegisterStrategy(IAIStrategy strategy)
        {
            ThrowIfDisposed();
            
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }
            
            _strategies[strategy.Name] = strategy;
        }
        
        /// <summary>
        /// 戦略を名前で選択
        /// </summary>
        /// <param name="strategyName">戦略名</param>
        /// <returns>選択成功の場合true</returns>
        /// <exception cref="ArgumentNullException">strategyNameがnullの場合</exception>
        public bool SelectStrategy(string strategyName)
        {
            ThrowIfDisposed();
            
            if (strategyName == null)
            {
                throw new ArgumentNullException(nameof(strategyName));
            }
            
            if (_strategies.TryGetValue(strategyName, out var strategy))
            {
                _currentStrategy = strategy;
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 現在の戦略でAIの手を決定
        /// </summary>
        /// <param name="board">現在の盤面状態</param>
        /// <param name="aiMark">AIのマーク</param>
        /// <returns>配置する位置</returns>
        public BoardPosition DecideMove(IReadOnlyList<CellState> board, CellState aiMark)
        {
            ThrowIfDisposed();
            
            return _currentStrategy.DecideMove(board, aiMark);
        }
        
        /// <summary>
        /// GameServiceと連携してAIの手を実行
        /// </summary>
        /// <param name="gameService">ゲームサービス</param>
        /// <returns>配置成功の場合true</returns>
        /// <exception cref="ArgumentNullException">gameServiceがnullの場合</exception>
        public bool ExecuteAIMove(GameService gameService)
        {
            ThrowIfDisposed();
            
            if (gameService == null)
            {
                throw new ArgumentNullException(nameof(gameService));
            }
            
            if (gameService.IsGameOver)
            {
                return false;
            }
            
            var currentTurn = gameService.CurrentTurn.CurrentValue;
            
            // 現在のターンがAIかどうか確認
            if (!IsCurrentTurnAI(currentTurn))
            {
                return false;
            }
            
            var board = gameService.Board.GetBoardSnapshot();
            var aiMark = currentTurn.CurrentMark;
            
            var position = DecideMove(board, aiMark);
            return gameService.PlaceMark(position);
        }
        
        /// <summary>
        /// 現在のターンがAIかどうかを判定
        /// </summary>
        /// <param name="turnInfo">ターン情報</param>
        /// <returns>AIのターンの場合true</returns>
        private static bool IsCurrentTurnAI(TurnInfo turnInfo)
        {
            return turnInfo.CurrentPlayerType == PlayerType.AI;
        }
        
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(AIService));
            }
        }
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _disposed = true;
            _strategies.Clear();
            _currentStrategy = null;
        }
    }
}
