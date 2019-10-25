Shader "Unlit/HexTitle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _Dissolve ("Dissolve", Range(0, 1)) = 0
        _Sheen ("Sheen Pos", Range(-1, 2)) = 0.5
        _SheenFalloff ("Sheen Falloff", Float) = 0.1
        [HDR] _SheenColor ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
            sampler2D _Noise;
            float4 _MainTex_ST;
            float4 _Noise_ST;

            fixed4 _Color, _SheenColor;
            float _Dissolve, _Sheen, _SheenFalloff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                float m = tex2D(_MainTex, i.uv).r;
                float d = saturate(m - (tex2D(_Noise, i.uv).r + _Dissolve));
                // float d = saturate(m - (i.uv.x + _Dissolve));
                clip(d - 0.05);
                return lerp(_SheenColor, _Color, pow(abs(i.uv.x + 0.5 * i.uv.y - _Sheen), _SheenFalloff));
            }
            ENDCG
        }
    }
}
