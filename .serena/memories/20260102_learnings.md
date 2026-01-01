# Learnings: Unity UI Rounded Corners Implementation

## URP Shader for UI

### Key Points
1. **URP互換性**: `HLSLPROGRAM`, `CBUFFER_START`, `SAMPLE_TEXTURE2D` を使用
2. **Tags必須**: `"RenderPipeline"="UniversalPipeline"` をSubShaderに追加
3. **Transform**: `TransformObjectToHClip()` (CGの`UnityObjectToClipPos`の代わり)

### SDF Approach
```hlsl
float roundedBoxSDF(float2 pixelPos, float2 halfSize, float radius) {
    float2 q = abs(pixelPos) - halfSize + radius;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - radius;
}
```
- ピクセル空間での計算でアスペクト比問題を解決
- `_RectSize`プロパティでUI要素サイズを受け取る

### Material Instance Pattern
- `UnityEngine.UI.Image`は`Renderer`を持たない
- `image.material = new Material(originalMaterial)` でインスタンス化
- `[ExecuteAlways]`でエディタ時もサイズ同期

## Unity MCP Tips
- `assign_material_to_renderer`はUI Imageに使用不可
- `set_component_property`でImage.materialを直接設定
- コンポーネント追加前にスクリプトがコンパイル済みか確認
