Shader "场景/光照shader_Vf_Fbd"
{
	Properties
	{
		_MainTex ("纹理贴图", 2D) = "white" {}
		_NormalMap("法线贴图",2D) = "white"{}
		_LightMap("BRDF光照贴图",2D) = "white"{}
		_CubeMap("立方体贴图",CUBE) =""{}
		_RefMap("反射贴图",2D) =""{}
		[Space][Enum(UnityEngine.Rendering.BlendMode)] _Blend ("混合参数1(Src着色器出来的颜色)", Float) = 1
        [Space][Enum(UnityEngine.Rendering.BlendMode)] _Blend2 ("混合参数2(Dst在屏幕中的颜色)", Float) = 1
        [Space][Enum(UnityEngine.Rendering.CullMode)] _Cull ("剔除模式", Float) = 1
        [Space][Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask ("颜色Mask", Float) = 1
        [KeywordEnum(LightAuto, OnlyDir, OnlyPoint)] _LightLay ("光源模式", Float) = 0

		// [KeywordEnum(None, Add, Multiply)] _Overlay ("Overlay mode", Float) = 0
		[Space][Toggle(_CubeMap_ON)] _CubeMap_Toggle("只有开启此选项，立方体贴图才会被使用",Float) =0
		[Space][Toggle(_NormalMap_ON)] _NormalMap_Toggle("只有开启此选项，法线贴图才会被使用",Float) =0
		[Space][Toggle(_LightMap_ON)] _LightMap_Toggle("只有开启此选项，光照贴图才会被使用(BRDF)",Float) =0
		[Space][Toggle(_RefMap_ON)] _RefMap_Toggle("只有开启此选项，反射贴图才会被使用",Float) =0
		// [Space][Toggle(_AO_OPEN)] _AO_Toggle("环境光遮蔽(AO)开关",Float) =0
		[Space][Toggle(_Shadow_ON)] _shadowToggle("unity内置阴影贴图启用开关",Float) =0
		[Space][Toggle(_PToggle_ON)] _PToggle("镜面高光开关",Float) =0
		[Space][Toggle(_PBack_ON)] _PBack("背面边缘渐变开关",Float) =0
		[Space][Toggle(_DToggle_ON)] _DToggle("漫反射开关",Float) =0
		[Space][Toggle(_DBack_ON)] _DBack("漫反射边缘渐变开关",Float) =0
		[Space][Toggle(_AToggle_ON)] _AToggle("环境光反射开关",Float) =0
		[Space]phone_Color("自定义镜面反射着色  (当开启镜面反射时有效)",Color) =(1,1,1,1)
		[Space]Diffuse_Color("自定义漫反射着色  (当开启漫反射时有效)",Color) =(1,1,1,1)
		[Space]Ambient_Color("自定义环境光反射着色  (当开启环境光反射时有效)",Color) =(1,1,1,1)
		[Space]refelect_Color("自定义立方体贴图反射  (当开启立方体反射时有效)",Color) =(1,1,1,1)
		// [Space]shadow_Color("自定义阴影颜色  (当开启阴影时有效)",Color) =(1,1,1,1)
		phone_power("镜面反射光强",Range(0.1,30)) =1
		CUBE_Reflect("立方体环境反射系数",Range(0.1,30)) =1
		Ref_power("菲涅尔系数",Range(0.1,10)) =1
		Diffuse_Power("漫反射强化",Range(0.1,10)) =1

	}

	SubShader
	{

		Pass
		{
			Tags { "RenderType"="Opaque" "Queue"="Opaque" "IgnoreProjector"="True" "LightMode"="ForwardBase"}//正向渲染路径仅支持一个平行光阴影投射灯
			Cull [_Cull] 
			ColorMask [_ColorWriteMask]
			AlphaTest off
			Fog {Mode off}
			Lighting off
			ZWrite  On 
			Blend [_Blend] [_Blend2]
			// Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma target 3.0
			#pragma exclude_renderers gles //无视gles
			// #pragma multi_compile _LightLay_LightAuto _LightLay_OnlyDir, _LightLay_OnlyPoint
			#pragma shader_feature _AToggle_ON  
			#pragma shader_feature _DToggle_ON
			#pragma shader_feature _PToggle_ON
			#pragma shader_feature _PBack_ON
			#pragma shader_feature _DBack_ON
			#pragma shader_feature _NormalMap_ON
			#pragma shader_feature _LightMap_ON
			// #pragma shader_feature _AO_ON
			#pragma shader_feature _CubeMap_ON
			#pragma shader_feature _Shadow_ON
			#pragma shader_feature _RefMap_ON

			
			#include "UnityCG.cginc"
			#include "Lighting.cginc" 
			#include "AutoLight.cginc" 
			
			#if _PToggle_ON
			float4 phone_Color;
			float phone_power;
			#endif
			#if _DToggle_ON
			float4 Diffuse_Color;
			#endif
			#if _AToggle_ON
			float4 Ambient_Color;
			#endif
			#if _CubeMap_ON
			float4 refelect_Color;
			float CUBE_Reflect;
			samplerCUBE _CubeMap;
			#endif

			sampler2D _MainTex;
			float4 _MainTex_ST;

			#if _NormalMap_ON
			sampler2D _NormalMap;
			float4  _NormalMap_ST;
			#endif

			#if _LightMap_ON
			sampler2D _LightMap;
			float4  _LightMap_ST;
			#endif

			#if _Shadow_ON
			float4 shadow_Color;
			#endif

			#if _RefMap_ON
			sampler2D _RefMap;
			float Ref_power;
			#endif

			float Diffuse_Power;

			struct v2f
			{
				
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 worldPos:TEXCOORD1 ; 
				float4 worldNormal:TEXCOORD2;
				float3 lightDir :TEXCOORD3;
				float3 viewDir :TEXCOORD4; 

				#if _LightMap_ON
				float4 BRDFCol :TEXCOORD5; 
				#else
				#endif

				#if _CubeMap_ON
				float3 R :TEXCOORD6; 
				#endif

				#if _Shadow_ON
				LIGHTING_COORDS(7,8)
				#endif
			};

			
			v2f vert ( appdata_full  v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				#if _NormalMap_ON
				o.worldNormal = normalize(mul(_Object2World,tex2Dlod(_NormalMap,o.uv)));
				#else
				o.worldNormal  = normalize(mul(_Object2World ,float4(v.normal ,0)));
				#endif

				o.worldPos  = normalize( mul(_Object2World ,v.vertex) );
				o.lightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));
				o.viewDir  =normalize(UnityWorldSpaceViewDir(o.worldPos));

				#if _LightMap_ON
				float difLight = dot (o.worldNormal, o.lightDir);  
				float rimLight = dot (o.worldNormal, o.viewDir); 
				o.BRDFCol = tex2Dlod(_LightMap,float2(difLight,rimLight));
				#else
				#endif

				#if _CubeMap_ON
				//way1
				// o.R = 2 * o.worldNormal.rgb *dot(o.worldNormal.rgb ,o.lightDir) -o.lightDir;
				//way2
				o.R = reflect(o.lightDir,o.worldNormal.rgb);
				#endif

				#if _Shadow_ON
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				#endif

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 Pcol=0;
				fixed4 Dcol =0;
				fixed4 Acol = 0;
				fixed4 RefCol =0;
				fixed4 col = 0;

				//镜面反射
				#if _PToggle_ON
				float H =normalize(i.lightDir + i .viewDir );
				float Pdot = 
					#if _PBack_ON
					max(0,dot(i.worldNormal ,H));
					#else
					dot(i.worldNormal ,H);
					#endif
				Pcol = _LightColor0 * phone_Color* pow(Pdot,phone_power);

				#endif
				// return  _LightColor0;
				// // return  1;
				// return  fixed4(Dcol.rgb,1);
				//漫反射
				#if _DToggle_ON
					float diffuse =
					#if _DBack_ON
					max(0,dot(i.lightDir,i.worldNormal ));
					#else
					dot(i.lightDir,i.worldNormal );
					#endif
					Dcol = _LightColor0 * Diffuse_Color * diffuse *Diffuse_Power;
				// return  Dcol;
				#endif

				//环境光
				#if _AToggle_ON
				Acol = UNITY_LIGHTMODEL_AMBIENT * Ambient_Color;
				#endif

				#if _CubeMap_ON
				float4 reflectiveColor = texCUBE(_CubeMap,i.R); 
				RefCol = refelect_Color * reflectiveColor * CUBE_Reflect;
				#endif

				//final Color 颜色
				col = tex2D(_MainTex, i.uv);


				#if _Shadow_ON
				float  atten = LIGHT_ATTENUATION(i);
					#if _LightMap_ON
					col = (col+ Pcol + Dcol )* i.BRDFCol * atten + Acol * atten+RefCol;
					#else
					col = col+  Pcol *atten+ Dcol* atten +Acol *atten +RefCol;
					#endif
					#if _RefMap_ON

					float  Fresnel =max(1,  pow(dot(i.worldNormal ,i.viewDir)+1, Ref_power) );
					fixed4 FresnelCol =tex2D(_RefMap,i.uv);
					col  = fixed4 ( col.rgb *(1- Fresnel) , col.a)+ fixed4 ( Fresnel *FresnelCol.rgb,FresnelCol.a);
					#endif
				#else
					#if _LightMap_ON
					col = (col+ Pcol + Dcol )* i.BRDFCol + Acol +RefCol;
					#else
					col += Pcol + Dcol + Acol +RefCol;

					#endif

					#if _RefMap_ON

					float  Fresnel =max(1,  pow(dot(i.worldNormal ,i.viewDir)+1, Ref_power) );
					fixed4 FresnelCol =tex2D(_RefMap,i.uv);
					col  = fixed4 ( col.rgb *(1- Fresnel) , col.a)+ fixed4 ( Fresnel *FresnelCol.rgb,FresnelCol.a);
					#endif
				#endif
				

		
				return col;
			}
			ENDCG
		}


		Pass
		{
			Tags { "RenderType"="Transparent" "Queue"="Transparent" "LightMode"="ForwardAdd"}//"LightMode"="ForwardBase"
			ZWrite Off Blend One One 
			Cull [_Cull] 
			ColorMask [_ColorWriteMask]
			AlphaTest off
			Fog {Mode off}
			Lighting off
			ZWrite  On 
			// Blend [_Blend] [_Blend2]
			// Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_fwdadd
			#pragma exclude_renderers gles //无视gles
			// #pragma multi_compile _LightLay_LightAuto _LightLay_OnlyDir, _LightLay_OnlyPoint
			#pragma shader_feature _AToggle_ON  
			#pragma shader_feature _DToggle_ON
			#pragma shader_feature _PToggle_ON
			#pragma shader_feature _PBack_ON
			#pragma shader_feature _DBack_ON
			#pragma shader_feature _NormalMap_ON
			#pragma shader_feature _LightMap_ON
			// #pragma shader_feature _AO_ON
			#pragma shader_feature _CubeMap_ON
			#pragma shader_feature _Shadow_ON
			#pragma shader_feature _RefMap_ON

			
			#include "UnityCG.cginc"
			#include "Lighting.cginc" 
			#include "AutoLight.cginc" 
			
			#if _PToggle_ON
			float4 phone_Color;
			float phone_power;
			#endif
			#if _DToggle_ON
			float4 Diffuse_Color;
			#endif
			#if _AToggle_ON
			float4 Ambient_Color;
			#endif
			#if _CubeMap_ON
			float4 refelect_Color;
			float CUBE_Reflect;
			samplerCUBE _CubeMap;
			#endif

			sampler2D _MainTex;
			float4 _MainTex_ST;

			#if _NormalMap_ON
			sampler2D _NormalMap;
			float4  _NormalMap_ST;
			#endif

			#if _LightMap_ON
			sampler2D _LightMap;
			float4  _LightMap_ST;
			#endif

			#if _Shadow_ON
			float4 shadow_Color;
			#endif

			#if _RefMap_ON
			sampler2D _RefMap;
			float Ref_power;
			#endif

			float Diffuse_Power;

			struct v2f
			{
				
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 worldPos:TEXCOORD1 ; 
				float4 worldNormal:TEXCOORD2;
				float3 lightDir :TEXCOORD3;
				float3 viewDir :TEXCOORD4; 

				#if _LightMap_ON
				float4 BRDFCol :TEXCOORD5; 
				#else
				#endif

				#if _CubeMap_ON
				float3 R :TEXCOORD6; 
				#endif
				#if _Shadow_ON
				LIGHTING_COORDS(7,8)
				#endif
			};

			
			v2f vert ( appdata_full  v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				#if _NormalMap_ON
				o.worldNormal = normalize(mul(_Object2World,tex2Dlod(_NormalMap,o.uv)));
				#else
				o.worldNormal  = normalize(mul(_Object2World ,float4(v.normal ,0)));
				#endif

				o.worldPos  = normalize( mul(_Object2World ,v.vertex) );
				o.lightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));
				o.viewDir  =normalize(UnityWorldSpaceViewDir(o.worldPos));

				#if _LightMap_ON
				float difLight = dot (o.worldNormal, o.lightDir);  
				float rimLight = dot (o.worldNormal, o.viewDir); 
				o.BRDFCol = tex2Dlod(_LightMap,float2(difLight,rimLight));
				#else
				#endif

				#if _CubeMap_ON
				//way1
				// o.R = 2 * o.worldNormal.rgb *dot(o.worldNormal.rgb ,o.lightDir) -o.lightDir;
				//way2
				o.R = reflect(o.lightDir,o.worldNormal.rgb);
				#endif

				#if _Shadow_ON
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				#endif

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 Pcol=0;
				fixed4 Dcol =0;
				fixed4 Acol = 0;
				fixed4 RefCol =0;
				fixed4 col = 0;

				//镜面反射
				#if _PToggle_ON
				float H =normalize(i.lightDir + i .viewDir );
				float Pdot = 
					#if _PBack_ON
					max(0,dot(i.worldNormal ,H));
					#else
					dot(i.worldNormal ,H);
					#endif
				Pcol = _LightColor0 * phone_Color* pow(Pdot,phone_power);

				#endif

				//漫反射
				#if _DToggle_ON
				float diffuse =
				#if _DBack_ON
				max(0,dot(i.lightDir,i.worldNormal ));
				#else
				dot(i.lightDir,i.worldNormal );
				#endif
				Dcol = _LightColor0 * Diffuse_Color * diffuse * Diffuse_Power;
				// return  Dcol;
				#endif

				//环境光
				#if _AToggle_ON
				Acol = UNITY_LIGHTMODEL_AMBIENT * Ambient_Color;
				#endif

				#if _CubeMap_ON
				float4 reflectiveColor = texCUBE(_CubeMap,i.R); 
				RefCol = refelect_Color * reflectiveColor * CUBE_Reflect;
				#endif

				//final Color 颜色
				col = tex2D(_MainTex, i.uv);

				#if _Shadow_ON
				float  atten = LIGHT_ATTENUATION(i);
					#if _LightMap_ON
					col = (col+ Pcol + Dcol )* i.BRDFCol * atten + Acol * atten+RefCol;
					#else
					col = col+  Pcol *atten+ Dcol* atten +Acol *atten +RefCol;
					#endif
					#if _RefMap_ON

					float  Fresnel =max(1,  pow(dot(i.worldNormal ,i.viewDir)+1, Ref_power) );
					fixed4 FresnelCol =tex2D(_RefMap,i.uv);
					col  = fixed4 ( col.rgb *(1- Fresnel) , col.a)+ fixed4 ( Fresnel *FresnelCol.rgb,FresnelCol.a);
					#endif
				#else
					#if _LightMap_ON

					col = (col+ Pcol + Dcol )* i.BRDFCol + Acol +RefCol;
					#else
					col += Pcol + Dcol + Acol +RefCol;
					#endif

					#if _RefMap_ON

					float  Fresnel =max(1,  pow(dot(i.worldNormal ,i.viewDir)+1, Ref_power) );
					fixed4 FresnelCol =tex2D(_RefMap,i.uv);
					col  = fixed4 ( col.rgb *(1- Fresnel) , col.a)+ fixed4 ( Fresnel *FresnelCol.rgb,FresnelCol.a);
					#endif
				#endif
				

		
				return col;
			}
			ENDCG
		}
	}

	FallBack  "Diffuse"
	// CustomEditor "SimpleShaderEditor"
}
