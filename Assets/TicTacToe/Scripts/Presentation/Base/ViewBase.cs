using System;
using R3;
using UnityEngine;
using VContainer;

namespace TicTacToe.Presentation.Base
{
    /// <summary>
    /// View基底クラス（非ジェネリック版）
    /// MonoBehaviourベースのViewにおける共通機能を提供
    /// </summary>
    public abstract class ViewBase : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();
        private bool _isBound;

        /// <summary>
        /// バインド済みかどうか
        /// </summary>
        public bool IsBound => _isBound;

        /// <summary>
        /// Subscription管理用のCompositeDisposable
        /// 派生クラスで使用可能
        /// </summary>
        protected CompositeDisposable Disposables => _disposables;

        /// <summary>
        /// バインド状態をセット（派生クラスから呼び出し）
        /// </summary>
        protected void SetBound()
        {
            _isBound = true;
        }

        protected virtual void OnDestroy()
        {
            _disposables.Dispose();
        }
    }

    /// <summary>
    /// View基底クラス（ジェネリック版）
    /// ViewModelとのバインディングパターンを提供
    /// </summary>
    /// <typeparam name="TViewModel">バインドするViewModelの型</typeparam>
    public abstract class ViewBase<TViewModel> : ViewBase
        where TViewModel : ViewModelBase
    {
        private TViewModel _viewModel;

        /// <summary>
        /// バインドされたViewModel
        /// </summary>
        protected TViewModel ViewModel => _viewModel;

        /// <summary>
        /// VContainerによるDI注入ポイント
        /// Start()後に呼ばれた場合は自動的にバインディングを実行
        /// </summary>
        [Inject]
        public void Construct(TViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            
            // Start()が既に呼ばれていて、まだバインドされていない場合はバインドを実行
            if (_startCalled && !IsBound)
            {
                PerformBinding();
            }
        }

        /// <summary>
        /// ViewModelを手動で設定（親Viewからの手動バインド用）
        /// Start()後に呼ばれた場合は自動的にバインディングを実行
        /// </summary>
        public void SetViewModel(TViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            
            // Start()が既に呼ばれていて、まだバインドされていない場合はバインドを実行
            if (_startCalled && !IsBound)
            {
                PerformBinding();
            }
        }

        private bool _startCalled;

        protected virtual void Start()
        {
            _startCalled = true;
            
            // ViewModelがまだ設定されていない場合は、後でSetViewModelから設定される想定
            // （CellViewのように親Viewから手動でバインドされるケース）
            if (_viewModel == null)
            {
                // VContainer経由で注入されるべきViewの場合のみ警告
                // ただし、親から手動でバインドされるViewもあるので、ここではログ出力しない
                return;
            }

            PerformBinding();
        }
        
        /// <summary>
        /// 実際のバインディング処理
        /// </summary>
        private void PerformBinding()
        {
            if (IsBound || _viewModel == null)
            {
                return;
            }
            
            // ViewModelが未初期化の場合は初期化
            if (!_viewModel.IsInitialized)
            {
                _viewModel.Initialize();
            }

            // バインディング実行
            OnBind(_viewModel);
            SetBound();
        }

        /// <summary>
        /// 派生クラスでオーバーライドしてViewModelとのバインディングを実装
        /// </summary>
        /// <param name="viewModel">バインドするViewModel</param>
        protected abstract void OnBind(TViewModel viewModel);

        protected override void OnDestroy()
        {
            // ViewModelのDispose（Viewが所有する場合）
            // 注: DIコンテナがライフタイムを管理する場合はここでDisposeしない
            // 現在の設計ではViewModelはDIコンテナ管理なのでコメントアウト
            // _viewModel?.Dispose();

            base.OnDestroy();
        }
    }
}
