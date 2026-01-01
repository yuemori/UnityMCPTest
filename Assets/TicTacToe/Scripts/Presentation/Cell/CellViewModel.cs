using System;
using R3;
using TicTacToe.Core.Domain;
using TicTacToe.Presentation.Base;

namespace TicTacToe.Presentation.Cell
{
    /// <summary>
    /// 個別セルのViewModel
    /// セルの状態表示とクリックイベントを管理
    /// </summary>
    public class CellViewModel : ViewModelBase
    {
        private readonly BoardPosition _position;
        private readonly ReactiveProperty<CellState> _cellState;
        private readonly ReactiveProperty<bool> _isClickable;
        private readonly Subject<BoardPosition> _onCellClicked;

        /// <summary>
        /// セルの盤面位置
        /// </summary>
        public BoardPosition Position => _position;

        /// <summary>
        /// セルの現在状態（Empty, X, O）
        /// </summary>
        public ReadOnlyReactiveProperty<CellState> CellState => _cellState;

        /// <summary>
        /// クリック可能かどうか（空かつゲーム中）
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsClickable => _isClickable;

        /// <summary>
        /// セルがクリックされたときに発火するObservable
        /// BoardViewModelでsubscribeして処理を行う
        /// </summary>
        public Observable<BoardPosition> OnCellClicked => _onCellClicked;

        /// <summary>
        /// CellViewModelを作成
        /// </summary>
        /// <param name="position">セルの盤面位置</param>
        public CellViewModel(BoardPosition position)
        {
            _position = position;
            _cellState = new ReactiveProperty<CellState>(Core.Domain.CellState.Empty);
            _isClickable = new ReactiveProperty<bool>(true);
            _onCellClicked = new Subject<BoardPosition>();
        }

        /// <summary>
        /// セルの状態を更新
        /// </summary>
        /// <param name="state">新しいセル状態</param>
        public void UpdateState(CellState state)
        {
            ThrowIfDisposed();
            _cellState.Value = state;
        }

        /// <summary>
        /// クリック可能状態を更新
        /// </summary>
        /// <param name="clickable">クリック可能かどうか</param>
        public void SetClickable(bool clickable)
        {
            ThrowIfDisposed();
            _isClickable.Value = clickable;
        }

        /// <summary>
        /// セルがクリックされた（Viewから呼び出し）
        /// </summary>
        public void OnClick()
        {
            ThrowIfDisposed();
            
            if (!_isClickable.CurrentValue)
            {
                return;
            }

            _onCellClicked.OnNext(_position);
        }

        /// <summary>
        /// セルをリセット（新しいゲーム開始時）
        /// </summary>
        public void Reset()
        {
            ThrowIfDisposed();
            _cellState.Value = Core.Domain.CellState.Empty;
            _isClickable.Value = true;
        }

        protected override void OnDispose()
        {
            _cellState.Dispose();
            _isClickable.Dispose();
            _onCellClicked.Dispose();
        }
    }
}
