Shader "Custom/HexTop"
{
    Properties
    {
        _Grass ("Grass Color", Color) = (1,1,1,1)
        _Grass1 ("Grass1 Color", Color) = (1,1,1,1)
        _GrassCutoff ("Grass Cutoff", Float) = 0.35

        _Mountain ("Mountain Color", Color) = (1,1,1,1)
        _MountainCutoff ("Moutain Cutoff", Float) = 0.35

        _Snow ("Snow Color", Color) = (1,1,1,1)
        _SnowCutoff ("Snow Cutoff", Float) = 0.5

        _Terrain ("Terrain Texture", 2D) = "white" {}
		_TerrainAmount ("Terrain Amount", Float) = 0.5
        _Noise ("Noise Texture", 2D) = "white" {}
        _NoiseAmount ("Noise Amount", Float) = 0.05
        _Mask ("Mask", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Offset ("Offset", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _Terrain;
        sampler2D _Mask;
        sampler2D _Noise;

        struct Input
        {
            float2 uv_Terrain;
            float3 worldPos;
            float3 localPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _Grass, _Grass1, _Mountain, _Snow;
        fixed4 _Noise_ST;
        fixed4 _Terrain_ST;

        float _Offset, _NoiseAmount, _TerrainAmount;
        float _GrassCutoff, _MountainCutoff, _SnowCutoff;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input o)
		{
            UNITY_INITIALIZE_OUTPUT(Input, o);
			float4 world = mul( unity_ObjectToWorld, v.vertex );
            float mask = tex2Dlod(_Mask, float4(v.texcoord.xy, 0, 0));
            v.vertex.z += tex2Dlod(_Terrain, float4(TRANSFORM_TEX(world.xz + float2(.5 * sin(world.x), 0), _Terrain) * 0.1, 0, 0)) * _Offset * mask;
			v.vertex.z *= _TerrainAmount;
            // v.vertex.z += tex2Dlod(_Terrain, float4(TRANSFORM_TEX(world.xz, _Terrain) * 0.1, 0, 0)) * _Offset * mask;
            o.localPos = v.vertex;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float uv = IN.localPos.z + _NoiseAmount * (tex2D(_Noise, TRANSFORM_TEX(IN.worldPos.xz, _Noise)) - 0.5);
            o.Albedo = uv > _MountainCutoff ? (uv > _SnowCutoff ? _Snow : _Mountain) : (uv > _GrassCutoff ? _Grass : _Grass1);

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
