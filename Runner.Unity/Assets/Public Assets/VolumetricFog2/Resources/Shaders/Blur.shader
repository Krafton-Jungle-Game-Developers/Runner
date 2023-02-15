Shader "Hidden/VolumetricFog2/Blur"
{
Properties
{
    _BlueNoise("BlueNoise Tex", 2D) = "black" {}
}
SubShader
{
    ZWrite Off ZTest Always Blend Off Cull Off

    HLSLINCLUDE
    #pragma target 3.0
    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    ENDHLSL

  Pass { // 0
      Name "Blur horizontally"
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #pragma multi_compile_local _ EDGE_PRESERVE EDGE_PRESERVE_UPSCALING
      #define BLUR_HORIZ
      #include "Blur.hlsl"
      ENDHLSL
  }    
      
  Pass { // 1
      Name "Blur vertically"
	  HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #pragma multi_compile_local _ EDGE_PRESERVE EDGE_PRESERVE_UPSCALING
      #define BLUR_VERT
      #include "Blur.hlsl"
      ENDHLSL
  }
      
  Pass { // 2
      Name "Final vertical blur and blend"
      Blend One OneMinusSrcAlpha
	  HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #pragma multi_compile_local _ EDGE_PRESERVE EDGE_PRESERVE_UPSCALING
      #pragma multi_compile_local _ DITHER
      #define BLUR_VERT
      #define FINAL_BLEND
      #include "Blur.hlsl"
      ENDHLSL
  }    
      
  Pass { // 3
      Name "Separated Blend (used with upscaling)"
      Blend One OneMinusSrcAlpha
	  HLSLPROGRAM
      #pragma vertex VertSimple
      #pragma fragment FragSeparatedBlend
      #pragma multi_compile_local _ DITHER
      #include "Blur.hlsl"
      ENDHLSL
  }
      
  Pass { // 4
      Name "Downscale depth"
	  HLSLPROGRAM
      #pragma vertex VertSimple
      #pragma fragment FragOnlyDepth
      #include "Blur.hlsl"
      ENDHLSL
  }

  Pass { // 5
      Name "Blur vertically final (used with downscaling)"
	  HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #define BLUR_VERT
      #define FINAL_BLEND
      #pragma multi_compile_local _ EDGE_PRESERVE EDGE_PRESERVE_UPSCALING
      #include "Blur.hlsl"
      ENDHLSL
  }
    
}
}
