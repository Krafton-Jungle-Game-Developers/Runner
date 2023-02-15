#ifndef RGI_BLUR
#define RGI_BLUR

	// Copyright 2022 Kronnect - All Rights Reserved.
    #define _BlurScale 1.0

    #define DEPTH_DIFF_MULTIPLIER 50
    #define NORMAL_DIFF_MULTIPLIER 0.5

#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED) || defined(SINGLE_PASS_STEREO)
    #define RGI_VERTEX_CROSS_UV_DATA
    #define RGI_VERTEX_OUTPUT_CROSS_UV(o)
    #define RGI_VERTEX_OUTPUT_GAUSSIAN_UV(o)

    #define RGI_FRAG_SETUP_CROSS_UV(i) float3 uvInc = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0); float2 uvN = i.uv + uvInc.zy; float2 uvE = i.uv + uvInc.xz; float2 uvW = i.uv - uvInc.xz; float2 uvS = i.uv - uvInc.zy;

    #if defined(RGI_BLUR_HORIZ)
        #define RGI_FRAG_SETUP_GAUSSIAN_UV(i) float2 inc = float2(_MainTex_TexelSize.x * 1.3846153846 * _BlurScale, 0); float2 uv1 = i.uv - inc; float2 uv2 = i.uv + inc; float2 inc2 = float2(_MainTex_TexelSize.x * 3.2307692308 * _BlurScale, 0); float2 uv3 = i.uv - inc2; float2 uv4 = i.uv + inc2;
    #else
        #define RGI_FRAG_SETUP_GAUSSIAN_UV(i) float2 inc = float2(0, _MainTex_TexelSize.y * 1.3846153846 * _BlurScale); float2 uv1 = i.uv - inc; float2 uv2 = i.uv + inc; float2 inc2 = float2(0, _MainTex_TexelSize.y * 3.2307692308 * _BlurScale); float2 uv3 = i.uv - inc2; float2 uv4 = i.uv + inc2;
    #endif

#else
    #define RGI_VERTEX_CROSS_UV_DATA float2 uvN : TEXCOORD1; float2 uvW: TEXCOORD2; float2 uvE: TEXCOORD3; float2 uvS: TEXCOORD4;

    #define RGI_VERTEX_OUTPUT_CROSS_UV(o) float3 uvInc = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0); o.uvN = o.uv + uvInc.zy; o.uvE = o.uv + uvInc.xz; o.uvW = o.uv - uvInc.xz; o.uvS = o.uv - uvInc.zy;
    #define RGI_FRAG_SETUP_CROSS_UV(i) float2 uv1 = i.uv1; float2 uv2 = i.uv2; float2 uv3 = i.uv3; float2 uv4 = i.uv4;

    #if defined(RGI_BLUR_HORIZ)
        #define RGI_VERTEX_OUTPUT_GAUSSIAN_UV(o) float2 inc = float2(_MainTex_TexelSize.x * 1.3846153846 * _BlurScale, 0); o.uv1 = o.uv - inc; o.uv2 = o.uv + inc; float2 inc2 = float2(_MainTex_TexelSize.x * 3.2307692308 * _BlurScale, 0); o.uv3 = o.uv - inc2; o.uv4 = o.uv + inc2;
    #else
        #define RGI_VERTEX_OUTPUT_GAUSSIAN_UV(o) float2 inc = float2(0, _MainTex_TexelSize.y * 1.3846153846 * _BlurScale); o.uv1 = o.uv - inc; o.uv2 = o.uv + inc; float2 inc2 = float2(0, _MainTex_TexelSize.y * 3.2307692308 * _BlurScale); o.uv3 = o.uv - inc2; o.uv4 = o.uv + inc2;
    #endif
    #define RGI_FRAG_SETUP_GAUSSIAN_UV(i) float2 uv1 = i.uv1; float2 uv2 = i.uv2; float2 uv3 = i.uv3; float2 uv4 = i.uv4;

#endif

#define uvN uv1
#define uvE uv2
#define uvW uv3
#define uvS uv4

 	struct VaryingsCross {
    	float4 positionCS : SV_POSITION;
    	float2 uv  : TEXCOORD0;
        RGI_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsCross VertBlur(AttributesFS input) {
    	VaryingsCross output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_TRANSFER_INSTANCE_ID(input, output);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        output.positionCS = float4(input.positionHCS.xyz, 1.0);

		#if UNITY_UV_STARTS_AT_TOP
		    output.positionCS.y *= -1;
		#endif

    	output.uv = input.uv;

        RGI_VERTEX_OUTPUT_GAUSSIAN_UV(output)

    	return output;
	}

    void ComputeSample(float depthM, float3 normM, float2 uv, float gaussWeight, inout float4 sum, inout float sumWeight) {
        float4 c = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv);
        float depth = c.w;
        float dDiff = abs(depthM - depth);
        float r = dDiff * DEPTH_DIFF_MULTIPLIER;

        float3 norm = GetWorldNormal(uv);
        float nDiff = 1 - max(0, dot(norm, normM));
        float n = nDiff * NORMAL_DIFF_MULTIPLIER;

        r += n;

        float g = exp(-r*r);
        float w = g * gaussWeight;
        sum += c * w;
        sumWeight += w;
    }

	float4 FragBlur (VaryingsCross input) : SV_Target  {

        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        input.uv = UnityStereoTransformScreenSpaceTex(input.uv);
        RGI_FRAG_SETUP_GAUSSIAN_UV(input)

        float2 uv = input.uv;

	    float4 rgbM = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv);
       
        float depthM = rgbM.w;
        if (depthM == 0) return 0; // exclude skybox

        float3 normM = GetWorldNormal(uv);
        float w = 0.2270270270;
        float4 sum = rgbM * w;
        float sumWeight = w;

        ComputeSample(depthM, normM, uv1, 0.3162162162, sum, sumWeight);
        ComputeSample(depthM, normM, uv2, 0.3162162162, sum, sumWeight);
        ComputeSample(depthM, normM, uv3, 0.0702702703, sum, sumWeight);
        ComputeSample(depthM, normM, uv4, 0.0702702703, sum, sumWeight);
       

        float4 res = sum / sumWeight;
        return float4(res.xyz, depthM);
	}


#endif // RGI_BLUR