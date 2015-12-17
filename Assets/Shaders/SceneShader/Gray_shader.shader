Shader "RPG/TurnGray" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Factor ("Gray Factor", Range(0, 1)) = 0
		_Gray ("Base (RGB)", 2D) = "white" {}
		_Col("Col",Color) =(0.299, 0.587, 0.114,1)
		[Space]_Delta("_Delta",Range(0.1,10)) =0.7
		[Space]_Lerp("Lerp",Range(0,1)) =0.3
		[Space][Toggle(Gray_ON)] _IsToGray("Target",Float) =0
	}
	SubShader {
		LOD 200
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			//"Queue" = "Geometry"
			//"RenderType"="Opaque"
		}
		
		ZTest Always Cull Off ZWrite Off Blend Off
	     Fog { Mode off }  



		 Pass
		 {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature Gray_ON

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half _Factor;
			half _IsToGray;
			half _Lerp;
			half _Delta;
			half4 _Col;


			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};
			uniform half4 _MainTex_TexelSize;

			float4 _MainTex_ST;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				#if SHADER_API_D3D9
        		  if (_MainTex_TexelSize.y < 0.0)
        			o.uv.y = 1.0 - o.uv.y;
        	    #endif
				
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 result = tex2D(_MainTex,i.uv);
				half v =(_Time.x * _Delta)%1;
				half ov= dot(result.rgb, _Col.rgb);
				#if Gray_ON
					result =  lerp(result,ov,v) ;
					return result; 
				#else
					result =  lerp(ov,result,v) ;
					return result;
				#endif

				
			}
			ENDCG
		 }

		 Pass
		 {
		 	//可开可不开，看情况
		 	Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _Gray;
			float4 _MainTex_ST;
			half _Factor;
			half _IsToGray;
			half _Lerp;
			half _Delta;
			half4 _Col;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 result = tex2D(_MainTex,i.uv);

				fixed4 color = tex2D(_Gray,i.uv);

				result = lerp(result, float4(0.5,0.5,0.5,0),color.a* _Lerp);

				return result;
			}
			ENDCG
		 }

	} 
}