Shader "Unlit/UpdatedSea"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _SeaColor("Color", Color) = (1,1,1,1)
        _LowerVerticesHeight("Lower Vertices Height", int) = 0.1
        _WaveSpeed("Wave Speed", Range(0,5)) = 0.1
        _MaximumWaveHeight("Maximum Wave Height", Range(1,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vertexFunction
            #pragma fragment fragmentFunction
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _SeaColor;
            int _LowerVerticesHeight;
            float _WaveSpeed;
            float _MaximumWaveHeight;

            float VertexHeight()
            {
                return _MaximumWaveHeight * sin(_Time.y * _WaveSpeed) + _MaximumWaveHeight + _LowerVerticesHeight;
            }

            v2f vertexFunction (appdata INPUT)
            {
                v2f OUTPUT;

                INPUT.vertex.y = INPUT.vertex.y > _LowerVerticesHeight ? VertexHeight() : INPUT.vertex.y;

                OUTPUT.vertex = UnityObjectToClipPos(INPUT.vertex);
                OUTPUT.uv = TRANSFORM_TEX(INPUT.uv, _MainTex);
                UNITY_TRANSFER_FOG(OUTPUT,OUTPUT.vertex);
                return OUTPUT;
            }

            fixed4 fragmentFunction (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 pixelColor = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, pixelColor);
                return pixelColor * _SeaColor;
            }
            ENDCG
        }
    }
}
