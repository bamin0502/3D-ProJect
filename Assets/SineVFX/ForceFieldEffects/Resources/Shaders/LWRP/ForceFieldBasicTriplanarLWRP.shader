// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SineVFX/ForceFieldAndShieldEffects/ForceFieldBasicTriplanarLWRP"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin]_FinalPower("Final Power", Range( 0 , 20)) = 4
		_FinalPowerAdjust("Final Power Adjust", Range( -1 , 1)) = -1
		_OpacityPower("Opacity Power", Range( 0 , 4)) = 1
		[Toggle(_LOCALNOISEPOSITION_ON)] _LocalNoisePosition("Local Noise Position", Float) = 0
		_NormalVertexOffset("Normal Vertex Offset", Float) = 0
		[Toggle(_CUBEMAPREFLECTIONENABLED_ON)] _CubemapReflectionEnabled("Cubemap Reflection Enabled", Float) = 0
		_CubemapReflection("Cubemap Reflection", CUBE) = "white" {}
		_Ramp("Ramp", 2D) = "white" {}
		_RampColorTint("Ramp Color Tint", Color) = (1,1,1,1)
		_RampMultiplyTiling("Ramp Multiply Tiling", Float) = 1
		[Toggle(_RAMPFLIP_ON)] _RampFlip("Ramp Flip", Float) = 0
		_MaskFresnelExp("Mask Fresnel Exp", Range( 0.2 , 8)) = 4
		[Toggle(_MASKDEPTHFADEENABLED_ON)] _MaskDepthFadeEnabled("Mask Depth Fade Enabled", Float) = 1
		_MaskDepthFadeDistance("Mask Depth Fade Distance", Float) = 0.25
		_MaskDepthFadeExp("Mask Depth Fade Exp", Range( 0.2 , 10)) = 4
		_NoiseMaskPower("Noise Mask Power", Range( 0 , 10)) = 1
		_NoiseMaskAdd("Noise Mask Add", Range( 0 , 1)) = 0.25
		_Noise01("Noise 01", 2D) = "white" {}
		_Noise01Tiling("Noise 01 Tiling", Float) = 1
		_Noise01ScrollSpeed("Noise 01 Scroll Speed", Float) = 0.25
		[Toggle(_NOISE02ENABLED_ON)] _Noise02Enabled("Noise 02 Enabled", Float) = 0
		_Noise02("Noise 02", 2D) = "white" {}
		_Noise02Tiling("Noise 02 Tiling", Float) = 1
		_Noise02ScrollSpeed("Noise 02 Scroll Speed", Float) = 0.25
		[Toggle(_NOISEDISTORTIONENABLED_ON)] _NoiseDistortionEnabled("Noise Distortion Enabled", Float) = 1
		_NoiseDistortion("Noise Distortion", 2D) = "white" {}
		_NoiseDistortionPower("Noise Distortion Power", Range( 0 , 2)) = 0.5
		_NoiseDistortionTiling("Noise Distortion Tiling", Float) = 0.5
		_MaskAppearLocalYRamap("Mask Appear Local Y Ramap", Float) = 0.5
		_MaskAppearLocalYAdd("Mask Appear Local Y Add", Float) = 0
		[Toggle(_MASKAPPEARINVERT_ON)] _MaskAppearInvert("Mask Appear Invert", Float) = 0
		_MaskAppearProgress("Mask Appear Progress", Range( -2 , 2)) = 0
		_MaskAppearNoise("Mask Appear Noise", 2D) = "white" {}
		_MaskAppearRamp("Mask Appear Ramp", 2D) = "white" {}
		[Toggle(_MASKAPPEARNOISETRIPLANAR_ON)] _MaskAppearNoiseTriplanar("Mask Appear Noise Triplanar", Float) = 0
		_MaskAppearNoiseTriplanarTiling("Mask Appear Noise Triplanar Tiling", Float) = 0.2
		[Toggle(_MASKAPPEARUSEWORLDPOSITION_ON)] _MaskAppearUseWorldPosition("Mask Appear Use World Position", Float) = 0
		_HitWaveNoiseNegate("Hit Wave Noise Negate", Range( 0 , 1)) = 1
		_HitWaveLength("Hit Wave Length", Float) = 0.5
		_HitWaveFadeDistance("Hit Wave Fade Distance", Float) = 6
		_HitWaveFadeDistancePower("Hit Wave Fade Distance Power", Float) = 1
		_HitWaveRampMask("Hit Wave Ramp Mask", 2D) = "white" {}
		[ASEEnd]_HitWaveDistortionPower("Hit Wave Distortion Power", Float) = 0

		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Off
		AlphaToMask Off
		
		HLSLINCLUDE
		#pragma target 3.0

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 100700
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature _RAMPFLIP_ON
			#pragma shader_feature _MASKDEPTHFADEENABLED_ON
			#pragma shader_feature _LOCALNOISEPOSITION_ON
			#pragma shader_feature _NOISEDISTORTIONENABLED_ON
			#pragma shader_feature _NOISE02ENABLED_ON
			#pragma shader_feature _MASKAPPEARINVERT_ON
			#pragma shader_feature _MASKAPPEARUSEWORLDPOSITION_ON
			#pragma shader_feature _MASKAPPEARNOISETRIPLANAR_ON
			#pragma shader_feature _CUBEMAPREFLECTIONENABLED_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RampColorTint;
			float4 _MaskAppearNoise_ST;
			float _NormalVertexOffset;
			float _HitWaveNoiseNegate;
			float _HitWaveLength;
			float _HitWaveFadeDistancePower;
			float _HitWaveFadeDistance;
			float _HitWaveDistortionPower;
			float _MaskAppearNoiseTriplanarTiling;
			float _MaskAppearProgress;
			float _MaskAppearLocalYAdd;
			float _MaskAppearLocalYRamap;
			float _NoiseMaskAdd;
			float _NoiseMaskPower;
			float _Noise02ScrollSpeed;
			float _Noise02Tiling;
			float _NoiseDistortionPower;
			float _NoiseDistortionTiling;
			float _Noise01ScrollSpeed;
			float _Noise01Tiling;
			float _MaskDepthFadeExp;
			float _MaskDepthFadeDistance;
			float _MaskFresnelExp;
			float _RampMultiplyTiling;
			float _FinalPower;
			float _FinalPowerAdjust;
			float _OpacityPower;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Ramp;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _Noise01;
			sampler2D _NoiseDistortion;
			sampler2D _Noise02;
			sampler2D _MaskAppearRamp;
			sampler2D _MaskAppearNoise;
			float4 _ControlParticlePosition[20];
			float _ControlParticleSize[20];
			sampler2D _HitWaveRampMask;
			int _AffectorCount;
			float _PSLossyScale;
			samplerCUBE _CubemapReflection;


			inline float4 TriplanarSampling34( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
			{
				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
				float3 nsign = sign( worldNormal );
				half4 xNorm; half4 yNorm; half4 zNorm;
				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
			}
			
			inline float4 TriplanarSampling303( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
			{
				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
				float3 nsign = sign( worldNormal );
				half4 xNorm; half4 yNorm; half4 zNorm;
				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
			}
			
			float ArrayCE374( float4 PositionCE, float SizeCE, float3 WorldPosCE, sampler2D HitWaveRampMaskCE, float HtWaveDistortionPowerCE, int AffectorCountCE, float FDCE, float FDPCE, float WLCE )
			{
				float MyResult = 0;
				float DistanceMask45;
				for (int i = 0; i < AffectorCountCE; i++){
				DistanceMask45 = distance( WorldPosCE , _ControlParticlePosition[i] );
				float myTemp01 = (1 - frac(clamp(((1 - _ControlParticleSize[i] - 1 + DistanceMask45 + HtWaveDistortionPowerCE) * WLCE), -1.0, 0.0)));
				float2 myTempUV01 = float2(myTemp01, 0.0);
				float myClampResult01 = clamp( (0.0 + (( -DistanceMask45 + FDCE ) - 0.0) * (FDPCE - 0.0) / (FDCE - 0.0)) , 0.0 , 1.0 );
				MyResult += (myClampResult01 * tex2D(HitWaveRampMaskCE,myTempUV01).r);
				}
				MyResult = clamp(MyResult, 0.0, 1.0);
				return MyResult;
			}
			
			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				
				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord3.xyz = ase_worldTangent;
				o.ase_texcoord4.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord5.xyz = ase_worldBitangent;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord6 = screenPos;
				
				o.ase_texcoord7 = v.vertex;
				o.ase_texcoord8.xyz = v.ase_texcoord.xyz;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;
				o.ase_texcoord8.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( ase_worldNormal * _NormalVertexOffset );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				#ifdef ASE_FOG
				o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN , FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 switchResult27 = (((ase_vface>0)?(float3(0,0,1)):(float3(0,0,-1))));
				float3 ase_worldTangent = IN.ase_texcoord3.xyz;
				float3 ase_worldNormal = IN.ase_texcoord4.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord5.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal8 = switchResult27;
				float3 worldNormal8 = float3(dot(tanToWorld0,tanNormal8), dot(tanToWorld1,tanNormal8), dot(tanToWorld2,tanNormal8));
				float dotResult1 = dot( ase_worldViewDir , worldNormal8 );
				float4 screenPos = IN.ase_texcoord6;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth11 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth11 = abs( ( screenDepth11 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _MaskDepthFadeDistance ) );
				float clampResult19 = clamp( ( 1.0 - distanceDepth11 ) , 0.0 , 1.0 );
				#ifdef _MASKDEPTHFADEENABLED_ON
				float staticSwitch330 = pow( clampResult19 , _MaskDepthFadeExp );
				#else
				float staticSwitch330 = 0.0;
				#endif
				float clampResult17 = clamp( max( pow( ( 1.0 - dotResult1 ) , _MaskFresnelExp ) , staticSwitch330 ) , 0.0 , 1.0 );
				float3 temp_output_57_0 = abs( ase_worldNormal );
				float3 temp_output_60_0 = ( temp_output_57_0 * temp_output_57_0 );
				float4 transform106 = mul(GetObjectToWorldMatrix(),float4(0,0,0,1));
				float3 appendResult101 = (float3(transform106.x , transform106.y , transform106.z));
				#ifdef _LOCALNOISEPOSITION_ON
				float3 staticSwitch103 = ( WorldPosition - appendResult101 );
				#else
				float3 staticSwitch103 = WorldPosition;
				#endif
				float3 FinalWorldPosition102 = staticSwitch103;
				float4 temp_cast_1 = (0.0).xxxx;
				float4 triplanar34 = TriplanarSampling34( _NoiseDistortion, WorldPosition, ase_worldNormal, 1.0, _NoiseDistortionTiling, 1.0, 0 );
				#ifdef _NOISEDISTORTIONENABLED_ON
				float4 staticSwitch42 = ( triplanar34 * _NoiseDistortionPower );
				#else
				float4 staticSwitch42 = temp_cast_1;
				#endif
				float4 break58 = ( float4( ( FinalWorldPosition102 * _Noise01Tiling ) , 0.0 ) + ( ( _TimeParameters.x ) * _Noise01ScrollSpeed ) + staticSwitch42 );
				float2 appendResult64 = (float2(break58.y , break58.z));
				float2 appendResult65 = (float2(break58.z , break58.x));
				float2 appendResult67 = (float2(break58.x , break58.y));
				float3 weightedBlendVar73 = temp_output_60_0;
				float weightedBlend73 = ( weightedBlendVar73.x*tex2D( _Noise01, appendResult64 ).r + weightedBlendVar73.y*tex2D( _Noise01, appendResult65 ).r + weightedBlendVar73.z*tex2D( _Noise01, appendResult67 ).r );
				float4 break51 = ( float4( ( FinalWorldPosition102 * _Noise02Tiling ) , 0.0 ) + ( ( _TimeParameters.x ) * _Noise02ScrollSpeed ) + staticSwitch42 );
				float2 appendResult59 = (float2(break51.y , break51.z));
				float2 appendResult56 = (float2(break51.z , break51.x));
				float2 appendResult54 = (float2(break51.x , break51.y));
				float3 weightedBlendVar69 = temp_output_60_0;
				float weightedBlend69 = ( weightedBlendVar69.x*tex2D( _Noise02, appendResult59 ).r + weightedBlendVar69.y*tex2D( _Noise02, appendResult56 ).r + weightedBlendVar69.z*tex2D( _Noise02, appendResult54 ).r );
				#ifdef _NOISE02ENABLED_ON
				float staticSwitch75 = weightedBlend69;
				#else
				float staticSwitch75 = 1.0;
				#endif
				float ResultNoise77 = ( weightedBlend73 * staticSwitch75 * _NoiseMaskPower );
				float clampResult80 = clamp( ( ( clampResult17 * ResultNoise77 ) + ( clampResult17 * _NoiseMaskAdd ) ) , 0.0 , 1.0 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				#ifdef _MASKAPPEARUSEWORLDPOSITION_ON
				float staticSwitch385 = ( FinalWorldPosition102.y / ase_objectScale.y );
				#else
				float staticSwitch385 = IN.ase_texcoord7.xyz.y;
				#endif
				float temp_output_109_0 = abs( ( (0.0 + (staticSwitch385 - 0.0) * (1.0 - 0.0) / (_MaskAppearLocalYRamap - 0.0)) + _MaskAppearLocalYAdd ) );
				#ifdef _MASKAPPEARINVERT_ON
				float staticSwitch118 = ( 1.0 - temp_output_109_0 );
				#else
				float staticSwitch118 = temp_output_109_0;
				#endif
				float2 uv_MaskAppearNoise = IN.ase_texcoord8.xyz.xy * _MaskAppearNoise_ST.xy + _MaskAppearNoise_ST.zw;
				float4 triplanar303 = TriplanarSampling303( _MaskAppearNoise, WorldPosition, ase_worldNormal, 1.0, _MaskAppearNoiseTriplanarTiling, 1.0, 0 );
				#ifdef _MASKAPPEARNOISETRIPLANAR_ON
				float staticSwitch304 = triplanar303.x;
				#else
				float staticSwitch304 = tex2D( _MaskAppearNoise, uv_MaskAppearNoise ).r;
				#endif
				float2 appendResult145 = (float2(( staticSwitch118 + _MaskAppearProgress + -staticSwitch304 ) , 0.0));
				float4 tex2DNode147 = tex2D( _MaskAppearRamp, appendResult145 );
				float MaskAppearValue119 = tex2DNode147.g;
				float MaskAppearEdges135 = tex2DNode147.r;
				float4 PositionCE374 = _ControlParticlePosition[0];
				float SizeCE374 = _ControlParticleSize[0];
				float3 WorldPosCE374 = WorldPosition;
				sampler2D HitWaveRampMaskCE374 = _HitWaveRampMask;
				float DistortionForHits368 = triplanar34.r;
				float HtWaveDistortionPowerCE374 = ( DistortionForHits368 * _HitWaveDistortionPower );
				int AffectorCountCE374 = _AffectorCount;
				float FD262 = ( _PSLossyScale * _HitWaveFadeDistance );
				float FDCE374 = FD262;
				float FDP319 = _HitWaveFadeDistancePower;
				float FDPCE374 = FDP319;
				float WL160 = ( _HitWaveLength / _PSLossyScale );
				float WLCE374 = WL160;
				float localArrayCE374 = ArrayCE374( PositionCE374 , SizeCE374 , WorldPosCE374 , HitWaveRampMaskCE374 , HtWaveDistortionPowerCE374 , AffectorCountCE374 , FDCE374 , FDPCE374 , WLCE374 );
				float HWArrayResult342 = localArrayCE374;
				float clampResult329 = clamp( ( ResultNoise77 + _HitWaveNoiseNegate ) , 0.0 , 1.0 );
				float clampResult139 = clamp( ( ( clampResult80 * MaskAppearValue119 ) + MaskAppearEdges135 + ( HWArrayResult342 * clampResult329 * MaskAppearValue119 ) ) , 0.0 , 1.0 );
				float ResultOpacity93 = clampResult139;
				float clampResult84 = clamp( ( _RampMultiplyTiling * ResultOpacity93 ) , 0.0 , 1.0 );
				#ifdef _RAMPFLIP_ON
				float staticSwitch87 = ( 1.0 - clampResult84 );
				#else
				float staticSwitch87 = clampResult84;
				#endif
				float2 appendResult88 = (float2(staticSwitch87 , 0.0));
				float clampResult367 = clamp( ( ResultOpacity93 - _FinalPowerAdjust ) , 0.0 , 1.0 );
				float4 temp_cast_3 = (0.0).xxxx;
				float3 ase_worldReflection = reflect(-ase_worldViewDir, ase_worldNormal);
				float3 normalizedWorldRefl = normalize( ase_worldReflection );
				#ifdef _CUBEMAPREFLECTIONENABLED_ON
				float4 staticSwitch336 = texCUBE( _CubemapReflection, normalizedWorldRefl );
				#else
				float4 staticSwitch336 = temp_cast_3;
				#endif
				
				float clampResult362 = clamp( ( ResultOpacity93 * _OpacityPower ) , 0.0 , 1.0 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( ( _RampColorTint * _FinalPower * tex2D( _Ramp, appendResult88 ) * clampResult367 ) + staticSwitch336 ).rgb;
				float Alpha = clampResult362;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 100700
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature _MASKDEPTHFADEENABLED_ON
			#pragma shader_feature _LOCALNOISEPOSITION_ON
			#pragma shader_feature _NOISEDISTORTIONENABLED_ON
			#pragma shader_feature _NOISE02ENABLED_ON
			#pragma shader_feature _MASKAPPEARINVERT_ON
			#pragma shader_feature _MASKAPPEARUSEWORLDPOSITION_ON
			#pragma shader_feature _MASKAPPEARNOISETRIPLANAR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RampColorTint;
			float4 _MaskAppearNoise_ST;
			float _NormalVertexOffset;
			float _HitWaveNoiseNegate;
			float _HitWaveLength;
			float _HitWaveFadeDistancePower;
			float _HitWaveFadeDistance;
			float _HitWaveDistortionPower;
			float _MaskAppearNoiseTriplanarTiling;
			float _MaskAppearProgress;
			float _MaskAppearLocalYAdd;
			float _MaskAppearLocalYRamap;
			float _NoiseMaskAdd;
			float _NoiseMaskPower;
			float _Noise02ScrollSpeed;
			float _Noise02Tiling;
			float _NoiseDistortionPower;
			float _NoiseDistortionTiling;
			float _Noise01ScrollSpeed;
			float _Noise01Tiling;
			float _MaskDepthFadeExp;
			float _MaskDepthFadeDistance;
			float _MaskFresnelExp;
			float _RampMultiplyTiling;
			float _FinalPower;
			float _FinalPowerAdjust;
			float _OpacityPower;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _Noise01;
			sampler2D _NoiseDistortion;
			sampler2D _Noise02;
			sampler2D _MaskAppearRamp;
			sampler2D _MaskAppearNoise;
			float4 _ControlParticlePosition[20];
			float _ControlParticleSize[20];
			sampler2D _HitWaveRampMask;
			int _AffectorCount;
			float _PSLossyScale;


			inline float4 TriplanarSampling34( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
			{
				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
				float3 nsign = sign( worldNormal );
				half4 xNorm; half4 yNorm; half4 zNorm;
				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
			}
			
			inline float4 TriplanarSampling303( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
			{
				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
				float3 nsign = sign( worldNormal );
				half4 xNorm; half4 yNorm; half4 zNorm;
				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
			}
			
			float ArrayCE374( float4 PositionCE, float SizeCE, float3 WorldPosCE, sampler2D HitWaveRampMaskCE, float HtWaveDistortionPowerCE, int AffectorCountCE, float FDCE, float FDPCE, float WLCE )
			{
				float MyResult = 0;
				float DistanceMask45;
				for (int i = 0; i < AffectorCountCE; i++){
				DistanceMask45 = distance( WorldPosCE , _ControlParticlePosition[i] );
				float myTemp01 = (1 - frac(clamp(((1 - _ControlParticleSize[i] - 1 + DistanceMask45 + HtWaveDistortionPowerCE) * WLCE), -1.0, 0.0)));
				float2 myTempUV01 = float2(myTemp01, 0.0);
				float myClampResult01 = clamp( (0.0 + (( -DistanceMask45 + FDCE ) - 0.0) * (FDPCE - 0.0) / (FDCE - 0.0)) , 0.0 , 1.0 );
				MyResult += (myClampResult01 * tex2D(HitWaveRampMaskCE,myTempUV01).r);
				}
				MyResult = clamp(MyResult, 0.0, 1.0);
				return MyResult;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				
				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord2.xyz = ase_worldTangent;
				o.ase_texcoord3.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord4.xyz = ase_worldBitangent;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord5 = screenPos;
				
				o.ase_texcoord6 = v.vertex;
				o.ase_texcoord7.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( ase_worldNormal * _NormalVertexOffset );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN , FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 switchResult27 = (((ase_vface>0)?(float3(0,0,1)):(float3(0,0,-1))));
				float3 ase_worldTangent = IN.ase_texcoord2.xyz;
				float3 ase_worldNormal = IN.ase_texcoord3.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord4.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal8 = switchResult27;
				float3 worldNormal8 = float3(dot(tanToWorld0,tanNormal8), dot(tanToWorld1,tanNormal8), dot(tanToWorld2,tanNormal8));
				float dotResult1 = dot( ase_worldViewDir , worldNormal8 );
				float4 screenPos = IN.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth11 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth11 = abs( ( screenDepth11 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _MaskDepthFadeDistance ) );
				float clampResult19 = clamp( ( 1.0 - distanceDepth11 ) , 0.0 , 1.0 );
				#ifdef _MASKDEPTHFADEENABLED_ON
				float staticSwitch330 = pow( clampResult19 , _MaskDepthFadeExp );
				#else
				float staticSwitch330 = 0.0;
				#endif
				float clampResult17 = clamp( max( pow( ( 1.0 - dotResult1 ) , _MaskFresnelExp ) , staticSwitch330 ) , 0.0 , 1.0 );
				float3 temp_output_57_0 = abs( ase_worldNormal );
				float3 temp_output_60_0 = ( temp_output_57_0 * temp_output_57_0 );
				float4 transform106 = mul(GetObjectToWorldMatrix(),float4(0,0,0,1));
				float3 appendResult101 = (float3(transform106.x , transform106.y , transform106.z));
				#ifdef _LOCALNOISEPOSITION_ON
				float3 staticSwitch103 = ( WorldPosition - appendResult101 );
				#else
				float3 staticSwitch103 = WorldPosition;
				#endif
				float3 FinalWorldPosition102 = staticSwitch103;
				float4 temp_cast_1 = (0.0).xxxx;
				float4 triplanar34 = TriplanarSampling34( _NoiseDistortion, WorldPosition, ase_worldNormal, 1.0, _NoiseDistortionTiling, 1.0, 0 );
				#ifdef _NOISEDISTORTIONENABLED_ON
				float4 staticSwitch42 = ( triplanar34 * _NoiseDistortionPower );
				#else
				float4 staticSwitch42 = temp_cast_1;
				#endif
				float4 break58 = ( float4( ( FinalWorldPosition102 * _Noise01Tiling ) , 0.0 ) + ( ( _TimeParameters.x ) * _Noise01ScrollSpeed ) + staticSwitch42 );
				float2 appendResult64 = (float2(break58.y , break58.z));
				float2 appendResult65 = (float2(break58.z , break58.x));
				float2 appendResult67 = (float2(break58.x , break58.y));
				float3 weightedBlendVar73 = temp_output_60_0;
				float weightedBlend73 = ( weightedBlendVar73.x*tex2D( _Noise01, appendResult64 ).r + weightedBlendVar73.y*tex2D( _Noise01, appendResult65 ).r + weightedBlendVar73.z*tex2D( _Noise01, appendResult67 ).r );
				float4 break51 = ( float4( ( FinalWorldPosition102 * _Noise02Tiling ) , 0.0 ) + ( ( _TimeParameters.x ) * _Noise02ScrollSpeed ) + staticSwitch42 );
				float2 appendResult59 = (float2(break51.y , break51.z));
				float2 appendResult56 = (float2(break51.z , break51.x));
				float2 appendResult54 = (float2(break51.x , break51.y));
				float3 weightedBlendVar69 = temp_output_60_0;
				float weightedBlend69 = ( weightedBlendVar69.x*tex2D( _Noise02, appendResult59 ).r + weightedBlendVar69.y*tex2D( _Noise02, appendResult56 ).r + weightedBlendVar69.z*tex2D( _Noise02, appendResult54 ).r );
				#ifdef _NOISE02ENABLED_ON
				float staticSwitch75 = weightedBlend69;
				#else
				float staticSwitch75 = 1.0;
				#endif
				float ResultNoise77 = ( weightedBlend73 * staticSwitch75 * _NoiseMaskPower );
				float clampResult80 = clamp( ( ( clampResult17 * ResultNoise77 ) + ( clampResult17 * _NoiseMaskAdd ) ) , 0.0 , 1.0 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				#ifdef _MASKAPPEARUSEWORLDPOSITION_ON
				float staticSwitch385 = ( FinalWorldPosition102.y / ase_objectScale.y );
				#else
				float staticSwitch385 = IN.ase_texcoord6.xyz.y;
				#endif
				float temp_output_109_0 = abs( ( (0.0 + (staticSwitch385 - 0.0) * (1.0 - 0.0) / (_MaskAppearLocalYRamap - 0.0)) + _MaskAppearLocalYAdd ) );
				#ifdef _MASKAPPEARINVERT_ON
				float staticSwitch118 = ( 1.0 - temp_output_109_0 );
				#else
				float staticSwitch118 = temp_output_109_0;
				#endif
				float2 uv_MaskAppearNoise = IN.ase_texcoord7.xy * _MaskAppearNoise_ST.xy + _MaskAppearNoise_ST.zw;
				float4 triplanar303 = TriplanarSampling303( _MaskAppearNoise, WorldPosition, ase_worldNormal, 1.0, _MaskAppearNoiseTriplanarTiling, 1.0, 0 );
				#ifdef _MASKAPPEARNOISETRIPLANAR_ON
				float staticSwitch304 = triplanar303.x;
				#else
				float staticSwitch304 = tex2D( _MaskAppearNoise, uv_MaskAppearNoise ).r;
				#endif
				float2 appendResult145 = (float2(( staticSwitch118 + _MaskAppearProgress + -staticSwitch304 ) , 0.0));
				float4 tex2DNode147 = tex2D( _MaskAppearRamp, appendResult145 );
				float MaskAppearValue119 = tex2DNode147.g;
				float MaskAppearEdges135 = tex2DNode147.r;
				float4 PositionCE374 = _ControlParticlePosition[0];
				float SizeCE374 = _ControlParticleSize[0];
				float3 WorldPosCE374 = WorldPosition;
				sampler2D HitWaveRampMaskCE374 = _HitWaveRampMask;
				float DistortionForHits368 = triplanar34.r;
				float HtWaveDistortionPowerCE374 = ( DistortionForHits368 * _HitWaveDistortionPower );
				int AffectorCountCE374 = _AffectorCount;
				float FD262 = ( _PSLossyScale * _HitWaveFadeDistance );
				float FDCE374 = FD262;
				float FDP319 = _HitWaveFadeDistancePower;
				float FDPCE374 = FDP319;
				float WL160 = ( _HitWaveLength / _PSLossyScale );
				float WLCE374 = WL160;
				float localArrayCE374 = ArrayCE374( PositionCE374 , SizeCE374 , WorldPosCE374 , HitWaveRampMaskCE374 , HtWaveDistortionPowerCE374 , AffectorCountCE374 , FDCE374 , FDPCE374 , WLCE374 );
				float HWArrayResult342 = localArrayCE374;
				float clampResult329 = clamp( ( ResultNoise77 + _HitWaveNoiseNegate ) , 0.0 , 1.0 );
				float clampResult139 = clamp( ( ( clampResult80 * MaskAppearValue119 ) + MaskAppearEdges135 + ( HWArrayResult342 * clampResult329 * MaskAppearValue119 ) ) , 0.0 , 1.0 );
				float ResultOpacity93 = clampResult139;
				float clampResult362 = clamp( ( ResultOpacity93 * _OpacityPower ) , 0.0 , 1.0 );
				
				float Alpha = clampResult362;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18934
1920;0;1920;1019;307.5778;1104.261;1.267308;True;False
Node;AmplifyShaderEditor.Vector4Node;107;-3508.097,752.5167;Float;False;Constant;_Vector3;Vector 3;22;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;106;-3321.187,753.8212;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;46;-3313.212,610.1014;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;101;-3112.062,755.5037;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;97;-2756.619,722.4866;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-4033.885,2002.944;Float;False;Property;_NoiseDistortionTiling;Noise Distortion Tiling;27;0;Create;True;0;0;0;False;0;False;0.5;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;31;-4018.065,1810.617;Float;True;Property;_NoiseDistortion;Noise Distortion;25;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.StaticSwitch;103;-2550.102,531.3108;Float;False;Property;_LocalNoisePosition;Local Noise Position;3;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-2248.758,531.0774;Float;False;FinalWorldPosition;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TriplanarNode;34;-3680.958,1859.948;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;33;-3596.479,2054.2;Float;False;Property;_NoiseDistortionPower;Noise Distortion Power;26;0;Create;True;0;0;0;False;0;False;0.5;0.25;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;35;-3122.899,2519.329;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;37;-3123.333,2422.892;Float;False;Property;_Noise02Tiling;Noise 02 Tiling;22;0;Create;True;0;0;0;False;0;False;1;0.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-3266.684,2065.509;Float;False;Constant;_Float3;Float 3;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;384;61.5184,2140.185;Inherit;False;102;FinalWorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;-3183.482,2319.404;Inherit;False;102;FinalWorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-3265.479,1963.201;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-3150.659,2663.93;Float;False;Property;_Noise02ScrollSpeed;Noise 02 Scroll Speed;23;0;Create;True;0;0;0;False;0;False;0.25;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;42;-3049.585,2001.81;Float;False;Property;_NoiseDistortionEnabled;Noise Distortion Enabled;24;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-3161.501,1493.246;Float;False;Property;_Noise01Tiling;Noise 01 Tiling;18;0;Create;True;0;0;0;False;0;False;1;0.125;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-2804.856,2576.829;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;388;297.5186,2130.184;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-2808.808,2335.084;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-3185.195,1736.679;Float;False;Property;_Noise01ScrollSpeed;Noise 01 Scroll Speed;19;0;Create;True;0;0;0;False;0;False;0.25;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;45;-3161.068,1587.082;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;104;-3220.016,1383.81;Inherit;False;102;FinalWorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ObjectScaleNode;386;310.5185,2356.182;Inherit;False;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;108;768.5477,2061.078;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-2846.979,1402.838;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;389;635.1165,2237.668;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-2605.978,2451.741;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-2843.026,1644.582;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;385;1062.518,2168.185;Float;False;Property;_MaskAppearUseWorldPosition;Mask Appear Use World Position;36;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;30;-957.0582,498.6822;Float;False;Constant;_Vector1;Vector 1;3;0;Create;True;0;0;0;False;0;False;0,0,-1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;112;1163.961,2520.31;Float;False;Property;_MaskAppearLocalYRamap;Mask Appear Local Y Ramap;28;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;51;-2427.38,2453.418;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.Vector3Node;29;-952.2623,334.0206;Float;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-2644.147,1519.495;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1076.871,750.2615;Float;False;Property;_MaskDepthFadeDistance;Mask Depth Fade Distance;13;0;Create;True;0;0;0;False;0;False;0.25;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;52;-1717.58,2058.876;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;54;-1928.165,2627.486;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;59;-1930.925,2320.196;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;58;-2472.36,1512.663;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;122;1390.114,2617.967;Float;False;Property;_MaskAppearLocalYAdd;Mask Appear Local Y Add;29;0;Create;True;0;0;0;False;0;False;0;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;56;-1929.544,2474.529;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;55;-2005.112,2726.895;Float;True;Property;_Noise02;Noise 02;21;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TFHCRemapNode;111;1487.195,2351.877;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;11;-804.8716,752.2615;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;57;-1493.934,2060.144;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwitchByFaceNode;27;-734.8457,428.3411;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;63;-1665.635,2284.347;Inherit;True;Property;_TextureSample2;Texture Sample 2;15;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;305;850.9712,3212.087;Float;False;Property;_MaskAppearNoiseTriplanarTiling;Mask Appear Noise Triplanar Tiling;35;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;121;1721.114,2426.967;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1326.231,2054.943;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-1969.093,1387.949;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;301;951.4485,2989.221;Float;True;Property;_MaskAppearNoise;Mask Appear Noise;32;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;62;-1660.436,2684.748;Inherit;True;Property;_TextureSample1;Texture Sample 1;15;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;61;-1665.636,2479.346;Inherit;True;Property;_TextureSample0;Texture Sample 0;15;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;65;-1967.715,1542.282;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;12;-609.8718,751.2615;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;3;-551.8963,118.224;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;8;-559.8963,426.2237;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;129;914.8154,2851.079;Inherit;False;0;301;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;67;-1966.333,1695.238;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;66;-2043.28,1794.647;Float;True;Property;_Noise01;Noise 01;17;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;72;-1703.803,1352.1;Inherit;True;Property;_TextureSample5;Texture Sample 5;15;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;71;-1703.804,1547.1;Inherit;True;Property;_TextureSample4;Texture Sample 4;15;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;19;-448.0177,751.9131;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-570.4011,871.4315;Float;False;Property;_MaskDepthFadeExp;Mask Depth Fade Exp;14;0;Create;True;0;0;0;False;0;False;4;4;0.2;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-1021.99,2615.873;Float;False;Constant;_Float8;Float 8;24;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;302;1431.747,2903.222;Inherit;True;Property;_TextureSample6;Texture Sample 6;32;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;1;-207.8957,210.224;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;109;1921.82,2473.289;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SummedBlendNode;69;-1052.032,2471.547;Inherit;False;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;303;1376.747,3121.922;Inherit;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;-1;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;70;-1698.605,1752.5;Inherit;True;Property;_TextureSample3;Texture Sample 3;15;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;304;1817.018,3015.634;Float;False;Property;_MaskAppearNoiseTriplanar;Mask Appear Noise Triplanar;34;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;75;-809.342,2525.265;Float;False;Property;_Noise02Enabled;Noise 02 Enabled;20;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;261;-3394.714,-342.3843;Float;False;Property;_HitWaveFadeDistance;Hit Wave Fade Distance;39;0;Create;True;0;0;0;False;0;False;6;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SummedBlendNode;73;-1090.203,1539.3;Inherit;False;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-1007.908,2085.772;Float;False;Property;_NoiseMaskPower;Noise Mask Power;15;0;Create;True;0;0;0;False;0;False;1;2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-3351.235,-581.7378;Float;False;Property;_HitWaveLength;Hit Wave Length;38;0;Create;True;0;0;0;False;0;False;0.5;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;15;-250.197,799.4316;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-208.8957,305.2238;Float;False;Property;_MaskFresnelExp;Mask Fresnel Exp;11;0;Create;True;0;0;0;False;0;False;4;4;0.2;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;110;2067.418,2387.488;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-3342.235,-481.7375;Float;False;Global;_PSLossyScale;_PSLossyScale;29;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;5;-83.89568,213.224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;331;-234.4778,917.4542;Float;False;Constant;_Float4;Float 4;36;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;131;2413.516,2662.577;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;330;-25.57727,844.65;Float;False;Property;_MaskDepthFadeEnabled;Mask Depth Fade Enabled;12;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;6;119.1042,249.224;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-526.1267,1897.027;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;118;2247.38,2445.417;Float;False;Property;_MaskAppearInvert;Mask Appear Invert;30;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;157;-3097.235,-552.7377;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;300;-3100.095,-396.1643;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;117;2266.95,2564.931;Float;False;Property;_MaskAppearProgress;Mask Appear Progress;31;0;Create;True;0;0;0;False;0;False;0;-0.5644736;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;368;-3303.401,1828.159;Float;False;DistortionForHits;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;317;-3441.051,-245.2482;Float;False;Property;_HitWaveFadeDistancePower;Hit Wave Fade Distance Power;40;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;146;2727.243,2744.832;Float;False;Constant;_Float0;Float 0;28;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;371;-3251.633,-887.4294;Float;False;Property;_HitWaveDistortionPower;Hit Wave Distortion Power;42;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;160;-2957.235,-554.7377;Float;False;WL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;377;-3345.274,-1617.244;Float;False;Global;_AffectorCount;_AffectorCount;42;0;Create;True;0;0;0;False;0;False;20;0;False;0;1;INT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-369.3619,1894.306;Float;False;ResultNoise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;262;-2947.781,-401.257;Float;False;FD;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;116;2667.38,2510.417;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;369;-3211.082,-997.5181;Inherit;False;368;DistortionForHits;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;319;-2952.051,-248.2482;Float;False;FDP;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;10;390.8629,470.9057;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;344;-2975.852,-1165.457;Float;True;Property;_HitWaveRampMask;Hit Wave Ramp Mask;41;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.WorldPosInputsNode;375;-2931.14,-1646.286;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GlobalArrayNode;373;-2977.654,-1747.753;Inherit;False;_ControlParticleSize;0;20;0;False;False;0;1;False;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;145;2966.52,2599.128;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;96;405.714,701.994;Float;False;Property;_NoiseMaskAdd;Noise Mask Add;16;0;Create;True;0;0;0;False;0;False;0.25;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;378;-2885.272,-1439.976;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.ClampOpNode;17;527.8627,470.9057;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;470.5437,603.4798;Inherit;False;77;ResultNoise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;370;-2895.985,-935.6062;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;380;-2929.787,-1321.885;Inherit;False;319;FDP;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;381;-2925.456,-1244.708;Inherit;False;160;WL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;379;-2927.053,-1395.936;Inherit;False;262;FD;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GlobalArrayNode;372;-2977.229,-1854.234;Inherit;False;_ControlParticlePosition;0;20;2;False;False;0;1;False;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;326;795.4557,1358.676;Inherit;False;77;ResultNoise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;147;3107.841,2575.91;Inherit;True;Property;_MaskAppearRamp;Mask Appear Ramp;33;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;374;-2504.506,-1411.387;Float;False;float MyResult = 0@$float DistanceMask45@$$for (int i = 0@ i < AffectorCountCE@ i++){$$DistanceMask45 = distance( WorldPosCE , _ControlParticlePosition[i] )@$$float myTemp01 = (1 - frac(clamp(((1 - _ControlParticleSize[i] - 1 + DistanceMask45 + HtWaveDistortionPowerCE) * WLCE), -1.0, 0.0)))@$$float2 myTempUV01 = float2(myTemp01, 0.0)@$$float myClampResult01 = clamp( (0.0 + (( -DistanceMask45 + FDCE ) - 0.0) * (FDPCE - 0.0) / (FDCE - 0.0)) , 0.0 , 1.0 )@$$MyResult += (myClampResult01 * tex2D(HitWaveRampMaskCE,myTempUV01).r)@$$}$MyResult = clamp(MyResult, 0.0, 1.0)@$$return MyResult@;1;Create;9;True;PositionCE;FLOAT4;0,0,0,0;In;;Float;False;True;SizeCE;FLOAT;0;In;;Float;False;True;WorldPosCE;FLOAT3;0,0,0;In;;Float;False;True;HitWaveRampMaskCE;SAMPLER2D;0.0;In;;Float;False;True;HtWaveDistortionPowerCE;FLOAT;0;In;;Float;False;True;AffectorCountCE;INT;0;In;;Float;False;True;FDCE;FLOAT;0;In;;Float;False;True;FDPCE;FLOAT;0;In;;Float;False;True;WLCE;FLOAT;0;In;;Float;False;ArrayCE;True;False;0;;False;9;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;SAMPLER2D;0.0;False;4;FLOAT;0;False;5;INT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;745.1597,645.2981;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;328;728.4275,1456.049;Float;False;Property;_HitWaveNoiseNegate;Hit Wave Noise Negate;37;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;744.5441,534.5797;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;949.9863,600.9141;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;327;1036.428,1400.049;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;342;-2003.846,-1320.985;Float;False;HWArrayResult;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;119;3449.934,2657.747;Float;False;MaskAppearValue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;135;3447.375,2564.795;Float;False;MaskAppearEdges;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;329;1160.428,1401.049;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;80;1079.904,598.0259;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;343;1069.342,1250.552;Inherit;False;342;HWArrayResult;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;338;1063.174,1544.602;Inherit;False;119;MaskAppearValue;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;120;989.554,771.2363;Inherit;False;119;MaskAppearValue;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;1342.792,806.6533;Inherit;False;135;MaskAppearEdges;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;325;1396.679,1348.026;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;1441.088,689.5173;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;137;1636.792,738.6533;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;139;1771.333,739.6516;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;2041.935,641.3013;Float;False;ResultOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;361;974.1491,-95.72071;Float;False;Property;_OpacityPower;Opacity Power;2;0;Create;True;0;0;0;False;0;False;1;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;113;1040.303,-199.6279;Inherit;False;93;ResultOpacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;360;1293.148,-153.7206;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;339;1243.052,158.5241;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;340;1203.73,304.2823;Float;False;Property;_NormalVertexOffset;Normal Vertex Offset;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-456.7105,-709.5768;Inherit;False;93;ResultOpacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;341;1490.137,230.2686;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldReflectionVector;332;458.7249,-545.0547;Inherit;False;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;387;620.5186,2079.184;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;337;801.0951,-345.8906;Float;False;Constant;_Float5;Float 5;38;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;89;842.3301,-1064.289;Float;False;Property;_RampColorTint;Ramp Color Tint;8;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,0.4893508,0.3014636,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;336;991.0951,-458.8906;Float;False;Property;_CubemapReflectionEnabled;Cubemap Reflection Enabled;5;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;334;1440.594,-677.2906;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;335;652.095,-544.8906;Inherit;True;Property;_CubemapReflection;Cubemap Reflection;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;1;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;367;171.5986,-411.5116;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;87;300.3785,-805.651;Float;False;Property;_RampFlip;Ramp Flip;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;85;127.8265,-883.5073;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;88;573.4828,-763.8051;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-189.2782,-771.1591;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;778.2527,-886.0862;Float;False;Property;_FinalPower;Final Power;0;0;Create;True;0;0;0;False;0;False;4;5;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;2;-561.8962,279.2238;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;82;-471.5705,-823.558;Float;False;Property;_RampMultiplyTiling;Ramp Multiply Tiling;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;84;-37.27811,-776.1592;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;377.1475,-699.7408;Float;False;Constant;_Float2;Float 2;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;363;19.02255,-408.6543;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;90;760.9977,-794.5371;Inherit;True;Property;_Ramp;Ramp;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;364;-330.4165,-289.8242;Float;False;Property;_FinalPowerAdjust;Final Power Adjust;1;0;Create;True;0;0;0;False;0;False;-1;-1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;1114.254,-910.0862;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;362;1444.468,-164.3043;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;398;1914.879,-396.0403;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;394;1914.879,-396.0403;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;395;1914.879,-396.0403;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;11;SineVFX/ForceFieldAndShieldEffects/ForceFieldBasicTriplanarLWRP;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;22;Surface;1;0;  Blend;0;0;Two Sided;0;0;Cast Shadows;0;0;  Use Shadow Threshold;0;0;Receive Shadows;0;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;DOTS Instancing;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,-1;0;  Type;0;0;  Tess;16,False,-1;0;  Min;10,False,-1;0;  Max;25,False,-1;0;  Edge Length;16,False,-1;0;  Max Displacement;25,False,-1;0;Vertex Position,InvertActionOnDeselection;1;0;0;5;False;True;False;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;396;1914.879,-396.0403;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;397;1914.879,-396.0403;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;106;0;107;0
WireConnection;101;0;106;1
WireConnection;101;1;106;2
WireConnection;101;2;106;3
WireConnection;97;0;46;0
WireConnection;97;1;101;0
WireConnection;103;1;46;0
WireConnection;103;0;97;0
WireConnection;102;0;103;0
WireConnection;34;0;31;0
WireConnection;34;3;32;0
WireConnection;38;0;34;0
WireConnection;38;1;33;0
WireConnection;42;1;36;0
WireConnection;42;0;38;0
WireConnection;43;0;35;2
WireConnection;43;1;40;0
WireConnection;388;0;384;0
WireConnection;41;0;105;0
WireConnection;41;1;37;0
WireConnection;50;0;104;0
WireConnection;50;1;44;0
WireConnection;389;0;388;1
WireConnection;389;1;386;2
WireConnection;48;0;41;0
WireConnection;48;1;43;0
WireConnection;48;2;42;0
WireConnection;49;0;45;2
WireConnection;49;1;47;0
WireConnection;385;1;108;2
WireConnection;385;0;389;0
WireConnection;51;0;48;0
WireConnection;53;0;50;0
WireConnection;53;1;49;0
WireConnection;53;2;42;0
WireConnection;54;0;51;0
WireConnection;54;1;51;1
WireConnection;59;0;51;1
WireConnection;59;1;51;2
WireConnection;58;0;53;0
WireConnection;56;0;51;2
WireConnection;56;1;51;0
WireConnection;111;0;385;0
WireConnection;111;2;112;0
WireConnection;11;0;13;0
WireConnection;57;0;52;0
WireConnection;27;0;29;0
WireConnection;27;1;30;0
WireConnection;63;0;55;0
WireConnection;63;1;59;0
WireConnection;121;0;111;0
WireConnection;121;1;122;0
WireConnection;60;0;57;0
WireConnection;60;1;57;0
WireConnection;64;0;58;1
WireConnection;64;1;58;2
WireConnection;62;0;55;0
WireConnection;62;1;54;0
WireConnection;61;0;55;0
WireConnection;61;1;56;0
WireConnection;65;0;58;2
WireConnection;65;1;58;0
WireConnection;12;0;11;0
WireConnection;8;0;27;0
WireConnection;67;0;58;0
WireConnection;67;1;58;1
WireConnection;72;0;66;0
WireConnection;72;1;64;0
WireConnection;71;0;66;0
WireConnection;71;1;65;0
WireConnection;19;0;12;0
WireConnection;302;0;301;0
WireConnection;302;1;129;0
WireConnection;1;0;3;0
WireConnection;1;1;8;0
WireConnection;109;0;121;0
WireConnection;69;0;60;0
WireConnection;69;1;63;1
WireConnection;69;2;61;1
WireConnection;69;3;62;1
WireConnection;303;0;301;0
WireConnection;303;3;305;0
WireConnection;70;0;66;0
WireConnection;70;1;67;0
WireConnection;304;1;302;1
WireConnection;304;0;303;1
WireConnection;75;1;68;0
WireConnection;75;0;69;0
WireConnection;73;0;60;0
WireConnection;73;1;72;1
WireConnection;73;2;71;1
WireConnection;73;3;70;1
WireConnection;15;0;19;0
WireConnection;15;1;16;0
WireConnection;110;0;109;0
WireConnection;5;0;1;0
WireConnection;131;0;304;0
WireConnection;330;1;331;0
WireConnection;330;0;15;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;76;0;73;0
WireConnection;76;1;75;0
WireConnection;76;2;74;0
WireConnection;118;1;109;0
WireConnection;118;0;110;0
WireConnection;157;0;156;0
WireConnection;157;1;158;0
WireConnection;300;0;158;0
WireConnection;300;1;261;0
WireConnection;368;0;34;1
WireConnection;160;0;157;0
WireConnection;77;0;76;0
WireConnection;262;0;300;0
WireConnection;116;0;118;0
WireConnection;116;1;117;0
WireConnection;116;2;131;0
WireConnection;319;0;317;0
WireConnection;10;0;6;0
WireConnection;10;1;330;0
WireConnection;145;0;116;0
WireConnection;145;1;146;0
WireConnection;378;0;377;0
WireConnection;17;0;10;0
WireConnection;370;0;369;0
WireConnection;370;1;371;0
WireConnection;147;1;145;0
WireConnection;374;0;372;0
WireConnection;374;1;373;0
WireConnection;374;2;375;0
WireConnection;374;3;344;0
WireConnection;374;4;370;0
WireConnection;374;5;378;0
WireConnection;374;6;379;0
WireConnection;374;7;380;0
WireConnection;374;8;381;0
WireConnection;95;0;17;0
WireConnection;95;1;96;0
WireConnection;78;0;17;0
WireConnection;78;1;79;0
WireConnection;94;0;78;0
WireConnection;94;1;95;0
WireConnection;327;0;326;0
WireConnection;327;1;328;0
WireConnection;342;0;374;0
WireConnection;119;0;147;2
WireConnection;135;0;147;1
WireConnection;329;0;327;0
WireConnection;80;0;94;0
WireConnection;325;0;343;0
WireConnection;325;1;329;0
WireConnection;325;2;338;0
WireConnection;114;0;80;0
WireConnection;114;1;120;0
WireConnection;137;0;114;0
WireConnection;137;1;138;0
WireConnection;137;2;325;0
WireConnection;139;0;137;0
WireConnection;93;0;139;0
WireConnection;360;0;113;0
WireConnection;360;1;361;0
WireConnection;341;0;339;0
WireConnection;341;1;340;0
WireConnection;387;0;386;2
WireConnection;387;1;388;1
WireConnection;336;1;337;0
WireConnection;336;0;335;0
WireConnection;334;0;92;0
WireConnection;334;1;336;0
WireConnection;335;1;332;0
WireConnection;367;0;363;0
WireConnection;87;1;84;0
WireConnection;87;0;85;0
WireConnection;85;0;84;0
WireConnection;88;0;87;0
WireConnection;88;1;86;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;84;0;83;0
WireConnection;363;0;81;0
WireConnection;363;1;364;0
WireConnection;90;1;88;0
WireConnection;92;0;89;0
WireConnection;92;1;91;0
WireConnection;92;2;90;0
WireConnection;92;3;367;0
WireConnection;362;0;360;0
WireConnection;395;2;334;0
WireConnection;395;3;362;0
WireConnection;395;5;341;0
ASEEND*/
//CHKSM=A705DC18A6F8840F363D66C728AD36EC59013992