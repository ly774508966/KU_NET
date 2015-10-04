Shader "Kubility/Compress_shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		 // _MainTint ("Diffuse Tint", Color) = (1,1,1,1)  
		_RTexture ("Red Channel Texture", 2D) = ""{}
		_GTexture ("Green Channel Texture", 2D) = ""{}
		_BTexture ("Blue Channel Texture", 2D) = ""{}
		_ATexture ("Alpha Channel Texture", 2D) = ""{}
		_BlendTex ("Blend Texture", 2D) = ""{}

	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200

		Pass {
			Cull  off
			AlphaTest off
			Lighting on
			ZWrite on
			// ZTest Always

			CGPROGRAM
			#pragma vertex  vert
			#pragma fragment Frag
			#include "UnityCG.cginc" 

			sampler2D _RTexture;
			sampler2D _GTexture;
			sampler2D _BTexture;
			sampler2D _ATexture;
			sampler2D _BlendTex;

			float4 _RTexture_ST;
			float4 _GTexture_ST;
			float4 _BTexture_ST;
			float4 _ATexture_ST;
			float4 _BlendTex_ST;

			struct full_data {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 texcoord3 : TEXCOORD3;
				float4 texcoord4 : TEXCOORD4;

			};

			struct  v2f 
			{
				float4 pos :SV_POSITION;
				float2 uv_RTexture :TEXCOORD0  ;
				float2 uv_GTexture :TEXCOORD1  ;
				float2 uv_BTexture :TEXCOORD2 ;
				float2 uv_ATexture :TEXCOORD3 ;
				float2 uv_BlendTex :TEXCOORD4  ;
				// float2 uv_MainTint :TEXCOORD5 ;
			};   

			v2f vert(  full_data  data)
			{
				v2f v;
				v.pos =mul(UNITY_MATRIX_MVP,data.vertex );
				
				v.uv_RTexture = TRANSFORM_TEX(data.texcoord,_RTexture);
				v.uv_GTexture = TRANSFORM_TEX(data.texcoord1,_GTexture);
				v.uv_BTexture = TRANSFORM_TEX(data.texcoord2,_BTexture);
				v.uv_ATexture = TRANSFORM_TEX(data.texcoord3,_ATexture);
				v.uv_BlendTex = TRANSFORM_TEX(data.texcoord4,_BlendTex);
				return  v;

			}

			float4 Frag(v2f f):SV_Target
			{
				float4  col =0;

				float4 rt = tex2D(_RTexture,f.uv_RTexture);
				float4 gt = tex2D(_GTexture,f.uv_GTexture);
				float4 bt= tex2D(_BTexture,f.uv_BTexture);
				float4 at= tex2D(_ATexture,f.uv_ATexture);
				float4 blend = tex2D(_BlendTex,f.uv_BlendTex);

				col = lerp(rt,gt,blend.g);
				col = lerp(col,bt,blend.b);
				col = lerp(col,at,blend.a);
				return float4(col.rgb,1);
			}

			 ENDCG   
		}
	} 
	FallBack "Diffuse"
}
