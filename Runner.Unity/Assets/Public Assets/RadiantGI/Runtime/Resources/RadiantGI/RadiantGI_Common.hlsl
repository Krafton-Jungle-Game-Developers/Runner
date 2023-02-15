#ifndef RGI_COMMON
#define RGI_COMMON

	// Copyright 2022 Kronnect - All Rights Reserved.

    TEXTURE2D_X(_MainTex);
    float4 _MainTex_TexelSize;

    TEXTURE2D(_NoiseTex);
    float4 _NoiseTex_TexelSize;

	TEXTURE2D_X(_MotionVectorTexture);
    TEXTURE2D_X(_GBuffer0);
    TEXTURE2D_X(_GBuffer1);

    TEXTURE2D_X(_DownscaledDepthRT);
    float4 _DownscaledDepthRT_TexelSize;

    #define dot2(x) dot(x, x)
    
    float4x4 _WorldToViewDir;

	float4 _SourceSize;
	#define SOURCE_SIZE _SourceSize.xy
	#define GOLDEN_RATIO_ACUM _SourceSize.z
    #define FRAME_NUMBER _SourceSize.w

    float4 _IndirectData;
    float4 _RayData;
    float3 _TemporalData;
    float4 _ExtraData;
    half4  _ExtraData2;
    half4  _ExtraData3;
    float4 _BoundsXZ;

    #define INDIRECT_INTENSITY _IndirectData.x
    #define INDIRECT_MAX_BRIGHTNESS _IndirectData.y
    #define INDIRECT_DISTANCE_ATTENUATION _IndirectData.z

    #define OBSCURANCE_INTENSITY _OcclusionData.x
    #define OBSCURANCE_POWER _OcclusionData.y

    #define RAY_COUNT _RayData.x
    #define RAY_MAX_LENGTH _RayData.y
    #define RAY_MAX_SAMPLES _RayData.z
    #define THICKNESS _RayData.w
    #define RAY_REUSE_INTENSITY _IndirectData.w

    #define TEMPORAL_RESPONSE_SPEED _TemporalData.x
    #define TEMPORAL_MAX_DEPTH_DIFFERENCE _TemporalData.y
    #define TEMPORAL_CHROMA_THRESHOLD _TemporalData.z

    #define JITTER_AMOUNT _ExtraData.x
    #define BLUR_SPREAD _ExtraData.y
    #define NORMALS_INFLUENCE _ExtraData.z
    #define LUMA_INFLUENCE _ExtraData.w

    #define LUMA_THRESHOLD _ExtraData2.x
    #define LUMA_MAX _ExtraData2.y
    #define COLOR_SATURATION _ExtraData2.z
    #define RSM_INTENSITY _ExtraData2.w

    #define AO_INFLUENCE _ExtraData3.x
    #define ONE_MINUS_AO_INFLUENCE (1.0 - _ExtraData3.x)
    #define NEAR_CAMERA_ATTENUATION _ExtraData3.z
    #define NEAR_FIELD_OBSCURANCE_INTENSITY _ExtraData3.w
    #define NEAR_FIELD_OBSCURANCE_SPREAD _ExtraData3.y

	struct AttributesFS {
		float4 positionHCS : POSITION;
		float2 uv          : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

 	struct VaryingsRGI {
    	float4 positionCS : SV_POSITION;
    	float2 uv  : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsRGI VertRGI(AttributesFS input) {
	    VaryingsRGI output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_TRANSFER_INSTANCE_ID(input, output);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = float4(input.positionHCS.xyz, 1.0);
        output.positionCS.y *= _ProjectionParams.x;

        output.uv = input.uv;
    	return output;
	}

    float GetRawDepth(float2 uv) {
        float depth = SAMPLE_TEXTURE2D_X_LOD(_CameraDepthTexture, sampler_PointClamp, uv, 0).r;
        return depth;
    }

    float RawToLinearEyeDepth(float rawDepth) {
        float eyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
        #if _ORTHO_SUPPORT
            #if UNITY_REVERSED_Z
                rawDepth = 1.0 - rawDepth;
            #endif
            float orthoEyeDepth = lerp(_ProjectionParams.y, _ProjectionParams.z, rawDepth);
            eyeDepth = lerp(eyeDepth, orthoEyeDepth, unity_OrthoParams.w);
        #endif
        return eyeDepth;
    }

    float GetLinearEyeDepth(float2 uv) {
        float rawDepth = GetRawDepth(uv);
        return RawToLinearEyeDepth(rawDepth);
    }

    float GetDownscaledRawDepth(float2 uv) {
	float depth = SAMPLE_TEXTURE2D_X_LOD(_DownscaledDepthRT, sampler_PointClamp, uv, 0).r;
	return depth;
    }

    float GetLinearEyeDownscaledDepth(float2 uv) {
        float rawDepth = GetDownscaledRawDepth(uv);
        return RawToLinearEyeDepth(rawDepth);
    }

    float3 GetViewSpacePosition(float2 uv, float rawDepth) {
        #if UNITY_REVERSED_Z
            float depth = 1.0 - rawDepth;
        #else
            float depth = rawDepth;
        #endif
        depth = 2.0 * depth - 1.0;
        float3 rayStart = ComputeViewSpacePosition(uv, depth, unity_CameraInvProjection);
        return rayStart;
    }

    float3 GetViewSpacePosition(float2 uv) {
        float rawDepth = GetRawDepth(uv);
        return GetViewSpacePosition(uv, rawDepth);
    }

    float3 GetWorldPosition(float2 uv, float rawDepth) {
        
         #if UNITY_REVERSED_Z
              float depth = rawDepth;
         #else
              float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, rawDepth);
         #endif

         // Reconstruct the world space positions.
         float3 worldPos = ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);

        return worldPos;
    }

    float3 GetWorldPosition(float2 uv) {
        float rawDepth = GetRawDepth(uv);
        return GetWorldPosition(uv, rawDepth);
    }


	half3 GetWorldNormal(uint2 uv) {
		half3 norm = LoadSceneNormals(uv);
		return norm;
	}

	half3 GetWorldNormal(float2 uv) {
		half3 norm = SampleSceneNormals(uv);
		return norm;
	}

    half2 GetVelocity(float2 uv) {
		half2 mv = SAMPLE_TEXTURE2D_X_LOD(_MotionVectorTexture, sampler_PointClamp, uv, 0).xy;
        return mv;
    }

    half GetLuma(half3 rgb) {
        const half3 lum = half3(0.299, 0.587, 0.114);
        return dot(rgb, lum);
    }

    void GetAlbedoAndSpecularColors(float2 uv, out half3 albedo, out half3 specular) {
        half4 pixelGBuffer0 = SAMPLE_TEXTURE2D_X(_GBuffer0, sampler_PointClamp, uv);
        albedo = pixelGBuffer0.rgb;

        half3 pixelSpecular = SAMPLE_TEXTURE2D_X(_GBuffer1, sampler_PointClamp, uv).rgb;

        uint materialFlags = UnpackMaterialFlags(pixelGBuffer0.a);
        if ((materialFlags & kMaterialFlagSpecularSetup) != 0) {
            specular = pixelSpecular;
        } else {
            specular = pixelSpecular.rrr;
        }
    }
        
    half3 GetSpecularColor(float2 uv) {
        half3 albedo, specular;
        GetAlbedoAndSpecularColors(uv, albedo, specular);
        return specular;
    }

    bool IsSkyBox(float rawDepth) {
        #if UNITY_REVERSED_Z
            return rawDepth <= 0;
		#else
            return rawDepth >= 1.0;
		#endif
    }

    bool IsOutsideBounds(float3 wpos) {
        return any(wpos.xz < _BoundsXZ.xy) || any(wpos.xz > _BoundsXZ.zw);
    }

#endif // RGI_COMMON