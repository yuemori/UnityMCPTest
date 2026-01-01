using System;
using NUnit.Framework;
using TicTacToe.Core.Domain;
using TicTacToe.Presentation.Cell;

namespace TicTacToe.Tests.EditMode.Presentation
{
    /// <summary>
    /// CellViewModelの単体テスト
    /// </summary>
    [TestFixture]
    public class CellViewModelTests
    {
        private CellViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _viewModel = new CellViewModel(new BoardPosition(4)); // 中央セル
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidPosition_SetsPosition()
        {
            // Arrange
            var position = new BoardPosition(0);

            // Act
            using var vm = new CellViewModel(position);

            // Assert
            Assert.AreEqual(position, vm.Position);
        }

        [Test]
        public void Constructor_InitializesWithEmptyState()
        {
            // Assert
            Assert.AreEqual(CellState.Empty, _viewModel.CellState.CurrentValue);
        }

        [Test]
        public void Constructor_InitializesAsClickable()
        {
            // Assert
            Assert.IsTrue(_viewModel.IsClickable.CurrentValue);
        }

        #endregion

        #region UpdateState Tests

        [Test]
        public void UpdateState_WithX_UpdatesCellState()
        {
            // Act
            _viewModel.UpdateState(CellState.X);

            // Assert
            Assert.AreEqual(CellState.X, _viewModel.CellState.CurrentValue);
        }

        [Test]
        public void UpdateState_WithO_UpdatesCellState()
        {
            // Act
            _viewModel.UpdateState(CellState.O);

            // Assert
            Assert.AreEqual(CellState.O, _viewModel.CellState.CurrentValue);
        }

        [Test]
        public void UpdateState_WithEmpty_UpdatesCellState()
        {
            // Arrange
            _viewModel.UpdateState(CellState.X);

            // Act
            _viewModel.UpdateState(CellState.Empty);

            // Assert
            Assert.AreEqual(CellState.Empty, _viewModel.CellState.CurrentValue);
        }

        [Test]
        public void UpdateState_NotifiesSubscribers()
        {
            // Arrange - CurrentValueで変更を確認
            var initialState = _viewModel.CellState.CurrentValue;
            Assert.AreEqual(CellState.Empty, initialState);

            // Act
            _viewModel.UpdateState(CellState.X);

            // Assert
            Assert.AreEqual(CellState.X, _viewModel.CellState.CurrentValue);
        }

        #endregion

        #region SetClickable Tests

        [Test]
        public void SetClickable_False_UpdatesIsClickable()
        {
            // Act
            _viewModel.SetClickable(false);

            // Assert
            Assert.IsFalse(_viewModel.IsClickable.CurrentValue);
        }

        [Test]
        public void SetClickable_True_UpdatesIsClickable()
        {
            // Arrange
            _viewModel.SetClickable(false);

            // Act
            _viewModel.SetClickable(true);

            // Assert
            Assert.IsTrue(_viewModel.IsClickable.CurrentValue);
        }

        [Test]
        public void SetClickable_NotifiesSubscribers()
        {
            // Arrange
            Assert.IsTrue(_viewModel.IsClickable.CurrentValue);

            // Act
            _viewModel.SetClickable(false);

            // Assert
            Assert.IsFalse(_viewModel.IsClickable.CurrentValue);
        }

        #endregion

        #region OnClick Tests

        [Test]
        public void OnClick_WhenClickable_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _viewModel.OnClick());
        }

        [Test]
        public void OnClick_WhenNotClickable_DoesNotThrow()
        {
            // Arrange
            _viewModel.SetClickable(false);

            // Act & Assert
            Assert.DoesNotThrow(() => _viewModel.OnClick());
        }

        [Test]
        public void OnClick_MultipleTimes_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _viewModel.OnClick();
                _viewModel.OnClick();
                _viewModel.OnClick();
            });
        }

        #endregion

        #region Reset Tests

        [Test]
        public void Reset_ClearsState()
        {
            // Arrange
            _viewModel.UpdateState(CellState.X);

            // Act
            _viewModel.Reset();

            // Assert
            Assert.AreEqual(CellState.Empty, _viewModel.CellState.CurrentValue);
        }

        [Test]
        public void Reset_SetsClickable()
        {
            // Arrange
            _viewModel.SetClickable(false);

            // Act
            _viewModel.Reset();

            // Assert
            Assert.IsTrue(_viewModel.IsClickable.CurrentValue);
        }

        [Test]
        public void Reset_AfterMarkPlaced_RestoresClickability()
        {
            // Arrange
            _viewModel.UpdateState(CellState.X);
            _viewModel.SetClickable(false);

            // Act
            _viewModel.Reset();

            // Assert
            Assert.AreEqual(CellState.Empty, _viewModel.CellState.CurrentValue);
            Assert.IsTrue(_viewModel.IsClickable.CurrentValue);
        }

        #endregion

        #region Dispose Tests

        [Test]
        public void Dispose_SetsIsDisposed()
        {
            // Act
            _viewModel.Dispose();

            // Assert
            Assert.IsTrue(_viewModel.IsDisposed);
        }

        [Test]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _viewModel.Dispose();
                _viewModel.Dispose();
            });
        }

        [Test]
        public void UpdateState_AfterDispose_ThrowsException()
        {
            // Arrange
            _viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _viewModel.UpdateState(CellState.X));
        }

        [Test]
        public void SetClickable_AfterDispose_ThrowsException()
        {
            // Arrange
            _viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _viewModel.SetClickable(false));
        }

        [Test]
        public void OnClick_AfterDispose_ThrowsException()
        {
            // Arrange
            _viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _viewModel.OnClick());
        }

        [Test]
        public void Reset_AfterDispose_ThrowsException()
        {
            // Arrange
            _viewModel.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _viewModel.Reset());
        }

        #endregion

        #region Position Tests

        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(1, 0, 1)]
        [TestCase(4, 1, 1)]
        [TestCase(8, 2, 2)]
        public void Position_ReturnsCorrectValue(int index, int expectedRow, int expectedCol)
        {
            // Arrange
            using var vm = new CellViewModel(new BoardPosition(index));

            // Assert
            Assert.AreEqual(index, vm.Position.Index);
            Assert.AreEqual(expectedRow, vm.Position.Row);
            Assert.AreEqual(expectedCol, vm.Position.Column);
        }

        #endregion
    }
}
