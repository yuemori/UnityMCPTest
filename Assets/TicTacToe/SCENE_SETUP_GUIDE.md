# TicTacToe Scene Setup Guide

Unity Editor での手動セットアップ手順書です。

## Prerequisites

- Unity 6000.3.2f1 以上
- VContainer パッケージ導入済み
- R3 (UniRX) パッケージ導入済み
- TextMeshPro 導入済み

## Scene Hierarchy

```
TicTacToeScene
├── GameLifetimeScope (GameObject)
│   └── GameLifetimeScope.cs (Component)
├── Canvas (UI)
│   ├── BoardPanel
│   │   ├── BoardView.cs
│   │   └── Cell_0~8 (GridLayoutGroup children)
│   │       └── CellView.cs (each)
│   ├── TurnIndicatorPanel
│   │   ├── TurnIndicatorView.cs
│   │   └── TurnText (TextMeshProUGUI)
│   └── ResultPanel
│       ├── ResultView.cs
│       ├── ResultText (TextMeshProUGUI)
│       └── RestartButton (Button)
├── EventSystem
└── Camera (Main Camera)
```

## Step 1: GameLifetimeScope Setup

1. 空の GameObject 作成 → 名前: `GameLifetimeScope`
2. `GameLifetimeScope.cs` コンポーネント追加
3. VContainer が自動的に認識

## Step 2: Canvas Setup

1. GameObject → UI → Canvas 作成
2. Canvas 設定:
   - Render Mode: `Screen Space - Overlay`
   - UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1920 x 1080`
   - Match: `0.5`

## Step 3: BoardPanel Setup

### 3.1 BoardPanel 作成

1. Canvas 下に空の GameObject 作成 → 名前: `BoardPanel`
2. RectTransform 設定:
   - Anchor: Center
   - Size: 600 x 600
3. `BoardView.cs` コンポーネント追加

### 3.2 GridLayoutGroup 設定

1. BoardPanel に `Grid Layout Group` 追加
2. 設定:
   - Cell Size: 180 x 180
   - Spacing: 15 x 15
   - Start Corner: Upper Left
   - Start Axis: Horizontal
   - Child Alignment: Middle Center
   - Constraint: Fixed Column Count = 3

### 3.3 Cell 作成 (x9)

1. BoardPanel 下に Button 作成 → 名前: `Cell_0`
2. Button 設定:
   - Transition: Color Tint
   - Normal Color: #FFFFFF
   - Highlighted Color: #F0F0F0
   - Pressed Color: #C0C0C0
   - Disabled Color: #808080
3. TextMeshPro - Text (UI) 子オブジェクト作成
   - Font Size: 120
   - Alignment: Center/Middle
   - Font Style: Bold
4. `CellView.cs` コンポーネント追加
5. Inspector で参照設定:
   - Button → 自身の Button
   - Mark Text → 子の TextMeshProUGUI
6. Cell_0 を複製して Cell_1 ~ Cell_8 作成

### 3.4 BoardView 参照設定

1. BoardView の Inspector で:
   - Cell Views 配列サイズ: 9
   - Element 0~8: Cell_0 ~ Cell_8 の CellView 参照

## Step 4: TurnIndicatorPanel Setup

1. Canvas 下に空の GameObject 作成 → 名前: `TurnIndicatorPanel`
2. RectTransform 設定:
   - Anchor: Top Center
   - Position Y: -100
   - Size: 400 x 80
3. CanvasGroup コンポーネント追加
4. `TurnIndicatorView.cs` コンポーネント追加
5. TextMeshPro - Text (UI) 子オブジェクト作成:
   - 名前: `TurnText`
   - Font Size: 48
   - Alignment: Center/Middle
6. Inspector で参照設定:
   - Turn Text → TurnText
   - Canvas Group → 自身の CanvasGroup

## Step 5: ResultPanel Setup

1. Canvas 下に空の GameObject 作成 → 名前: `ResultPanel`
2. RectTransform 設定:
   - Anchor: Stretch
   - Left/Right/Top/Bottom: 0
3. Image コンポーネント追加:
   - Color: #000000 (Alpha: 0.7)
4. CanvasGroup コンポーネント追加
5. `ResultView.cs` コンポーネント追加

### 5.1 ResultText 作成

1. ResultPanel 下に TextMeshPro - Text (UI) 作成
   - 名前: `ResultText`
   - Anchor: Center
   - Position Y: 50
   - Font Size: 72
   - Alignment: Center/Middle

### 5.2 RestartButton 作成

1. ResultPanel 下に Button 作成
   - 名前: `RestartButton`
   - Anchor: Center
   - Position Y: -80
   - Size: 200 x 60
2. Button 子の TextMeshPro 設定:
   - Text: "Restart"
   - Font Size: 36

### 5.3 ResultView 参照設定

1. Inspector で:
   - Result Text → ResultText
   - Restart Button → RestartButton
   - Canvas Group → 自身の CanvasGroup

## Step 6: VContainer Injection

View コンポーネントは `[Inject]` 属性で ViewModel を受け取ります。
GameLifetimeScope がシーン読み込み時に自動的に Inject を行います。

```csharp
// 各Viewの例
public class BoardView : ViewBase<BoardViewModel>
{
    [Inject]
    public void Construct(BoardViewModel viewModel)
    {
        BindViewModel(viewModel);
    }
}
```

## Step 7: Testing

1. Play Mode で動作確認
2. セルクリック → マーク配置
3. AI 自動応答
4. 勝利/引き分け判定
5. リスタートボタン

## Troubleshooting

### "NullReferenceException" on Start

- GameLifetimeScope がシーンに存在するか確認
- View コンポーネントの参照が設定されているか確認

### ボタンが反応しない

- EventSystem がシーンに存在するか確認
- Button の Interactable が true か確認
- Raycast Target が有効か確認

### AI が動かない

- GameEntryPoint が正しく登録されているか確認
- AIService のコンストラクタが呼ばれているか確認

## Color Scheme (推奨)

| Element     | Color              |
| ----------- | ------------------ |
| X Mark      | #3498DB (青)       |
| O Mark      | #E74C3C (赤)       |
| AI Thinking | #95A5A6 (グレー)   |
| Board BG    | #ECF0F1            |
| Cell BG     | #FFFFFF            |
| Win Text    | #27AE60 (緑)       |
| Draw Text   | #F39C12 (オレンジ) |
