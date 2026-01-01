using System;
using VContainer.Unity;
using TicTacToe.Presentation.Mediators;
using TicTacToe.Presentation.Board;
using TicTacToe.Presentation.TurnIndicator;
using TicTacToe.Presentation.Result;

namespace TicTacToe.Infrastructure
{
    /// <summary>
    /// ゲームのエントリーポイント
    /// VContainerのIStartableを実装し、シーン読み込み時に自動実行
    /// </summary>
    public class GameEntryPoint : IStartable, IDisposable
    {
        private readonly GameMediator _gameMediator;
        private readonly BoardViewModel _boardViewModel;
        private readonly TurnIndicatorViewModel _turnIndicatorViewModel;
        private readonly ResultViewModel _resultViewModel;
        
        // Viewへの参照（手動インジェクション用）
        private readonly BoardView _boardView;
        private readonly TurnIndicatorView _turnIndicatorView;
        private readonly ResultView _resultView;

        public GameEntryPoint(
            GameMediator gameMediator,
            BoardViewModel boardViewModel,
            TurnIndicatorViewModel turnIndicatorViewModel,
            ResultViewModel resultViewModel,
            BoardView boardView,
            TurnIndicatorView turnIndicatorView,
            ResultView resultView)
        {
            _gameMediator = gameMediator ?? throw new ArgumentNullException(nameof(gameMediator));
            _boardViewModel = boardViewModel ?? throw new ArgumentNullException(nameof(boardViewModel));
            _turnIndicatorViewModel = turnIndicatorViewModel ?? throw new ArgumentNullException(nameof(turnIndicatorViewModel));
            _resultViewModel = resultViewModel ?? throw new ArgumentNullException(nameof(resultViewModel));
            _boardView = boardView;
            _turnIndicatorView = turnIndicatorView;
            _resultView = resultView;
        }

        /// <summary>
        /// シーン読み込み時に呼び出される
        /// ViewModelの初期化とゲーム開始
        /// </summary>
        public void Start()
        {
            // ViewModelの初期化（OnInitializeを呼び出し）
            _boardViewModel.Initialize();
            _turnIndicatorViewModel.Initialize();
            _resultViewModel.Initialize();
            _gameMediator.Initialize();
            
            // ViewへのViewModel手動インジェクション
            // VContainerの[Inject]が自動で呼ばれない場合の対策
            if (_boardView != null)
            {
                _boardView.Construct(_boardViewModel);
            }
            if (_turnIndicatorView != null)
            {
                _turnIndicatorView.Construct(_turnIndicatorViewModel);
            }
            if (_resultView != null)
            {
                _resultView.Construct(_resultViewModel);
            }

            // ゲーム開始
            _gameMediator.StartNewGame();
        }

        /// <summary>
        /// シーン破棄時に呼び出される
        /// リソースのクリーンアップ
        /// </summary>
        public void Dispose()
        {
            _gameMediator.Dispose();
            _resultViewModel.Dispose();
            _turnIndicatorViewModel.Dispose();
            _boardViewModel.Dispose();
        }
    }
}
