# TicTacToe Implementation Plan

## Overview
Unity 6でのTic Tac Toe AI対戦ゲーム実装計画。

## Phase Summary

| Phase | 内容 | 状態 | 依存 |
|-------|------|------|------|
| Phase 0 | 環境構築 | ⏳ Pending | - |
| Phase 1 | Core/Domain | ⏳ Pending | Phase 0 |
| Phase 2 | Core/Repository & Service | ⏳ Pending | Phase 1 |
| Phase 3 | AI Implementation | ⏳ Pending | Phase 2 |
| Phase 4 | Presentation Base | ⏳ Pending | Phase 2 |
| Phase 5 | Board & Cell UI | ⏳ Pending | Phase 4 |
| Phase 6 | Game Flow UI | ⏳ Pending | Phase 5 |
| Phase 7 | VContainer Integration | ⏳ Pending | Phase 6 |
| Phase 8 | Audio & Animation | ⏳ Pending | Phase 7 |
| Phase 9 | Testing & Polish | ⏳ Pending | Phase 8 |

## Phase Details

### Phase 0: Environment Setup
**目標**: 開発環境の準備
**成果物**:
- [ ] VContainer パッケージ追加
- [ ] UniRX パッケージ追加
- [ ] ディレクトリ構造作成
  - [ ] Assets/TicTacToe/Scenes/
  - [ ] Assets/TicTacToe/Prefabs/UI/
  - [ ] Assets/TicTacToe/Scripts/Core/Domain/
  - [ ] Assets/TicTacToe/Scripts/Core/Repositories/
  - [ ] Assets/TicTacToe/Scripts/Core/Services/
  - [ ] Assets/TicTacToe/Scripts/Core/Strategies/
  - [ ] Assets/TicTacToe/Scripts/Presentation/
  - [ ] Assets/TicTacToe/Scripts/Infrastructure/
  - [ ] Assets/TicTacToe/Tests/
- [ ] Assembly Definition Files作成
  - [ ] TicTacToe.Core.asmdef
  - [ ] TicTacToe.Presentation.asmdef
  - [ ] TicTacToe.Infrastructure.asmdef
  - [ ] TicTacToe.Tests.EditMode.asmdef
  - [ ] TicTacToe.Tests.PlayMode.asmdef
- [ ] TicTacToeScene.unity 作成

### Phase 1: Core/Domain
**目標**: ドメインモデルの定義
**成果物**:
- [ ] CellState.cs
- [ ] PlayerType.cs
- [ ] BoardPosition.cs
- [ ] GameResult.cs
- [ ] TurnInfo.cs
- [ ] BoardPosition, GameResult 単体テスト

### Phase 2: Core/Repository & Service
**目標**: ゲームロジックの基盤
**成果物**:
- [ ] BoardRepository.cs
- [ ] GameService.cs（勝敗判定含む）
- [ ] 単体テスト

### Phase 3: AI Implementation
**目標**: AI対戦機能
**成果物**:
- [ ] IAIStrategy.cs
- [ ] RandomAIStrategy.cs
- [ ] AIService.cs
- [ ] 単体テスト

### Phase 4: Presentation Base
**目標**: MVVM基盤
**成果物**:
- [ ] ViewModel/View基底パターン確立
- [ ] VContainer LifetimeScope設計

### Phase 5: Board & Cell UI
**目標**: 盤面UI実装
**成果物**:
- [ ] CellViewModel.cs / CellView.cs
- [ ] BoardViewModel.cs / BoardView.cs
- [ ] CellButton.prefab
- [ ] BoardPanel.prefab

### Phase 6: Game Flow UI
**目標**: ゲームフロー完成
**成果物**:
- [ ] TurnIndicatorViewModel.cs / TurnIndicatorView.cs
- [ ] ResultViewModel.cs / ResultView.cs
- [ ] GameMediator.cs
- [ ] TurnIndicator.prefab
- [ ] ResultPanel.prefab

### Phase 7: VContainer Integration
**目標**: DI統合
**成果物**:
- [ ] GameLifetimeScope.cs
- [ ] CellViewFactory.cs
- [ ] 全コンポーネントDI接続

### Phase 8: Audio & Animation
**目標**: 演出追加
**成果物**:
- [ ] AudioService.cs
- [ ] 効果音アセット (place_mark, win, draw)
- [ ] マーク配置アニメーション
- [ ] 勝利演出

### Phase 9: Testing & Polish
**目標**: 品質仕上げ
**成果物**:
- [ ] 統合テスト
- [ ] エッジケース対応
- [ ] UI調整
- [ ] 最終動作確認

## Notes
- 各Phaseは前のPhaseの完了後に開始
- テストは各Phase内で作成
- UIは全てPrefab化
