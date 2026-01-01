using System;

namespace TicTacToe.Core.Domain
{
    /// <summary>
    /// ゲームの状態を表す列挙型
    /// </summary>
    public enum GameState
    {
        /// <summary>ゲーム進行中</summary>
        InProgress = 0,
        
        /// <summary>勝者が決定</summary>
        Win = 1,
        
        /// <summary>引き分け</summary>
        Draw = 2
    }
    
    /// <summary>
    /// ゲーム結果を表す構造体
    /// </summary>
    public readonly struct GameResult : IEquatable<GameResult>
    {
        /// <summary>ゲームの状態</summary>
        public GameState State { get; }
        
        /// <summary>勝者のマーク（Winの場合のみ有効）</summary>
        public CellState? Winner { get; }
        
        /// <summary>勝利ライン（Winの場合のみ有効）</summary>
        public WinLine? WinningLine { get; }
        
        /// <summary>ゲームが終了しているかどうか</summary>
        public bool IsGameOver => State != GameState.InProgress;
        
        private GameResult(GameState state, CellState? winner, WinLine? winningLine)
        {
            State = state;
            Winner = winner;
            WinningLine = winningLine;
        }
        
        /// <summary>
        /// ゲーム進行中の結果を作成
        /// </summary>
        public static GameResult InProgress() => new GameResult(GameState.InProgress, null, null);
        
        /// <summary>
        /// 勝利の結果を作成
        /// </summary>
        /// <param name="winner">勝者のマーク（XまたはO）</param>
        /// <param name="winningLine">勝利ライン</param>
        /// <exception cref="ArgumentException">勝者がEmptyの場合</exception>
        public static GameResult Win(CellState winner, WinLine winningLine)
        {
            if (winner == CellState.Empty)
            {
                throw new ArgumentException("Winner cannot be Empty", nameof(winner));
            }
            return new GameResult(GameState.Win, winner, winningLine);
        }
        
        /// <summary>
        /// 引き分けの結果を作成
        /// </summary>
        public static GameResult Draw() => new GameResult(GameState.Draw, null, null);
        
        public bool Equals(GameResult other) =>
            State == other.State && Winner == other.Winner && WinningLine == other.WinningLine;
        
        public override bool Equals(object obj) => obj is GameResult other && Equals(other);
        
        public override int GetHashCode() => HashCode.Combine(State, Winner, WinningLine);
        
        public override string ToString() => State switch
        {
            GameState.InProgress => "InProgress",
            GameState.Win => $"Win: {Winner}",
            GameState.Draw => "Draw",
            _ => "Unknown"
        };
        
        public static bool operator ==(GameResult left, GameResult right) => left.Equals(right);
        
        public static bool operator !=(GameResult left, GameResult right) => !left.Equals(right);
    }
    
    /// <summary>
    /// 勝利ラインの種類を表す列挙型
    /// </summary>
    public enum WinLine
    {
        /// <summary>1行目（横）</summary>
        Row0 = 0,
        /// <summary>2行目（横）</summary>
        Row1 = 1,
        /// <summary>3行目（横）</summary>
        Row2 = 2,
        /// <summary>1列目（縦）</summary>
        Column0 = 3,
        /// <summary>2列目（縦）</summary>
        Column1 = 4,
        /// <summary>3列目（縦）</summary>
        Column2 = 5,
        /// <summary>左上から右下への対角線</summary>
        DiagonalMain = 6,
        /// <summary>右上から左下への対角線</summary>
        DiagonalAnti = 7
    }
}
