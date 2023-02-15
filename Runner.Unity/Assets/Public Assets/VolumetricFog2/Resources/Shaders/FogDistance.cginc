#ifndef VOLUMETRIC_FOG_2_FOG_DISTANCE
#define VOLUMETRIC_FOG_2_FOG_DISTANCE

float4 _DistanceData;
TEXTURE2D(_DepthGradientTex);
SAMPLER(_depthGradient_linear_clamp_sampler);
TEXTURE2D(_HeightGradientTex);
SAMPLER(_heightGradient_linear_clamp_sampler);

inline half ApplyFogDistance(float3 rayStart, float3 wpos) {
    float3 vd = rayStart - wpos;
    float voidDistance = dot(vd, vd) * _DistanceData.w;
    half alpha = saturate(1.0 + (voidDistance - 1.0) * _DistanceData.y);
    return alpha;
}


inline half4 ApplyDepthGradient(float3 rayStart, float3 wpos) {
    float2 vd = rayStart.xz - wpos.xz;
    float depthGradientCoord = dot(vd, vd) * _DistanceData.z;
    half4 depthColor = SAMPLE_TEXTURE2D_LOD(_DepthGradientTex, _depthGradient_linear_clamp_sampler, float2(depthGradientCoord, 0), 0);
    return depthColor;
}


inline half4 ApplyHeightGradient(float3 wpos) {
    float heightGradientCoord = saturate( (wpos.y - BOUNDS_BOTTOM) / BOUNDS_SIZE_Y );
    half4 heightColor = SAMPLE_TEXTURE2D_LOD(_HeightGradientTex, _heightGradient_linear_clamp_sampler, float2(heightGradientCoord, 0), 0);
    return heightColor;
}


#endif