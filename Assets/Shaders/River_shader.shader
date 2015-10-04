Shader "Kubility/River_shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Speed ("speed", Range(0,1)) = 0.5

	}
	SubShader {
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue" ="Transparent"}
		LOD 200

		Cull  off
		// ZWrite off  
		AlphaTest off
		Lighting off 
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Speed;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			float  scrollX = IN.uv_MainTex.x;//+  _Speed * _Time.x;
			float  scrollY = IN.uv_MainTex.y+ _Speed * _Time.x;

			fixed4 c = tex2D (_MainTex, float2 (scrollX,scrollY)) * _Color;
			o.Albedo = c.rgb;


			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
