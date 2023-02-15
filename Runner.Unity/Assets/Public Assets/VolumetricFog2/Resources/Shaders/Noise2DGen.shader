Shader "VolumetricFog2/Noise2DGen" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Color ("Color", Color) = (1,1,1)
    }

    SubShader {
    ZTest Always Cull Off ZWrite Off
    Pass
       {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
          
        #include "UnityCG.cginc"
        #include "Lighting.cginc"

        struct appdata {
              float4 vertex : POSITION;
              float2 uv : TEXCOORD0;
         };

        struct v2f {
            float4 vertex : SV_POSITION;
            float4 uv : TEXCOORD0;
         };

        sampler2D _MainTex;
        half3 _Color;
        half3 _SunDir;
        half3 _SpecularColor;
        half _SpecularIntensity;
        float _SpecularThreshold;

        v2f vert (appdata v) {
              v2f o;
             o.vertex = UnityObjectToClipPos(v.vertex);
             o.uv = float4(v.uv, 0, 0);
             return o;
        }
          
        half4 frag (v2f i) : SV_Target {

                half3 specularColor = _SpecularColor * ((1.0 + _SpecularIntensity) * _SpecularIntensity);

                half lcr = _Color.r * 0.5;
                half lcg = _Color.g * 0.5;
                half lcb = _Color.b * 0.5;
                half scr = specularColor.r * 0.5;
                half scg = specularColor.g * 0.5;
                half scb = specularColor.b * 0.5;

                half2 nlight = normalize(-_SunDir.xz) * 0.3;
                half a = tex2D(_MainTex, i.uv.xy).r;
                half an = tex2D(_MainTex, i.uv.xy + nlight).r;
                half r = saturate ((a - an) * _SpecularThreshold);
                half cor = lcr + scr * r;
                half cog = lcg + scg * r;
                half cob = lcb + scb * r;

                return half4(cor, cog, cob, a);
            }
            ENDCG
      }
  }
}

