//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist 2
// Created by Kronnect
//------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace VolumetricFogAndMist2 {

    public partial class VolumetricFog : MonoBehaviour {

        const string SURFACE_CAM_NAME = "SurfaceCam";

        public enum HeightmapCaptureResolution {
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024
        }

        RenderTexture rt;

        Camera cam;
        int camStartFrameCount;
        Matrix4x4 camMatrix;
        Vector3 lastCamPos;

        void DisposeSurfaceCapture() {
            DisableSurfaceCapture();
            if (rt != null) {
                rt.Release();
                DestroyImmediate(rt);
            }
        }

        void CheckSurfaceCapture() {
            if (cam == null) {
                Transform childCam = transform.Find(SURFACE_CAM_NAME);
                if (childCam != null) {
                    cam = childCam.GetComponent<Camera>();
                    if (cam == null) {
                        DestroyImmediate(childCam.gameObject);
                    }
                }
            }
        }

        void DisableSurfaceCapture() {
            if (cam != null) {
                cam.enabled = false;
            }
        }


        void SurfaceCaptureSupportCheck() {

            Transform childCam = transform.Find(SURFACE_CAM_NAME);
            if (childCam != null) {
                cam = childCam.GetComponent<Camera>();
            }

            if (!activeProfile.terrainFit) {
                DisposeSurfaceCapture();
                return;
            }

            if (cam == null) {
                if (childCam != null) {
                    DestroyImmediate(childCam.gameObject);
                }
                if (cam == null) {
                    GameObject camObj = new GameObject(SURFACE_CAM_NAME, typeof(Camera));
                    camObj.transform.SetParent(transform, false);
                    cam = camObj.GetComponent<Camera>();
                    cam.depthTextureMode = DepthTextureMode.None;
                    cam.clearFlags = CameraClearFlags.Depth;
                    cam.allowHDR = false;
                    cam.allowMSAA = false;
                }

                cam.stereoTargetEye = StereoTargetEyeMask.None;
            }

            UniversalAdditionalCameraData camData = cam.GetComponent<UniversalAdditionalCameraData>();
            if (camData == null) {
                camData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
            }
            if (camData != null) {
                camData.dithering = false;
                camData.renderPostProcessing = false;
                camData.renderShadows = false;
                camData.requiresColorTexture = false;
                camData.requiresDepthTexture = false;
                camData.stopNaN = false;

#if UNITY_2021_3_OR_NEWER
                CheckAndAssignDepthRenderer(camData);
#endif
            }

            cam.orthographic = true;
            cam.nearClipPlane = 1f;

            if (rt != null && rt.width != (int)activeProfile.terrainFitResolution) {
                if (cam.targetTexture == rt) {
                    cam.targetTexture = null;
                }
                rt.Release();
                DestroyImmediate(rt);
            }

            if (rt == null) {
                rt = new RenderTexture((int)activeProfile.terrainFitResolution, (int)activeProfile.terrainFitResolution, 24, RenderTextureFormat.Depth);
                rt.antiAliasing = 1;
            }

            int thisLayer = 1 << gameObject.layer;
            if ((activeProfile.terrainLayerMask & thisLayer) != 0) {
                activeProfile.terrainLayerMask &= ~thisLayer; // exclude fog layer
            }

            cam.cullingMask = activeProfile.terrainLayerMask;
            cam.targetTexture = rt;

            if (activeProfile.terrainFit) {
                ScheduleHeightmapCapture();
            } else {
                cam.enabled = false;
            }
        }

#if UNITY_2021_3_OR_NEWER
        UniversalRendererData depthRendererData;
        void CheckAndAssignDepthRenderer(UniversalAdditionalCameraData camData) {
            UniversalRenderPipelineAsset pipe = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            if (pipe == null) return;

            if (depthRendererData == null) {
                depthRendererData = Resources.Load<UniversalRendererData>("Shaders/VolumetricFogDepthRenderer");
                if (depthRendererData == null) {
                    Debug.LogError("Volumetric Fog Depth Renderer asset not found.");
                    return;
                }
                depthRendererData.postProcessData = null;
            }
            int depthRendererIndex = -1;
            for (int k = 0; k < pipe.m_RendererDataList.Length; k++) {
                if (pipe.m_RendererDataList[k] == depthRendererData) {
                    depthRendererIndex = k;
                    break;
                }
            }
            if (depthRendererIndex < 0) {
                depthRendererIndex = pipe.m_RendererDataList.Length;
                System.Array.Resize<ScriptableRendererData>(ref pipe.m_RendererDataList, depthRendererIndex + 1);
                pipe.m_RendererDataList[depthRendererIndex] = depthRendererData;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(pipe);
#endif
            }
            camData.SetRenderer(depthRendererIndex);
        }
#endif

        /// <summary>
        /// Updates shadows on this volumetric fog
        /// </summary>
        public void ScheduleHeightmapCapture() {
            if (cam != null) {
                cam.enabled = true;
                camStartFrameCount = Time.frameCount;
                if (!fogMat.IsKeywordEnabled(ShaderParams.SKW_SURFACE)) {
                    fogMat.EnableKeyword(ShaderParams.SKW_SURFACE);
                }

            }
        }


        void SetupCameraCaptureMatrix() {

            Vector3 camPos = transform.position + new Vector3(0, transform.lossyScale.y * 0.51f, 0);
            cam.farClipPlane = 10000;
            cam.transform.position = camPos;
            cam.transform.eulerAngles = new Vector3(90, 0, 0);
            Vector3 size = transform.lossyScale;
            cam.orthographicSize = Mathf.Max(size.x * 0.5f, size.z * 0.5f);

            ComputeSufaceTransform(cam.projectionMatrix, cam.worldToCameraMatrix);

            fogMat.SetMatrix(ShaderParams.SurfaceCaptureMatrix, camMatrix);
            fogMat.SetTexture(ShaderParams.SurfaceDepthTexture, cam.targetTexture);
            fogMat.SetVector(ShaderParams.SurfaceData, new Vector4(camPos.y, activeProfile.terrainFogHeight, activeProfile.terrainFogMinAltitude, activeProfile.terrainFogMaxAltitude));
        }

        void SurfaceCaptureUpdate() {

            if (!activeProfile.terrainFit) return;
            if (cam == null) return;

            SetupCameraCaptureMatrix();

            if (!cam.enabled && lastCamPos != cam.transform.position) {
                lastCamPos = cam.transform.position;
                ScheduleHeightmapCapture();
                requireUpdateMaterial = true;
            } else if (Time.frameCount > camStartFrameCount + 1 && Application.isPlaying) {
                cam.enabled = false;
            }
        }

        static Matrix4x4 identityMatrix = Matrix4x4.identity;

        void ComputeSufaceTransform(Matrix4x4 proj, Matrix4x4 view) {
            // Currently CullResults ComputeDirectionalShadowMatricesAndCullingPrimitives doesn't
            // apply z reversal to projection matrix. We need to do it manually here.
            if (SystemInfo.usesReversedZBuffer) {
                proj.m20 = -proj.m20;
                proj.m21 = -proj.m21;
                proj.m22 = -proj.m22;
                proj.m23 = -proj.m23;
            }

            Matrix4x4 worldToShadow = proj * view;

            var textureScaleAndBias = identityMatrix;
            textureScaleAndBias.m00 = 0.5f;
            textureScaleAndBias.m11 = 0.5f;
            textureScaleAndBias.m22 = 0.5f;
            textureScaleAndBias.m03 = 0.5f;
            textureScaleAndBias.m23 = 0.5f;
            textureScaleAndBias.m13 = 0.5f;

            // Apply texture scale and offset to save a MAD in shader.
            camMatrix = textureScaleAndBias * worldToShadow;
        }


    }


}