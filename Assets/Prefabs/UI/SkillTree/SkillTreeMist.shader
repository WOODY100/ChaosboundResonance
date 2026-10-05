Shader "Chaosbound/SkillTreeMist"
{
    Properties
    {
        _MainTex ("Mist Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)

        _RotationSpeed ("Rotation Speed", Range(-1, 1)) = 0.1
        _Phase ("Phase", Range(0, 6.283185)) = 0

        _ScrollX ("Scroll X", Range(-0.1, 0.1)) = 0.005
        _ScrollY ("Scroll Y", Range(-0.1, 0.1)) = 0.003
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;

            float _RotationSpeed;
            float _Phase;

            float _ScrollX;
            float _ScrollY;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseSample =
                    tex2D(_MainTex, i.uv);

                float2 center =
                    float2(0.5, 0.5);

                // Continuous rotation with
                // deterministic per-node variation.
                float angle =
                    (_RotationSpeed * _Time.y) +
                    (_Phase * 3.0);

                float sine =
                    sin(angle);

                float cosine =
                    cos(angle);

                float2 localUV =
                    i.uv - center;

                float2 rotatedUV =
                    float2(
                        localUV.x * cosine -
                        localUV.y * sine,

                        localUV.x * sine +
                        localUV.y * cosine
                    ) + center;

                // Continuous displacement with
                // deterministic per-node starting offset.
                float2 scroll =
                    float2(
                        _ScrollX,
                        _ScrollY
                    ) * _Time.y + _Phase;

                rotatedUV += scroll;

                fixed4 movingSample =
                    tex2D(
                        _MainTex,
                        rotatedUV
                    );

                float finalAlpha =
                    baseSample.a *
                    movingSample.a;

                fixed3 finalRGB =
                    movingSample.rgb;

                fixed4 finalColor =
                    fixed4(
                        finalRGB,
                        finalAlpha
                    );

                return finalColor * i.color;
            }

            ENDCG
        }
    }
}