#ifndef RGI_CAPTURE_NORMALS
#define RGI_CAPTURE_NORMALS

	// Copyright 2022 Kronnect - All Rights Reserved.

    UNITY_DECLARE_SCREENSPACE_TEXTURE(_RadiantShadowMapWorldPos);
    float4 _RadiantShadowMapWorldPos_TexelSize;

	struct AttributesFS {
		float4 pos  : POSITION;
		float2 uv   : TEXCOORD0;
	};

 	struct VaryingsRGI {
    	float4 pos : SV_POSITION;
    	float2 uv  : TEXCOORD0;
	};


	VaryingsRGI VertRGI(AttributesFS input) {
	    VaryingsRGI output;
        output.pos = UnityObjectToClipPos(input.pos);
        output.uv = input.uv;
    	return output;
	}


    float4 FragNormals(VaryingsRGI input) : SV_Target {

        float2 uv0 = input.uv;
        float2 uv1 = uv0 + float2(1, 0) * _RadiantShadowMapWorldPos_TexelSize.x;
        float2 uv2 = uv0 + float2(0, 1) * _RadiantShadowMapWorldPos_TexelSize.y;

        float3 wpos0 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_RadiantShadowMapWorldPos, uv0);
        float3 wpos1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_RadiantShadowMapWorldPos, uv1);
        float3 wpos2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_RadiantShadowMapWorldPos, uv2);

        float3 normal = normalize(cross(wpos2 - wpos0, wpos1 - wpos0));

        //normal = normalize(cross(ddx(wpos0), -ddy(wpos0)));

        return float4(normal, 1.0);
    }


#endif // RGI_CAPTURE_NORMALS