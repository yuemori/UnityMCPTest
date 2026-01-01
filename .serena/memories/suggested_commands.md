# Suggested Commands

## Unity Editor Operations (via Unity-MCP)
Unity MCPを通じてエディタ操作を行う。直接コマンドラインでは操作しない。

## Git Commands (PowerShell)
```powershell
# 状態確認
git status
git branch
git log --oneline -10

# ブランチ操作
git checkout -b feature/[feature-name]
git checkout main

# コミット
git add -A
git commit -m "message"

# 差分確認
git diff
git diff --staged
```

## File Operations (PowerShell)
```powershell
# ディレクトリ作成
New-Item -ItemType Directory -Path "Assets/TicTacToe/Scripts"

# ファイル一覧
Get-ChildItem -Path "Assets" -Recurse

# ファイル検索
Get-ChildItem -Path "Assets" -Recurse -Filter "*.cs"
```

## Package Management
VContainerとUniRXはUnity Package Managerで追加：
1. Window > Package Manager
2. + > Add package from git URL
   - VContainer: `https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer`
   - UniRX: `https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts`

## Testing
```powershell
# Unity Test Runnerはエディタから実行
# Window > General > Test Runner
# または Unity-MCP経由で run_tests を使用
```

## Build
Unity EditorのBuild Settings (File > Build Settings) から実行
