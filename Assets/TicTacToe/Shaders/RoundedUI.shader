Shader "UI/RoundedCorners"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // ピクセル単位で指定（実際のUIサイズに依存）
        _CornerRadius ("Corner Radius (pixels)", Float) = 20
        _BorderWidth ("Border Width (pixels)", Float) = 3
        _BorderColor ("Border Color", Color) = (1,1,1,1)
        
        // UIのサイズ（スクリプトから設定）
        _RectSize ("Rect Size", Vector) = (100, 100, 0, 0)
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                half4 color     : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _TextureSampleAdd;
                float4 _ClipRect;
                float4 _MainTex_ST;
                float _CornerRadius;
                float _BorderWidth;
                half4 _BorderColor;
                float4 _RectSize;
            CBUFFER_END

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = TransformObjectToHClip(v.vertex.xyz);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            // Signed distance function for rounded box (in pixel space)
            float roundedBoxSDF(float2 pixelPos, float2 halfSize, float radius)
            {
                float2 q = abs(pixelPos) - halfSize + radius;
                return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - radius;
            }

            half4 frag(v2f IN) : SV_Target
            {
                // Get rect size
                float2 rectSize = _RectSize.xy;
                
                // Prevent division by zero
                rectSize = max(rectSize, float2(1.0, 1.0));
                
                // Convert UV (0-1) to pixel coordinates centered at origin
                float2 pixelPos = (IN.texcoord - 0.5) * rectSize;
                
                // Half size in pixels
                float2 halfSize = rectSize * 0.5;
                
                // Clamp corner radius to not exceed half of smallest dimension
                float maxRadius = min(halfSize.x, halfSize.y);
                float radius = min(_CornerRadius, maxRadius);
                
                // Calculate SDF in pixel space
                float dist = roundedBoxSDF(pixelPos, halfSize, radius);
                
                // Anti-aliasing (1 pixel width)
                float aa = 1.0;
                
                // Fill (inside)
                float fillAlpha = 1.0 - smoothstep(-aa, aa, dist);
                
                // Border (in pixels)
                float borderStart = -_BorderWidth;
                float borderEnd = 0.0;
                float borderAlpha = smoothstep(borderStart - aa, borderStart + aa, dist) * 
                                   (1.0 - smoothstep(borderEnd - aa, borderEnd + aa, dist));
                
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord) + _TextureSampleAdd;
                half4 color = texColor * IN.color;
                
                // Blend fill color with border color
                color.rgb = lerp(color.rgb, _BorderColor.rgb, borderAlpha * _BorderColor.a);
                color.a *= fillAlpha;

                #ifdef UNITY_UI_CLIP_RECT
                float2 inside = step(_ClipRect.xy, IN.worldPosition.xy) * step(IN.worldPosition.xy, _ClipRect.zw);
                color.a *= inside.x * inside.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
            ENDHLSL
        }
    }
    
    Fallback "UI/Default"
}
