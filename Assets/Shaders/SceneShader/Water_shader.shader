Shader "场景/水面泛光"
{
	Properties
	{
		_Count("传递能量大小",Range(1,10))= 5
		_Color("主要颜色",Color)=(0.0, 0.35, 0.5)
		_power("强度",Range(0.1,10)) =3
	}
	SubShader
	{

		Pass
		{
			Tags { "RendeType"="Opaque" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			int _Count;
			float4 _Color;
			float _power;

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


			fixed4 frag (v2f i) : SV_Target
			{
				float time = _Time.y ;// 
				float2 p = i.uv-1.8 ; //

				float2 temp= p*4;
				float c = 1.0;
				float inten = 0.005;

				for (int n = 0; n < _Count; n++) //n 为采样系数
				{
					float t = time ;//*(1.0 - (3.5 / float(n+1)));// time *( 1- 3.5 /n) ~= 至少-14 ~ 12* time
					temp= p + float2(cos(t - temp.x) + sin(t + temp.y), sin(t - temp.y) + cos(t + temp.x));
					c += 1/length(float2(p.x / (sin(temp.x+t)),p.y / (cos(temp.y+t))));
				}
				c /= _Count;
				c = 1.17 -pow(c, _power);
				float3 colour =float3(pow(abs(c), 8.0));
				colour = clamp(colour + _Color.rgb, 0.0, 1.0);
    
				return float4(colour,1);
			}
			ENDCG
		}
	}
}
