---
name: pr-create
description: "GitHub Pull Request creation with intelligent title generation and issue linking"
category: workflow
complexity: standard
mcp-servers: [github, sequential]
personas: [go-expert, react-expert, devops-architect]
---

# /pr-create - Pull Request Creation

> **Context Framework Note**: This behavioral instruction activates when users type `/pr-create` patterns. It automates the process of pushing current branch to GitHub and creating a Pull Request with intelligent content generation.

## Triggers

- Pull Request creation needs with automated push and GitHub integration
- Issue linking requirements for PR workflow automation
- Japanese-language PR content generation for team collaboration
- Branch-based development workflow optimization

## Context Trigger Pattern

```
/pr-create [title] [@issue-url] [--base branch] [--draft] [--force]
```

**Usage**: Type this in conversation to create a Pull Request from current branch with intelligent content generation and GitHub integration.

## Behavioral Flow

1. **Branch Validation**: Check current branch is not main/master
2. **Repository Analysis**: Extract owner/repo from git remote configuration
3. **Content Generation**: Generate PR title and description based on commits and changes
4. **Issue Integration**: Parse issue URLs and extract linking information
5. **GitHub Push**: Push current branch to remote repository
6. **PR Creation**: Use GitHub MCP to create Pull Request with generated content
7. **Success Notification**: Provide PR URL and next steps guidance

Key behaviors:

- **Japanese Content Generation**: All PR titles and descriptions in Japanese for team collaboration
- **Intelligent Title Generation**: From arguments, issue titles, or commit analysis
- **Template Integration**: Use existing `.github/pull_request_template.md` structure
- **Issue Linking**: Automatic `close #xxx` generation from issue URLs
- **Safety Validation**: Prevent execution on main branch and validate repository state

## MCP Integration

- **GitHub MCP**: Primary integration for PR creation and issue information
  - `get_issue`: Extract issue information for title and linking
  - `create_pull_request`: Create PR with generated content
  - `get_me`: Validate GitHub authentication and permissions
- **Sequential MCP**: Complex diff analysis and content generation when needed
  - Multi-file change analysis for comprehensive PR descriptions
  - Structured reasoning for complex change summarization

## Tool Coordination

- **Run**: Git operations for branch validation, push, and repository analysis
- **Read**: Template file reading and commit message analysis
- **Grep**: Change detection and file pattern analysis for content generation
- **TodoWrite**: Progress tracking for multi-step PR creation workflow

## Command Arguments

### Basic Usage

```bash
# Auto-generate title from commits/changes
/pr-create

# Explicit title
/pr-create "ユーザー認証機能の追加"

# Issue-based title generation
/pr-create @https://github.com/portalkey/portalkey-server/issues/123

# Combined usage
/pr-create "ログインバグの修正" @https://github.com/portalkey/portalkey-server/issues/456

# Custom base branch
/pr-create "本番環境向けホットフィックス" --base main

# Draft PR
/pr-create "WIP: 新機能の実装" --draft

# Force push (use with caution)
/pr-create "緊急修正" --force
```

### Issue URL Formats

```bash
# Full GitHub URL
@https://github.com/portalkey/portalkey-server/issues/123

# Short format (same repository)
@#123

# Multiple issues
@#123,456,789
```

## Content Generation Logic

### Title Generation Priority

1. **Explicit Argument**: Use provided title as-is
2. **Issue Title**: Extract and adapt from linked issue
3. **Commit Analysis**: Generate from recent commit messages
4. **Diff Analysis**: Fallback to file change analysis

### Description Generation

```markdown
<!-- close #123 -->

## 概要

{コミットメッセージまたは変更内容から生成した概要}

## やったこと

{変更ファイルの分析結果}

- フロントエンド: {frontend changes}
- バックエンド: {backend changes}
- 設定ファイル: {config changes}

## 見てほしいところ

{重要な変更点の自動検出結果、または「特になし」}

## 画面のスクリーンショット

{フロントエンド変更がある場合は言及、なければ「特になし」}

## 備考

{追加情報があれば記載、なければ「特になし」}
```

### Change Analysis Patterns

```yaml
file_patterns:
  frontend: ["frontend/**/*.tsx", "frontend/**/*.ts", "frontend/**/*.css"]
  backend: ["pkg/**/*.go", "cmd/**/*.go", "proto/**/*.proto"]
  config: ["*.yml", "*.yaml", "*.json", "Makefile", "package.json"]
  docs: ["*.md", "docs/**/*", "README*"]
  tests: ["**/*_test.go", "frontend/**/*.test.*", "**/*.spec.*"]

change_descriptions:
  frontend: "フロントエンド機能の{action}"
  backend: "バックエンドAPIの{action}"
  config: "設定ファイルの{action}"
  docs: "ドキュメントの{action}"
  tests: "テストコードの{action}"

action_mapping:
  added: "追加"
  modified: "修正"
  deleted: "削除"
  renamed: "リネーム"
```

## Error Handling

### GitHub MCP Unavailable

```bash
❌ GitHub MCPサーバーが利用できません。
💡 手動でPRを作成するか、MCP設定を確認してください。

# Fallback: Provide manual instructions
git push origin $(git rev-parse --abbrev-ref HEAD)
# Then create PR manually at: https://github.com/portalkey/portalkey-server/compare
```

### Main Branch Protection

```bash
❌ mainブランチでは実行できません。
💡 feature branchを作成してから実行してください。

git checkout -b feature/your-feature-name
```

### Push Failures

```bash
❌ git pushに失敗しました。
💡 リモートブランチの状態を確認してください。

# Common solutions:
git pull origin $(git rev-parse --abbrev-ref HEAD)  # Pull latest changes
git push --force-with-lease origin $(git rev-parse --abbrev-ref HEAD)  # Force push safely
```

### API Rate Limits

```bash
❌ GitHub API制限に達しました。
💡 しばらく待ってから再実行してください。
⏰ 制限リセット時刻: {reset_time}
```

## Examples

### Basic PR Creation

```bash
/pr-create
# Auto-detects: "refactor: simplify --framework option to go|react only"
# Generates Japanese title: "フレームワークオプションのリファクタリング"
# Creates comprehensive description from commit analysis
```

### Issue-Linked PR

```bash
/pr-create @https://github.com/portalkey/portalkey-server/issues/456
# Fetches issue: "ログイン時のエラーハンドリング改善"
# Generates title: "ログイン時のエラーハンドリング改善"
# Adds "close #456" to description
```

### Complex Feature PR

```bash
/pr-create "リアルタイム通信機能の実装" @#123,456
# Uses explicit title
# Links multiple issues: "close #123, #456"
# Analyzes frontend + backend changes for comprehensive description
```

### Draft PR for WIP

```bash
/pr-create "WIP: 新しいダッシュボード機能" --draft
# Creates draft PR for work-in-progress
# Allows early feedback without triggering CI/CD
```

## Integration Patterns

### With Existing Workflow

```bash
# Typical development flow
git checkout -b feature/new-dashboard
# ... make changes ...
git add .
git commit -m "feat: add dashboard components"
/pr-create "ダッシュボード機能の追加"  # Automated PR creation
```

### With Issue Workflow

```bash
# Issue-driven development
/pr-create @https://github.com/portalkey/portalkey-server/issues/789
# Automatically links issue and generates appropriate title/description
```

### Emergency Hotfix

```bash
git checkout -b hotfix/critical-bug
# ... fix critical issue ...
/pr-create "緊急: 本番環境のクリティカルバグ修正" --base main --force
```

## Boundaries

**Will:**

- Create Pull Requests with intelligent Japanese content generation
- Integrate with GitHub issues for automated linking and title generation
- Provide comprehensive error handling and fallback instructions
- Analyze code changes for meaningful PR descriptions

**Will Not:**

- Execute on main/master branches without explicit override
- Create PRs when GitHub MCP is unavailable (provides manual instructions)
- Override repository security settings or bypass branch protection rules
- Generate inappropriate or inaccurate technical descriptions

**Safety Features:**

- Branch validation prevents accidental main branch execution
- Force push requires explicit `--force` flag
- GitHub MCP availability check before execution
- Comprehensive error messages with recovery instructions

--- End Command ---
