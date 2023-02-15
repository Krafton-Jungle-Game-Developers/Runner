#ifndef VOLUMETRIC_FOG_2_SHADOWS_URP
#define VOLUMETRIC_FOG_2_SHADOWS_URP

#if VF2_RECEIVE_SHADOWS

float GetLightAttenuation(float3 wpos) {
	float4 shadowCoord = TransformWorldToShadowCoord(wpos);
	float atten = MainLightRealtimeShadow(shadowCoord);
    return atten;
}

#endif

#endif // VOLUMETRIC_FOG_2_SHADOWS_URP