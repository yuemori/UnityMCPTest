using System;
using R3;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Services;
using TicTacToe.Presentation.Base;
using TicTacToe.Presentation.Board;
using TicTacToe.Presentation.Result;
using TicTacToe.Presentation.TurnIndicator;

namespace TicTacToe.Presentation.Mediators
{
    /// <summary>
    /// ゲーム全体の画面協調を担当するMediator
    /// ViewModel間の連携とAIターンの制御を行う
    /// </summary>
    public class GameMediator : ViewModelBase
    {
        private readonly GameService _gameService;
        private readonly AIService _aiService;
        private readonly BoardViewModel _boardViewModel;
        private readonly TurnIndicatorViewModel _turnIndicatorViewModel;
        private readonly ResultViewModel _resultViewModel;

        private readonly ReactiveProperty<bool> _isGameStarted;
        private readonly Subject<Unit> _onGameStarted;
        private readonly Subject<Unit> _onGameEnded;

        /// <summary>
        /// ゲームが開始されているかどうか
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsGameStarted => _isGameStarted;

        /// <summary>
        /// ゲームが開始されたときに発火するObservable
        /// </summary>
        public Observable<Unit> OnGameStarted => _onGameStarted;

        /// <summary>
        /// ゲームが終了したときに発火するObservable
        /// </summary>
        public Observable<Unit> OnGameEnded => _onGameEnded;

        /// <summary>
        /// AIの思考ディレイ（ミリ秒）
        /// </summary>
        public int AIThinkingDelayMs { get; set; } = 500;

        /// <summary>
        /// リザルト表示までのディレイ（ミリ秒）
        /// </summary>
        public int ResultShowDelayMs { get; set; } = 1200;

        /// <summary>
        /// GameMediatorを作成
        /// </summary>
        /// <param name="gameService">ゲームサービス</param>
        /// <param name="aiService">AIサービス</param>
        /// <param name="boardViewModel">盤面ViewModel</param>
        /// <param name="turnIndicatorViewModel">ターン表示ViewModel</param>
        /// <param name="resultViewModel">結果表示ViewModel</param>
        public GameMediator(
            GameService gameService,
            AIService aiService,
            BoardViewModel boardViewModel,
            TurnIndicatorViewModel turnIndicatorViewModel,
            ResultViewModel resultViewModel)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _boardViewModel = boardViewModel ?? throw new ArgumentNullException(nameof(boardViewModel));
            _turnIndicatorViewModel = turnIndicatorViewModel ?? throw new ArgumentNullException(nameof(turnIndicatorViewModel));
            _resultViewModel = resultViewModel ?? throw new ArgumentNullException(nameof(resultViewModel));

            _isGameStarted = new ReactiveProperty<bool>(false);
            _onGameStarted = new Subject<Unit>();
            _onGameEnded = new Subject<Unit>();
        }

        protected override void OnInitialize()
        {
            // 各ViewModelを初期化
            if (!_boardViewModel.IsInitialized)
            {
                _boardViewModel.Initialize();
            }
            if (!_turnIndicatorViewModel.IsInitialized)
            {
                _turnIndicatorViewModel.Initialize();
            }
            if (!_resultViewModel.IsInitialized)
            {
                _resultViewModel.Initialize();
            }

            // リスタートリクエストを購読
            _resultViewModel.OnRestartRequested
                .Subscribe(_ => StartNewGame())
                .AddTo(Disposables);

            // ターン変更を購読してAIターンを処理
            _gameService.CurrentTurn
                .Subscribe(turn => HandleTurnChange(turn))
                .AddTo(Disposables);

            // ゲーム結果を購読
            _gameService.CurrentGameResult
                .Subscribe(result => HandleGameResult(result))
                .AddTo(Disposables);

            // リザルト表示リクエストを購読（ディレイ付きで表示）
            _resultViewModel.OnShowResult
                .Subscribe(resultType => ShowResultWithDelay(resultType))
                .AddTo(Disposables);
        }

        /// <summary>
        /// 新しいゲームを開始
        /// </summary>
        /// <param name="humanFirst">人間が先手かどうか（デフォルト: ランダム）</param>
        public void StartNewGame(bool? humanFirst = null)
        {
            ThrowIfDisposed();

            // 先手後手を決定
            var isHumanFirst = humanFirst ?? (new Random().Next(2) == 0);
            var xPlayerType = isHumanFirst ? PlayerType.Human : PlayerType.AI;
            var oPlayerType = isHumanFirst ? PlayerType.AI : PlayerType.Human;

            // 各ViewModelをリセット
            _boardViewModel.Reset();
            _turnIndicatorViewModel.Reset();
            _resultViewModel.Reset();

            // ゲーム開始
            _gameService.StartNewGame(xPlayerType, oPlayerType);
            _isGameStarted.Value = true;
            _onGameStarted.OnNext(Unit.Default);

            // AIが先手の場合、即座にAIの手を実行
            var turn = _gameService.CurrentTurn.CurrentValue;
            if (turn.CurrentPlayerType == PlayerType.AI)
            {
                ExecuteAITurnAsync();
            }
        }

        /// <summary>
        /// ターン変更を処理
        /// </summary>
        private void HandleTurnChange(TurnInfo turn)
        {
            ThrowIfDisposed();

            if (!_isGameStarted.CurrentValue)
            {
                return;
            }

            if (_gameService.IsGameOver)
            {
                return;
            }

            // AIのターンの場合、AIの手を実行
            if (turn.CurrentPlayerType == PlayerType.AI)
            {
                ExecuteAITurnAsync();
            }
        }

        /// <summary>
        /// ゲーム結果を処理
        /// </summary>
        private void HandleGameResult(GameResult result)
        {
            ThrowIfDisposed();

            if (result.IsGameOver)
            {
                _onGameEnded.OnNext(Unit.Default);
            }
        }

        /// <summary>
        /// ディレイ後にリザルトを表示
        /// </summary>
        private async void ShowResultWithDelay(ResultType resultType)
        {
            if (IsDisposed) return;

            try
            {
                // 盤面の結果を確認できるようにディレイ
                if (ResultShowDelayMs > 0)
                {
                    await System.Threading.Tasks.Task.Delay(ResultShowDelayMs);
                }

                // Dispose後のチェック
                if (IsDisposed) return;

                // リザルトを表示
                _resultViewModel.SetVisible(true);
            }
            catch (Exception)
            {
                // Disposeされた場合の例外を無視
            }
        }

        /// <summary>
        /// AIターンを非同期で実行
        /// </summary>
        private async void ExecuteAITurnAsync()
        {
            ThrowIfDisposed();

            if (_gameService.IsGameOver)
            {
                return;
            }

            // AI思考中の表示
            _turnIndicatorViewModel.SetAIThinking(true);

            try
            {
                // 思考ディレイ（UXのため）
                if (AIThinkingDelayMs > 0)
                {
                    await System.Threading.Tasks.Task.Delay(AIThinkingDelayMs);
                }

                // Dispose後のチェック
                if (IsDisposed || _gameService.IsGameOver)
                {
                    return;
                }

                // AIの手を実行
                _aiService.ExecuteAIMove(_gameService);
            }
            finally
            {
                if (!IsDisposed)
                {
                    _turnIndicatorViewModel.SetAIThinking(false);
                }
            }
        }

        /// <summary>
        /// 現在のゲームを中断
        /// </summary>
        public void StopGame()
        {
            ThrowIfDisposed();
            _isGameStarted.Value = false;
        }

        protected override void OnDispose()
        {
            _isGameStarted.Dispose();
            _onGameStarted.Dispose();
            _onGameEnded.Dispose();

            // 注: 子のViewModelはDIコンテナが管理するため、ここではDisposeしない
        }
    }
}
