Shader "VolumetricFog2/FoWBlur" {
Properties {
	_MainTex ("", 2D) = "white" {}
	_Color ("", Color) = (1,1,1,1)
}

SubShader {

	CGINCLUDE
	#include "UnityCG.cginc"

    struct appdata {
    	float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
    };

	struct v2fCross {
	    float4 pos : SV_POSITION;
	    float2 uv: TEXCOORD0;
	    float2 uv1: TEXCOORD1;
	    float2 uv2: TEXCOORD2;
	    float2 uv3: TEXCOORD3;
	    float2 uv4: TEXCOORD4;
	};
	
	sampler _MainTex;
	float4 _MainTex_ST;
	float4 _MainTex_TexelSize;
	
	v2fCross vertBlurH(appdata v) {
	    v2fCross o;
    	o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv = v.texcoord;
		float2 inc = float2(_MainTex_TexelSize.x * 1.3846153846, 0);	
    	o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc, _MainTex_ST);	
    	o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc, _MainTex_ST);	
		float2 inc2 = float2(_MainTex_TexelSize.x * 3.2307692308, 0);	
		o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc2, _MainTex_ST);
    	o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc2, _MainTex_ST);	
		return o;

	}	
	
	v2fCross vertBlurV(appdata v) {
    	v2fCross o;
    	o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv = v.texcoord;
    	float2 inc = float2(0, _MainTex_TexelSize.y * 1.3846153846);	
    	o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc, _MainTex_ST);	
    	o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc, _MainTex_ST);	
    	float2 inc2 = float2(0, _MainTex_TexelSize.y * 3.2307692308);	
    	o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc2, _MainTex_ST);	
    	o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc2, _MainTex_ST);	
    	return o;
	}
	
	#define PIX(uv) tex2D(_MainTex,uv)
	
	float4 fragBlur (v2fCross i): SV_Target {
		float4 pixel = PIX(i.uv) * 0.2270270270 + (PIX(i.uv1) + PIX(i.uv2)) * 0.3162162162 + (PIX(i.uv3) + PIX(i.uv4)) * 0.0702702703; 
        return pixel;
	}	
	
	
	ENDCG

	Pass { // Blur horizontally
	    ZTest Always Cull Off ZWrite Off
       	Fog { Mode Off }
		CGPROGRAM
		#pragma vertex vertBlurH
		#pragma fragment fragBlur
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma target 3.0
		ENDCG
	}
	
	Pass { // Blur vertically
	    ZTest Always Cull Off ZWrite Off
       	Fog { Mode Off }
		CGPROGRAM
		#pragma vertex vertBlurV
		#pragma fragment fragBlur
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma target 3.0
		ENDCG
	}
	
}

Fallback Off
}
