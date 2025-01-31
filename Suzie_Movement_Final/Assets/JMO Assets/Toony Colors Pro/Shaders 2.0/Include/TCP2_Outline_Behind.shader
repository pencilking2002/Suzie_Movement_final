// Toony Colors Pro+Mobile 2
// (c) 2014,2015 Jean Moreno

Shader "Hidden/Toony Colors Pro 2/Outline Only Behind"
{
	Properties
	{
		//OUTLINE
		_Outline ("Outline Width", Float) = 1
		_OutlineColor ("Outline Color", Color) = (0.2, 0.2, 0.2, 1)
		
		//If taking colors from texture
		_MainTex ("Base (RGB) Gloss (A) ", 2D) = "white" {}
		_TexLod ("Texture Outline LOD", Range(0,10)) = 5
		
		//ZSmooth
		_ZSmooth ("Z Correction", Range(-3.0,3.0)) = -0.5
		
		//Z Offset
		_Offset1 ("Z Offset 1", Float) = 0
		_Offset2 ("Z Offset 2", Float) = 0
		
		//Blending
		_SrcBlendOutline ("#BLEND# Blending Source", Float) = 0
		_DstBlendOutline ("#BLEND# Blending Dest", Float) = 0
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		//Outline Toony Colors 2
		Pass
		{
			Name "OUTLINE"
			
			Cull Off
			ZWrite Off
			Offset [_Offset1],[_Offset2]
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			
			#include "UnityCG.cginc"
			
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile TCP2_NONE TCP2_ZSMOOTH_ON
			#pragma multi_compile TCP2_NONE TCP2_OUTLINE_CONST_SIZE
			#pragma multi_compile TCP2_NONE TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV2_AS_NORMALS
			#pragma multi_compile TCP2_NONE TCP2_OUTLINE_TEXTURED
			
			#pragma target 3.0
			#pragma glsl
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			#if TCP2_OUTLINE_TEXTURED
				float3 texcoord : TEXCOORD0;
			#endif
			#if TCP2_COLORS_AS_NORMALS
				float4 color : COLOR;
			#elif TCP2_TANGENT_AS_NORMALS
				float4 tangent : TANGENT;
			#elif TCP2_UV2_AS_NORMALS
				float2 uv2 : TEXCOORD1;
			#endif
			}; 
			
			struct v2f
			{
				float4 pos : SV_POSITION;
			#if TCP2_OUTLINE_TEXTURED
				float3 texlod : TEXCOORD1;
			#endif
			};
			
			float _Outline;
			float _ZSmooth;
			fixed4 _OutlineColor;
			
		#if TCP2_OUTLINE_TEXTURED
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _TexLod;
		#endif
			
			v2f vert (a2v v)
			{
				v2f o;
				
			//Correct Z artefacts
			#if TCP2_ZSMOOTH_ON
				float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
				
				#ifdef TCP2_COLORS_AS_NORMALS
					//Vertex Color for Normals
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, (v.color.xyz*2) - 1 );
				#elif TCP2_TANGENT_AS_NORMALS
					//Tangent for Normals
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.tangent.xyz);
				#elif TCP2_UV2_AS_NORMALS
					//UV2 for Normals
					float3 normal;
					//unpack uv2
					v.uv2.x = v.uv2.x * 255.0/16.0;
					normal.x = floor(v.uv2.x) / 15.0;
					normal.y = frac(v.uv2.x) * 16.0 / 15.0;
					//get z
					normal.z = v.uv2.y;
					//transform
					normal = mul( (float3x3)UNITY_MATRIX_IT_MV, normal*2-1);
				#else
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
				#endif
				
				normal.z = -_ZSmooth;
				
				#ifdef TCP2_OUTLINE_CONST_SIZE
					//Camera-independent outline size
					float dist = distance(_WorldSpaceCameraPos, mul(_Object2World, v.vertex));
					pos = pos + float4(normalize(normal),0) * _Outline * 0.01 * dist;
				#else
					pos = pos + float4(normalize(normal),0) * _Outline * 0.01;
				#endif
				
			#else
			
				#ifdef TCP2_COLORS_AS_NORMALS
					//Vertex Color for Normals
					float3 normal = (v.color.xyz*2) - 1;
				#elif TCP2_TANGENT_AS_NORMALS
					//Tangent for Normals
					float3 normal = v.tangent.xyz;
				#elif TCP2_UV2_AS_NORMALS
					//UV2 for Normals
					float3 n;
					//unpack uv2
					v.uv2.x = v.uv2.x * 255.0/16.0;
					n.x = floor(v.uv2.x) / 15.0;
					n.y = frac(v.uv2.x) * 16.0 / 15.0;
					//get z
					n.z = v.uv2.y;
					//transform
					n = n*2 - 1;
					float3 normal = n;
				#else
					float3 normal = v.normal;
				#endif
				
				//Camera-independent outline size
				#ifdef TCP2_OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(_Object2World, v.vertex));
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(normal,0) * _Outline * 0.01 * dist);
				#else
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(normal,0) * _Outline * 0.01);
				#endif
			#endif
				
				o.pos = mul(UNITY_MATRIX_P, pos);
				
			#if TCP2_OUTLINE_TEXTURED
				half2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texlod = tex2Dlod(_MainTex, float4(uv, 0, _TexLod)).rgb;
			#endif
				
				return o;
			}
			
			float4 frag (v2f IN) : COLOR
			{
			#if TCP2_OUTLINE_TEXTURED
				return float4(IN.texlod, 1) * _OutlineColor;
			#else
				return _OutlineColor;
			#endif
			}
		ENDCG
		}
		
		//Outline Toony Colors 2 - Blended
		Pass
		{
			Name "OUTLINE_BLENDING"
			
			Cull Off
			ZWrite Off
			Offset [_Offset1],[_Offset2]
			Tags { "LightMode"="ForwardBase" "Queue"="Transparent" "IgnoreProjectors"="True" "RenderType"="Transparent" }
			Blend [_SrcBlendOutline] [_DstBlendOutline]
			
			CGPROGRAM
			
			#include "UnityCG.cginc"
			
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile TCP2_NONE TCP2_ZSMOOTH_ON
			#pragma multi_compile TCP2_NONE TCP2_OUTLINE_CONST_SIZE
			#pragma multi_compile TCP2_NONE TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV2_AS_NORMALS
			#pragma multi_compile TCP2_NONE TCP2_OUTLINE_TEXTURED
			
			#pragma target 3.0
			#pragma glsl
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			#if TCP2_OUTLINE_TEXTURED
				float3 texcoord : TEXCOORD0;
			#endif
			#if TCP2_COLORS_AS_NORMALS
				float4 color : COLOR;
			#elif TCP2_TANGENT_AS_NORMALS
				float4 tangent : TANGENT;
			#elif TCP2_UV2_AS_NORMALS
				float2 uv2 : TEXCOORD1;
			#endif
			}; 
			
			struct v2f
			{
				float4 pos : SV_POSITION;
			#if TCP2_OUTLINE_TEXTURED
				float3 texlod : TEXCOORD1;
			#endif
			};
			
			float _Outline;
			float _ZSmooth;
			fixed4 _OutlineColor;
			
		#if TCP2_OUTLINE_TEXTURED
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _TexLod;
		#endif
			
			v2f vert (a2v v)
			{
				v2f o;
				
			//Correct Z artefacts
			#if TCP2_ZSMOOTH_ON
				float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
				
				#ifdef TCP2_COLORS_AS_NORMALS
					//Vertex Color for Normals
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, (v.color.xyz*2) - 1 );
				#elif TCP2_TANGENT_AS_NORMALS
					//Tangent for Normals
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.tangent.xyz);
				#elif TCP2_UV2_AS_NORMALS
					//UV2 for Normals
					float3 normal;
					//unpack uv2
					v.uv2.x = v.uv2.x * 255.0/16.0;
					normal.x = floor(v.uv2.x) / 15.0;
					normal.y = frac(v.uv2.x) * 16.0 / 15.0;
					//get z
					normal.z = v.uv2.y;
					//transform
					normal = mul( (float3x3)UNITY_MATRIX_IT_MV, normal*2-1);
				#else
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
				#endif
				
				normal.z = -_ZSmooth;
				
				#ifdef TCP2_OUTLINE_CONST_SIZE
					//Camera-independent outline size
					float dist = distance(_WorldSpaceCameraPos, mul(_Object2World, v.vertex));
					pos = pos + float4(normalize(normal),0) * _Outline * 0.01 * dist;
				#else
					pos = pos + float4(normalize(normal),0) * _Outline * 0.01;
				#endif
				
			#else
			
				#ifdef TCP2_COLORS_AS_NORMALS
					//Vertex Color for Normals
					float3 normal = (v.color.xyz*2) - 1;
				#elif TCP2_TANGENT_AS_NORMALS
					//Tangent for Normals
					float3 normal = v.tangent.xyz;
				#elif TCP2_UV2_AS_NORMALS
					//UV2 for Normals
					float3 n;
					//unpack uv2
					v.uv2.x = v.uv2.x * 255.0/16.0;
					n.x = floor(v.uv2.x) / 15.0;
					n.y = frac(v.uv2.x) * 16.0 / 15.0;
					//get z
					n.z = v.uv2.y;
					//transform
					n = n*2 - 1;
					float3 normal = n;
				#else
					float3 normal = v.normal;
				#endif
				
				//Camera-independent outline size
				#ifdef TCP2_OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(_Object2World, v.vertex));
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(normal,0) * _Outline * 0.01 * dist);
				#else
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(normal,0) * _Outline * 0.01);
				#endif
			#endif
				
				o.pos = mul(UNITY_MATRIX_P, pos);
				
			#if TCP2_OUTLINE_TEXTURED
				half2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texlod = tex2Dlod(_MainTex, float4(uv, 0, _TexLod)).rgb;
			#endif
				
				return o;
			}
			
			float4 frag (v2f IN) : COLOR
			{
			#if TCP2_OUTLINE_TEXTURED
				return float4(IN.texlod, 1) * _OutlineColor;
			#else
				return _OutlineColor;
			#endif
			}
		ENDCG
		}
	}
}
