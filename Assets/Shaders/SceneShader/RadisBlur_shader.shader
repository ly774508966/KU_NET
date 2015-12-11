Shader "场景/RadisBlur_shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        // _fSampleStrength("采样力度", Float) = 2.2 //采样力度
        _Count("采样次数",Range(3,10)) = 5
        _percentage("采样系数",Range(0.001,0.1)) =0.01
        _CenterX("中心坐标X",Range(0,1)) =0.5
        _CenterY("中心坐标X",Range(0,1)) =0.5
        _ColorPower("曝光度",Range(0,10)) =1
	}
	SubShader
	{

		Cull Off 
		ZWrite Off 
		ColorMask RGB
		Fog {Mode off}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

            // half _fSampleStrength;
            sampler2D _MainTex;
            half4 _MainTex_ST;
            int _Count;
            half _percentage;

            half _CenterX;
            half _CenterY;
            half _ColorPower;

			struct v2f
			{
				half2 uv : TEXCOORD0;
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
				half2 center = half2(_CenterX, _CenterY);

				half2 texcoord = i.uv -center;

				fixed4 col = tex2D(_MainTex, texcoord) * _ColorPower;
				fixed4 ColTemp = col;
				float scale = 1;
	
				for(int j= 0 ; j < _Count; ++j)
				{
					ColTemp+= tex2D(_MainTex, texcoord * scale + center);
					scale = 1 + (float(j * _percentage ));
				}
				ColTemp /= _Count;

				// half t = saturate(dist * _fSampleStrength);  

				return ColTemp;//lerp(col,ColTemp,t);
			}
			ENDCG
		}
	}
}
