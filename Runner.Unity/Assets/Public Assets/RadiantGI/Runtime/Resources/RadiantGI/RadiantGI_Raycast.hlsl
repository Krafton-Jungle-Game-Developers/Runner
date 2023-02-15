#ifndef RGI_RAYCAST
#define RGI_RAYCAST

	// Copyright 2022 Kronnect - All Rights Reserved.

	#define BINARY_SEARCH_ITERATIONS 6
	TEXTURE2D_X(_PrevResolve);

	TEXTURECUBE(_Probe1Cube);
	TEXTURECUBE(_Probe2Cube);
	SAMPLER(sampler_LinearRepeat_Cube1);
	SAMPLER(sampler_LinearRepeat_Cube2);
	half2 _ProbeData;
	half4 _Probe1HDR, _Probe2HDR;

	struct VaryingsRaycast {
		float4 positionCS : SV_POSITION;
		float4 uv  : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	VaryingsRaycast VertRaycast(AttributesFS input) {
		VaryingsRaycast output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_TRANSFER_INSTANCE_ID(input, output);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = float4(input.positionHCS.xyz, 1.0);

		#if UNITY_UV_STARTS_AT_TOP
			output.positionCS.y *= -1;
		#endif

		output.uv.xy = input.uv;
		float4 projPos = output.positionCS * 0.5;
		projPos.xy = projPos.xy + projPos.w;
		output.uv.zw = projPos.xy;
		return output;
	}


	float4 sposStart;
	float k0, q0;

	void PrepareRay(float2 uv, float3 rayStart) {
		float4 sposStart = mul(unity_CameraProjection, float4(rayStart, 1.0));
		#if _ORTHO_SUPPORT
			float4 orthoSposStart = float4(uv.xy * 2 - 1, rayStart.z, 1.0);
			sposStart = lerp(sposStart, orthoSposStart, unity_OrthoParams.w);
		#endif
		k0 = rcp(sposStart.w);
		q0 = rayStart.z * k0;
	}

	half4 Raycast(float2 uv, float3 rayStart, float3 rayDir, float jitterFactor) {

		float  rayLength = RAY_MAX_LENGTH;
		rayDir *= jitterFactor;
		if (rayStart.z + rayDir.z * rayLength < _ProjectionParams.y) {
			rayLength = abs( (rayStart.z - _ProjectionParams.y) / rayDir.z );
		}
		float3 rayEnd = rayStart + rayDir * rayLength;

		float4 sposEnd = mul(unity_CameraProjection, float4(rayEnd, 1.0));
		#if _ORTHO_SUPPORT
			float4 orthoSposEnd = float4(rayEnd.xy * 2 - 1, rayEnd.z, 1.0);
			sposEnd = lerp(sposEnd, orthoSposEnd, unity_OrthoParams.w);
		#endif

		float k1 = rcp(sposEnd.w);
		float q1 = rayEnd.z * k1;
		float4 p = float4(uv, q0, k0);

		// length in pixels
		float2 uv1 = (sposEnd.xy * rcp(rayEnd.z) + 1.0) * 0.5;

		float2 duv = uv1 - uv;
		float2 duvPixel = abs(duv * _DownscaledDepthRT_TexelSize.zw);
		float pixelDistance = max(duvPixel.x, duvPixel.y);
		pixelDistance = max(1, pixelDistance);

		int sampleCount = (int)min(pixelDistance, RAY_MAX_SAMPLES);
		float4 pincr = float4(duv, q1-q0, k1-k0) * rcp(sampleCount);

		float zdist = 0;
		float pz = 0;
		float sceneDepth;

		for (int k = 0; k < sampleCount; k++) {
			p += pincr;
			if (any(floor(p.xy)!=0)) return 0; // out of screen space
			sceneDepth = GetLinearEyeDownscaledDepth(p.xy);
			pz = p.z / p.w;
			float depthDiff = pz - sceneDepth;
			if (depthDiff > 0.02 && depthDiff < THICKNESS) {
				zdist = 1;
				break;
			}
		}

		UNITY_BRANCH
		if (zdist) {
			float4 hitp = p;
			#if _USES_BINARY_SEARCH
				if (p.z > 0) pincr = -pincr;
				float4 stepPincr = pincr;
				float reduction = 1.0;

				UNITY_UNROLL
				for (int j = 0; j < BINARY_SEARCH_ITERATIONS; j++) {
					reduction *= 0.5;
					p += stepPincr * reduction;
					sceneDepth = GetLinearEyeDownscaledDepth(p.xy);
					pz = p.z / p.w;
					float depthDiff = pz - sceneDepth;
					stepPincr = sign(depthDiff) * pincr;
					if (depthDiff > 0.02 && depthDiff < THICKNESS) {
						hitp = p;
					}
				}
			#endif

			zdist = rayLength * (hitp.z / hitp.w - rayStart.z) / (0.0001 + rayEnd.z - rayStart.z);

			// quadratic distance attenuation
			half distSqr = zdist * zdist;
			half distAtten = rcp(1.0 + distSqr);

			// indirect term
			half3 indirect = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_PointClamp, hitp.xy, 0).rgb; // point clamp to avoid color bleed
			indirect = clamp(indirect, 0, 32); // keep source data under reasonable range and avoid NaN
			half invDistSqrWeight = lerp(1.0, distAtten, INDIRECT_DISTANCE_ATTENUATION);
			indirect *= invDistSqrWeight;

			return half4(indirect, 1.0);
		}

		return 0; // miss
	}


	float3 GetTangent(float3 v) {
		return abs(v.x) > abs(v.z) ? float3(-v.y, v.x, 0.0) : float3(0.0, -v.z, v.y);
	}

	// Based on Advanced Computer Graphics / Rendering Equation book by Matthias Teschner
	float3 GetWeighedCosineDirection(float2 noise, float3 norm) {
		float phi = 2.0f * PI * noise.y;
		float cphi, sphi;
		sincos(phi, sphi, cphi);
		float sqrN = sqrt(noise.x);
		float x = cphi * sqrN;
		float y = sphi * sqrN;
		float z = sqrt(1.0f - noise.x);

		float3 tangent = normalize(GetTangent(norm));
		float3 bitangent = cross(tangent, norm);

		float3 v = tangent * x + bitangent * y + norm * z;
		return v;
	}

	float3 GetJitteredNormal(float2 uv, float3 noises, float3 norm, float goldenRatio) {
		noises.x = frac(noises.x + goldenRatio);
		float3 jitteredNormal = GetWeighedCosineDirection(noises.xz, norm);
		return jitteredNormal;
	}

	half3 SampleProbe(TEXTURECUBE(_ProbeCube), SAMPLER(_SamplerCube), half4 probeHDRData, half3 normalWS) {
		half4 probeSample = SAMPLE_TEXTURECUBE_LOD(_ProbeCube, _SamplerCube, normalWS, 0);
		#if !defined(UNITY_USE_NATIVE_HDR)
			probeSample.rgb = DecodeHDREnvironment(probeSample, probeHDRData);
		#endif
		return probeSample.rgb;
	}

	half4 FragRaycast (VaryingsRaycast input) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float2 uv = UnityStereoTransformScreenSpaceTex(input.uv.xy);
		
		float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_PointClamp, input.uv.xy).r;

		 // exclude skybox
		if (IsSkyBox(rawDepth)) return 0;

		float3 rayStart = GetViewSpacePosition(input.uv.zw, rawDepth);
		float2 pos = uv * SOURCE_SIZE;
		float3 normalWS = GetWorldNormal((uint2)pos);
		float3 normalVS = mul((float3x3)_WorldToViewDir, normalWS);
		normalVS.z *= -1.0;

		half4 indirect = 0;

//        float screenNoise = dot(float2(171.0, 231.0), pos); // Alternate noise generator
//        screenNoise = frac( screenNoise / 91.111 );
		
		float screenNoise = InterleavedGradientNoise(pos, FRAME_NUMBER);
		float2 noise = SAMPLE_TEXTURE2D_LOD(_NoiseTex, sampler_PointRepeat, pos * _NoiseTex_TexelSize.xy, 0).xw;

		float3 noises = float3(noise.xy, screenNoise);
		float jitterFactor = noises.y * JITTER_AMOUNT + 1.0;

		PrepareRay(uv, rayStart);

		#if _USES_MULTIPLE_RAYS
			for (int k=0;k<RAY_COUNT;k++) {
				float3 rayDir = GetJitteredNormal(uv, noises, normalVS, GOLDEN_RATIO_ACUM);
				half4 indirectSample = Raycast(uv, rayStart, rayDir, jitterFactor);
				GOLDEN_RATIO_ACUM += 0.618033989f; // GOLDEN_RATIO;
				indirect += indirectSample;
			}
			indirect /= RAY_COUNT;
		#else
			float3 rayDir = GetJitteredNormal(uv, noises, normalVS, GOLDEN_RATIO_ACUM);
			indirect = Raycast(uv, rayStart, rayDir, jitterFactor);
		#endif

		#if _FALLBACK_1_PROBE
			UNITY_BRANCH
			if (indirect.w == 0)  { // miss ray; gather data from reflection probe
				half3 probeSample = SampleProbe(_Probe1Cube, sampler_LinearRepeat_Cube1, _Probe1HDR, normalWS);
				indirect.rgb = probeSample * _ProbeData.x;
			}
		#elif _FALLBACK_2_PROBES
			UNITY_BRANCH
			if (indirect.w == 0)  { // miss ray; gather data from reflection probe
				half3 probe1Sample = SampleProbe(_Probe1Cube, sampler_LinearRepeat_Cube1, _Probe1HDR, normalWS);
				half3 probe2Sample = SampleProbe(_Probe2Cube, sampler_LinearRepeat_Cube2, _Probe2HDR, normalWS);
				indirect.rgb = probe1Sample * _ProbeData.x + probe2Sample * _ProbeData.y;
			}
		#endif

		half luma = GetLuma(indirect.rgb);
		indirect *= saturate(INDIRECT_MAX_BRIGHTNESS / (luma + 0.001));
		indirect *= INDIRECT_INTENSITY * (luma > LUMA_THRESHOLD);

		half eyeDepth = RawToLinearEyeDepth(rawDepth);

		#if _REUSE_RAYS
			UNITY_BRANCH
			if (indirect.w == 0)  { // miss ray; reuse ray from previous frame
				float2 velocity = GetVelocity(uv);
                float2 prevUV = saturate(uv - velocity);
//                if (all(floor(prevUV) == 0)) { // we use saturate above because it's better to have some input near the previous position than nothing
					half4 prevResolve = SAMPLE_TEXTURE2D_X_LOD(_PrevResolve, sampler_LinearClamp, prevUV, 0);
					half depthDiff = abs(prevResolve.w - eyeDepth);
					if (depthDiff < TEMPORAL_MAX_DEPTH_DIFFERENCE) {
						indirect.rgb = prevResolve.rgb * RAY_REUSE_INTENSITY;
					}
//                }
			}
		#endif

		indirect.w = eyeDepth;

		return indirect;
	}


#endif // RGI_RAYCAST