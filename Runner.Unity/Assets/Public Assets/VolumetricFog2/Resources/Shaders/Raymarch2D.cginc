#ifndef VOLUMETRIC_FOG_2_RAYMARCH
#define VOLUMETRIC_FOG_2_RAYMARCH 

sampler2D _MainTex;
sampler3D _DetailTex;
float jitter;
float _NoiseScale;
half4 _Color;

float3 _SunDir;
half _ShadowIntensity;
half _DeepObscurance;
half _LightDiffusionIntensity, _LightDiffusionPower;
float3 _WindDirection, _DetailWindDirection;
half4 _LightColor;
half  _Density;

float2 _MaxDistanceData;
#define FOG_MAX_LENGTH _MaxDistanceData.x
#define FOG_MAX_LENGTH_FALLOFF _MaxDistanceData.y

float4 _BoundsBorder;
#define BORDER_SIZE_SPHERE _BoundsBorder.x
#define BORDER_START_SPHERE _BoundsBorder.y
#define BORDER_SIZE_BOX _BoundsBorder.xz
#define BORDER_START_BOX _BoundsBorder.yw

float3 _BoundsData;
#define BOUNDS_VERTICAL_OFFSET _BoundsData.x
#define BOUNDS_BOTTOM _BoundsData.y
#define BOUNDS_SIZE_Y _BoundsData.z


half4 _DetailData; // x = strength, y = offset, z = scale, w = importance
half4 _DetailColor;
#define DETAIL_STRENGTH _DetailData.x
#define DETAIL_OFFSET _DetailData.y
#define DETAIL_SCALE _DetailData.z
#define USE_BASE_NOISE _DetailData.w

float4 _RayMarchSettings;
  
#define FOG_STEPPING _RayMarchSettings.x
#define DITHERING _RayMarchSettings.y
#define JITTERING _RayMarchSettings.z
#define MIN_STEPPING _RayMarchSettings.w

float loop_t;

#include "PointLights.cginc"
#include "FogVoids.cginc"
#include "FogOfWar.cginc"
#include "FogDistance.cginc"
#include "Surface.cginc"

TEXTURE2D(_BlueNoise);
SAMPLER(sampler_PointRepeat);
float4 _BlueNoise_TexelSize;

float3 _VFRTSize;

void SetJitter(float4 scrPos) {

    float2 screenSize = lerp(_ScreenParams.xy, _VFRTSize.xy, _VFRTSize.z);
    float2 uv = (scrPos.xy / scrPos.w) * screenSize;

    #if defined(FOG_BLUE_NOISE)
        float2 noiseUV = uv * _BlueNoise_TexelSize.xy;
        jitter =  SAMPLE_TEXTURE2D(_BlueNoise, sampler_PointRepeat, noiseUV).r;
    #else
        //Jitter = frac(dot(float2(2.4084507, 3.2535211), (scrPos.xy / scrPos.w) * _ScreenParams.xy));
        const float3 magic = float3( 0.06711056, 0.00583715, 52.9829189 );
        jitter = frac( magic.z * frac( dot( uv, magic.xy ) ) );
    #endif
}


inline float3 ProjectOnPlane(float3 v, float3 planeNormal) {
    float sqrMag = dot(planeNormal, planeNormal);
    float dt = dot(v, planeNormal);
	return v - planeNormal * dt / sqrMag;
}

inline float3 GetRayStart(float3 wpos) {
    float3 cameraPosition = GetCameraPositionWS();
    #if defined(ORTHO_SUPPORT)
	    float3 cameraForward = UNITY_MATRIX_V[2].xyz;
	    float3 rayStart = ProjectOnPlane(wpos - cameraPosition, cameraForward) + cameraPosition;
        return lerp(cameraPosition, rayStart, unity_OrthoParams.w);
    #else
        return cameraPosition;
    #endif
}


half4 SampleDensity(float3 wpos) {

    float3 boundsCenter = _BoundsCenter;
    float3 boundsExtents = _BoundsExtents;

    SurfaceApply(boundsCenter, boundsExtents);

#if VF2_DETAIL_NOISE
    #if !defined(USE_WORLD_SPACE_NOISE)
        wpos.xyz -= boundsCenter;
    #endif
    half detail = tex3Dlod(_DetailTex, float4(wpos * DETAIL_SCALE - _DetailWindDirection, 0)).a;
    half4 density = _DetailColor;
    if (USE_BASE_NOISE) {
        #if defined(USE_WORLD_SPACE_NOISE)
            wpos.y -= boundsCenter.y;
        #endif
        wpos.y /= boundsExtents.y;
        density = tex2Dlod(_MainTex, float4(wpos.xz * _NoiseScale - _WindDirection.xz, 0, 0));
        density.a -= abs(wpos.y);
    }
    density.a += (detail + DETAIL_OFFSET) * DETAIL_STRENGTH;
#else
    #if defined(USE_WORLD_SPACE_NOISE)
        wpos.y -= boundsCenter.y;
    #else
        wpos.xyz -= boundsCenter;
    #endif
    wpos.y /= boundsExtents.y;
    half4 density = tex2Dlod(_MainTex, float4(wpos.xz * _NoiseScale - _WindDirection.xz, 0, 0));
    density.a -= abs(wpos.y);
#endif

    return density;
}


#define dot2(x) dot(x,x)

void AddFog(float3 rayStart, float3 wpos, float rs, half4 baseColor, inout half4 sum) {

   half4 density = SampleDensity(wpos);

   float3 rotatedWPos = wpos;
   #if defined(FOG_ROTATION)
        rotatedWPos = Rotate(rotatedWPos);
    #endif

   #if VF2_VOIDS
        density.a -= ApplyFogVoids(rotatedWPos);
   #endif

   #if defined(FOG_BORDER)
   #if VF2_SHAPE_SPHERE
        float3 delta = wpos - _BoundsCenter;
        delta.y += BOUNDS_VERTICAL_OFFSET;
        float distSqr = dot2(delta);
        float border = 1.0 - saturate( (distSqr - BORDER_START_SPHERE) / BORDER_SIZE_SPHERE );
        density.a *= border * border;
   #else
        float2 dist2 = abs(wpos.xz - _BoundsCenter.xz);
        float2 border2 = saturate( (dist2 - BORDER_START_BOX) / BORDER_SIZE_BOX );
        float border = 1.0 - max(border2.x, border2.y);
        density.a *= border * border;
   #endif
   #endif

   if (density.a > 0) {
        half4 fgCol = baseColor * half4((1.0 - density.a * _DeepObscurance).xxx, density.a);
        #if VF2_RECEIVE_SHADOWS
            half shadowAtten = GetLightAttenuation(wpos);
            fgCol.rgb *= lerp(1.0, shadowAtten, _ShadowIntensity);
        #endif
        #if VF2_NATIVE_LIGHTS
            int additionalLightCount = GetAdditionalLightsCount();
            for (int i = 0; i < additionalLightCount; ++i) {
                #if UNITY_VERSION >= 202030
                    Light light = GetAdditionalLight(i, rotatedWPos, 1.0.xxxx);
                #else
                    Light light = GetAdditionalLight(i, rotatedWPos);
                #endif
                fgCol.rgb += light.color * (light.distanceAttenuation * light.shadowAttenuation);
            }
        #endif
        #if VF2_LIGHT_COOKIE
            half3 cookieColor = SampleMainLightCookie(wpos);
            fgCol.rgb *= cookieColor;
        #endif
		#if VF2_DEPTH_GRADIENT
			fgCol *= ApplyDepthGradient(rayStart, wpos);
		#endif
		#if VF2_HEIGHT_GRADIENT
			fgCol *= ApplyHeightGradient(wpos);
		#endif

        fgCol.rgb *= density.rgb * fgCol.aaa;
        #if VF2_FOW
            fgCol *= ApplyFogOfWar(rotatedWPos);
        #endif
		#if VF2_DISTANCE
			fgCol *= ApplyFogDistance(rayStart, wpos);
		#endif

        fgCol *= min(1.0, _Density * rs);
        sum += fgCol * (1.0 - sum.a);
   }
}


half4 GetFogColor(float3 rayStart, float3 viewDir, float t0, float t1) {

    t0 = min(t0 + jitter * JITTERING, t1);
    float len = t1 - t0;
    float rs = MIN_STEPPING + max(log(len), 0) / FOG_STEPPING;     // stepping ratio with atten detail with distance
    half4 sum = half4(0,0,0,0);
    half diffusion = 1.0 + pow(max(dot(viewDir, _SunDir.xyz), 0), _LightDiffusionPower) * _LightDiffusionIntensity;
    half3 diffusionColor = _LightColor.rgb * diffusion;
    half4 lightColor = half4(diffusionColor, 1.0);

    float3 wpos = rayStart + viewDir * t0;
    float3 endPos = rayStart + viewDir * t1;
    SurfaceComputeEndPoints(wpos, endPos);

    wpos.y -= BOUNDS_VERTICAL_OFFSET;
    viewDir *= rs;

    float energyStep = rs;
    rs /= len + 0.001;
    rs = max(rs, 1.0 / MAX_ITERATIONS);
    
    float t = 0;

    // Use this Unroll macro to support WebGL. Increase 50 value if needed.
    #if defined(WEBGL_COMPATIBILITY_MODE)
        UNITY_UNROLLX(50)
    #elif VF2_LIGHT_COOKIE
        UNITY_LOOP
    #endif
    while (t < 1.0) {
        loop_t = t;
        AddFog(rayStart, wpos, energyStep, lightColor, sum);
        if (sum.a > 0.99) {
            sum.a = 1;
            sum *= _LightColor.a;
            return sum;
        }
        t += rs;
        wpos += viewDir;
    }
    AddFog(rayStart, endPos, len * (rs - (t-1.0)), lightColor, sum);

    sum = max(0, sum - jitter * DITHERING);
    sum *= _LightColor.a;
    return sum;
}


#endif // VOLUMETRIC_FOG_2_RAYMARCH