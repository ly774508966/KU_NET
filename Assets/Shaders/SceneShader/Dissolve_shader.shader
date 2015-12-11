Shader "场景/Dissolve_shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DissolveTex("噪声纹理",2D) =""{}
		_Tile("噪波图的平铺系数，平铺倍数与之成反比", Range (0.1, 1)) = 1 
		_DissolveValue("阀值",Range(0,1)) =0.5
		_DissolveColor("溶解范围源颜色",Color) = (1,0,0,1)
		_DissolveDstColor("溶解范围目标颜色",Color) =(1,1,0,1)  
		_ColorPower("溶解色亮度",Range(0.01,3)) =0.5
		_DissolveSize("预溶解阀值",Range(0,1)) =0.5

	}
	SubShader
	{
		Pass
		{
			Tags { "RenderType"="Opaque" "Queue" ="Opaque" "IgnoreProjector"="true"}
			Cull Off 
			// ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _DissolveTex;
			float4 _MainTex_ST;
			float _DissolveValue;
			float _ColorPower;
			float4 _DissolveColor;
			float4 _DissolveDstColor;
			float _Tile;

			float _DissolveSize;

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert ( appdata_base  v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}
			
			

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 outcol  =col;

				float a = tex2D(_DissolveTex,i.uv/_Tile).r;
				float result = a -_DissolveValue;

				clip(result);

				if(_DissolveValue > 0 && result >0 && result < _DissolveSize)
				{

					outcol =outcol *lerp(_DissolveColor,_DissolveDstColor,result/_DissolveSize) * _ColorPower ;
				}

				return outcol;
			}
			ENDCG
		}
	}
}
