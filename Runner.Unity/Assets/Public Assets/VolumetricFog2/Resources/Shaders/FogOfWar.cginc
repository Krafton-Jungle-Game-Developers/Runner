#ifndef VOLUMETRIC_FOG_2_FOW
#define VOLUMETRIC_FOG_2_FOW


CBUFFER_START(VolumetricFog2FogOfWarBuffers)
    sampler2D _FogOfWar;
    float3 _FogOfWarCenter;
    float3 _FogOfWarSize;
    float3 _FogOfWarCenterAdjusted;
CBUFFER_END


half4 ApplyFogOfWar(float3 wpos) {
    float2 fogTexCoord = wpos.xz / _FogOfWarSize.xz - _FogOfWarCenterAdjusted.xz;
    half4 fowColor = tex2Dlod(_FogOfWar, float4(fogTexCoord, 0, 0));
    return half4(fowColor.rgb * fowColor.a, fowColor.a);
}

#endif