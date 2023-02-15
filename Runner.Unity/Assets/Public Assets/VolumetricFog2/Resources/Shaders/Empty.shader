Shader "VolumetricFog2/Empty"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0.9,0.9,0.9,0.3)
    }
    SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent+100" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Always
			Cull Front
			ZWrite Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
                #include "UnityCG.cginc"

                half4 _Color;

				struct appdata
				{
					float4 vertex : POSITION;
				};

				struct v2f
				{
					float4 pos     : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex.xyz);
					#if defined(UNITY_REVERSED_Z)
						o.pos.z = 1.0e-9f;
					#else
						o.pos.z = o.pos.w - 1.0e-6f;
					#endif
					return o;
				}

				half4 frag(v2f i) : SV_Target
				{ 
					return _Color;
				}
				ENDCG
			}

		}
}