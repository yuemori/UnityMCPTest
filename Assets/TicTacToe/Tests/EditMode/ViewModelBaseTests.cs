using System;
using NUnit.Framework;
using TicTacToe.Presentation.Base;

namespace TicTacToe.Tests.EditMode.Presentation
{
    /// <summary>
    /// ViewModelBase単体テスト
    /// </summary>
    public class ViewModelBaseTests
    {
        #region Test Fixtures

        /// <summary>
        /// テスト用のシンプルなDisposable実装
        /// </summary>
        private class TestDisposable : IDisposable
        {
            private readonly Action _onDispose;
            public bool IsDisposed { get; private set; }

            public TestDisposable(Action onDispose = null)
            {
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                if (IsDisposed) return;
                IsDisposed = true;
                _onDispose?.Invoke();
            }
        }

        /// <summary>
        /// テスト用の具象ViewModelクラス
        /// </summary>
        private class TestViewModel : ViewModelBase
        {
            public int InitializeCount { get; private set; }
            public int DisposeCount { get; private set; }
            public bool OnInitializeCalled { get; private set; }
            public bool OnDisposeCalled { get; private set; }

            protected override void OnInitialize()
            {
                InitializeCount++;
                OnInitializeCalled = true;
            }

            protected override void OnDispose()
            {
                DisposeCount++;
                OnDisposeCalled = true;
            }

            /// <summary>
            /// Disposablesへの追加をテストするためのメソッド
            /// </summary>
            public void AddTestDisposable(IDisposable disposable)
            {
                Disposables.Add(disposable);
            }

            /// <summary>
            /// ThrowIfDisposedをテストするためのメソッド
            /// </summary>
            public void TestThrowIfDisposed()
            {
                ThrowIfDisposed();
            }
        }

        #endregion

        #region Initialize Tests

        [Test]
        public void Initialize_ShouldSetIsInitializedToTrue()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            viewModel.Initialize();

            // Assert
            Assert.IsTrue(viewModel.IsInitialized);
        }

        [Test]
        public void Initialize_ShouldCallOnInitialize()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            viewModel.Initialize();

            // Assert
            Assert.IsTrue(viewModel.OnInitializeCalled);
            Assert.AreEqual(1, viewModel.InitializeCount);
        }

        [Test]
        public void Initialize_CalledMultipleTimes_ShouldOnlyInitializeOnce()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            viewModel.Initialize();
            viewModel.Initialize();
            viewModel.Initialize();

            // Assert
            Assert.AreEqual(1, viewModel.InitializeCount);
        }

        [Test]
        public void Initialize_AfterDispose_ShouldThrowObjectDisposedException()
        {
            // Arrange
            var viewModel = new TestViewModel();
            viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => viewModel.Initialize());
        }

        #endregion

        #region Dispose Tests

        [Test]
        public void Dispose_ShouldSetIsDisposedToTrue()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            viewModel.Dispose();

            // Assert
            Assert.IsTrue(viewModel.IsDisposed);
        }

        [Test]
        public void Dispose_ShouldCallOnDispose()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            viewModel.Dispose();

            // Assert
            Assert.IsTrue(viewModel.OnDisposeCalled);
            Assert.AreEqual(1, viewModel.DisposeCount);
        }

        [Test]
        public void Dispose_CalledMultipleTimes_ShouldOnlyDisposeOnce()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            viewModel.Dispose();
            viewModel.Dispose();
            viewModel.Dispose();

            // Assert
            Assert.AreEqual(1, viewModel.DisposeCount);
        }

        [Test]
        public void Dispose_ShouldDisposeCompositeDisposable()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var testDisposable = new TestDisposable();
            viewModel.AddTestDisposable(testDisposable);

            // Act
            viewModel.Dispose();

            // Assert
            Assert.IsTrue(testDisposable.IsDisposed);
        }

        #endregion

        #region ThrowIfDisposed Tests

        [Test]
        public void ThrowIfDisposed_WhenNotDisposed_ShouldNotThrow()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act & Assert
            Assert.DoesNotThrow(() => viewModel.TestThrowIfDisposed());
        }

        [Test]
        public void ThrowIfDisposed_WhenDisposed_ShouldThrowObjectDisposedException()
        {
            // Arrange
            var viewModel = new TestViewModel();
            viewModel.Dispose();

            // Act & Assert
            var exception = Assert.Throws<ObjectDisposedException>(() => viewModel.TestThrowIfDisposed());
            Assert.IsTrue(exception.ObjectName.Contains("TestViewModel"));
        }

        #endregion

        #region Property Tests

        [Test]
        public void IsInitialized_BeforeInitialize_ShouldBeFalse()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Assert
            Assert.IsFalse(viewModel.IsInitialized);
        }

        [Test]
        public void IsDisposed_BeforeDispose_ShouldBeFalse()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Assert
            Assert.IsFalse(viewModel.IsDisposed);
        }

        #endregion

        #region CompositeDisposable Tests

        [Test]
        public void Disposables_ShouldBeAvailableForSubscriptions()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var disposed = false;
            var testDisposable = new TestDisposable(() => disposed = true);

            // Act
            viewModel.AddTestDisposable(testDisposable);
            viewModel.Dispose();

            // Assert
            Assert.IsTrue(disposed);
        }

        [Test]
        public void Disposables_MultipleDisposables_AllShouldBeDisposed()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var disposed1 = false;
            var disposed2 = false;
            var disposed3 = false;

            viewModel.AddTestDisposable(new TestDisposable(() => disposed1 = true));
            viewModel.AddTestDisposable(new TestDisposable(() => disposed2 = true));
            viewModel.AddTestDisposable(new TestDisposable(() => disposed3 = true));

            // Act
            viewModel.Dispose();

            // Assert
            Assert.IsTrue(disposed1);
            Assert.IsTrue(disposed2);
            Assert.IsTrue(disposed3);
        }

        #endregion

        #region Integration Tests

        [Test]
        public void FullLifecycle_InitializeThenDispose_ShouldWorkCorrectly()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act & Assert - Before Initialize
            Assert.IsFalse(viewModel.IsInitialized);
            Assert.IsFalse(viewModel.IsDisposed);

            // Act - Initialize
            viewModel.Initialize();

            // Assert - After Initialize
            Assert.IsTrue(viewModel.IsInitialized);
            Assert.IsFalse(viewModel.IsDisposed);
            Assert.IsTrue(viewModel.OnInitializeCalled);

            // Act - Dispose
            viewModel.Dispose();

            // Assert - After Dispose
            Assert.IsTrue(viewModel.IsInitialized);
            Assert.IsTrue(viewModel.IsDisposed);
            Assert.IsTrue(viewModel.OnDisposeCalled);
        }

        [Test]
        public void MethodsAfterDispose_ShouldThrow()
        {
            // Arrange
            var viewModel = new TestViewModel();
            viewModel.Dispose();

            // Assert - Methods should throw after dispose
            Assert.Throws<ObjectDisposedException>(() => viewModel.TestThrowIfDisposed());
        }

        #endregion
    }
}
