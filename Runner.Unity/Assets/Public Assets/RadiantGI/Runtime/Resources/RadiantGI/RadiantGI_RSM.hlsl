#ifndef RGI_WIDE
#define RGI_WIDE

// Copyright 2022 Kronnect - All Rights Reserved.

TEXTURE2D_X(_RadiantShadowMapColors);
TEXTURE2D_X(_RadiantShadowMapNormals);
TEXTURE2D_X(_RadiantShadowMapWorldPos);
float4 _RadiantShadowMapColors_TexelSize;


TEXTURE2D_X(_RadiantShadowMapRSM);
float4x4 _RadiantWorldToShadowMap;

#define SAMPLE_COUNT 32
static float2 offsets[] = {
    float2(0.1092819, -1.2452140), float2(1.0755790, 1.4029010), float2(-2.1319680, -0.3771141),
    float2(2.1093880, -1.3418210), float2(-0.7256145, 2.6992570), float2(-1.4112360, -2.7172440), float2(3.1065140, 1.1344910),
    float2(-3.2680540, 1.3490090), float2(1.5894190, -3.3965060), float2(1.1830280, 3.7716650), float2(-3.5869800, -2.0787220),
    float2(4.2291300, -0.9297680), float2(-2.5920690, 3.6869620), float2(-0.6010606, -4.6382910), float2(3.7018480, 3.1199110),
    float2(-4.9957320, 0.2065974), float2(3.6532180, -3.6354530), float2(-0.2449574, 5.2976420), float2(-3.4909930, -4.1833590),
    float2(5.5402510, 0.7454209), float2(-4.7020520, 3.2715800), float2(1.2868110, -5.7200650), float2(2.9805090, 5.2013560),
    float2(-5.8340360, -1.8612000), float2(5.6736900, -2.6214070), float2(-2.4606510, 5.8796460), float2(-2.1983310, -6.1118640),
    float2(5.8549900, 3.0771960), float2(-6.5091140, 1.7158030), float2(3.7028470, -5.7588180), float2(1.1788310, 6.8591480),
    float2(-5.5905680, -4.3296190), float2(7.1561880, -0.5929008), float2(-4.9495820, 5.3503920), float2(0.0360481, -7.3950160),
    float2(5.0391630, 5.5548990), float2(-7.5710140, -0.7016501), float2(6.1378870, -4.6584760), float2(-1.3971590, 7.6802030),
    float2(-4.2106480, -6.6910780), float2(7.7192780, 2.1154980), float2(-7.2072710, 3.6986930), float2(2.8493120, -7.6856360),
    float2(3.1263100, 7.6796020), float2(-7.5774060, -3.5910310), float2(8.1015960, -2.4978510), float2(-4.3329320, 7.3934640),
    float2(-1.8182850, -8.4672280), float2(7.1334430, 5.0672090), float2(-8.7709830, 1.0931570), float2(5.7860390, -6.7977490),
    float2(0.3285438, 9.0078960), float2(-6.3875520, -6.4816530), float2(9.1736150, 0.4690015), float2(-7.1464080, 5.9047870),
    float2(1.2925040, -9.2644270), float2(5.3521360, 7.7728580), float2(-9.2773140, -2.1346290), float2(8.3538210, -4.7330090),
    float2(-2.9877480, 9.2099720), float2(-4.0515210, -8.8824490), float2(9.0608430, 3.8440030), float2(-9.3522910, 3.3124590)
};


/*

#define SAMPLE_COUNT 18
static float2 offsets[SAMPLE_COUNT] = {
  float2(-0.7393085f, 3.280662f),
  float2( -2.47004f, 2.328731f ),
  float2( -0.6732481f, 1.042242f ),
  float2( 0.6072469f, 1.525136f ),
  float2( 0.9831414f, 3.14807f ),
  float2( 1.894908f, 0.6981092f ),
  float2( 0.5978739f, 0.0825575f ),
  float2( 2.06167f, -0.6915861f ),
  float2( -1.294738f, -0.2353872f ),
  float2( 3.23345f, 1.27049f ),
  float2( -2.976625f, 0.1078734f ),
  float2( -1.566728f, -2.490001f ),
  float2( 0.022746f, -1.93031f ),
  float2( -2.484528f, -1.378844f ),
  float2( 1.984003f, -2.342571f ),
  float2( -0.2734823f, -3.234874f ),
  float2( 3.35825f, -0.3363621f ),
  float2( 2.090277f, 2.286526f )
};



#define SAMPLE_COUNT 51
static float2 offsets[SAMPLE_COUNT] = {
  float2( -3.412354f, -5.284019f ),
  float2( -3.254736f, -3.330548f ),
  float2( -5.816196f, -3.46201f ),
  float2( -1.492874f, -6.775923f ),
  float2( -1.363082f, -4.576695f ),
  float2( 0.5120577f, -7.036356f ),
  float2( 0.8487682f, -5.458392f ),
  float2( -2.482593f, -1.455628f ),
  float2( -5.299086f, -4.989777f ),
  float2( -4.506917f, -2.084349f ),
  float2( -0.9384603f, -2.567248f ),
  float2( 2.992283f, -6.154196f ),
  float2( -6.816841f, -0.8215709f ),
  float2( -6.176595f, 0.8565118f ),
  float2( -4.742037f, 1.505409f ),
  float2( -4.878184f, -0.06490026f ),
  float2( -2.135786f, 2.793251f ),
  float2( -3.964286f, 3.217015f ),
  float2( -3.306162f, 0.3788477f ),
  float2( -6.248262f, 3.178992f ),
  float2( -0.9384207f, 1.089011f ),
  float2( 2.814241f, -4.365722f ),
  float2( 4.603385f, -5.340619f ),
  float2( -2.667056f, 4.525906f ),
  float2( -3.413226f, 6.194156f ),
  float2( -5.432701f, 4.530499f ),
  float2( 0.5791532f, -1.817929f ),
  float2( 0.3678799f, -3.957979f ),
  float2( 0.2722762f, 4.098415f ),
  float2( 0.2881673f, 2.266828f ),
  float2( -1.376121f, 5.507777f ),
  float2( 4.755373f, -3.748413f ),
  float2( -0.8779002f, -0.7270181f ),
  float2( 2.496858f, -0.6773291f ),
  float2( 1.174333f, 0.8396815f ),
  float2( 3.040598f, -2.650676f ),
  float2( 2.469612f, 2.670604f ),
  float2( 6.43206f, -3.162919f ),
  float2( 4.905748f, -1.159387f ),
  float2( 6.753581f, -1.277563f ),
  float2( 4.555129f, 4.07637f ),
  float2( 3.289762f, 5.106628f ),
  float2( 4.039778f, 1.9229f ),
  float2( 1.417698f, 5.273744f ),
  float2( 6.49455f, 0.9413887f ),
  float2( 5.521292f, 2.777053f ),
  float2( 2.831632f, 0.8962862f ),
  float2( 2.649947f, 6.684711f ),
  float2( 0.9264914f, 6.971447f ),
  float2( 4.845179f, 5.70947f ),
  float2( 4.915648f, 0.6151388f )
};
*/

/*
#define SAMPLE_COUNT 64
static float2 offsets[SAMPLE_COUNT] = {
    float2(-0.48438, 0.00000), float2(-0.46875, -0.25000), float2(-0.45313, 0.25000), float2(-0.43750, -0.37500), float2(-0.42188, 0.12500), float2(-0.40625, -0.12500), float2(-0.39063, 0.37500), float2(-0.37500, -0.43750), float2(-0.35938, 0.06250), float2(-0.34375, -0.18750), float2(-0.32813, 0.31250), float2(-0.31250, -0.31250), float2(-0.29688, 0.18750), float2(-0.28125, -0.06250), float2(-0.26563, 0.43750), float2(-0.25000, -0.46875), float2(-0.23438, 0.03125), float2(-0.21875, -0.21875), float2(-0.20313, 0.28125), float2(-0.18750, -0.34375), float2(-0.17188, 0.15625), float2(-0.15625, -0.09375), float2(-0.14063, 0.40625), float2(-0.12500, -0.40625), float2(-0.10938, 0.09375), float2(-0.09375, -0.15625), float2(-0.07813, 0.34375), float2(-0.06250, -0.28125), float2(-0.04688, 0.21875), float2(-0.03125, -0.03125), float2(-0.01563, 0.46875), float2(0.00000, -0.48438), float2(0.01563, 0.01563), float2(0.03125, -0.23438), float2(0.04688, 0.26563), float2(0.06250, -0.35938), float2(0.07813, 0.14063), float2(0.09375, -0.10938), float2(0.10938, 0.39063), float2(0.12500, -0.42188), float2(0.14063, 0.07813), float2(0.15625, -0.17188), float2(0.17188, 0.32813), float2(0.18750, -0.29688), float2(0.20313, 0.20313), float2(0.21875, -0.04688), float2(0.23438, 0.45313), float2(0.25000, -0.45313), float2(0.26563, 0.04688), float2(0.28125, -0.20313), float2(0.29688, 0.29688), float2(0.31250, -0.32813), float2(0.32813, 0.17188), float2(0.34375, -0.07813), float2(0.35938, 0.42188), float2(0.37500, -0.39063), float2(0.39063, 0.10938), float2(0.40625, -0.14063), float2(0.42188, 0.35938), float2(0.43750, -0.26563), float2(0.45313, 0.23438), float2(0.46875, -0.01563), float2(0.48438, 0.48438), float2(0.50000, -0.49219)
};
*/

float2 GetRSMCoords(float3 worldPos) {
    float4 shadowClipPos = mul(_RadiantWorldToShadowMap, float4(worldPos, 1.0));
    shadowClipPos.xy /= shadowClipPos.w;
    shadowClipPos.xy = shadowClipPos.xy * 0.5 + 0.5;
    return shadowClipPos.xy;
}

half4 FragRSM (VaryingsRGI input) : SV_Target  {
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    input.uv = UnityStereoTransformScreenSpaceTex(input.uv);
    float2 uv = input.uv;

    float rawDepth = GetRawDepth(uv);
    float depth = LinearEyeDepth(rawDepth, _ZBufferParams);
    if (depth > _ProjectionParams.z - 20) return 0; // exclude skybox
   
    float3 worldPosM = GetWorldPosition(uv, rawDepth);
    half3 normalM = GetWorldNormal(uv);
    float2 rsmCoords = GetRSMCoords(worldPosM);

    float2 pos = uv * SOURCE_SIZE;
    float screenNoise = SAMPLE_TEXTURE2D_LOD(_NoiseTex, sampler_PointRepeat, pos * _NoiseTex_TexelSize.xy, 0).x;
    screenNoise = frac(screenNoise + GOLDEN_RATIO_ACUM);

    float si, co;
    sincos(screenNoise * 2.0 * PI, si, co);
    float2x2 rotMatrix = float2x2(co, -si, si, co);
    
    float texelMultiplier = _RadiantShadowMapColors_TexelSize.z / 128;
    float2 texelSize = _RadiantShadowMapColors_TexelSize.xy * texelMultiplier;
    half3 indirect = 0;

    for (int k = 0; k < SAMPLE_COUNT; k++) {

        float2 offset = offsets[k];
        offset = mul(offset, rotMatrix);
        float2 uvN = rsmCoords + offset * texelSize;

        float2 noise = SAMPLE_TEXTURE2D_LOD(_NoiseTex, sampler_PointRepeat, uvN * SOURCE_SIZE, 0).xw;
        uvN += (noise.xy - 0.5) * 0.03;

        half4 colorN = SAMPLE_TEXTURE2D_X_LOD(_RadiantShadowMapColors, sampler_LinearClamp, uvN, 0);
        half3 normalN = SAMPLE_TEXTURE2D_X_LOD(_RadiantShadowMapNormals, sampler_LinearClamp, uvN, 0).xyz;
        float3 worldPosN = SAMPLE_TEXTURE2D_X_LOD(_RadiantShadowMapWorldPos, sampler_LinearClamp, uvN, 0).xyz;

        half3 toM = worldPosM - worldPosN;
        half3 toMnorm = normalize(toM); // neighbour to center pixel
        half w = max(0, dot(toMnorm, normalN));

        half3 toNnorm = -toMnorm; // center pixel to neighbour
        w *= max(0, dot(toNnorm, normalM));

        // quadratic distance attenuation
        half dist = dot2(toM);
        half distSqr = 1.0 + dist * dist;
        half distAtten = rcp(distSqr);
        half invDistSqrWeight = lerp(1.0, distAtten, INDIRECT_DISTANCE_ATTENUATION);
   		w *= invDistSqrWeight;

        indirect += colorN.rgb * w;
    }

    indirect *= RSM_INTENSITY;

    half luma = GetLuma(indirect);
    indirect *= saturate(INDIRECT_MAX_BRIGHTNESS / (luma + 0.001));
    indirect *= INDIRECT_INTENSITY * (luma > LUMA_THRESHOLD);

    return half4(indirect, 0.0);
   
}

#endif // RGI_WIDE