namespace TicTacToe.Core.Domain
{
    /// <summary>
    /// セルの状態を表す列挙型
    /// </summary>
    public enum CellState
    {
        /// <summary>空のセル</summary>
        Empty = 0,
        
        /// <summary>Xマーク（先手）</summary>
        X = 1,
        
        /// <summary>Oマーク（後手）</summary>
        O = 2
    }
}
