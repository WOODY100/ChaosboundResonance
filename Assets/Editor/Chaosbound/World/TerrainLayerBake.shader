Shader "Hidden/Chaosbound/TerrainLayerNormalBake"
{
    Properties
    {
        _Alphamap ("Alphamap", 2D) = "black" {}

        _Layer0 ("Normal Layer 0", 2D) = "bump" {}
        _Layer1 ("Normal Layer 1", 2D) = "bump" {}
        _Layer2 ("Normal Layer 2", 2D) = "bump" {}
        _Layer3 ("Normal Layer 3", 2D) = "bump" {}

        _Tile0 ("Tile 0", Vector) = (1,1,0,0)
        _Tile1 ("Tile 1", Vector) = (1,1,0,0)
        _Tile2 ("Tile 2", Vector) = (1,1,0,0)
        _Tile3 ("Tile 3", Vector) = (1,1,0,0)

        _NormalScale0 ("Normal Scale 0", Float) = 1
        _NormalScale1 ("Normal Scale 1", Float) = 1
        _NormalScale2 ("Normal Scale 2", Float) = 1
        _NormalScale3 ("Normal Scale 3", Float) = 1

        _HasNormal0 ("Has Normal 0", Float) = 0
        _HasNormal1 ("Has Normal 1", Float) = 0
        _HasNormal2 ("Has Normal 2", Float) = 0
        _HasNormal3 ("Has Normal 3", Float) = 0

        _TerrainSize ("Terrain Size", Vector) = (12,12,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Blend Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _Alphamap;

            sampler2D _Layer0;
            sampler2D _Layer1;
            sampler2D _Layer2;
            sampler2D _Layer3;

            float4 _Tile0;
            float4 _Tile1;
            float4 _Tile2;
            float4 _Tile3;

            float _NormalScale0;
            float _NormalScale1;
            float _NormalScale2;
            float _NormalScale3;

            float _HasNormal0;
            float _HasNormal1;
            float _HasNormal2;
            float _HasNormal3;

            float4 _TerrainSize;

            int _LayerCount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 LayerUV(
                float2 terrainUV,
                float4 tile)
            {
                float2 worldPosition =
                    terrainUV * _TerrainSize.xy;

                float2 safeSize =
                    max(
                        tile.xy,
                        float2(0.0001, 0.0001));

                return
                    worldPosition / safeSize +
                    tile.zw;
            }

            float3 SampleLayerNormal(
                sampler2D layer,
                float2 uv,
                float scale,
                float hasNormal)
            {
                // A Terrain Layer without a Normal Map contributes
                // a perfectly flat tangent-space normal.
                float3 normal =
                    float3(0.0, 0.0, 1.0);

                if (hasNormal > 0.5)
                {
                    normal =
                        UnpackNormal(
                            tex2D(layer, uv));

                    normal.xy *= scale;

                    normal =
                        normalize(normal);
                }

                return normal;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 weights =
                    tex2D(_Alphamap, i.uv);

                float3 result =
                    float3(0.0, 0.0, 0.0);

                if (_LayerCount > 0)
                {
                    float3 n0 =
                        SampleLayerNormal(
                            _Layer0,
                            LayerUV(i.uv, _Tile0),
                            _NormalScale0,
                            _HasNormal0);

                    result +=
                        n0 * weights.r;
                }

                if (_LayerCount > 1)
                {
                    float3 n1 =
                        SampleLayerNormal(
                            _Layer1,
                            LayerUV(i.uv, _Tile1),
                            _NormalScale1,
                            _HasNormal1);

                    result +=
                        n1 * weights.g;
                }

                if (_LayerCount > 2)
                {
                    float3 n2 =
                        SampleLayerNormal(
                            _Layer2,
                            LayerUV(i.uv, _Tile2),
                            _NormalScale2,
                            _HasNormal2);

                    result +=
                        n2 * weights.b;
                }

                if (_LayerCount > 3)
                {
                    float3 n3 =
                        SampleLayerNormal(
                            _Layer3,
                            LayerUV(i.uv, _Tile3),
                            _NormalScale3,
                            _HasNormal3);

                    result +=
                        n3 * weights.a;
                }

                // The C# side accumulates groups and normalizes the
                // final tangent-space normal after all Terrain Layers
                // have been processed.
                return float4(result, 1.0);
            }
            ENDHLSL
        }
    }
}
