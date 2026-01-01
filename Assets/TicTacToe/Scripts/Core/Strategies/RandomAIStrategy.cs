using System;
using System.Collections.Generic;
using TicTacToe.Core.Domain;

namespace TicTacToe.Core.Strategies
{
    /// <summary>
    /// ランダムに空きマスを選択するAI戦略
    /// </summary>
    public class RandomAIStrategy : IAIStrategy
    {
        private readonly Random _random;
        
        /// <summary>
        /// 戦略の名前
        /// </summary>
        public string Name => "Random";
        
        /// <summary>
        /// デフォルトのRandomインスタンスで初期化
        /// </summary>
        public RandomAIStrategy() : this(new Random())
        {
        }
        
        /// <summary>
        /// 指定したRandomインスタンスで初期化（テスト用）
        /// </summary>
        /// <param name="random">乱数生成器</param>
        public RandomAIStrategy(Random random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }
        
        /// <summary>
        /// 空きマスからランダムに1つ選択
        /// </summary>
        /// <param name="board">現在の盤面状態</param>
        /// <param name="aiMark">AIのマーク</param>
        /// <returns>配置する位置</returns>
        /// <exception cref="ArgumentNullException">boardがnullの場合</exception>
        /// <exception cref="ArgumentException">aiMarkがEmptyの場合</exception>
        /// <exception cref="InvalidOperationException">空きマスがない場合</exception>
        public BoardPosition DecideMove(IReadOnlyList<CellState> board, CellState aiMark)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }
            
            if (aiMark == CellState.Empty)
            {
                throw new ArgumentException("AI mark cannot be Empty", nameof(aiMark));
            }
            
            // 空きマスを収集
            var emptyPositions = new List<BoardPosition>();
            for (int i = 0; i < board.Count; i++)
            {
                if (board[i] == CellState.Empty)
                {
                    emptyPositions.Add(new BoardPosition(i));
                }
            }
            
            if (emptyPositions.Count == 0)
            {
                throw new InvalidOperationException("No empty positions available");
            }
            
            // ランダムに選択
            var selectedIndex = _random.Next(emptyPositions.Count);
            return emptyPositions[selectedIndex];
        }
    }
}
