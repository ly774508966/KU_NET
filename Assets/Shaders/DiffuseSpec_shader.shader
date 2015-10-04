Shader "Kubility/DiffuseSpec_shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SpeColor ("SpecularColor",Color) =(1,1,1,1)
		_SpecTex("_SpecTex",2D) =""{}
		_Glo("Gloss",Range(0,10)) =1
		// _Glossiness ("Smoothness", Range(0,1)) = 0.5

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf SimpleSpec

		// Use shader model 3.0 target, to get nicer looking lighting
		// #pragma target 3.0

		sampler2D _MainTex;
		sampler2D  _SpecTex;
		fixed4 _Color;
		fixed4 _SpeColor;
		float _Glo;

		struct Input {
			float2 uv_MainTex;
			float2 uv_SpecTex;
		};

		struct CustomSurfaceOutput {
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed3 SpecularColor; 
			half Specular;
			fixed Gloss;
			fixed Alpha;
		};

		// half _Glossiness;
		// half _Metallic;


		void surf (Input IN, inout CustomSurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 scol = tex2D(_SpecTex,IN.uv_SpecTex) * _SpeColor;
			o.SpecularColor = scol.rgb;
			o.Specular  = scol.r; 
			o.Albedo = c.rgb;

			o.Alpha = c.a;
		}
		half4 LightingSimpleSpec(CustomSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			float h = normalize(lightDir+ viewDir );
			float diff = dot(s.Normal,lightDir);

			float pl = dot(s.Normal,h);

			float3 specol =pow(pl,s.Specular ) * _Glo* s.SpecularColor.rgb * _LightColor0.rgb;

			half4 col ;
			col.rgb = specol.rgb + s.Albedo * _LightColor0.rgb * diff;
			col.a = s.Alpha;
			return  col;
		
		}  


		ENDCG
	} 
	FallBack "Diffuse"
}
