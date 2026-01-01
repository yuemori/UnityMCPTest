# Domain Model Definition

## Entities & Value Objects

### CellState (Enum)
マス目の状態を表す列挙型。

```csharp
namespace TicTacToe.Core.Domain
{
    public enum CellState
    {
        Empty = 0,
        X = 1,
        O = 2
    }
}
```

### PlayerType (Enum)
プレイヤーの種類を表す列挙型。

```csharp
namespace TicTacToe.Core.Domain
{
    public enum PlayerType
    {
        Human,
        AI
    }
}
```

### BoardPosition (Value Object)
盤面上の位置を表す値オブジェクト。

```csharp
namespace TicTacToe.Core.Domain
{
    public readonly struct BoardPosition : IEquatable<BoardPosition>
    {
        public int Row { get; }      // 0-2
        public int Column { get; }   // 0-2
        public int Index => Row * 3 + Column;  // 0-8

        public BoardPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public static BoardPosition FromIndex(int index)
            => new BoardPosition(index / 3, index % 3);

        public bool Equals(BoardPosition other)
            => Row == other.Row && Column == other.Column;

        public override bool Equals(object obj)
            => obj is BoardPosition other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Row, Column);

        public static bool operator ==(BoardPosition left, BoardPosition right)
            => left.Equals(right);

        public static bool operator !=(BoardPosition left, BoardPosition right)
            => !left.Equals(right);
    }
}
```

### GameResult (Value Object)
ゲーム結果を表す値オブジェクト。

```csharp
namespace TicTacToe.Core.Domain
{
    public enum GameResultType
    {
        InProgress,
        Win,
        Draw
    }

    public readonly struct GameResult
    {
        public GameResultType Type { get; }
        public CellState Winner { get; }           // Win時のみ有効
        public IReadOnlyList<int> WinLine { get; } // 勝利ライン（3つのインデックス）

        public bool IsFinished => Type != GameResultType.InProgress;

        public static GameResult InProgress()
            => new GameResult(GameResultType.InProgress, CellState.Empty, null);

        public static GameResult Win(CellState winner, IReadOnlyList<int> winLine)
            => new GameResult(GameResultType.Win, winner, winLine);

        public static GameResult Draw()
            => new GameResult(GameResultType.Draw, CellState.Empty, null);

        private GameResult(GameResultType type, CellState winner, IReadOnlyList<int> winLine)
        {
            Type = type;
            Winner = winner;
            WinLine = winLine;
        }
    }
}
```

### TurnInfo (Value Object)
現在のターン情報。

```csharp
namespace TicTacToe.Core.Domain
{
    public readonly struct TurnInfo
    {
        public CellState CurrentMark { get; }    // X or O
        public PlayerType CurrentPlayer { get; } // Human or AI
        public int TurnNumber { get; }           // 1-9

        public TurnInfo(CellState currentMark, PlayerType currentPlayer, int turnNumber)
        {
            CurrentMark = currentMark;
            CurrentPlayer = currentPlayer;
            TurnNumber = turnNumber;
        }
    }
}
```

## Domain Rules

### 盤面ルール
- 3x3の9マス
- 各マスは Empty, X, O のいずれか
- 一度配置されたマークは変更不可

### 勝利条件
以下のいずれかのラインを同じマークで揃える：
```
横: [0,1,2], [3,4,5], [6,7,8]
縦: [0,3,6], [1,4,7], [2,5,8]
斜: [0,4,8], [2,4,6]
```

### ターン進行
1. ゲーム開始時、先手（X）をランダムに Human または AI に割り当て
2. 交互にマークを配置
3. 勝敗決定または9マス埋まるまで継続

## Class Diagram

```
┌─────────────────┐     ┌─────────────────┐
│   CellState     │     │   PlayerType    │
│   <<enum>>      │     │   <<enum>>      │
├─────────────────┤     ├─────────────────┤
│ Empty           │     │ Human           │
│ X               │     │ AI              │
│ O               │     └─────────────────┘
└─────────────────┘
         │
         │ uses
         ▼
┌─────────────────┐     ┌─────────────────┐
│ BoardPosition   │     │   TurnInfo      │
│ <<value>>       │     │   <<value>>     │
├─────────────────┤     ├─────────────────┤
│ + Row: int      │     │ + CurrentMark   │
│ + Column: int   │     │ + CurrentPlayer │
│ + Index: int    │     │ + TurnNumber    │
├─────────────────┤     └─────────────────┘
│ + FromIndex()   │
│ + Equals()      │
└─────────────────┘
         │
         │ referenced by
         ▼
┌─────────────────┐
│   GameResult    │
│   <<value>>     │
├─────────────────┤
│ + Type          │
│ + Winner        │
│ + WinLine       │
│ + IsFinished    │
├─────────────────┤
│ + InProgress()  │
│ + Win()         │
│ + Draw()        │
└─────────────────┘
```
