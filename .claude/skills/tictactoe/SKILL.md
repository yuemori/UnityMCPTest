---
name: tictactoe
description: Tic Tac Toe game development for Unity. Use for game logic, AI implementation, MVVM architecture, and UI development specific to this project.
---

# Tic Tac Toe Game Development Skill

Unity 6でのTic Tac Toe（三目並べ）AI対戦ゲーム開発のためのスキル定義。

## When to Use This Skill

このスキルは以下の作業時にトリガーされる：
- TicTacToeゲームのロジック実装
- 盤面管理、勝敗判定、ターン管理
- AI対戦ロジックの実装
- MVVM + Clean Architectureに基づくUI実装
- VContainer/UniRXを使用したDI・イベント実装

## Project Overview

### Game Specification
- **ゲームモード**: AI対戦オンリー（難易度なし）
- **先手後手**: ランダム決定
- **UI**: 2D uGUI（PC向け、マウス操作）
- **演出**: アニメーション、効果音あり
- **スコア/リプレイ**: なし

### Tech Stack
- **Engine**: Unity 6000.3.2f1
- **DI**: VContainer
- **Reactive**: UniRX
- **UI**: uGUI (Canvas)
- **Architecture**: MVVM + Clean Architecture

## Quick Reference

### Directory Structure
```
Assets/TicTacToe/
├── Scenes/
│   └── TicTacToeScene.unity
├── Prefabs/UI/
│   ├── BoardPanel.prefab
│   ├── CellButton.prefab
│   ├── ResultPanel.prefab
│   └── TurnIndicator.prefab
├── Scripts/
│   ├── Core/                    # Domain Layer
│   │   ├── Domain/              # Entities, Value Objects
│   │   ├── Repositories/        # Data Containers
│   │   ├── Services/            # Facades (Transaction Boundary)
│   │   └── Strategies/          # AI Algorithms
│   ├── Presentation/            # MVVM Layer
│   │   ├── Board/               # BoardView, BoardViewModel
│   │   ├── Cell/                # CellView, CellViewModel
│   │   ├── Result/              # ResultView, ResultViewModel
│   │   ├── TurnIndicator/       # TurnIndicatorView, TurnIndicatorViewModel
│   │   └── Mediators/           # GameMediator
│   ├── Infrastructure/          # DI, Factories
│   │   ├── Installers/
│   │   └── Factories/
│   └── EntryPoints/
├── Tests/
│   ├── EditMode/
│   └── PlayMode/
├── Audio/SE/
├── Art/Sprites/
└── Animations/
```

### Namespace Convention
```csharp
namespace TicTacToe.Core.Domain { }
namespace TicTacToe.Core.Repositories { }
namespace TicTacToe.Core.Services { }
namespace TicTacToe.Core.Strategies { }
namespace TicTacToe.Presentation.Board { }
namespace TicTacToe.Presentation.Cell { }
namespace TicTacToe.Presentation.Mediators { }
namespace TicTacToe.Infrastructure.Installers { }
```

### Key Patterns

#### Domain Models
```csharp
// CellState.cs
public enum CellState { Empty, X, O }

// PlayerType.cs
public enum PlayerType { Human, AI }

// BoardPosition.cs
public readonly struct BoardPosition
{
    public int Row { get; }
    public int Column { get; }
}
```

#### Service Pattern
```csharp
// GameService.cs - Facade / Transaction Boundary
public class GameService
{
    private readonly BoardRepository _boardRepository;
    private readonly IAIStrategy _aiStrategy;
    
    public IObservable<GameResult> OnGameEnd { get; }
    public IObservable<PlayerType> OnTurnChanged { get; }
    
    public void PlaceMark(BoardPosition position) { }
    public void StartNewGame() { }
}
```

#### MVVM Pattern
```csharp
// ViewModel
public class CellViewModel : IDisposable
{
    public IReadOnlyReactiveProperty<CellState> State { get; }
    public IObservable<Unit> OnClicked { get; }
}

// View
public class CellView : MonoBehaviour
{
    [Inject] private CellViewModel _viewModel;
    [SerializeField] private Button _button;
    [SerializeField] private Image _markImage;
}
```

#### AI Strategy
```csharp
public interface IAIStrategy
{
    BoardPosition DecideMove(IReadOnlyList<CellState> board, CellState aiMark);
}

public class RandomAIStrategy : IAIStrategy { }
// Future: MinimaxAIStrategy, etc.
```

## Reference Files

- **[architecture.md](references/architecture.md)** - 設計方針・アーキテクチャ決定
- **[domain_model.md](references/domain_model.md)** - ドメインモデル定義
- **[implementation_guide.md](references/implementation_guide.md)** - 実装ガイド・手順

## Integration with Other Skills

- **claude_skill_unity**: Unity汎用パターン、MonoBehaviourライフサイクル
- **Serena Memory**: 実装進捗、セッション引き継ぎ

## Notes

- VContainer/UniRXは要パッケージ追加
- UIは全てPrefab化（Sceneに直接配置しない）
- テストはCore層を中心に作成
