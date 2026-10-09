Shader "Hidden/Chaosbound/TerrainLayerAlbedoBake"
{
    Properties
    {
        _Alphamap ("Alphamap", 2D) = "black" {}
        _Layer0 ("Layer 0", 2D) = "white" {}
        _Layer1 ("Layer 1", 2D) = "white" {}
        _Layer2 ("Layer 2", 2D) = "white" {}
        _Layer3 ("Layer 3", 2D) = "white" {}

        _Tile0 ("Tile 0", Vector) = (1,1,0,0)
        _Tile1 ("Tile 1", Vector) = (1,1,0,0)
        _Tile2 ("Tile 2", Vector) = (1,1,0,0)
        _Tile3 ("Tile 3", Vector) = (1,1,0,0)
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
            Blend One One

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _Alphamap;
            sampler2D _Layer0;
            sampler2D _Layer1;
            sampler2D _Layer2;
            sampler2D _Layer3;

            float4 _Layer0_ST;
            float4 _Layer1_ST;
            float4 _Layer2_ST;
            float4 _Layer3_ST;

            float4 _Tile0;
            float4 _Tile1;
            float4 _Tile2;
            float4 _Tile3;
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

            float2 LayerUV(float2 terrainUV, float4 tile)
            {
                float2 worldPosition =
                    terrainUV * _TerrainSize.xy;

                float2 safeSize =
                    max(tile.xy, float2(0.0001, 0.0001));

                return worldPosition / safeSize + tile.zw;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 weights = tex2D(_Alphamap, i.uv);

                fixed4 result = 0;

                if (_LayerCount > 0)
                    result += tex2D(
                        _Layer0,
                        LayerUV(i.uv, _Tile0)) * weights.r;

                if (_LayerCount > 1)
                    result += tex2D(
                        _Layer1,
                        LayerUV(i.uv, _Tile1)) * weights.g;

                if (_LayerCount > 2)
                    result += tex2D(
                        _Layer2,
                        LayerUV(i.uv, _Tile2)) * weights.b;

                if (_LayerCount > 3)
                    result += tex2D(
                        _Layer3,
                        LayerUV(i.uv, _Tile3)) * weights.a;

                result.a = 1;
                return result;
            }
            ENDHLSL
        }
    }
}
