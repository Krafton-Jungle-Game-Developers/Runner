using UnityEngine;

namespace VolumetricFogAndMist2 {

    public static class ShaderParams {
        public static int DetailOffset = Shader.PropertyToID("_DetailOffset");
        public static int DetailColor = Shader.PropertyToID("_DetailColor");
        public static int DetailData = Shader.PropertyToID("_DetailData");
        public static int Density = Shader.PropertyToID("_Density");
        public static int ShadowIntensity = Shader.PropertyToID("_ShadowIntensity");
        public static int LightDiffusionIntensity = Shader.PropertyToID("_LightDiffusionIntensity");
        public static int LightDiffusionPower = Shader.PropertyToID("_LightDiffusionPower");
        public static int DeepObscurance = Shader.PropertyToID("_DeepObscurance");
        public static int NoiseScale = Shader.PropertyToID("_NoiseScale");
        public static int SpecularThreshold = Shader.PropertyToID("_SpecularThreshold");
        public static int SpecularIntensity = Shader.PropertyToID("_SpecularIntensity");
        public static int SpecularColor = Shader.PropertyToID("_SpecularColor");
        public static int NoiseFinalMultiplier = Shader.PropertyToID("_NoiseFinalMultiplier");
        public static int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
        public static int TurbulenceAmount = Shader.PropertyToID("_TurbulenceAmount");
        public static int FogOfWarCenterAdjusted = Shader.PropertyToID("_FogOfWarCenterAdjusted");
        public static int FogOfWarSize = Shader.PropertyToID("_FogOfWarSize");
        public static int FogOfWarCenter = Shader.PropertyToID("_FogOfWarCenter");
        public static int FogOfWarTexture = Shader.PropertyToID("_FogOfWar");
        public static int DistanceData = Shader.PropertyToID("_DistanceData");
        public static int MaxDistanceData = Shader.PropertyToID("_MaxDistanceData");
        public static int DepthGradientTexture = Shader.PropertyToID("_DepthGradientTex");
        public static int HeightGradientTexture = Shader.PropertyToID("_HeightGradientTex");
        public static int DetailTexture = Shader.PropertyToID("_DetailTex");
        public static int RaymarchSettings = Shader.PropertyToID("_RayMarchSettings");
        public static int SunDir = Shader.PropertyToID("_SunDir");
        public static int Color = Shader.PropertyToID("_Color");
        public static int MainTex = Shader.PropertyToID("_MainTex");
        public static int WindDirection = Shader.PropertyToID("_WindDirection");
        public static int DetailWindDirection = Shader.PropertyToID("_DetailWindDirection");
        public static int LightColor = Shader.PropertyToID("_LightColor");
        public static int BoundsBorder = Shader.PropertyToID("_BoundsBorder");
        public static int BoundsExtents = Shader.PropertyToID("_BoundsExtents");
        public static int BoundsCenter = Shader.PropertyToID("_BoundsCenter");
        public static int BoundsData = Shader.PropertyToID("_BoundsData");
        public static int RotationInvMatrix = Shader.PropertyToID("_InvRotMatrix");
        public static int RotationMatrix = Shader.PropertyToID("_RotMatrix");
        public static int SurfaceCaptureMatrix = Shader.PropertyToID("_SurfaceCaptureMatrix");
        public static int SurfaceDepthTexture = Shader.PropertyToID("_SurfaceDepthTexture");
        public static int SurfaceData = Shader.PropertyToID("_SurfaceData");
        public static int PointLightCount = Shader.PropertyToID("_VF2_PointLightCount");
        public static int PointLightInsideAtten = Shader.PropertyToID("_VF2_PointLightInsideAtten");
        public static int PointLightPositions = Shader.PropertyToID("_VF2_FogPointLightPosition");
        public static int PointLightColors = Shader.PropertyToID("_VF2_PointLightColor");
        public static int VoidCount = Shader.PropertyToID("_VF2_FogVoidCount");
        public static int VoidSizes = Shader.PropertyToID("_VF2_FogVoidSizes");
        public static int VoidPositions = Shader.PropertyToID("_VF2_FogVoidPositions");
        public static int VoidMatrices = Shader.PropertyToID("_VF2_FogVoidMatrices");
        public const string CustomDepthTextureName = "_CustomDepthTexture";
        public static int CustomDepthTexture = Shader.PropertyToID(CustomDepthTextureName);
        public static int CustomDepthAlphaCutoff = Shader.PropertyToID("_AlphaCutOff");
        public static int CustomDepthBaseMap = Shader.PropertyToID("_BaseMap");
        public static int SpotLightCount = Shader.PropertyToID("_VF2_SpotLightCount");
        public static int SpotLightPositions = Shader.PropertyToID("_VF2_FogSpotLightPosition");
        public static int SpotLightDirections = Shader.PropertyToID("_VF2_FogSpotLightDirection");
        public static int SpotLightColors = Shader.PropertyToID("_VF2_SpotLightColor");
        public static int BlueNoiseTexture = Shader.PropertyToID("_BlueNoise");

        public const string SKW_CUSTOM_DEPTH_ALPHA_TEST = "DEPTH_PREPASS_ALPHA_TEST";

        public const string SKW_SHAPE_BOX = "VF2_SHAPE_BOX";
        public const string SKW_SHAPE_SPHERE = "VF2_SHAPE_SPHERE";
        public const string SKW_POINT_LIGHTS = "VF2_POINT_LIGHTS";
        public const string SKW_NATIVE_LIGHTS = "VF2_NATIVE_LIGHTS";
        public const string SKW_VOIDS = "VF2_VOIDS";
        public const string SKW_FOW = "VF2_FOW";
        public const string SKW_RECEIVE_SHADOWS = "VF2_RECEIVE_SHADOWS";
        public const string SKW_DISTANCE = "VF2_DISTANCE";
        public const string SKW_DETAIL_NOISE = "VF2_DETAIL_NOISE";
        public const string SKW_SURFACE = "VF2_SURFACE";
        public const string SKW_DEPTH_PREPASS = "VF2_DEPTH_PREPASS";
        public const string SKW_DEPTH_GRADIENT = "VF2_DEPTH_GRADIENT";
        public const string SKW_HEIGHT_GRADIENT = "VF2_HEIGHT_GRADIENT";
        public const string SKW_DIRECTIONAL_COOKIE = "VF2_LIGHT_COOKIE";

    }

}

