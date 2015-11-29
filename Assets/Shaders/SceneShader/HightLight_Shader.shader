Shader "场景/光照shader_Vf"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		// _PToggle("_PToggle",Range(0,1)) =0
		// _DToggle("_DToggle",Range(0,1)) =0
		// _AToggle("_AToggle",Range(0,1)) =0
		// phone_power("phone_power",Range(0,100)) =1
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "LightMode"="ForwardBase"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			// #pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			float _PToggle;
			float _DToggle;
			float _AToggle;
			float phone_power;

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				// float3 lightDir :TEXCOORD0 ;
				// float3 viewDir :TEXCOORD1 ; 
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert ( appdata_full  v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				// o.lightDir = WorldSpaceLightDir(v.vertex);
				// o.viewDir  =WorldSpaceViewDir(v.vertex );

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// float nh =
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
		
				return phone_power;
			}
			ENDCG
		}
	}
	CustomEditor "SimpleShaderEditor"
}
