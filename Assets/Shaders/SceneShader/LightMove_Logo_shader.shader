Shader "场景/闪光移动"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("混合颜色参数",Color) =(0.3,0.5,0.3)
		_power("颜色强度",Range(0.01,2)) =0.1
		_speed("速度",Range(0.01,2)) =0.01
		_Rag("力度",Range(0,360)) =30
		_delta("间隔",Range(-2,2)) =0.6
	}
	SubShader
	{
		// No culling or depth
		// Cull Off ZWrite Off ZTest Always

		Pass
		{
			Tags { "RenderType" ="Opaque" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _speed;
			float4 _Color;
			float _power;
			float _Rag;
			float _delta;

			float2 rorate(float2 src,float _degree)
			{
				float d2r = 0.017;
				// return  float2(src.x +(1-src.y)/tan(_degree *d2r),src.y);
				return  mul(float2x2(cos(d2r * _degree),sin(d2r * _degree),-sin(d2r *_degree),cos(d2r * _degree)),src );

			}

			fixed4 frag (v2f i) : SV_Target
			{


				float2 uv =rorate(i.uv * _delta,_Rag);
				float temp = 1/length(float2(uv.x*0.1 /sin(uv.x+ _Time.y*_speed),uv.y *0.1/cos(uv.y+_Time.y* _speed))) ;
				float4 col;

				float c=pow(temp, _power);
				
				col = clamp(c+ _Color,0.01,1);
				fixed4 Texcol = tex2D(_MainTex,i.uv);
				col = lerp(col,Texcol,smoothstep(0,1,c));

				return col;
			}
			ENDCG
		}
	}
}
