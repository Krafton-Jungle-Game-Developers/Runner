#ifndef VOLUMETRIC_FOG_2_SURFACE
#define VOLUMETRIC_FOG_2_SURFACE

#if VF2_SURFACE

    float4x4 _SurfaceCaptureMatrix;
    TEXTURE2D_FLOAT(_SurfaceDepthTexture);
    SAMPLER(sampler_SurfaceDepthTexture);
    float4 _SurfaceData;

    #define SURFACE_CAM_ALTITUDE _SurfaceData.x
    #define TERRAIN_FOG_HEIGHT _SurfaceData.y
    #define TERRAIN_FOG_MIN_ALTITUDE _SurfaceData.z
    #define TERRAIN_FOG_MAX_ALTITUDE _SurfaceData.w

    float4 surfaceTexCoordsStart, surfaceTexCoordsEnd;

    void SurfaceComputeEndPoints(float3 wposStart, float3 wposEnd) {
		#if defined(FOG_ROTATION)
			wposStart = Rotate(wposStart);
            wposEnd = Rotate(wposEnd);
	    #endif

        surfaceTexCoordsStart = mul(_SurfaceCaptureMatrix, float4(wposStart, 1.0));
        surfaceTexCoordsStart.xy /= surfaceTexCoordsStart.w;
        surfaceTexCoordsEnd = mul(_SurfaceCaptureMatrix, float4(wposEnd, 1.0));
        surfaceTexCoordsEnd.xy /= surfaceTexCoordsEnd.w;
    }

    void SurfaceApply(inout float3 boundsCenter, inout float3 boundsExtents) {
        float2 surfaceTexCoords = lerp(surfaceTexCoordsStart.xy, surfaceTexCoordsEnd.xy, loop_t);
        float surfaceDepth = SAMPLE_TEXTURE2D_LOD(_SurfaceDepthTexture, sampler_SurfaceDepthTexture, surfaceTexCoords, 0).r;
        #if UNITY_REVERSED_Z
            surfaceDepth = 1.0 - surfaceDepth;
        #endif
        boundsExtents.y = TERRAIN_FOG_HEIGHT;
	    boundsCenter.y = clamp(SURFACE_CAM_ALTITUDE - surfaceDepth * 10000, TERRAIN_FOG_MIN_ALTITUDE, TERRAIN_FOG_MAX_ALTITUDE);
    }

#else

    #define SurfaceComputeEndPoints(wposStart, wposEnd)
    #define SurfaceApply(boundsCenter, boundsExtents)

#endif // VF2_SURFACE

#endif // VOLUMETRIC_FOG_2_SURFACE