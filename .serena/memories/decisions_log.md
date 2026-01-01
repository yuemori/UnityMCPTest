# Decisions Log

## 2026-01-01: R3 NuGet統合方式の決定

### 背景
R3.Unity（UPM Git パッケージ）は R3 コアライブラリ（NuGet）に依存している。
UPMパッケージとしてR3.Unityをインストールすると、R3 NuGetが見つからずコンパイルエラーになる。

### 決定
NuGetForUnity を使用して R3 NuGet パッケージと依存関係をインストールする。

### 理由
- R3.Unity は Unity 固有の機能（UnityFrameProvider, SerializableReactiveProperty等）を提供
- R3 コアは NuGet パッケージとして配布されている
- NuGetForUnity は Unity で NuGet パッケージを管理する標準的な方法

### 実装
1. manifest.json に NuGetForUnity 追加
2. Assets/packages.config に R3 と依存パッケージを定義
3. NuGet/Restore Packages で依存関係をインストール
4. DLL の .meta ファイルで Editor プラットフォームを有効化

### 必要な NuGet パッケージ
- R3 1.2.9
- System.Threading.Channels 8.0.0
- Microsoft.Bcl.TimeProvider 8.0.1
- Microsoft.Bcl.AsyncInterfaces 8.0.0

---

## 2026-01-01: アーキテクチャパターンの決定

### 決定内容
- **MVVM + Clean Architecture** 採用
- **Coordinator → Mediator** に命名変更（MVPパターンとの混同回避）
- **View/ViewModel コロケーション**：機能単位で同一ディレクトリに配置
- **UseCase層省略**：Service内に統合（シンプルなゲームロジックのため）
- **AI実装**：IAIStrategy + RandomAIStrategy（Strategy Pattern）

### 理由
- Unity の MonoBehaviour との親和性
- テスタビリティの確保
- 将来の拡張性（AIアルゴリズム追加等）

---

## 2026-01-01: 技術スタック決定

### 採用技術
| 技術 | 用途 | バージョン |
|------|------|------------|
| Unity | ゲームエンジン | 6000.3.2f1 |
| VContainer | DI コンテナ | Latest (Git) |
| R3 | Reactive Extensions | 1.2.9 |
| R3.Unity | Unity 統合 | Latest (Git) |
| NuGetForUnity | NuGet 管理 | Latest (Git) |
| uGUI | UI フレームワーク | Built-in |

### 理由
- VContainer: 軽量で Unity 最適化された DI
- R3: UniRx の後継、より現代的な API
- uGUI: 標準 UI、学習コスト低い
