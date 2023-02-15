#ifndef RGI_NFO
#define RGI_NFO

    static float2 randomOffsets[16] = {
            float2(-0.0897,-0.4940),
            float2( 0.1379, 0.2486),
            float2(-0.6999,-0.0451),
            float2( 0.3371, 0.5679),
            float2( 0.0689,-0.1598),
            float2(-0.0146, 0.1402),
            float2( 0.0352,-0.0631),
            float2( 0.0100,-0.1924),
            float2( 0.0560, 0.0069),
            float2(-0.4776, 0.2847),
            float2(-0.3577,-0.5301),
            float2( 0.5381, 0.1856),
            float2( 0.0103,-0.5869),
            float2(-0.3169, 0.1063),
            float2( 0.7119,-0.0154),
            float2(-0.0533, 0.0596),
        };

        
	half4 FragRGI (VaryingsRGI i) : SV_Target { 

        UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float rawDepth = GetRawDepth(uv);
        if (IsSkyBox(rawDepth)) return 0;

        float3 wpos = GetWorldPosition(uv, rawDepth);
        if (IsOutsideBounds(wpos)) return 0;

        float2 pos = uv * SOURCE_SIZE / 2;
        float2 noise = SAMPLE_TEXTURE2D_LOD(_NoiseTex, sampler_PointRepeat, pos * _NoiseTex_TexelSize.xy, 0).xw;

        float eyeDepth = RawToLinearEyeDepth(rawDepth);
        float samplingRadius = NEAR_FIELD_OBSCURANCE_SPREAD / eyeDepth;

        float3 normalWS = GetWorldNormal(uv);
        half occlusion = 0.001;

        for(int k=0; k < 16; k++) {

            float2 offset = samplingRadius * reflect(randomOffsets[k], noise);
            float2 occuv = saturate(uv + offset);
            float depth  = GetDownscaledRawDepth(occuv);
            float3 occwpos = GetWorldPosition(occuv, depth);
            half3  occdir = (half3)(occwpos - wpos);
            half   occmag = dot2(occdir * 1.75) + 0.1;
            half   occ = saturate(dot(normalWS, occdir / occmag) - 0.15);
            occlusion += occ;
        }
                
        half nfo = NEAR_FIELD_OBSCURANCE_INTENSITY * occlusion / 16.0;
        return nfo;
	}



#endif // NFO