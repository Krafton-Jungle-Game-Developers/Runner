Shader "Hidden/VolumetricFog2/DepthOnly"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaCutOff("Alpha CutOff", Float) = 0
    }
    SubShader
    {
        ColorMask 0
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ DEPTH_PREPASS_ALPHA_TEST

            #include "UnityCG.cginc"

            half _AlphaCutOff;
            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                #if DEPTH_PREPASS_ALPHA_TEST
                    float2 uv : TEXCOORD0;
                #endif
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                #if DEPTH_PREPASS_ALPHA_TEST
                    float2 uv : TEXCOORD0;
                #endif
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                #if DEPTH_PREPASS_ALPHA_TEST
                    o.uv = v.uv;
                #endif
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                #if DEPTH_PREPASS_ALPHA_TEST
                    half4 color = tex2D(_MainTex, i.uv);
                    clip(color.a - _AlphaCutOff);
                #endif
                return 0;
            }
            ENDCG
        }
    }
}
