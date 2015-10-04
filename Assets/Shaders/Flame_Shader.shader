Shader "Kubility/Flame_Shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ScreenSize ("Screen Size",Vector) =(1,1,0,0)
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

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		float4 _ScreenSize;

		void surf (Input IN, inout SurfaceOutput o) {

			float2 pos = ( IN.uv_MainTex.xy / _ScreenSize.xy )*8.-float2(4,5);
			float time = _Time.y;
			if(pos.y>-6.){
				pos.y += 0.1*sin(time*3)+0.13*cos(time*2+0.6)+0.1*sin(time*3+0.4)+0.2*frac(sin(time*400));
			}

			float3 color = 0;
			
			float p =0.1;
			
			float y = -pow(pos.x,3.2)/(2.0*p)*3.3;
			
			
			float dir = length(pos-float2(pos.x,y))*sin(0.3);
			
			if(dir < 0.7){
				color.rg += smoothstep(0.0,1.0,0.75-dir);
				color.g /=2.4;
			}
			color += pow(color.r,1.1);

			o.Albedo = color.rgb;

			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}



