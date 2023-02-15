#ifndef RGI_TACUM
#define RGI_TACUM

	// Copyright 2022 Kronnect - All Rights Reserved.
    TEXTURE2D_X(_PrevResolve);

	half4 FragRGI (VaryingsRGI i) : SV_Target { 

        UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float delta = unity_DeltaTime.z * TEMPORAL_RESPONSE_SPEED;

        half4 newData = SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);

        half2 velocity = GetVelocity(uv);
        float2 prevUV = uv - velocity;

        if (any(floor(prevUV))!=0) {
            return newData;
        }

        half4 prevData = SAMPLE_TEXTURE2D_X(_PrevResolve, sampler_PointClamp, prevUV);

        half4 newDataN = SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv + float2(0, 1) * _MainTex_TexelSize.xy);
        half4 newDataS = SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv + float2(0, -1) * _MainTex_TexelSize.xy);
        half4 newDataW = SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv + float2(-1, 0) * _MainTex_TexelSize.xy);
        half4 newDataE = SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv + float2(1, 0) * _MainTex_TexelSize.xy);

        half4 newDataMin = min( newData, min( min(newDataN, newDataS), min(newDataW, newDataE) ));
        half4 newDataMax = max( newData, max( max(newDataN, newDataS), max(newDataW, newDataE) ));

        half4 newDataMinExt = newDataMin * (1 - TEMPORAL_CHROMA_THRESHOLD);
        half4 newDataMaxExt = newDataMax * (1 + TEMPORAL_CHROMA_THRESHOLD);
        
        // reduce noise by clamping history to present by certain threshold
        prevData = clamp(prevData, min(newDataMinExt, newDataMaxExt), max(newDataMinExt, newDataMaxExt));

        half4 res = lerp(prevData, newData, saturate(delta));
        return res;

	}



#endif // RGI