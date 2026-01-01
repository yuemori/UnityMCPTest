# Task Completion Checklist

## Before Marking Task Complete

### Code Quality
- [ ] コードがコンパイルエラーなく通る
- [ ] 命名規則に従っている
- [ ] 適切なnamespaceが設定されている
- [ ] 不要なusing文が削除されている
- [ ] publicメンバーにXMLドキュメントコメントがある（重要なもの）

### Architecture
- [ ] レイヤー依存関係が正しい（Presentation → Core、逆はNG）
- [ ] DIコンテナに適切に登録されている
- [ ] インターフェースを通じた疎結合になっている

### Unity Specific
- [ ] .metaファイルが生成されている（Unity側で自動）
- [ ] SerializeFieldが適切に設定されている
- [ ] Prefabへの参照が切れていない

### Testing
- [ ] Core層の重要なロジックにUnit Testがある
- [ ] テストが全てパスする

### Memory Update
- [ ] `current_status.md` を更新
- [ ] 重要な決定があれば `decisions_log.md` に追記

## Session End
- [ ] `session_handoff.md` を更新
- [ ] 未完了タスクを明記
- [ ] 次のアクションを記載
