#ifndef RGI_CAPTURE_WORLDPOS
#define RGI_CAPTURE_WORLDPOS

	// Copyright 2022 Kronnect - All Rights Reserved.

    UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthAttachment);

	struct AttributesFS {
		float4 pos  : POSITION;
		float2 uv   : TEXCOORD0;
	};

 	struct VaryingsWorldPos {
    	float4 pos : SV_POSITION;
    	float2 uv  : TEXCOORD0;
        float3 cameraToFarPlane : TEXCOORD1;
	};


    float3 _ClipDir;
    float4x4 _ClipToWorld;
    float _FarClipPlane;

	VaryingsWorldPos VertRGI(AttributesFS input) {
	    VaryingsWorldPos output;
        output.pos = UnityObjectToClipPos(input.pos);
        output.uv = input.uv;
               
        float2 clipXY = output.pos.xy / output.pos.w;
        float4 farPlaneClip = float4(clipXY, 1.0, 1.0);
        farPlaneClip.y *= _ProjectionParams.x;  
        float4 farPlaneWorld4 = mul(_ClipToWorld, farPlaneClip);
        float3 farPlaneWorld = farPlaneWorld4.xyz / farPlaneWorld4.w;
        output.cameraToFarPlane = farPlaneWorld - _WorldSpaceCameraPos;

    	return output;
	}


    float GetDepth(float2 uv) {
        float depth01 = UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthAttachment, uv));
        #if UNITY_REVERSED_Z
            depth01 = 1.0 - depth01;
        #endif
        return depth01;
    }

    float3 GetWorldPos(float3 cameraToFarPlane, float depth01) {
        float3 worldPos = cameraToFarPlane - _ClipDir * (_FarClipPlane * (1.0 - depth01)) + _WorldSpaceCameraPos;
        return worldPos;
    }

    float4 FragWorldPos(VaryingsWorldPos input) : SV_Target {
        float depth01 = GetDepth(input.uv);
        float3 worldPos = GetWorldPos(input.cameraToFarPlane, depth01);
        return float4(worldPos, 1.0);
    }


#endif // RGI_CAPTURE_WORLDPOS