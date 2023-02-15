#ifndef VOLUMETRIC_FOG_2_COMMONS_URP
#define VOLUMETRIC_FOG_2_COMMONS_URP

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

// ***** Custom options ************

//#define ORTHO_SUPPORT
//#define USE_ALTERNATE_RECONSTRUCT_API

#define MAX_ITERATIONS 500

//#define FOG_BLUE_NOISE

//#define WEBGL_COMPATIBILITY_MODE

//#define FOG_VOID_ROTATION

#define USE_WORLD_SPACE_NOISE

//#define FOG_ROTATION

//#define FOG_BORDER

// ***** Common URP code ***********

#if defined(USE_ALTERNATE_RECONSTRUCT_API)
   #define CLAMP_RAY_DEPTH(rayStart, scrPos, t1) ClampRayDepthAlt(rayStart, scrPos, t1)
#else
   #define CLAMP_RAY_DEPTH(rayStart, scrPos, t1) ClampRayDepth(rayStart, scrPos, t1)
#endif

TEXTURE2D_X_FLOAT(_CustomDepthTexture);
SAMPLER(sampler_CustomDepthTexture);
int VF2_FLIP_DEPTH_TEXTURE;


inline float GetRawDepth(float2 uv) {
    float sceneDepth = SampleSceneDepth(VF2_FLIP_DEPTH_TEXTURE ? float2(uv.x, 1.0 - uv.y) : uv);
    #if VF2_DEPTH_PREPASS
        float customDepth = SAMPLE_TEXTURE2D_X(_CustomDepthTexture, sampler_CustomDepthTexture, uv ).r;
        #if UNITY_REVERSED_Z
            sceneDepth = max(sceneDepth, customDepth);
        #else
            sceneDepth = min(sceneDepth, customDepth);
        #endif
    #endif
    return sceneDepth;
}


void ClampRayDepth(float3 rayStart, float4 scrPos, inout float t1) {

    float2 uv =  scrPos.xy / scrPos.w;

    // World position reconstruction
    float depth  = GetRawDepth(uv);
    float4 raw   = mul(UNITY_MATRIX_I_VP, float4(uv * 2.0 - 1.0, depth, 1.0));
    float3 worldPos  = raw.xyz / raw.w;

    float z = distance(rayStart, worldPos.xyz);
    t1 = min(t1, z);
}


// Alternate reconstruct API; URP 7.4 doesn't set UNITY_MATRIX_I_VP correctly in VR, so we need to use this alternate method

inline float GetLinearEyeDepth(float2 uv) {
    float rawDepth = SampleSceneDepth(VF2_FLIP_DEPTH_TEXTURE ? float2(uv.x, 1.0 - uv.y) : uv);
  	float sceneLinearDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
    #if defined(ORTHO_SUPPORT)
        #if UNITY_REVERSED_Z
              rawDepth = 1.0 - rawDepth;
        #endif
        float orthoDepth = lerp(_ProjectionParams.y, _ProjectionParams.z, rawDepth);
        sceneLinearDepth = lerp(sceneLinearDepth, orthoDepth, unity_OrthoParams.w);
    #endif
    #if VF2_DEPTH_PREPASS
        float customRawDepth = SAMPLE_TEXTURE2D_X(_CustomDepthTexture, sampler_CustomDepthTexture, uv).r;
        float customLinearDepth = LinearEyeDepth(customRawDepth, _ZBufferParams);
        #if defined(ORTHO_SUPPORT)
            #if UNITY_REVERSED_Z
                customRawDepth = 1.0 - customRawDepth;
            #endif
            float customOrthoDepth = lerp(_ProjectionParams.y, _ProjectionParams.z, customRawDepth);
            customLinearDepth = lerp(customLinearDepth, customOrthoDepth, unity_OrthoParams.w);
        #endif
        sceneLinearDepth = min(sceneLinearDepth, customLinearDepth);
    #endif
    return sceneLinearDepth;
}


void ClampRayDepthAlt(float3 rayStart, float4 scrPos, inout float t1) {
    float2 uv =  scrPos.xy / scrPos.w;
    float vz = GetLinearEyeDepth(uv);

    #if defined(ORTHO_SUPPORT)
        if (unity_OrthoParams.w) {
            t1 = min(t1, vz);
            return;
        }
    #endif
    float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
    float2 suv = uv;
    #if UNITY_SINGLE_PASS_STEREO
        // If Single-Pass Stereo mode is active, transform the
        // coordinates to get the correct output UV for the current eye.
        float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
        suv = (suv - scaleOffset.zw) / scaleOffset.xy;
    #endif
    float3 vpos = float3((suv * 2 - 1) / p11_22, -1) * vz;
    float4 wpos = mul(unity_CameraToWorld, float4(vpos, 1));
    float z = distance(rayStart, wpos.xyz);
    t1 = min(t1, z);
}


#endif // VOLUMETRIC_FOG_2_COMMONS_URP

