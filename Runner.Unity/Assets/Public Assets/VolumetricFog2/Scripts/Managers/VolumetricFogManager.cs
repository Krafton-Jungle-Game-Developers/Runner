//#define FOG_VOID_ROTATION

using UnityEngine;

namespace VolumetricFogAndMist2 {

    [ExecuteInEditMode]
    public class VolumetricFogManager : MonoBehaviour, IVolumetricFogManager {

        public string managerName {
            get {
                return "Volumetric Fog Manager";
            }
        }

        static PointLightManager _pointLightManager;
        static FogVoidManager _fogVoidManager;
        static VolumetricFogManager _instance;

        public Camera mainCamera;
        [Tooltip("Directional light used as the Sun")]
        public Light sun;
        [Tooltip("Directional light used as the Moon")]
        public Light moon;
        [Tooltip("Layer to be used for fog elements. This layer will be excluded from the depth pre-pass.")]
        public int fogLayer = 1;
        [Tooltip("Flip depth texture. Use only as a workaround to a bug in URP if the depth shows inverted in GameView. Alternatively you can enable MSAA or HDR instead of using this option.")]
        public bool flipDepthTexture;
        [Tooltip("Enable this option to choose this manager when others could be loaded from sub-scenes")]
        public bool mainManager;
        [Tooltip("Optionally specify which transparent layers must be included in the depth prepass. Use only to avoid fog clipping with certain transparent objects.")]
        public LayerMask includeTransparent;
        [Tooltip("Optionally specify which semi-transparent (materials using alpha clipping or cut-off) must be included in the depth prepass. Use only to avoid fog clipping with certain transparent objects.")]
        public LayerMask includeSemiTransparent;
        [Tooltip("Optionally determines the alpha cut off for semitransparent objects")]
        [Range(0, 1)]
        public float alphaCutOff;
        [Range(1, 8)]
        public float downscaling = 1;
        [Range(0, 6)]
        public int blurPasses;
        [Range(1, 8)]
        public float blurDownscaling = 1;
        [Range(0.1f, 4)]
        public float blurSpread = 1f;
        [Tooltip("Uses 32 bit floating point pixel format for rendering & blur fog volumes.")]
        public bool blurHDR = true;
        [Tooltip("Enable to use an edge-aware blur.")]
        public bool blurEdgePreserve = true;
        [Tooltip("Ignores blur when fog color intensity is below this value.")]
        public float blurEdgeDepthThreshold = 0.008f;
        [Range(0, 0.2f)]
        public float ditherStrength;

        // used to keep shader option in sync with scripting side
#if FOG_VOID_ROTATION
        public static bool allowFogVoidRotation => true;
#else
        public static bool allowFogVoidRotation => false;
#endif

        const string SKW_FLIP_DEPTH_TEXTURE = "VF2_FLIP_DEPTH_TEXTURE";

        public static VolumetricFogManager instance {
            get {
                if (_instance == null) {
                    _instance = Tools.CheckMainManager();
                }
                return _instance;
            }
        }

        public static VolumetricFogManager GetManagerIfExists() {
            if (_instance == null) {
                _instance = FindObjectOfType<VolumetricFogManager>();
            }
            return _instance;
        }

        public static PointLightManager pointLightManager {
            get {
                Tools.CheckManager(ref _pointLightManager);
                return _pointLightManager;
            }
        }

        public static FogVoidManager fogVoidManager {
            get {
                Tools.CheckManager(ref _fogVoidManager);
                return _fogVoidManager;
            }
        }

        void OnEnable() {
            // Forces other managers to be found
            _pointLightManager = null;
            _fogVoidManager = null;
            // Ensures no other fog manager exist
            VolumetricFogManager[] managers = FindObjectsOfType<VolumetricFogManager>(true);
            if (managers.Length > 1) {
                bool isThisTheMainManager = mainManager;
                for (int k = 0; k < managers.Length; k++) {
                    if (!managers[k].mainManager) DestroyImmediate(managers[k].gameObject);
                }
                if (!isThisTheMainManager) return;
            }
            SetupCamera();
            SetupLights();
            SetupDepthPrePass();
            Tools.CheckManager(ref _pointLightManager);
            Tools.CheckManager(ref _fogVoidManager);
        }

        void OnValidate() {
            blurEdgeDepthThreshold = Mathf.Max(0.0001f, blurEdgeDepthThreshold);
            SetupDepthPrePass();
        }

        void SetupCamera() {
            Tools.CheckCamera(ref mainCamera);
            if (mainCamera != null) {
                mainCamera.depthTextureMode |= DepthTextureMode.Depth;
            }
        }

        void SetupLights() {
            Light[] lights = FindObjectsOfType<Light>();
            for (int k = 0; k < lights.Length; k++) {
                Light l = lights[k];
                if (l.type == LightType.Directional) {
                    if (sun == null) {
                        sun = l;
                    }
                    return;
                }
            }
        }

        void SetupDepthPrePass() {
            Shader.SetGlobalInt(SKW_FLIP_DEPTH_TEXTURE, flipDepthTexture ? 1 : 0);
            DepthRenderPrePassFeature.DepthRenderPass.SetupLayerMasks(includeTransparent & ~(1 << fogLayer), includeSemiTransparent & ~(1 << fogLayer));
        }

        /// <summary>
        /// Creates a new fog volume
        /// </summary>
        public static GameObject CreateFogVolume(string name) {
            GameObject go = Resources.Load<GameObject>("Prefabs/FogVolume2D");
            go = Instantiate(go);
            go.name = name;
            return go;
        }

        /// <summary>
        /// Creates a new fog void
        /// </summary>
        public static GameObject CreateFogVoid(string name) {
            return new GameObject(name, typeof(FogVoid));
        }


        /// <summary>
        /// Creates a new fog sub-volume
        /// </summary>
        public static GameObject CreateFogSubVolume(string name) {
            GameObject go = Resources.Load<GameObject>("Prefabs/FogSubVolume");
            go = Instantiate(go);
            go.name = name;
            return go;
        }

    }
}
