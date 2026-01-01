using System;
using System.Collections.Generic;
using R3;
using TicTacToe.Core.Domain;

namespace TicTacToe.Core.Repositories
{
    /// <summary>
    /// 盤面データを管理するリポジトリ
    /// </summary>
    public class BoardRepository : IDisposable
    {
        private readonly CellState[] _cells;
        private readonly ReactiveProperty<int> _moveCount;
        private bool _disposed;
        
        /// <summary>
        /// 盤面が更新されたときに発火するObservable
        /// </summary>
        public Observable<BoardPosition> OnCellChanged => _onCellChanged;
        private readonly Subject<BoardPosition> _onCellChanged;
        
        /// <summary>
        /// 配置されたマークの数
        /// </summary>
        public ReadOnlyReactiveProperty<int> MoveCount => _moveCount;
        
        /// <summary>
        /// 盤面が空かどうか
        /// </summary>
        public bool IsEmpty => _moveCount.CurrentValue == 0;
        
        /// <summary>
        /// 盤面が満杯かどうか
        /// </summary>
        public bool IsFull => _moveCount.CurrentValue == BoardPosition.TotalCells;
        
        public BoardRepository()
        {
            _cells = new CellState[BoardPosition.TotalCells];
            _moveCount = new ReactiveProperty<int>(0);
            _onCellChanged = new Subject<BoardPosition>();
        }
        
        /// <summary>
        /// 指定位置のセル状態を取得
        /// </summary>
        /// <param name="position">盤面位置</param>
        /// <returns>セル状態</returns>
        public CellState GetCell(BoardPosition position)
        {
            ThrowIfDisposed();
            return _cells[position.Index];
        }
        
        /// <summary>
        /// 指定位置にマークを配置
        /// </summary>
        /// <param name="position">盤面位置</param>
        /// <param name="mark">配置するマーク（XまたはO）</param>
        /// <returns>配置成功の場合true</returns>
        /// <exception cref="ArgumentException">markがEmptyの場合</exception>
        public bool TryPlaceMark(BoardPosition position, CellState mark)
        {
            ThrowIfDisposed();
            
            if (mark == CellState.Empty)
            {
                throw new ArgumentException("Cannot place Empty mark", nameof(mark));
            }
            
            if (_cells[position.Index] != CellState.Empty)
            {
                return false;
            }
            
            _cells[position.Index] = mark;
            _moveCount.Value++;
            _onCellChanged.OnNext(position);
            return true;
        }
        
        /// <summary>
        /// 盤面をリセット
        /// </summary>
        public void Reset()
        {
            ThrowIfDisposed();
            
            for (int i = 0; i < BoardPosition.TotalCells; i++)
            {
                _cells[i] = CellState.Empty;
            }
            _moveCount.Value = 0;
        }
        
        /// <summary>
        /// すべての空きセルの位置を取得
        /// </summary>
        /// <returns>空きセルの位置リスト</returns>
        public IReadOnlyList<BoardPosition> GetEmptyPositions()
        {
            ThrowIfDisposed();
            
            var emptyPositions = new List<BoardPosition>();
            for (int i = 0; i < BoardPosition.TotalCells; i++)
            {
                if (_cells[i] == CellState.Empty)
                {
                    emptyPositions.Add(new BoardPosition(i));
                }
            }
            return emptyPositions;
        }
        
        /// <summary>
        /// 盤面の読み取り専用コピーを取得
        /// </summary>
        /// <returns>盤面のコピー</returns>
        public IReadOnlyList<CellState> GetBoardSnapshot()
        {
            ThrowIfDisposed();
            
            var snapshot = new CellState[BoardPosition.TotalCells];
            Array.Copy(_cells, snapshot, BoardPosition.TotalCells);
            return snapshot;
        }
        
        /// <summary>
        /// 指定位置が空かどうかを判定
        /// </summary>
        /// <param name="position">盤面位置</param>
        /// <returns>空の場合true</returns>
        public bool IsPositionEmpty(BoardPosition position)
        {
            ThrowIfDisposed();
            return _cells[position.Index] == CellState.Empty;
        }
        
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(BoardRepository));
            }
        }
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _disposed = true;
            _onCellChanged.Dispose();
            _moveCount.Dispose();
        }
    }
}
