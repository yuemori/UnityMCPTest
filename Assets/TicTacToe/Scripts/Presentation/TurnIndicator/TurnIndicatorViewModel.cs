using System;
using R3;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Services;
using TicTacToe.Presentation.Base;

namespace TicTacToe.Presentation.TurnIndicator
{
    /// <summary>
    /// ターン表示のViewModel
    /// 現在のターン情報をUIに提供
    /// </summary>
    public class TurnIndicatorViewModel : ViewModelBase
    {
        private readonly GameService _gameService;
        private readonly ReactiveProperty<string> _turnText;
        private readonly ReactiveProperty<CellState> _currentMark;
        private readonly ReactiveProperty<bool> _isAIThinking;
        private readonly ReactiveProperty<bool> _isVisible;

        /// <summary>
        /// 表示するターンテキスト（例: "X's Turn", "O's Turn", "AI Thinking..."）
        /// </summary>
        public ReadOnlyReactiveProperty<string> TurnText => _turnText;

        /// <summary>
        /// 現在のマーク（色分け用）
        /// </summary>
        public ReadOnlyReactiveProperty<CellState> CurrentMark => _currentMark;

        /// <summary>
        /// AIが思考中かどうか
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsAIThinking => _isAIThinking;

        /// <summary>
        /// ターンインジケータが表示されるべきかどうか（ゲーム中のみ表示）
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsVisible => _isVisible;

        /// <summary>
        /// TurnIndicatorViewModelを作成
        /// </summary>
        /// <param name="gameService">ゲームサービス</param>
        public TurnIndicatorViewModel(GameService gameService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _turnText = new ReactiveProperty<string>("");
            _currentMark = new ReactiveProperty<CellState>(CellState.Empty);
            _isAIThinking = new ReactiveProperty<bool>(false);
            _isVisible = new ReactiveProperty<bool>(false);
        }

        protected override void OnInitialize()
        {
            // ターン情報の変更を購読
            _gameService.CurrentTurn
                .Subscribe(turn => UpdateTurnDisplay(turn))
                .AddTo(Disposables);

            // ゲーム結果の変更を購読（ゲーム終了時に非表示に）
            _gameService.CurrentGameResult
                .Subscribe(result => UpdateVisibility(result))
                .AddTo(Disposables);
        }

        /// <summary>
        /// ターン表示を更新
        /// </summary>
        private void UpdateTurnDisplay(TurnInfo turn)
        {
            ThrowIfDisposed();

            if (turn.CurrentMark == CellState.Empty)
            {
                // 初期状態（ゲーム開始前）
                _turnText.Value = "";
                _currentMark.Value = CellState.Empty;
                _isAIThinking.Value = false;
                return;
            }

            _currentMark.Value = turn.CurrentMark;

            if (turn.CurrentPlayerType == PlayerType.AI)
            {
                _turnText.Value = "AI Thinking...";
                _isAIThinking.Value = true;
            }
            else
            {
                _turnText.Value = turn.CurrentMark == CellState.X 
                    ? "X's Turn" 
                    : "O's Turn";
                _isAIThinking.Value = false;
            }
        }

        /// <summary>
        /// 表示/非表示を更新
        /// </summary>
        private void UpdateVisibility(GameResult result)
        {
            ThrowIfDisposed();

            // ゲーム終了時は非表示
            _isVisible.Value = !result.IsGameOver;

            if (result.IsGameOver)
            {
                _isAIThinking.Value = false;
            }
        }

        /// <summary>
        /// AIの思考状態を設定（Mediatorから呼び出し）
        /// </summary>
        /// <param name="isThinking">AIが思考中かどうか</param>
        public void SetAIThinking(bool isThinking)
        {
            ThrowIfDisposed();
            _isAIThinking.Value = isThinking;

            if (isThinking)
            {
                _turnText.Value = "AI Thinking...";
            }
            else
            {
                var turn = _gameService.CurrentTurn.CurrentValue;
                if (turn.CurrentMark != CellState.Empty)
                {
                    _turnText.Value = turn.CurrentMark == CellState.X 
                        ? "X's Turn" 
                        : "O's Turn";
                }
            }
        }

        /// <summary>
        /// 表示を強制的に設定（Mediatorから呼び出し）
        /// </summary>
        /// <param name="visible">表示するかどうか</param>
        public void SetVisible(bool visible)
        {
            ThrowIfDisposed();
            _isVisible.Value = visible;
        }

        /// <summary>
        /// リセット（新しいゲーム開始時）
        /// </summary>
        public void Reset()
        {
            ThrowIfDisposed();
            _turnText.Value = "";
            _currentMark.Value = CellState.Empty;
            _isAIThinking.Value = false;
            _isVisible.Value = true;
        }

        protected override void OnDispose()
        {
            _turnText.Dispose();
            _currentMark.Dispose();
            _isAIThinking.Dispose();
            _isVisible.Dispose();
        }
    }
}
