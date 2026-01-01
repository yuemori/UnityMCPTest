# Implementation Guide

## Prerequisites

### Package Installation
VContainerとUniRXをPackage Managerで追加：

```
Window > Package Manager > + > Add package from git URL

VContainer:
https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer

UniRX:
https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts
```

## Implementation Phases

### Phase 0: Environment Setup
**目標**: 開発環境の準備

1. VContainer, UniRXパッケージ追加
2. ディレクトリ構造作成
3. Assembly Definition Files (.asmdef) 作成
4. TicTacToeScene作成

**成果物**:
- `Assets/TicTacToe/` ディレクトリ構造
- `TicTacToe.Core.asmdef`
- `TicTacToe.Presentation.asmdef`
- `TicTacToe.Infrastructure.asmdef`

### Phase 1: Core/Domain
**目標**: ドメインモデルの定義

1. `CellState.cs` - マス状態enum
2. `PlayerType.cs` - プレイヤー種別enum
3. `BoardPosition.cs` - 座標値オブジェクト
4. `GameResult.cs` - ゲーム結果値オブジェクト
5. `TurnInfo.cs` - ターン情報

**テスト**: BoardPosition, GameResult の単体テスト

### Phase 2: Core/Repository & Service
**目標**: ゲームロジックの基盤

1. `BoardRepository.cs` - 盤面状態管理
   - `IReadOnlyList<CellState> Cells`
   - `void PlaceMark(BoardPosition, CellState)`
   - `void Reset()`

2. `GameService.cs` - ゲーム進行Facade
   - `IObservable<TurnInfo> OnTurnChanged`
   - `IObservable<GameResult> OnGameEnd`
   - `void StartNewGame()`
   - `void PlaceMark(BoardPosition)`

3. 勝敗判定ロジック

**テスト**: BoardRepository, GameService の単体テスト

### Phase 3: AI Implementation
**目標**: AI対戦機能

1. `IAIStrategy.cs` - AIインターフェース
2. `RandomAIStrategy.cs` - ランダムAI実装
3. `AIService.cs` - AI思考サービス

**テスト**: RandomAIStrategy の単体テスト

### Phase 4: Presentation Base
**目標**: MVVM基盤

1. ViewModel基底パターン確立
2. View基底パターン確立
3. VContainer LifetimeScope設計

### Phase 5: Board & Cell UI
**目標**: 盤面UI実装

1. `CellViewModel.cs` / `CellView.cs`
2. `BoardViewModel.cs` / `BoardView.cs`
3. `CellButton.prefab` / `BoardPanel.prefab`

### Phase 6: Game Flow UI
**目標**: ゲームフロー完成

1. `TurnIndicatorViewModel.cs` / `TurnIndicatorView.cs`
2. `ResultViewModel.cs` / `ResultView.cs`
3. `GameMediator.cs` - 画面協調

### Phase 7: VContainer Integration
**目標**: DI統合

1. `GameLifetimeScope.cs`
2. `CellViewFactory.cs`
3. 全コンポーネントのDI接続

### Phase 8: Audio & Animation
**目標**: 演出追加

1. `AudioService.cs`
2. 効果音アセット配置
3. マーク配置アニメーション
4. 勝利演出

### Phase 9: Testing & Polish
**目標**: 品質仕上げ

1. 統合テスト作成
2. エッジケース対応
3. UI調整

## Code Templates

### ViewModel Template
```csharp
using System;
using UniRx;
using VContainer;

namespace TicTacToe.Presentation.{Feature}
{
    public class {Feature}ViewModel : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        // Reactive Properties
        public IReadOnlyReactiveProperty<T> SomeState => _someState;
        private readonly ReactiveProperty<T> _someState = new();

        // Commands (as Observable)
        public IObservable<Unit> OnSomeAction => _onSomeAction;
        private readonly Subject<Unit> _onSomeAction = new();

        [Inject]
        public {Feature}ViewModel(/* dependencies */)
        {
            // Setup subscriptions
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _someState.Dispose();
            _onSomeAction.Dispose();
        }
    }
}
```

### View Template
```csharp
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using VContainer;

namespace TicTacToe.Presentation.{Feature}
{
    public class {Feature}View : MonoBehaviour
    {
        [SerializeField] private Button _someButton;
        [SerializeField] private Text _someText;

        private {Feature}ViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Construct({Feature}ViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            // Bind ViewModel to UI
            _viewModel.SomeState
                .Subscribe(value => _someText.text = value.ToString())
                .AddTo(_disposables);

            // Bind UI events to ViewModel
            _someButton.OnClickAsObservable()
                .Subscribe(_ => /* notify viewModel */)
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
```

### Service Template
```csharp
using System;
using UniRx;
using VContainer;

namespace TicTacToe.Core.Services
{
    public class {Name}Service
    {
        // Events
        public IObservable<T> OnSomethingHappened => _onSomethingHappened;
        private readonly Subject<T> _onSomethingHappened = new();

        // Dependencies
        private readonly SomeRepository _repository;

        [Inject]
        public {Name}Service(SomeRepository repository)
        {
            _repository = repository;
        }

        // Public API
        public void DoSomething()
        {
            // Business logic
            _onSomethingHappened.OnNext(result);
        }
    }
}
```

## Testing Guidelines

### Unit Test Location
```
Assets/TicTacToe/Tests/EditMode/
├── Core/
│   ├── BoardRepositoryTests.cs
│   ├── GameServiceTests.cs
│   └── RandomAIStrategyTests.cs
└── TicTacToe.Tests.EditMode.asmdef
```

### Test Template
```csharp
using NUnit.Framework;
using TicTacToe.Core.Domain;
using TicTacToe.Core.Repositories;

namespace TicTacToe.Tests.EditMode.Core
{
    public class BoardRepositoryTests
    {
        private BoardRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new BoardRepository();
        }

        [Test]
        public void PlaceMark_ShouldUpdateCellState()
        {
            // Arrange
            var position = new BoardPosition(0, 0);

            // Act
            _repository.PlaceMark(position, CellState.X);

            // Assert
            Assert.AreEqual(CellState.X, _repository.Cells[position.Index]);
        }
    }
}
```

## Common Pitfalls

### 避けるべきパターン
1. **View直接参照**: ViewModelを経由せずServiceを呼ばない
2. **循環参照**: レイヤー間の依存方向を厳守
3. **MonoBehaviour乱用**: ロジックはPOCOクラスに
4. **Dispose忘れ**: UniRXのSubscriptionは必ずDispose

### 推奨パターン
1. **Constructor Injection**: フィールドインジェクションより優先
2. **IReadOnlyReactiveProperty**: 外部への公開は読み取り専用
3. **CompositeDisposable**: 複数Subscriptionの一括管理
