//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist 2
// Created by Kronnect
//------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VolumetricFogAndMist2 {
    public class VolumetricFogRenderFeature : ScriptableRendererFeature {
        static class ShaderParams {
            public const string LightBufferName = "_LightBuffer";
            public static int LightBuffer = Shader.PropertyToID(LightBufferName);
            public static int LightBufferSize = Shader.PropertyToID("_VFRTSize");
            public static int MainTex = Shader.PropertyToID("_MainTex");
            public static int BlurRT = Shader.PropertyToID("_BlurTex");
            public static int BlurRT2 = Shader.PropertyToID("_BlurTex2");
            public static int MiscData = Shader.PropertyToID("_MiscData");
            public static int ForcedInvisible = Shader.PropertyToID("_ForcedInvisible");
            public static int DownsampledDepth = Shader.PropertyToID("_DownsampledDepth");
            public static int BlueNoiseTexture = Shader.PropertyToID("_BlueNoise");
            public static int BlurScale = Shader.PropertyToID("_BlurScale");
            public static int Downscaling = Shader.PropertyToID("_Downscaling");

            public const string SKW_DITHER = "DITHER";
            public const string SKW_EDGE_PRESERVE = "EDGE_PRESERVE";
            public const string SKW_EDGE_PRESERVE_UPSCALING = "EDGE_PRESERVE_UPSCALING";
        }

        public static int GetScaledSize(int size, float factor) {
            size = (int)(size / factor);
            size /= 2;
            if (size < 1)
                size = 1;
            return size * 2;
        }

        class VolumetricFogRenderPass : ScriptableRenderPass {
        

            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.transparent, -1);
            readonly List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
            const int renderingLayer = 1 << 49;
            const string m_ProfilerTag = "Volumetric Fog Light Buffer Rendering";
            RTHandle m_LightBuffer;

            public VolumetricFogRenderPass() {
                shaderTagIdList.Clear();
                shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                RenderTargetIdentifier lightBuffer = new RenderTargetIdentifier(ShaderParams.LightBuffer, 0, CubemapFace.Unknown, -1);
                m_LightBuffer = RTHandles.Alloc(lightBuffer, name: ShaderParams.LightBufferName);
            }

            public void CleanUp() {
                RTHandles.Release(m_LightBuffer);
            }

            public void Setup(VolumetricFogRenderFeature settings) {
                renderPassEvent = settings.renderPassEvent;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                RenderTextureDescriptor lightBufferDesc = cameraTextureDescriptor;
                VolumetricFogManager manager = VolumetricFogManager.GetManagerIfExists();
                if (manager != null) {
                    if (manager.downscaling > 1f) {
                        int size = GetScaledSize(cameraTextureDescriptor.width, manager.downscaling);
                        lightBufferDesc.width = size;
                        lightBufferDesc.height = size;
                    }
                    lightBufferDesc.colorFormat = manager.blurHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
                    cmd.SetGlobalVector(ShaderParams.LightBufferSize, new Vector4(lightBufferDesc.width, lightBufferDesc.height, manager.downscaling > 1f ? 1f: 0, 0));
                }
                lightBufferDesc.depthBufferBits = 0;
                lightBufferDesc.useMipMap = false;
                lightBufferDesc.msaaSamples = 1;
                cmd.GetTemporaryRT(ShaderParams.LightBuffer, lightBufferDesc, FilterMode.Bilinear);
                ConfigureTarget(m_LightBuffer);
                ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 0));
                ConfigureInput(ScriptableRenderPassInput.Depth);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
                VolumetricFogManager manager = VolumetricFogManager.GetManagerIfExists();

                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                cmd.SetGlobalInt(ShaderParams.ForcedInvisible, 0);
                context.ExecuteCommandBuffer(cmd);

                if (manager == null || (manager.downscaling <= 1f && manager.blurPasses < 1)) {
                    CommandBufferPool.Release(cmd);
                    return;
                }

                foreach (VolumetricFog vg in VolumetricFog.volumetricFogs) {
                    if (vg != null) {
                        vg.meshRenderer.renderingLayerMask = renderingLayer;
                    }
                }

                cmd.Clear();

                var sortFlags = SortingCriteria.CommonTransparent;
                var drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortFlags);
                var filterSettings = filteringSettings;
                filterSettings.layerMask = 1 << manager.fogLayer;
                filterSettings.renderingLayerMask = renderingLayer;

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings);

                CommandBufferPool.Release(cmd);

            }

            /// Cleanup any allocated resources that were created during the execution of this render pass.
            public override void FrameCleanup(CommandBuffer cmd) {
            }
        }


        class BlurRenderPass : ScriptableRenderPass {

            enum Pass {
                BlurHorizontal = 0,
                BlurVertical = 1,
                BlurVerticalAndBlend = 2,
                Blend = 3,
                DownscaleDepth = 4,
                BlurVerticalFinal = 5
            }

            ScriptableRenderer renderer;
            Material mat;
            RenderTextureDescriptor rtSourceDesc;

            public void Setup(Shader shader, ScriptableRenderer renderer, VolumetricFogRenderFeature settings) {
                this.renderPassEvent = settings.renderPassEvent;
                this.renderer = renderer;
                if (mat == null) {
                    mat = CoreUtils.CreateEngineMaterial(shader);
                    Texture2D noiseTex = Resources.Load<Texture2D>("Textures/blueNoiseVF128");
                    mat.SetTexture(ShaderParams.BlueNoiseTexture, noiseTex);
                }
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                rtSourceDesc = cameraTextureDescriptor;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
                VolumetricFogManager manager = VolumetricFogManager.GetManagerIfExists();
                if (manager == null || (manager.downscaling <= 1f && manager.blurPasses < 1)) {
                    Cleanup();
                    return;
                }

                Camera cam = renderingData.cameraData.camera;
                if ((cam.cullingMask & (1 << manager.fogLayer)) == 0) return;

                mat.SetVector(ShaderParams.MiscData, new Vector4(manager.ditherStrength * 0.1f, 0, manager.blurEdgeDepthThreshold, 0));
                if (manager.ditherStrength > 0) {
                    mat.EnableKeyword(ShaderParams.SKW_DITHER);
                } else {
                    mat.DisableKeyword(ShaderParams.SKW_DITHER);
                }
                mat.DisableKeyword(ShaderParams.SKW_EDGE_PRESERVE);
                mat.DisableKeyword(ShaderParams.SKW_EDGE_PRESERVE_UPSCALING);
                if (manager.blurPasses > 0 && manager.blurEdgePreserve) {
                    mat.EnableKeyword(manager.downscaling > 1f ? ShaderParams.SKW_EDGE_PRESERVE_UPSCALING : ShaderParams.SKW_EDGE_PRESERVE);
                }

#if UNITY_2022_1_OR_NEWER
                RTHandle source = renderer.cameraColorTargetHandle;
#else
                RenderTargetIdentifier source = renderer.cameraColorTarget;
#endif

                var cmd = CommandBufferPool.Get("Volumetric Fog Render Feature");

                cmd.SetGlobalInt(ShaderParams.ForcedInvisible, 1);

                RenderTextureDescriptor rtBlurDesc = rtSourceDesc;
                rtBlurDesc.width = GetScaledSize(rtSourceDesc.width, manager.downscaling);
                rtBlurDesc.height = GetScaledSize(rtSourceDesc.height, manager.downscaling);
                rtBlurDesc.useMipMap = false;
                rtBlurDesc.colorFormat = manager.blurHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
                rtBlurDesc.msaaSamples = 1;
                rtBlurDesc.depthBufferBits = 0;

                bool usingDownscaling = manager.downscaling > 1f;
                if (usingDownscaling) {
                    RenderTextureDescriptor rtDownscaledDepth = rtBlurDesc;
                    rtDownscaledDepth.colorFormat = RenderTextureFormat.RFloat;
                    cmd.GetTemporaryRT(ShaderParams.DownsampledDepth, rtDownscaledDepth, FilterMode.Bilinear);
                    FullScreenBlit(cmd, source, ShaderParams.DownsampledDepth, mat, (int)Pass.DownscaleDepth);
                }

                if (manager.blurPasses < 1) {
                    cmd.SetGlobalFloat(ShaderParams.BlurScale, manager.blurSpread);
                    FullScreenBlit(cmd, ShaderParams.LightBuffer, source, mat, (int)Pass.Blend);
                } else {
                    rtBlurDesc.width = GetScaledSize(rtSourceDesc.width, manager.blurDownscaling);
                    rtBlurDesc.height = GetScaledSize(rtSourceDesc.height, manager.blurDownscaling);
                    cmd.GetTemporaryRT(ShaderParams.BlurRT, rtBlurDesc, FilterMode.Bilinear);
                    cmd.GetTemporaryRT(ShaderParams.BlurRT2, rtBlurDesc, FilterMode.Bilinear);
                    cmd.SetGlobalFloat(ShaderParams.BlurScale, manager.blurSpread * manager.blurDownscaling);
                    FullScreenBlit(cmd, ShaderParams.LightBuffer, ShaderParams.BlurRT, mat, (int)Pass.BlurHorizontal);
                    cmd.SetGlobalFloat(ShaderParams.BlurScale, manager.blurSpread);
                    for (int k = 0; k < manager.blurPasses - 1; k++) {
                        FullScreenBlit(cmd, ShaderParams.BlurRT, ShaderParams.BlurRT2, mat, (int)Pass.BlurVertical);
                        FullScreenBlit(cmd, ShaderParams.BlurRT2, ShaderParams.BlurRT, mat, (int)Pass.BlurHorizontal);
                    }
                    if (usingDownscaling) {
                        FullScreenBlit(cmd, ShaderParams.BlurRT, ShaderParams.BlurRT2, mat, (int)Pass.BlurVerticalFinal);
                        FullScreenBlit(cmd, ShaderParams.BlurRT2, source, mat, (int)Pass.Blend);
                    } else {
                        FullScreenBlit(cmd, ShaderParams.BlurRT, source, mat, (int)Pass.BlurVerticalAndBlend);
                    }

                    cmd.ReleaseTemporaryRT(ShaderParams.BlurRT2);
                    cmd.ReleaseTemporaryRT(ShaderParams.BlurRT);
                }
                cmd.ReleaseTemporaryRT(ShaderParams.LightBuffer);
                if (usingDownscaling) {
                    cmd.ReleaseTemporaryRT(ShaderParams.DownsampledDepth);
                }
                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }

            static Mesh _fullScreenMesh;

            Mesh fullscreenMesh {
                get {
                    if (_fullScreenMesh != null) {
                        return _fullScreenMesh;
                    }
                    float num = 1f;
                    float num2 = 0f;
                    Mesh val = new Mesh();
                    _fullScreenMesh = val;
                    _fullScreenMesh.SetVertices(new List<Vector3> {
            new Vector3 (-1f, -1f, 0f),
            new Vector3 (-1f, 1f, 0f),
            new Vector3 (1f, -1f, 0f),
            new Vector3 (1f, 1f, 0f)
        });
                    _fullScreenMesh.SetUVs(0, new List<Vector2> {
            new Vector2 (0f, num2),
            new Vector2 (0f, num),
            new Vector2 (1f, num2),
            new Vector2 (1f, num)
        });
                    _fullScreenMesh.SetIndices(new int[6] { 0, 1, 2, 2, 1, 3 }, (MeshTopology)0, 0, false);
                    _fullScreenMesh.UploadMeshData(true);
                    return _fullScreenMesh;
                }
            }

            void FullScreenBlit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int passIndex) {
                destination = new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1);
                cmd.SetRenderTarget(destination);
                cmd.SetGlobalTexture(ShaderParams.MainTex, source);
                cmd.DrawMesh(fullscreenMesh, Matrix4x4.identity, material, 0, passIndex);
            }

            public override void FrameCleanup(CommandBuffer cmd) {
            }


            public void Cleanup() {
                CoreUtils.Destroy(mat);
                Shader.SetGlobalInt(ShaderParams.ForcedInvisible, 0);
            }

        }

        [SerializeField, HideInInspector]
        Shader shader;
        VolumetricFogRenderPass fogRenderPass;
        BlurRenderPass blurRenderPass;
        public static bool installed;

        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;

        [Tooltip("Specify which cameras can execute this render feature. If you have several cameras in your scene, make sure only the correct cameras use this feature in order to optimize performance.")]
        public LayerMask cameraLayerMask = -1;

        [Tooltip("Ignores reflection probes from executing this render feature")]
        public bool ignoreReflectionProbes = true;

        void OnDisable() {
            installed = false;
            if (blurRenderPass != null) {
                blurRenderPass.Cleanup();
            }
        }

        private void OnDestroy() {
            if (fogRenderPass != null) {
                fogRenderPass.CleanUp();
            }
        }

        public override void Create() {
            name = "Volumetric Fog 2";
            fogRenderPass = new VolumetricFogRenderPass();
            blurRenderPass = new BlurRenderPass();
            shader = Shader.Find("Hidden/VolumetricFog2/Blur");
            if (shader == null) {
                Debug.LogWarning("Could not load Volumetric Fog blur shader.");
            }
        }

        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            Camera cam = renderingData.cameraData.camera;

            if ((cameraLayerMask & (1 << cam.gameObject.layer)) == 0) return;
            if (ignoreReflectionProbes && cam.cameraType == CameraType.Reflection) return;
            if (cam.targetTexture != null && cam.targetTexture.format == RenderTextureFormat.Depth) return; // ignore occlusion cams!

            fogRenderPass.Setup(this);
            blurRenderPass.Setup(shader, renderer, this);
            renderer.EnqueuePass(fogRenderPass);
            renderer.EnqueuePass(blurRenderPass);
            installed = true;
        }
    }
}
