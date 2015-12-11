Shader "场景/Bloom_shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BumpTex("_BumpTex",2D) =""{}
		_specMap("_specMap",2D) =""{}
		_Shininess ("_Shininess", Range(0, 10)) = 5
		_SpecularIntensity("SpecularIntensity",Range(0.1,50)) = 2
		_Bloom("bx",Range(0,10)) =1
		_TextureSize ("贴图大小",Range(128,512)) = 512
		_BlurPower("_BlurPower",Range(0,3)) =0.214
	}

	SubShader
	{
		LOD 100

		Pass
		{
			Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }

			Lighting off
			ColorMask RGB
			AlphaTest  off
			// ZTest Always 
			Cull Off 
			// ZWrite Off
			Fog { Mode off} 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
			#include "UnityCG.cginc"
			#include "Lighting.cginc" 

			struct v2f
			{

				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 LDir:TEXCOORD1 ; 
				float3 NDir:NORMAL ;
				float3 TDir:TANGENT ;
				float3 BDir:BINORMAL ;
				float3 lightDirInTangent:TEXCOORD2;
				float3 viewDirInTangent:TEXCOORD3 ;
			};

			sampler2D _MainTex;
			fixed4 _MainTex_ST;

			sampler2D  _BumpTex;
			sampler2D  _specMap;

			half _Shininess;
			half _SpecularIntensity;
			half _Bloom;
			half _TextureSize;
			half _BlurPower;


			
			v2f vert ( appdata_full  v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

				float4 WorldPos = mul(_Object2World ,v.vertex);
				float4 ViewPos = mul(UNITY_MATRIX_MV ,v.vertex);

				o.NDir = mul(_Object2World ,float4(v.normal,0)).xyz;
				o.TDir = mul(_Object2World ,v.tangent).xyz;
				o.BDir = normalize(cross(o.NDir,o.TDir));
				o.LDir = normalize(_WorldSpaceLightPos0 ).xyz;

				float3x3 tbn = float3x3(o.TDir,o.BDir,o.NDir);

				o.lightDirInTangent = normalize(mul(tbn, o.LDir )).xyz ;
    			o.viewDirInTangent  = normalize(mul(tbn, -ViewPos.xyz)).xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half texScaler =  1.0/_TextureSize;
 				half texOffset = -0.5/_TextureSize;
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				fixed3 n_lightDirInTangent = - normalize(i.lightDirInTangent);
				fixed3 n_viewDirInTangent = normalize(i.viewDirInTangent);

				fixed3 bump = normalize(tex2D(_BumpTex,i.uv).xyz *2 -1);

				fixed lighting = dot(bump,n_lightDirInTangent);
				fixed blighting = n_lightDirInTangent.z;


				fixed specular = dot(-reflect(n_lightDirInTangent,bump),n_viewDirInTangent);

				fixed4 specColor = tex2D(_specMap,i.uv);

				half gauss0 = 1.0/32.0;
			    half gauss1 = 5.0/32.0;
			    half gauss2 =15.0/32.0;
			    half gauss3 =22.0/32.0;
			    float4 gaussFilter[7];

			    gaussFilter[0]  = float4( -3.0*texScaler , 0.0, 0.0, gauss0); 
			    gaussFilter[1]  = float4( -2.0*texScaler , 0.0, 0.0, gauss1); 
			    gaussFilter[2]  = float4( -1.0*texScaler , 0.0, 0.0, gauss2); 
			    gaussFilter[3]  = float4(  0.0*texScaler , 0.0, 0.0, gauss3);
			    gaussFilter[4]  = float4( +1.0*texScaler , 0.0, 0.0, gauss2);
			    gaussFilter[5]  = float4( +2.0*texScaler , 0.0, 0.0, gauss1);
			    gaussFilter[6]  = float4( +3.0*texScaler , 0.0, 0.0, gauss0);
			 
			  	col += _LightColor0 *lighting +lighting * blighting *_SpecularIntensity *pow(specular,_Shininess) *specColor;

			    int j;
			    for (j=0;j<7;j++)
			    {
			    	col += tex2D(_MainTex, i.uv + gaussFilter[j].xy *_Bloom) * gaussFilter[j].w ;
			    	col += tex2D(_MainTex, i.uv + gaussFilter[j].yx *_Bloom) * gaussFilter[j].w ;
			    }

				return col*_BlurPower ;//fixed4 (i.viewDirInTangent,1);
			}
			ENDCG
		}

	}
}
