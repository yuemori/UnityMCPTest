using System;

namespace TicTacToe.Core.Domain
{
    /// <summary>
    /// 現在のターン情報を表す構造体
    /// </summary>
    public readonly struct TurnInfo : IEquatable<TurnInfo>
    {
        /// <summary>現在のターンのマーク（XまたはO）</summary>
        public CellState CurrentMark { get; }
        
        /// <summary>現在のプレイヤー種別</summary>
        public PlayerType CurrentPlayerType { get; }
        
        /// <summary>ターン番号（1から開始）</summary>
        public int TurnNumber { get; }
        
        /// <summary>Xプレイヤーの種別</summary>
        public PlayerType XPlayerType { get; }
        
        /// <summary>Oプレイヤーの種別</summary>
        public PlayerType OPlayerType { get; }
        
        /// <summary>現在のプレイヤーがAIかどうか</summary>
        public bool IsCurrentPlayerAI => CurrentPlayerType == PlayerType.AI;
        
        /// <summary>現在のプレイヤーが人間かどうか</summary>
        public bool IsCurrentPlayerHuman => CurrentPlayerType == PlayerType.Human;
        
        /// <summary>
        /// ターン情報を作成
        /// </summary>
        /// <param name="currentMark">現在のターンのマーク</param>
        /// <param name="turnNumber">ターン番号</param>
        /// <param name="xPlayerType">Xプレイヤーの種別</param>
        /// <param name="oPlayerType">Oプレイヤーの種別</param>
        /// <exception cref="ArgumentException">currentMarkがEmptyの場合</exception>
        /// <exception cref="ArgumentOutOfRangeException">turnNumberが0以下の場合</exception>
        public TurnInfo(CellState currentMark, int turnNumber, PlayerType xPlayerType, PlayerType oPlayerType)
        {
            if (currentMark == CellState.Empty)
            {
                throw new ArgumentException("Current mark cannot be Empty", nameof(currentMark));
            }
            if (turnNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(turnNumber), 
                    "Turn number must be greater than 0");
            }
            
            CurrentMark = currentMark;
            TurnNumber = turnNumber;
            XPlayerType = xPlayerType;
            OPlayerType = oPlayerType;
            CurrentPlayerType = currentMark == CellState.X ? xPlayerType : oPlayerType;
        }
        
        /// <summary>
        /// 初期ターン情報を作成（先手X、ターン1）
        /// </summary>
        public static TurnInfo CreateInitial(PlayerType xPlayerType, PlayerType oPlayerType) =>
            new TurnInfo(CellState.X, 1, xPlayerType, oPlayerType);
        
        /// <summary>
        /// 次のターン情報を作成
        /// </summary>
        public TurnInfo NextTurn()
        {
            var nextMark = CurrentMark == CellState.X ? CellState.O : CellState.X;
            return new TurnInfo(nextMark, TurnNumber + 1, XPlayerType, OPlayerType);
        }
        
        /// <summary>
        /// 相手のマークを取得
        /// </summary>
        public CellState GetOpponentMark() => CurrentMark == CellState.X ? CellState.O : CellState.X;
        
        public bool Equals(TurnInfo other) =>
            CurrentMark == other.CurrentMark &&
            TurnNumber == other.TurnNumber &&
            XPlayerType == other.XPlayerType &&
            OPlayerType == other.OPlayerType;
        
        public override bool Equals(object obj) => obj is TurnInfo other && Equals(other);
        
        public override int GetHashCode() => HashCode.Combine(CurrentMark, TurnNumber, XPlayerType, OPlayerType);
        
        public override string ToString() => 
            $"Turn {TurnNumber}: {CurrentMark} ({CurrentPlayerType})";
        
        public static bool operator ==(TurnInfo left, TurnInfo right) => left.Equals(right);
        
        public static bool operator !=(TurnInfo left, TurnInfo right) => !left.Equals(right);
    }
}
