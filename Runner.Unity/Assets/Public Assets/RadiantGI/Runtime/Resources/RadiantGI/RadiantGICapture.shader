Shader "Hidden/Kronnect/RadiantGICapture" {
Properties {
}

HLSLINCLUDE
    #pragma target 3.0
ENDHLSL


Subshader {	

    ZWrite Off ZTest Always Cull Off

    HLSLINCLUDE
    #pragma target 3.0
    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    #include "UnityCG.cginc"
    ENDHLSL

  Pass {
      Name "World Positions"
      HLSLPROGRAM
      #pragma vertex VertRGI
      #pragma fragment FragWorldPos
      #include "RadiantGICapture_WorldPos.hlsl"
      ENDHLSL
  }

  Pass {
      Name "Normals"
      HLSLPROGRAM
      #pragma vertex VertRGI
      #pragma fragment FragNormals
      #include "RadiantGICapture_Normals.hlsl"
      ENDHLSL
  }

}
FallBack Off
}
