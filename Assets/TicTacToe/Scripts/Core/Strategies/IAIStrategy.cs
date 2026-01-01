using System.Collections.Generic;
using TicTacToe.Core.Domain;

namespace TicTacToe.Core.Strategies
{
    /// <summary>
    /// AI戦略のインターフェース
    /// Strategy Patternで異なるAIアルゴリズムを差し替え可能にする
    /// </summary>
    public interface IAIStrategy
    {
        /// <summary>
        /// 戦略の名前（識別・デバッグ用）
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// AIの手を決定する
        /// </summary>
        /// <param name="board">現在の盤面状態（9マスのCellState配列）</param>
        /// <param name="aiMark">AIのマーク（XまたはO）</param>
        /// <returns>配置する位置</returns>
        BoardPosition DecideMove(IReadOnlyList<CellState> board, CellState aiMark);
    }
}
