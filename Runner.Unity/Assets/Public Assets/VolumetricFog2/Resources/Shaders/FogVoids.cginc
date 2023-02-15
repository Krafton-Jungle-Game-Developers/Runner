#ifndef VOLUMETRIC_FOG_2_FOG_VOIDS
#define VOLUMETRIC_FOG_2_FOG_VOIDS

#define FOG_MAX_VOID 8

CBUFFER_START(VolumetricFog2FogVoidBuffers)
    float4 _VF2_FogVoidPositions[FOG_MAX_VOID];
    float4 _VF2_FogVoidSizes[FOG_MAX_VOID];
    #if defined(FOG_VOID_ROTATION)
        float4x4 _VF2_FogVoidMatrices[FOG_MAX_VOID];
    #endif
    int _VF2_FogVoidCount;
CBUFFER_END

half ApplyFogVoids(float3 wpos) {

    float sdf = 10.0;
    for (int k=0;k<_VF2_FogVoidCount;k++) {

        // sqr distance to void center
        #if defined(FOG_VOID_ROTATION)
            float3 vd = mul(_VF2_FogVoidMatrices[k], float4(wpos.xyz, 1.0)).xyz;
            vd *= vd;
        #else
            float3 vd = _VF2_FogVoidPositions[k].xyz - wpos.xyz;
            vd *= vd;
            // relative to void size
            vd *= _VF2_FogVoidSizes[k].xyz;
        #endif

        // rect
        float rect = max(vd.x, max(vd.y, vd.z));

        // circle
        float circ = vd.x + vd.y + vd.z;

        // roundness
        float voidd = lerp(rect, circ, _VF2_FogVoidSizes[k].w);

        // falloff
        voidd = lerp(1.0, voidd, _VF2_FogVoidPositions[k].w);

        // merge sdf
        sdf = min(sdf, voidd);
    }
    sdf = 1.0 - sdf;
    return saturate(sdf);
}

#endif