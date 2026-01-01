using System;
using R3;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Services;
using TicTacToe.Presentation.Base;

namespace TicTacToe.Presentation.Result
{
    /// <summary>
    /// 結果タイプ（演出の種類を決定）
    /// </summary>
    public enum ResultType
    {
        None,
        HumanWin,    // 人間の勝利
        AIWin,       // AIの勝利（人間の敗北）
        Draw         // 引き分け
    }

    /// <summary>
    /// ゲーム結果表示のViewModel
    /// 勝敗結果をUIに提供
    /// </summary>
    public class ResultViewModel : ViewModelBase
    {
        private readonly GameService _gameService;
        private readonly ReactiveProperty<string> _resultText;
        private readonly ReactiveProperty<CellState> _winnerMark;
        private readonly ReactiveProperty<bool> _isVisible;
        private readonly ReactiveProperty<bool> _isWin;
        private readonly ReactiveProperty<bool> _isDraw;
        private readonly ReactiveProperty<ResultType> _resultType;
        private readonly Subject<Unit> _onRestartRequested;
        private readonly Subject<ResultType> _onShowResult;

        /// <summary>
        /// 表示する結果テキスト（例: "Xの勝ち!", "Oの勝ち!", "引き分け"）
        /// </summary>
        public ReadOnlyReactiveProperty<string> ResultText => _resultText;

        /// <summary>
        /// 勝者のマーク（色分け用、引き分けの場合はEmpty）
        /// </summary>
        public ReadOnlyReactiveProperty<CellState> WinnerMark => _winnerMark;

        /// <summary>
        /// 結果パネルが表示されるべきかどうか
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsVisible => _isVisible;

        /// <summary>
        /// 勝敗が決まったかどうか
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsWin => _isWin;

        /// <summary>
        /// 引き分けかどうか
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsDraw => _isDraw;

        /// <summary>
        /// 結果タイプ（演出切り替え用）
        /// </summary>
        public ReadOnlyReactiveProperty<ResultType> CurrentResultType => _resultType;

        /// <summary>
        /// リスタートがリクエストされたときに発火するObservable
        /// </summary>
        public Observable<Unit> OnRestartRequested => _onRestartRequested;

        /// <summary>
        /// 結果を表示するときに発火するObservable（ディレイ後に発火）
        /// </summary>
        public Observable<ResultType> OnShowResult => _onShowResult;

        /// <summary>
        /// ResultViewModelを作成
        /// </summary>
        /// <param name="gameService">ゲームサービス</param>
        public ResultViewModel(GameService gameService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _resultText = new ReactiveProperty<string>("");
            _winnerMark = new ReactiveProperty<CellState>(CellState.Empty);
            _isVisible = new ReactiveProperty<bool>(false);
            _isWin = new ReactiveProperty<bool>(false);
            _isDraw = new ReactiveProperty<bool>(false);
            _resultType = new ReactiveProperty<ResultType>(ResultType.None);
            _onRestartRequested = new Subject<Unit>();
            _onShowResult = new Subject<ResultType>();
        }

        protected override void OnInitialize()
        {
            // ゲーム結果の変更を購読（即座には表示しない、データのみ準備）
            _gameService.CurrentGameResult
                .Subscribe(result => PrepareResultData(result))
                .AddTo(Disposables);
        }

        /// <summary>
        /// 結果データを準備（まだ表示しない）
        /// </summary>
        private void PrepareResultData(GameResult result)
        {
            ThrowIfDisposed();

            if (!result.IsGameOver)
            {
                // ゲーム進行中はデータリセット（表示はそのまま）
                return;
            }

            // 結果データを設定
            switch (result.State)
            {
                case GameState.Win:
                    _isWin.Value = true;
                    _isDraw.Value = false;
                    _winnerMark.Value = result.Winner ?? CellState.Empty;
                    _resultText.Value = result.Winner == CellState.X 
                        ? "X Wins!" 
                        : "O Wins!";
                    
                    // 勝者が人間かAIかを判定
                    var turn = _gameService.CurrentTurn.CurrentValue;
                    var winnerMark = result.Winner ?? CellState.Empty;
                    var winnerPlayerType = winnerMark == CellState.X 
                        ? turn.XPlayerType 
                        : turn.OPlayerType;
                    _resultType.Value = winnerPlayerType == PlayerType.Human 
                        ? ResultType.HumanWin 
                        : ResultType.AIWin;
                    break;

                case GameState.Draw:
                    _isWin.Value = false;
                    _isDraw.Value = true;
                    _winnerMark.Value = CellState.Empty;
                    _resultText.Value = "Draw";
                    _resultType.Value = ResultType.Draw;
                    break;

                default:
                    _isWin.Value = false;
                    _isDraw.Value = false;
                    _winnerMark.Value = CellState.Empty;
                    _resultText.Value = "";
                    _resultType.Value = ResultType.None;
                    break;
            }

            // 結果表示をリクエスト（Mediatorがディレイ後に表示を制御）
            if (result.IsGameOver)
            {
                _onShowResult.OnNext(_resultType.Value);
            }
        }

        /// <summary>
        /// リスタートボタンがクリックされた（Viewから呼び出し）
        /// </summary>
        public void OnRestartClick()
        {
            ThrowIfDisposed();
            _onRestartRequested.OnNext(Unit.Default);
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
        /// 非表示にしてリセット（新しいゲーム開始時）
        /// </summary>
        public void Reset()
        {
            ThrowIfDisposed();
            _isVisible.Value = false;
            _resultText.Value = "";
            _winnerMark.Value = CellState.Empty;
            _isWin.Value = false;
            _isDraw.Value = false;
        }

        protected override void OnDispose()
        {
            _resultText.Dispose();
            _winnerMark.Dispose();
            _isVisible.Dispose();
            _isWin.Dispose();
            _isDraw.Dispose();
            _resultType.Dispose();
            _onRestartRequested.Dispose();
            _onShowResult.Dispose();
        }
    }
}
