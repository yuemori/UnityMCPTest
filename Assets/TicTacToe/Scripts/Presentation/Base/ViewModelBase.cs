using System;
using R3;

namespace TicTacToe.Presentation.Base
{
    /// <summary>
    /// ViewModel基底クラス
    /// R3のCompositeDisposableを使用したライフサイクル管理を提供
    /// </summary>
    public abstract class ViewModelBase : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private bool _disposed;
        private bool _initialized;

        /// <summary>
        /// Disposeされたかどうか
        /// </summary>
        public bool IsDisposed => _disposed;

        /// <summary>
        /// 初期化されたかどうか
        /// </summary>
        public bool IsInitialized => _initialized;

        /// <summary>
        /// Subscription管理用のCompositeDisposable
        /// 派生クラスで使用可能
        /// </summary>
        protected CompositeDisposable Disposables => _disposables;

        /// <summary>
        /// ViewModelを初期化
        /// 派生クラスはOnInitializeをオーバーライドして初期化処理を実装
        /// </summary>
        public void Initialize()
        {
            ThrowIfDisposed();

            if (_initialized)
            {
                return;
            }

            _initialized = true;
            OnInitialize();
        }

        /// <summary>
        /// 派生クラスでオーバーライドして初期化処理を実装
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// 派生クラスでオーバーライドしてクリーンアップ処理を実装
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        /// Disposed状態をチェックし、Disposedの場合は例外をスロー
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                OnDispose();
                _disposables.Dispose();
            }

            _disposed = true;
        }
    }
}
