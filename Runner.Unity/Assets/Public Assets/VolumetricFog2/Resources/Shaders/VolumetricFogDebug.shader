Shader "Hidden/VolumetricFog2/VolumeDebug"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent+101" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 dd = abs(i.uv.xy - 0.5) * 2.0;
				float d = max(dd.x, dd.y);
				half b = 0.05 + 0.95 * (d > 0.995);
				return half4(1,1,1,b);
            }
            ENDCG
        }
    }
}
