Shader "Chaosbound/SkillTreeConnectionTest"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}

        _MainTexScrollX
            ("Main Texture Scroll X", Range(-100, 100)) = -12

        _MainTexScrollY
            ("Main Texture Scroll Y", Range(-100, 100)) = 0

        _MainTexOffset
            ("Main Texture Offset", Vector) = (0, 1, 0, 0)

        _TrailWidthGradientTex
            ("Trail Width Gradient Texture", 2D) = "white" {}

        _TrailWidthPower
            ("Trail Width Power", Range(0.1, 10)) = 1

        _TrailWidthMultiplier
            ("Trail Width Multiplier", Range(0.1, 10)) = 4

        _Color
            ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Cull Off
        Lighting Off
        ZWrite Off

        Blend One One

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _TrailWidthGradientTex;

            float _MainTexScrollX;
            float _MainTexScrollY;
            float4 _MainTexOffset;

            float _TrailWidthPower;
            float _TrailWidthMultiplier;

            fixed4 _Color;
            float4 _ClipRect;

            v2f vert(appdata v)
            {
                v2f o;

                o.worldPosition =
                    v.vertex;

                o.vertex =
                    UnityObjectToClipPos(
                        v.vertex);

                o.uv =
                    v.uv;

                o.color =
                    v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float width =
                    tex2D(
                        _TrailWidthGradientTex,
                        float2(
                            i.uv.x,
                            0.5)
                    ).r;

                width =
                    pow(
                        max(
                            width,
                            0.0001),
                        _TrailWidthPower);

                width *=
                    _TrailWidthMultiplier;

                float2 shapeUV =
                    i.uv;

                shapeUV.y =
                    (shapeUV.y * 2.0 - 1.0)
                    / width
                    * 0.5
                    + 0.5;

                clip(shapeUV.y);
                clip(1.0 - shapeUV.y);

                shapeUV +=
                    float2(
                        _MainTexScrollX,
                        _MainTexScrollY)
                    * _Time.xy;

                shapeUV +=
                    _MainTexOffset.xy;

                fixed4 textureColor =
                    tex2D(
                        _MainTex,
                        shapeUV);

                #ifdef UNITY_UI_CLIP_RECT

                float clipValue =
                    UnityGet2DClipping(
                        i.worldPosition.xy,
                        _ClipRect);

                clip(
                    clipValue - 0.001
                );

                #endif

                return textureColor * i.color;
            }

            ENDCG
        }
    }
}