using System;

namespace TicTacToe.Core.Domain
{
    /// <summary>
    /// 盤面上の位置を表す構造体（0-8のインデックス、または行・列で指定）
    /// </summary>
    public readonly struct BoardPosition : IEquatable<BoardPosition>
    {
        /// <summary>盤面のサイズ（3x3）</summary>
        public const int BoardSize = 3;
        
        /// <summary>セルの総数</summary>
        public const int TotalCells = BoardSize * BoardSize;
        
        /// <summary>0-8のインデックス値</summary>
        public int Index { get; }
        
        /// <summary>行（0-2）</summary>
        public int Row => Index / BoardSize;
        
        /// <summary>列（0-2）</summary>
        public int Column => Index % BoardSize;
        
        /// <summary>
        /// インデックスから位置を作成
        /// </summary>
        /// <param name="index">0-8のインデックス</param>
        /// <exception cref="ArgumentOutOfRangeException">インデックスが範囲外の場合</exception>
        public BoardPosition(int index)
        {
            if (index < 0 || index >= TotalCells)
            {
                throw new ArgumentOutOfRangeException(nameof(index), 
                    $"Index must be between 0 and {TotalCells - 1}, but was {index}");
            }
            Index = index;
        }
        
        /// <summary>
        /// 行と列から位置を作成
        /// </summary>
        /// <param name="row">行（0-2）</param>
        /// <param name="column">列（0-2）</param>
        /// <exception cref="ArgumentOutOfRangeException">行または列が範囲外の場合</exception>
        public BoardPosition(int row, int column)
        {
            if (row < 0 || row >= BoardSize)
            {
                throw new ArgumentOutOfRangeException(nameof(row),
                    $"Row must be between 0 and {BoardSize - 1}, but was {row}");
            }
            if (column < 0 || column >= BoardSize)
            {
                throw new ArgumentOutOfRangeException(nameof(column),
                    $"Column must be between 0 and {BoardSize - 1}, but was {column}");
            }
            Index = row * BoardSize + column;
        }
        
        /// <summary>
        /// インデックスが有効な範囲内かどうかを判定
        /// </summary>
        public static bool IsValidIndex(int index) => index >= 0 && index < TotalCells;
        
        /// <summary>
        /// 行と列が有効な範囲内かどうかを判定
        /// </summary>
        public static bool IsValidPosition(int row, int column) =>
            row >= 0 && row < BoardSize && column >= 0 && column < BoardSize;
        
        /// <summary>
        /// インデックスから位置を作成（無効な場合はnull）
        /// </summary>
        public static BoardPosition? TryCreate(int index) =>
            IsValidIndex(index) ? new BoardPosition(index) : null;
        
        /// <summary>
        /// 行と列から位置を作成（無効な場合はnull）
        /// </summary>
        public static BoardPosition? TryCreate(int row, int column) =>
            IsValidPosition(row, column) ? new BoardPosition(row, column) : null;
        
        public bool Equals(BoardPosition other) => Index == other.Index;
        
        public override bool Equals(object obj) => obj is BoardPosition other && Equals(other);
        
        public override int GetHashCode() => Index;
        
        public override string ToString() => $"({Row}, {Column})";
        
        public static bool operator ==(BoardPosition left, BoardPosition right) => left.Equals(right);
        
        public static bool operator !=(BoardPosition left, BoardPosition right) => !left.Equals(right);
        
        /// <summary>
        /// intからの暗黙的変換
        /// </summary>
        public static implicit operator BoardPosition(int index) => new BoardPosition(index);
    }
}
