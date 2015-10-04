Shader "Kubility/Fresnel_shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FresColor("fres color",Color  ) =(1,1,1,1)
		_FresAmount("FresAmonut",Range(0,30))=2
		_SpeColor ("SpeColor ",Color  ) =(1,1,1,1)

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		// #pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		fixed4 _FresColor;
		float _FresAmount;
		fixed4 _SpeColor;

		struct Input {
			float2 uv_MainTex;
			float3 worldRefl;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput  o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			float fresDot =saturate( dot(o.Normal,normalize( IN.viewDir) ));
			float fresnel = min(1, pow(1- fresDot, _FresAmount));
			o.Albedo = c.rgb ;
			o.Emission = fresnel * _FresColor;
			o.Specular = _SpeColor;
			o.Gloss  = 1;  

			o.Alpha = c.a;
		}


		ENDCG
	} 

	FallBack "Diffuse"
}
