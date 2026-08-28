Shader "Chaosbound/UI/HealthOrbFill"
{
    Properties
    {
        [PerRendererData]
        _MainTex ("UI Texture", 2D) = "white" {}

        _FillTex ("Fill Texture 1", 2D) = "white" {}
        _FillTex2 ("Fill Texture 2", 2D) = "white" {}

        _Color ("Color", Color) = (1,0,0,1)

        _Brightness ("Brightness", Range(0,3)) = 1
        _Contrast ("Contrast", Range(0.1,3)) = 1
        _NoiseBlend ("Noise Blend", Range(0,1)) = 0.35

        _Tiling ("Texture Tiling 1", Vector) = (1,1,0,0)
        _ScrollSpeed ("Scroll Speed 1", Vector) = (0,0,0,0)

        _Tiling2 ("Texture Tiling 2", Vector) = (1,1,0,0)
        _ScrollSpeed2 ("Scroll Speed 2", Vector) = (0,0,0,0)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)]
        _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
            Name "HealthOrbFill"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma target 2.0

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 texcoord      : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;

            sampler2D _FillTex;
            sampler2D _FillTex2;

            float4 _Tiling;
            float4 _ScrollSpeed;

            float4 _Tiling2;
            float4 _ScrollSpeed2;

            fixed4 _Color;

            float _Brightness;
            float _Contrast;
            float _NoiseBlend;

            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;

                OUT.worldPosition = v.vertex;

                OUT.vertex =
                    UnityObjectToClipPos(v.vertex);

                OUT.texcoord =
                    v.texcoord;

                OUT.color =
                    v.color;

                return OUT;
            }

            float SampleTexture(
                sampler2D textureSampler,
                float2 uv)
            {
                fixed4 sample =
                    tex2D(
                        textureSampler,
                        uv);

                return dot(
                    sample.rgb,
                    float3(0.299, 0.587, 0.114));
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // --------------------------------------------------
                // UI shape
                // --------------------------------------------------

                fixed4 uiColor =
                    tex2D(
                        _MainTex,
                        IN.texcoord);

                // --------------------------------------------------
                // Layer 1
                // --------------------------------------------------

                float2 uv1 =
                    IN.texcoord *
                    _Tiling.xy;

                uv1 +=
                    _Time.y *
                    _ScrollSpeed.xy;

                float noise1 =
                    SampleTexture(
                        _FillTex,
                        uv1);

                // --------------------------------------------------
                // Layer 2
                // --------------------------------------------------

                float2 uv2 =
                    IN.texcoord *
                    _Tiling2.xy;

                uv2 +=
                    _Time.y *
                    _ScrollSpeed2.xy;

                float noise2 =
                    SampleTexture(
                        _FillTex2,
                        uv2);

                // --------------------------------------------------
                // Combine layers
                // --------------------------------------------------

                float noise =
                    lerp(
                        noise1,
                        (noise1 + noise2) * 0.5,
                        _NoiseBlend);

                // --------------------------------------------------
                // Contrast
                // --------------------------------------------------

                noise =
                    saturate(
                        (noise - 0.5) *
                        _Contrast +
                        0.5);

                // --------------------------------------------------
                // Brightness
                // --------------------------------------------------

                noise =
                    saturate(
                        noise *
                        _Brightness);

                // --------------------------------------------------
                // Final color
                // --------------------------------------------------

                fixed4 color =
                    _Color;

                color.rgb *=
                    noise;

                color.a *=
                    uiColor.a;

                color.a *=
                    IN.color.a;

                #ifdef UNITY_UI_CLIP_RECT

                color.a *=
                    UnityGet2DClipping(
                        IN.worldPosition.xy,
                        _ClipRect);

                #endif

                #ifdef UNITY_UI_ALPHACLIP

                clip(color.a - 0.001);

                #endif

                return color;
            }

            ENDCG
        }
    }
}