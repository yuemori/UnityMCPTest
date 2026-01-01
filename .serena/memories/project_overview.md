# UnityTest Project Overview

## Purpose
Unity 6でのサンプルゲーム開発プロジェクト。AI Agentによる実装テストを目的とする。
現在のターゲット: **Tic Tac Toe（三目並べ）** AI対戦ゲーム

## Tech Stack
- **Engine**: Unity 6000.3.2f1 (Unity 6)
- **Render Pipeline**: Universal Render Pipeline (URP) 17.3.0
- **Language**: C# (.NET)
- **UI Framework**: uGUI (com.unity.ugui 2.0.0)
- **Input**: Unity Input System 1.17.0
- **DI Container**: VContainer（要追加）
- **Reactive**: UniRX（要追加）

## MCP Integration
- **Unity-MCP**: com.coplaydev.unity-mcp（導入済み）
- **Cursor IDE**: com.boxqkrtm.ide.cursor（導入済み）
- **Serena MCP**: プロジェクト管理・メモリ永続化

## AI Agent Skills
- `.claude/skills/claude_skill_unity/` - Unity汎用スキル（3,367ページ）
- `.claude/skills/tictactoe/` - TicTacToe固有スキル（作成予定）

## Architecture
- **Pattern**: MVVM + Clean Architecture
- **DI**: VContainer
- **Event**: UniRX (ReactiveProperty)
- **Namespace**: ディレクトリ構造に対応（TicTacToe.Core.*, TicTacToe.Presentation.*）
