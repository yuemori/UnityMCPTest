# Decisions Log

## 2026-01-01: Initial Architecture Decisions

### DEC-001: Architecture Pattern
**決定**: MVVM + Clean Architecture
**理由**: 
- UIとロジックの分離
- テスタビリティの向上
- VContainer/UniRXとの親和性

### DEC-002: DI Container
**決定**: VContainer
**理由**: 
- Unity特化で高パフォーマンス
- MonoBehaviour/純粋C#クラス両対応

### DEC-003: Reactive Extensions
**決定**: UniRX
**理由**: 
- ReactivePropertyによる状態バインディング
- MVVMパターンとの親和性

### DEC-004: Manager命名の排除
**決定**: "Manager"を使わず責務別に命名
**置き換え**:
- GameManager → GameService
- UIManager → GameMediator

### DEC-005: View/ViewModel配置
**決定**: 機能単位でコロケーション
**例**: `Presentation/Board/{BoardView.cs, BoardViewModel.cs}`

### DEC-006: Coordinator → Mediator
**決定**: CoordinatorではなくMediatorを使用
**理由**: MVP PatternのPresenterとの混同回避

### DEC-007: UseCase層省略
**決定**: UseCase層を作らずServiceに統合
**理由**: 小規模プロジェクトでは過剰設計

### DEC-008: AI Strategy
**決定**: IAIStrategy + RandomAIStrategy
**理由**: 将来の拡張性（Minimax等）を確保

### DEC-009: UI Framework
**決定**: uGUI (Canvas)
**理由**: 
- 実績のある安定したフレームワーク
- 豊富なドキュメント

### DEC-010: 知識管理体制
**決定**: AI Skills（静的知識）+ Serena Memory（動的状態）
**理由**:
- Skills: 自動ロード、再現性高
- Memory: セッション間引き継ぎ
