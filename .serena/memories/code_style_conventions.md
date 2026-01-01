# Code Style & Conventions

## Naming Conventions
- **Namespace**: ディレクトリ構造に対応
  - `TicTacToe.Core.Domain`
  - `TicTacToe.Core.Services`
  - `TicTacToe.Presentation.Board`
- **Class**: PascalCase (`GameService`, `BoardViewModel`)
- **Interface**: I prefix (`IAIStrategy`, `IDisposable`)
- **Public Members**: PascalCase (`CurrentPlayer`, `PlaceMark()`)
- **Private Fields**: _camelCase (`_boardRepository`, `_gameState`)
- **Parameters/Locals**: camelCase (`position`, `cellState`)
- **Constants**: PascalCase (`MaxPlayers`, `BoardSize`)

## File Organization
- 1ファイル1クラス（例外: 小さな値オブジェクト、enum）
- ファイル名 = クラス名
- View/ViewModelはコロケーション（同一ディレクトリ）

## Architecture Layers
```
Presentation (View, ViewModel, Mediator)
    ↓ 依存
Infrastructure (Installers, Factories)
    ↓ DI登録
Core (Services, Repositories, Domain)
```

## MVVM Pattern
- **View**: MonoBehaviour、UIバインディングのみ
- **ViewModel**: ReactiveProperty、ロジック委譲
- **Model**: Core層のService/Repository

## Unity Specific
- `[SerializeField]` for Inspector-exposed private fields
- `RequireComponent` attribute when component dependencies exist
- Avoid `Find*` methods in Update loops (cache references)
- Use `async/await` with UniTask or Coroutines for async operations
