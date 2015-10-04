Shader "Kubility/Ramp"{
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RampTex("Ramp",2D) =""{}

	}

// SubShader {
//   Tags { "RenderType"="Opaque" }
//   LOD 200
//   // forward rendering base pass:
//   // 基本的向前渲染通道:
//   Pass {
   
//   	Name "FORWARD"
//   	Tags { "LightMode" = "ForwardBase" }
//   	CGPROGRAM
//    #pragma vertex vert_surf //定义顶点代码
//    #pragma fragment frag_surf //定义判断代码
//    // #pragma multi_compile_fog //定义雾
//    #pragma multi_compile_fwdbase//定义基本向前类型
//    #include "HLSLSupport.cginc"
//    #include "UnityShaderVariables.cginc"
//    #define UNITY_PASS_FORWARDBASE //定义基本向前通道
//    #include "UnityCG.cginc"
//    #include "Lighting.cginc"
//    #include "AutoLight.cginc"
//    #define INTERNAL_DATA
//    #define WorldReflectionVector(data,normal) data.worldRefl
//    #define WorldNormalVector(data,normal) normal

//    //#pragma surface surf Lambert
//    sampler2D _MainTex;
//    fixed4 _Color;

//    struct Input {
//    	float2 uv_MainTex;
//    };



//    struct appdata_input {
//     float4 vertex : POSITION;
//     float4 tangent : TANGENT;
//     float3 normal : NORMAL;
//     float4 texcoord : TEXCOORD0; // 自带贴图
//     float4 texcoord1 : TEXCOORD1; // 光照贴图
//     float4 texcoord2 : TEXCOORD2; // 动态贴图
//     float4 texcoord3 : TEXCOORD3;
//    #if defined(SHADER_API_XBOX360)
//     half4 texcoord4 : TEXCOORD4;
//     half4 texcoord5 : TEXCOORD5;
//    #endif
//     fixed4 color : COLOR;
//    };
//    // vertex-to-fragment interpolation data
//    //如果定义过【关闭光照贴图】，则使用这个结构体
//    #ifdef LIGHTMAP_OFF
//    struct v2f_surf {
//      float4 pos : SV_POSITION;   //【对象坐标】顶点位置
//      float2 pack0 : TEXCOORD0;   //【对象坐标】UV纹理信息
//      half3 worldNormal : TEXCOORD1; //【世界空间】中的法线信息
//      float3 worldPos : TEXCOORD2;  //【世界空间】中的位置
//      #if UNITY_SHOULD_SAMPLE_SH
//      half3 sh : TEXCOORD3; // SH  //【世界空间】中【SH纹理】，没启用【光照贴图】时生效
//      #endif
//      SHADOW_COORDS(4)     //【阴影纹理】float4 _ShadowCoord : TEXCOORD4
//      UNITY_FOG_COORDS(5)    //【雾数据】float fogCoord : TEXCOORD5
//      #if SHADER_TARGET >= 30
//      float4 lmap : TEXCOORD6;   //【对象坐标】光照贴图
//      #endif
//    };
//    #endif
//    //如果没定义过【关闭光照贴图】，则使用这个结构体
//    //好啰嗦。。。尼玛就是【开启光照贴图】，下面直接正常理解
//    #ifndef LIGHTMAP_OFF
//    struct v2f_surf {
//      float4 pos : SV_POSITION;   //【对象坐标】顶点位置
//      float2 pack0 : TEXCOORD0;   //【对象坐标】UV纹理信息
//      half3 worldNormal : TEXCOORD1; //【世界空间】中的法线信息
//      float3 worldPos : TEXCOORD2;  //【世界空间】中的位置
//      float4 lmap : TEXCOORD3;   //【对象坐标】光照贴图
//      SHADOW_COORDS(4)     //【阴影纹理】 float4 _ShadowCoord : TEXCOORD4
//      UNITY_FOG_COORDS(5)    //【雾数据】 float fogCoord : TEXCOORD5
//      //如果【定义过融合侧光】
//      #ifdef DIRLIGHTMAP_COMBINED  
//      fixed3 tSpace0 : TEXCOORD6;
//      fixed3 tSpace1 : TEXCOORD7;
//      fixed3 tSpace2 : TEXCOORD8;
//      #endif
//    };
//    #endif

//     void surf (Input IN, inout SurfaceOutput o) 
//    {
//     fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
//     o.Albedo = o.Normal;
//     o.Alpha = c.a;
//    }

//    float4 _MainTex_ST;
//    v2f_surf vert_surf (appdata_input v) {
//      v2f_surf o;
//      //将给定类型的名称变量初始化为零
//      UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
//      //从模型坐标-世界坐标-视坐标-（视觉平截体乘以投影矩阵并进行透视除法）-剪裁坐标
//      o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//      //获取输入的纹理坐标集，并且使用_MainTex_ST采样图，支持在视检器调节缩放和偏移值
//      o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
//      //将【顶点位置】从【对象坐标】转换到【世界坐标】
//      float3 worldPos = mul(_Object2World, v.vertex).xyz;
//      //将【法线信息】从【对象坐标】转换到【世界坐标】
//      fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
//      //如果【启用光照贴图】 并且 【定义过融合测光】
//      #if !defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_COMBINED)
//      //将顶点【切线方向】从【对象坐标】转换到【世界坐标】
//      fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
//      //副法线 = 【法线】和【切线】叉乘，垂直于N与T
//      fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
//      #endif
//      //如果【启用光照贴图】 并且 【定义过融合测光】
//      #if !defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_COMBINED)
//      o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
//      o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
//      o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
//      #endif
//      //输出【世界坐标】中的顶点位置
//      o.worldPos = worldPos;
//      //输出【世界坐标】中的法线信息
//      o.worldNormal = worldNormal;
//      #ifndef DYNAMICLIGHTMAP_OFF
//      //如果启用【动态贴图】，就计算对应【动态贴图】坐标 （*缩放+偏移）
//      o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
//      #endif
//      #ifndef LIGHTMAP_OFF
//      //如果启用【光照贴图】，就计算对应的【光照贴图】坐标 （*缩放+偏移）
//      o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
//      #endif
//      // SH/ambient and vertex lights
//      // 如果没开启【光照贴图】，顶点光照就是球面调和函数的结果
//      #ifdef LIGHTMAP_OFF
//     #if UNITY_SHOULD_SAMPLE_SH
//       #if UNITY_SAMPLE_FULL_SH_PER_PIXEL
//      o.sh = 0;
//       #elif (SHADER_TARGET < 30)
//      o.sh = ShadeSH9 (float4(worldNormal,1.0));
//       #else
//      o.sh = ShadeSH3Order (half4(worldNormal, 1.0));
//       #endif
//       // Add approximated illumination from non-important point lights
//       // 如果开启【顶点光照】，增加环境光
//       #ifdef VERTEXLIGHT_ON
//      o.sh += Shade4PointLights (
//        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
//        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
//        unity_4LightAtten0, worldPos, worldNormal);
//       #endif
//     #endif
//      #endif // LIGHTMAP_OFF
//      //将顶点从【对象坐标】转到【世界坐标】，再从【世界坐标】转到【阴影坐标】
//      //TRANSFER_SHADOW 封装了 a._ShadowCoord = mul( unity_World2Shadow[0], mul( _Object2World, v.vertex ) );
//      TRANSFER_SHADOW(o);
//         //从顶点程序中输出雾数据，参数o为输出的结构体，o.pos为裁剪空间顶点位置
//      // UNITY_TRANSFER_FOG(o,o.pos);
//      return o;
//    }
//    fixed4 frag_surf (v2f_surf IN) : SV_Target {
//      Input surfIN;
//      //初始化surfIN
//      UNITY_INITIALIZE_OUTPUT(Input, surfIN);
//      //获取模型的UV
//      surfIN.uv_MainTex = IN.pack0.xy;
//      //获取【世界坐标】中的【顶点位置】
//      float3 worldPos = IN.worldPos;
//      #ifndef USING_DIRECTIONAL_LIGHT
//      //如果定义【点光源】，光照方向为【顶点】到【点光源】的方向，归一
//     fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
//      #else
//      //如果定义【平行光】，光照方向为_WorldSpaceLightPos0.xyz
//     fixed3 lightDir = _WorldSpaceLightPos0.xyz;
//      #endif
    
//      #ifdef UNITY_COMPILER_HLSL
//      SurfaceOutput o = (SurfaceOutput)0;
//      #else
//      SurfaceOutput o;
//      #endif
//      o.Albedo = 0.0;
//      o.Emission = 0.0;
//      o.Specular = 0.0;
//      o.Alpha = 0.0;
//      o.Gloss = 0.0;
//      fixed3 normalWorldVertex = fixed3(0,0,1);
//      //SurfaceOutput中的法线，是【世界坐标】中的法线
//      o.Normal = IN.worldNormal;
//      //normalWorldVertex为【世界坐标】中的法线
//      normalWorldVertex = IN.worldNormal;
//      //--------------------------------------------------------------------------
//      //调用表面着色器接口
//      //这里的o就是surf版给我们使用的参数,可以改颜色，法线，光泽度等
//      surf (surfIN, o);
//      //---------------------------------------------------------------------------
//      //包含光，阴影衰减的计算
//      UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
//      //声明输出的颜色
//      fixed4 c = 0;
//      //全局光照
//      UnityGI gi;
//      //初始化gi
//      UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
//      //初始化漫反射色
//      gi.indirect.diffuse = 0;
//      //初始化镜面高光色
//      gi.indirect.specular = 0;
//      //启用【光照贴图】时
//      #if !defined(LIGHTMAP_ON)
//       //gi的【灯光颜色】为【当前灯光颜色】
//       gi.light.color = _LightColor0.rgb;
//       //方向为【光照方向】
//       gi.light.dir = lightDir;
//       //法线和光源方向的点积，取大于0
//       gi.light.ndotl = LambertTerm (o.Normal, gi.light.dir);
//      #endif
//      // Call GI (lightmaps/SH/reflections) lighting function
//      UnityGIInput giInput;
//      //初始化giInput
//      UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
//      //初始化光源
//      giInput.light = gi.light;
//      //初始化当前顶点位置（世界坐标）
//      giInput.worldPos = worldPos;
//      //初始化衰减系数
//      giInput.atten = atten;
//      #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
//       //如果开启光照贴图 并且 开启动态贴图
//     //【光照贴图UV】就来输入时的【光照贴图纹理】
//     giInput.lightmapUV = IN.lmap;
//      #else
//     giInput.lightmapUV = 0.0;
//      #endif
//      #if UNITY_SHOULD_SAMPLE_SH
//     //使用SH时，ambient为SH
//     giInput.ambient = IN.sh;
//      #else
//     giInput.ambient.rgb = 0.0;
//      #endif
//      //HDR，boxMax等几个参数暂时看不下去了，以后有时间再看
//      giInput.probeHDR[0] = unity_SpecCube0_HDR;
//      giInput.probeHDR[1] = unity_SpecCube1_HDR;
//      #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
//     giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
//      #endif
//      #if UNITY_SPECCUBE_BOX_PROJECTION
//     giInput.boxMax[0] = unity_SpecCube0_BoxMax;
//     giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
//     giInput.boxMax[1] = unity_SpecCube1_BoxMax;
//     giInput.boxMin[1] = unity_SpecCube1_BoxMin;
//     giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
//      #endif
//      //Unity全局光照的计算，封装了不少代码
//      //参数：SurfaceOutput,UnityGIInput,输出UnityGI

//      LightingLambert_GI(o, giInput, gi);
//      //实时光照Lambert
//      //就是我们开始学习的漫反射
//      //里面包含类似这样代码...max (0, dot (s.Normal, light.dir))...
//      //参数：SurfaceOutput，UnityGI
//      // o中包含模型颜色，法线等；gi中包含光方向，颜色等
//      c += LightingLambert (o, gi);
//      //同意使用雾，雾数据，颜色
//      UNITY_APPLY_FOG(IN.fogCoord, c);
//      //使用不透明，c.a = 1.0
//      UNITY_OPAQUE_ALPHA(c.a);
//      return c;
//    }
//    // 简单总结：将模型顶点法线等信息转到世界坐标中，与灯光信息进行一系列计算
//    ENDCG
//   }//endpass
//  }//endsubshader


	SubShader
	{
		// No culling or depth
		Tags { "RenderType" ="Transparent" "IgnoreProjector" ="True" "LightMode "="ForwardBase" }

		// ZWrite Off
		Cull Off
		// ZTest Always
		AlphaTest Off
		// Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc" 

			sampler2D _MainTex;
			sampler2D _RampTex;
			float4  _MainTex_ST;
			float4 _RampTex_ST;

			struct v2f
			{
				
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal :TEXCOORD1  ;
				float3 viewDir :TEXCOORD2 ;
				float3 LightDir :TEXCOORD3 ;
	
			};

			v2f vert ( appdata_base  v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.normal =normalize( mul(_Object2World ,float4(v.normal, 0.0) )).xyz ;
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir  =normalize(WorldSpaceViewDir(v.vertex)); //normalize ( _WorldSpaceCameraPos - mul(_Object2World ,v.vertex ) );
				o.LightDir = normalize( -WorldSpaceLightDir(v.vertex ));
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				float4  _MainTex_col = tex2D(_MainTex, i.uv);

				float difflight = dot(i.normal,i.LightDir);
				float ramlight =  dot(i.normal,i.viewDir );
				float fixeddiff = 0.5 * difflight + 0.5;
				float4  _RampTex_col = tex2D(_RampTex,float2 (fixeddiff,ramlight));

				float4  col ;

				col.rgb = UNITY_LIGHTMODEL_AMBIENT + _MainTex_col.rgb*  _RampTex_col.rgb *_LightColor0.rgb ;
				col.a = _MainTex_col.a;

				return col;
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags { "RenderType" ="Transparent" }

		// ZWrite Off
		Cull Off
		// ZTest Always
		AlphaTest Off

		CGPROGRAM
		#pragma surface surf SimpleBRDF
		sampler2D _MainTex;
		sampler2D  _RampTex;

		struct Input
		{
			float2 uv_MainTex;
		} ;

		void surf(Input data,inout SurfaceOutput output )
		{
			float4 col = tex2D(_MainTex,data.uv_MainTex);

			output.Albedo = col.rgb;
			output.Alpha = col.a;

		}

		half4 LightingSimpleBRDF (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
		
			float difflight = max(0, dot(s.Normal,lightDir));
			float ramlight= max(0,dot(s.Normal,viewDir ));
			float fixedDiff = 0.5 * difflight + 0.5;
			// float fixedRam = 0.5 * ramlight + 0.5;

			float4 col = tex2D(_RampTex,float2 (fixedDiff,ramlight));
			float4 finalCol;
			finalCol.rgb =s.Albedo  *_LightColor0.rgb * col.rgb ;
			finalCol.a = s.Alpha;
			return  finalCol;

		}  
		ENDCG
	}
	FallBack "Diffuse"
}

