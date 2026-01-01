using System;
using System.Collections.Generic;
using R3;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Repositories;

namespace TicTacToe.Core.Services
{
    /// <summary>
    /// ゲームロジックを管理するサービス
    /// </summary>
    public class GameService : IDisposable
    {
        /// <summary>
        /// 勝利判定用のライン定義（8通り）
        /// </summary>
        private static readonly (int[] Indices, WinLine Line)[] WinPatterns = new[]
        {
            // 横3行
            (new[] { 0, 1, 2 }, WinLine.Row0),
            (new[] { 3, 4, 5 }, WinLine.Row1),
            (new[] { 6, 7, 8 }, WinLine.Row2),
            // 縦3列
            (new[] { 0, 3, 6 }, WinLine.Column0),
            (new[] { 1, 4, 7 }, WinLine.Column1),
            (new[] { 2, 5, 8 }, WinLine.Column2),
            // 対角線2本
            (new[] { 0, 4, 8 }, WinLine.DiagonalMain),
            (new[] { 2, 4, 6 }, WinLine.DiagonalAnti)
        };
        
        private readonly BoardRepository _boardRepository;
        private readonly ReactiveProperty<TurnInfo> _currentTurn;
        private readonly ReactiveProperty<GameResult> _gameResult;
        private readonly Subject<(BoardPosition Position, CellState Mark)> _onMarkPlaced;
        private bool _disposed;
        
        /// <summary>
        /// 現在のターン情報
        /// </summary>
        public ReadOnlyReactiveProperty<TurnInfo> CurrentTurn => _currentTurn;
        
        /// <summary>
        /// 現在のゲーム結果
        /// </summary>
        public ReadOnlyReactiveProperty<GameResult> CurrentGameResult => _gameResult;
        
        /// <summary>
        /// マークが配置されたときに発火するObservable
        /// </summary>
        public Observable<(BoardPosition Position, CellState Mark)> OnMarkPlaced => _onMarkPlaced;
        
        /// <summary>
        /// ゲームが終了しているかどうか
        /// </summary>
        public bool IsGameOver => _gameResult.CurrentValue.IsGameOver;
        
        /// <summary>
        /// 盤面リポジトリへの参照（読み取り用）
        /// </summary>
        public BoardRepository Board => _boardRepository;
        
        public GameService(BoardRepository boardRepository)
        {
            _boardRepository = boardRepository ?? throw new ArgumentNullException(nameof(boardRepository));
            _currentTurn = new ReactiveProperty<TurnInfo>();
            _gameResult = new ReactiveProperty<GameResult>(GameResult.InProgress());
            _onMarkPlaced = new Subject<(BoardPosition, CellState)>();
        }
        
        /// <summary>
        /// 新しいゲームを開始
        /// </summary>
        /// <param name="xPlayerType">Xプレイヤーの種別</param>
        /// <param name="oPlayerType">Oプレイヤーの種別</param>
        public void StartNewGame(PlayerType xPlayerType, PlayerType oPlayerType)
        {
            ThrowIfDisposed();
            
            _boardRepository.Reset();
            _currentTurn.Value = TurnInfo.CreateInitial(xPlayerType, oPlayerType);
            _gameResult.Value = GameResult.InProgress();
        }
        
        /// <summary>
        /// 指定位置にマークを配置
        /// </summary>
        /// <param name="position">配置位置</param>
        /// <returns>配置成功の場合true</returns>
        public bool PlaceMark(BoardPosition position)
        {
            ThrowIfDisposed();
            
            if (IsGameOver)
            {
                return false;
            }
            
            var currentMark = _currentTurn.CurrentValue.CurrentMark;
            
            if (!_boardRepository.TryPlaceMark(position, currentMark))
            {
                return false;
            }
            
            _onMarkPlaced.OnNext((position, currentMark));
            
            // 勝敗判定
            var result = CheckGameResult();
            _gameResult.Value = result;
            
            // ゲーム続行の場合、次のターンへ
            if (!result.IsGameOver)
            {
                _currentTurn.Value = _currentTurn.CurrentValue.NextTurn();
            }
            
            return true;
        }
        
        /// <summary>
        /// 現在の盤面状態から勝敗を判定
        /// </summary>
        /// <returns>ゲーム結果</returns>
        public GameResult CheckGameResult()
        {
            ThrowIfDisposed();
            
            var board = _boardRepository.GetBoardSnapshot();
            
            // 勝利判定
            foreach (var (indices, winLine) in WinPatterns)
            {
                var cell0 = board[indices[0]];
                var cell1 = board[indices[1]];
                var cell2 = board[indices[2]];
                
                if (cell0 != CellState.Empty && cell0 == cell1 && cell1 == cell2)
                {
                    return GameResult.Win(cell0, winLine);
                }
            }
            
            // 引き分け判定（盤面が満杯で勝者なし）
            if (_boardRepository.IsFull)
            {
                return GameResult.Draw();
            }
            
            // ゲーム続行
            return GameResult.InProgress();
        }
        
        /// <summary>
        /// 指定のマークで勝利できるかチェック（AI用）
        /// </summary>
        /// <param name="mark">チェックするマーク</param>
        /// <returns>勝利可能な位置（なければnull）</returns>
        public BoardPosition? FindWinningMove(CellState mark)
        {
            ThrowIfDisposed();
            
            if (mark == CellState.Empty)
            {
                throw new ArgumentException("Mark cannot be Empty", nameof(mark));
            }
            
            var board = _boardRepository.GetBoardSnapshot();
            
            foreach (var (indices, _) in WinPatterns)
            {
                var markCount = 0;
                var emptyIndex = -1;
                
                foreach (var index in indices)
                {
                    if (board[index] == mark)
                    {
                        markCount++;
                    }
                    else if (board[index] == CellState.Empty)
                    {
                        emptyIndex = index;
                    }
                }
                
                // 2つ揃っていて1つ空いている場合、勝利可能
                if (markCount == 2 && emptyIndex >= 0)
                {
                    return new BoardPosition(emptyIndex);
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 空きセルの位置リストを取得
        /// </summary>
        public IReadOnlyList<BoardPosition> GetAvailableMoves()
        {
            ThrowIfDisposed();
            return _boardRepository.GetEmptyPositions();
        }
        
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(GameService));
            }
        }
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _disposed = true;
            _currentTurn.Dispose();
            _gameResult.Dispose();
            _onMarkPlaced.Dispose();
        }
    }
}
