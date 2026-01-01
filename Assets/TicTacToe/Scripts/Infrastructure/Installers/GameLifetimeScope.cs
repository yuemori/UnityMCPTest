using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;
using TicTacToe.Core.Strategies;
using TicTacToe.Presentation.Board;
using TicTacToe.Presentation.Cell;
using TicTacToe.Presentation.TurnIndicator;
using TicTacToe.Presentation.Result;
using TicTacToe.Presentation.Mediators;

namespace TicTacToe.Infrastructure
{
    /// <summary>
    /// TicTacToeゲームのDI設定
    /// VContainerによる依存性注入の構成
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Scene References (Auto-injected)")]
        [SerializeField] private BoardView _boardView;
        [SerializeField] private TurnIndicatorView _turnIndicatorView;
        [SerializeField] private ResultView _resultView;

        protected override void Configure(IContainerBuilder builder)
        {
            // === Core Layer ===
            // Repository (データコンテナ)
            builder.Register<BoardRepository>(Lifetime.Singleton);
            
            // System.Random (AI用乱数生成器) - インスタンスを直接登録
            builder.RegisterInstance(new System.Random());
            
            // AI Strategy (アルゴリズム)
            builder.Register<RandomAIStrategy>(Lifetime.Singleton).As<IAIStrategy>();
            
            // Services (ビジネスロジック)
            builder.Register<GameService>(Lifetime.Singleton);
            builder.Register<AIService>(Lifetime.Singleton);
            
            // === Presentation Layer ===
            // ViewModels (状態管理)
            builder.Register<BoardViewModel>(Lifetime.Singleton);
            builder.Register<TurnIndicatorViewModel>(Lifetime.Singleton);
            builder.Register<ResultViewModel>(Lifetime.Singleton);
            
            // Mediator (画面協調)
            builder.Register<GameMediator>(Lifetime.Singleton);
            
            // === View Layer (Scene Components) ===
            // Viewコンポーネントを登録（シーン上のMonoBehaviour）
            if (_boardView != null)
            {
                builder.RegisterComponent(_boardView);
            }
            if (_turnIndicatorView != null)
            {
                builder.RegisterComponent(_turnIndicatorView);
            }
            if (_resultView != null)
            {
                builder.RegisterComponent(_resultView);
            }
            
            // === Entry Points ===
            // GameEntryPoint登録（ゲーム開始制御）
            builder.RegisterEntryPoint<GameEntryPoint>();
        }
    }
}
