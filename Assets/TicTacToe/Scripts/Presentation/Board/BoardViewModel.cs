using System;
using System.Collections.Generic;
using R3;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Services;
using TicTacToe.Presentation.Base;
using TicTacToe.Presentation.Cell;

namespace TicTacToe.Presentation.Board
{
    /// <summary>
    /// 盤面全体のViewModel
    /// 9つのCellViewModelを管理し、GameServiceと連携
    /// </summary>
    public class BoardViewModel : ViewModelBase
    {
        private readonly GameService _gameService;
        private readonly CellViewModel[] _cells;
        private readonly ReactiveProperty<bool> _isBoardInteractable;

        /// <summary>
        /// 各セルのViewModelリスト（0-8のインデックス）
        /// </summary>
        public IReadOnlyList<CellViewModel> Cells => _cells;

        /// <summary>
        /// 盤面がインタラクト可能かどうか（ゲーム中かつHumanのターン）
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsBoardInteractable => _isBoardInteractable;

        /// <summary>
        /// BoardViewModelを作成
        /// </summary>
        /// <param name="gameService">ゲームサービス</param>
        public BoardViewModel(GameService gameService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _cells = new CellViewModel[BoardPosition.TotalCells];
            _isBoardInteractable = new ReactiveProperty<bool>(true);

            // 9つのセルViewModelを作成
            for (int i = 0; i < BoardPosition.TotalCells; i++)
            {
                _cells[i] = new CellViewModel(new BoardPosition(i));
            }
        }

        protected override void OnInitialize()
        {
            // 各セルのクリックイベントを購読
            foreach (var cell in _cells)
            {
                cell.OnCellClicked
                    .Subscribe(position => HandleCellClick(position))
                    .AddTo(Disposables);
            }

            // マーク配置イベントを購読してセル状態を更新
            _gameService.OnMarkPlaced
                .Subscribe(tuple => UpdateCellState(tuple.Position, tuple.Mark))
                .AddTo(Disposables);

            // ゲーム結果の変更を購読して盤面のインタラクション状態を更新
            _gameService.CurrentGameResult
                .Subscribe(result => UpdateBoardInteractable(result))
                .AddTo(Disposables);

            // 現在のターン情報を購読してクリック可能状態を更新
            _gameService.CurrentTurn
                .Subscribe(turn => UpdateCellsClickableState(turn))
                .AddTo(Disposables);

            // 初期状態を同期
            SyncBoardState();
        }

        /// <summary>
        /// セルがクリックされたときの処理
        /// </summary>
        private void HandleCellClick(BoardPosition position)
        {
            ThrowIfDisposed();

            if (!_isBoardInteractable.CurrentValue)
            {
                return;
            }

            // GameServiceにマーク配置を委譲
            _gameService.PlaceMark(position);
        }

        /// <summary>
        /// セル状態を更新
        /// </summary>
        private void UpdateCellState(BoardPosition position, CellState mark)
        {
            ThrowIfDisposed();

            var cell = _cells[position.Index];
            cell.UpdateState(mark);
            cell.SetClickable(false); // マークが置かれたセルはクリック不可
        }

        /// <summary>
        /// ゲーム結果に基づいて盤面のインタラクション状態を更新
        /// </summary>
        private void UpdateBoardInteractable(GameResult result)
        {
            ThrowIfDisposed();

            var isInteractable = !result.IsGameOver;
            _isBoardInteractable.Value = isInteractable;

            // ゲーム終了時は全セルをクリック不可に
            if (result.IsGameOver)
            {
                foreach (var cell in _cells)
                {
                    cell.SetClickable(false);
                }
            }
        }

        /// <summary>
        /// ターン情報に基づいてセルのクリック可能状態を更新
        /// </summary>
        private void UpdateCellsClickableState(TurnInfo turn)
        {
            ThrowIfDisposed();

            if (turn == null)
            {
                return;
            }

            // Humanのターンのみクリック可能
            var isHumanTurn = turn.CurrentPlayerType == PlayerType.Human;
            var isGameInProgress = !_gameService.IsGameOver;

            foreach (var cell in _cells)
            {
                // 空のセルのみクリック可能
                var isEmpty = cell.CellState.CurrentValue == CellState.Empty;
                cell.SetClickable(isEmpty && isHumanTurn && isGameInProgress);
            }
        }

        /// <summary>
        /// GameServiceの現在の盤面状態と同期
        /// </summary>
        private void SyncBoardState()
        {
            ThrowIfDisposed();

            var board = _gameService.Board.GetBoardSnapshot();
            var currentTurn = _gameService.CurrentTurn.CurrentValue;
            var isHumanTurn = currentTurn != null && currentTurn.CurrentPlayerType == PlayerType.Human;
            var isGameInProgress = !_gameService.IsGameOver;

            for (int i = 0; i < BoardPosition.TotalCells; i++)
            {
                _cells[i].UpdateState(board[i]);
                var isEmpty = board[i] == CellState.Empty;
                _cells[i].SetClickable(isEmpty && isHumanTurn && isGameInProgress);
            }
        }

        /// <summary>
        /// 盤面をリセット（新しいゲーム開始時）
        /// </summary>
        public void Reset()
        {
            ThrowIfDisposed();

            foreach (var cell in _cells)
            {
                cell.Reset();
            }

            _isBoardInteractable.Value = true;
        }

        protected override void OnDispose()
        {
            // 子のCellViewModelもDispose
            foreach (var cell in _cells)
            {
                cell.Dispose();
            }

            _isBoardInteractable.Dispose();
        }
    }
}
