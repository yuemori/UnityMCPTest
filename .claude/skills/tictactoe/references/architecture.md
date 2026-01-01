# Architecture Decision Record

## Overview

TicTacToeプロジェクトのアーキテクチャ決定事項。

## Architecture Pattern

### MVVM + Clean Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                      EntryPoints                                │
│                    (GameEntryPoint)                             │
└────────────────────────────┬────────────────────────────────────┘
                             │ 依存
┌────────────────────────────▼────────────────────────────────────┐
│                      Infrastructure                             │
│              (Installers, Factories, VContainer)                │
└────────────────────────────┬────────────────────────────────────┘
                             │ DI登録
          ┌──────────────────┴──────────────────┐
          ▼                                     ▼
┌─────────────────────┐               ┌─────────────────────────┐
│    Presentation     │               │          Core           │
│ ┌─────────────────┐ │               │ ┌─────────────────────┐ │
│ │      View       │ │               │ │      Services       │ │
│ │ (MonoBehaviour) │◄├───────────────│ │     (Facade)        │ │
│ │   UI Binding    │ │    Subscribe  │ └──────────┬──────────┘ │
│ └────────┬────────┘ │               │            │            │
│          │ Bind     │               │ ┌──────────▼──────────┐ │
│ ┌────────▼────────┐ │               │ │    Repositories     │ │
│ │    ViewModel    │ │               │ │  (Data Container)   │ │
│ │ (Reactive       │◄├───────────────│ └──────────┬──────────┘ │
│ │  Property)      │ │    Inject     │            │            │
│ └─────────────────┘ │               │ ┌──────────▼──────────┐ │
│                     │               │ │       Domain        │ │
│ ┌─────────────────┐ │               │ │  (Entity, Enum)     │ │
│ │    Mediator     │ │               │ └─────────────────────┘ │
│ │ (複数VM協調)    │ │               │                         │
│ └─────────────────┘ │               │ ┌─────────────────────┐ │
│                     │               │ │     Strategies      │ │
│                     │               │ │   (AI Algorithms)   │ │
│                     │               │ └─────────────────────┘ │
└─────────────────────┘               └─────────────────────────┘
```

## Design Decisions

### DD-001: DI Container - VContainer

**決定**: VContainerを使用
**理由**:
- Unity特化で高パフォーマンス
- MonoBehaviour/純粋C#クラス両対応
- LifetimeScopeによるスコープ管理
- Source Generator対応で高速

### DD-002: Reactive Extensions - UniRX

**決定**: UniRXを使用
**理由**:
- ReactivePropertyによる状態バインディング
- IObservableベースのイベント伝播
- Unity統合（MainThreadDispatcher等）
- MVVMパターンとの親和性

### DD-003: Managerクラスの排除

**決定**: "Manager"という命名を避け、責務別に分解
**理由**:
- "Manager"は責務が曖昧になりがち
- 単一責任原則の遵守

**置き換え**:
| 従来 | 新命名 | 配置 |
|------|--------|------|
| GameManager | GameService | Core/Services/ |
| UIManager | GameMediator | Presentation/Mediators/ |
| AudioManager | AudioService | Core/Services/ |

### DD-004: View/ViewModel Collocation

**決定**: 機能単位でView/ViewModelを同一ディレクトリに配置
**理由**:
- 関連ファイルが近接し変更時の見通しが良い
- 新機能追加時はディレクトリごと追加
- Prefabとの1:1対応が明確

**構造**:
```
Presentation/
├── Board/
│   ├── BoardView.cs
│   └── BoardViewModel.cs
├── Cell/
│   ├── CellView.cs
│   └── CellViewModel.cs
```

### DD-005: UseCase層の省略

**決定**: 今回はUseCase層を省略し、Serviceに統合
**理由**:
- TicTacToeは小規模でユースケースがシンプル
- Service内で十分表現可能
- 将来的な分離は容易

### DD-006: AI Strategy Pattern

**決定**: Strategy Patternで AI実装を抽象化
**理由**:
- 異なるAIアルゴリズムの差し替えが容易
- テスト時のモック化が容易
- 将来の拡張（Minimax等）に対応

**Interface**:
```csharp
public interface IAIStrategy
{
    BoardPosition DecideMove(IReadOnlyList<CellState> board, CellState aiMark);
}
```

### DD-007: UI as Prefabs

**決定**: UIコンポーネントは全てPrefab化
**理由**:
- Scene肥大化の防止
- 再利用性の向上
- バージョン管理の容易さ
- チーム開発でのコンフリクト低減

## Dependency Rules

### 許可される依存
- View → ViewModel（バインディング）
- ViewModel → Service（ロジック委譲）
- Service → Repository（データアクセス）
- Service → Strategy（アルゴリズム委譲）
- 全レイヤー → Domain（エンティティ参照）

### 禁止される依存
- Core → Presentation（逆依存）
- View → Service（ViewModelをバイパス）
- Repository → Service（逆依存）

## Testing Strategy

- **Unit Test**: Core層（Service, Repository, Strategy）
- **Integration Test**: ゲームフロー全体
- **配置**: Assets/TicTacToe/Tests/
