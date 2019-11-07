Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color 0", Color) = (1,1,1,1)
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _Steepness ("Steepness", Range(0, 1)) = 0.5
        _Wavelength ("Wavelength", Float) = 10
		_Direction ("Direction (2D)", Vector) = (1,0,0,0)
        _FoamTex ("Foam Texture", 2D) = "white" {}
        // _Speed ("Speed", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert addshadow alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _FoamTex;
		sampler2D _CameraDepthTexture;

        struct Input
        {
            float2 uv_MainTex;
            // float3 localPos;
			float4 screenPos;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color, _Color1, _FoamColor;

        float _Steepness, _Wavelength;//, _Speed;
		float2 _Direction;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input o)
		{
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float3 p = v.vertex.xyz;
            float k = UNITY_PI / _Wavelength;
            float c = sqrt(9.8 / k);
			float2 d = normalize(_Direction);
			float f = k * (dot(d, p.xz) - c * _Time.y);
            float a = _Steepness / k;
            p.x += d.x * (a * cos(f));
			p.y = a * sin(f);
			p.z += d.y * (a * cos(f));
            
            float3 tangent = float3(
				1 - d.x * d.x * (_Steepness * sin(f)),
				d.x * (_Steepness * cos(f)),
				-d.x * d.y * (_Steepness * sin(f))
			);
			float3 binormal = float3(
				-d.x * d.y * (_Steepness * sin(f)),
				d.y * (_Steepness * cos(f)),
				1 - d.y * d.y * (_Steepness * sin(f))
			);
			float3 normal = normalize(cross(binormal, tangent));

			v.vertex.xyz = p;
            v.normal = normal;
            // o.localPos = v.vertex.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)));
            float surfZ = -mul(UNITY_MATRIX_V, float4(IN.worldPos.xyz, 1)).z;
            float intersect = 1 - (sceneZ - surfZ);
            fixed4 m = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            float foam0 = 1 - tex2D (_FoamTex, IN.uv_MainTex * 7 - float2(_Time.x * 3, cos(IN.uv_MainTex.x))).r;
            float foam1 = 1 - tex2D (_FoamTex, IN.uv_MainTex * 9 + float2(sin(IN.uv_MainTex.y), _Time.x * 3)).b;
            float mask = (foam0 * foam1) * 0.95;
            mask = saturate(pow(mask, 1));

            
            // float3 edge = _FoamColor.rgb * pow(intersect, 3) * _FoamColor.a;
            // o.Albedo = foam0 * foam1;
            // o.Albedo = saturate(_Color + saturate(edge - 0.8 * float3(mask, mask, mask)));
            o.Albedo = intersect - .3 * mask < 0.65 ? _Color : _FoamColor; 
            // o.Albedo = mask;
            // o.Albedo = lerp(_Color, _Color1, IN.localPos.y);//c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = m.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
