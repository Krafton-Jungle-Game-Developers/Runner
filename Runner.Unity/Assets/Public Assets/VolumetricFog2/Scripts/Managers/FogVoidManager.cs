using System.Collections.Generic;
using UnityEngine;

namespace VolumetricFogAndMist2 {

    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public class FogVoidManager : MonoBehaviour, IVolumetricFogManager {

        public static bool usingVoids;

        public string managerName {
            get {
                return "Fog Void Manager";
            }
        }

        public const int MAX_FOG_VOID = 8;

        [Header("Void Search Settings")]
        public Transform trackingCenter;
        [Tooltip("Fog voids are sorted by camera distance every certain time interval to ensure the nearest 8 voids are used.")]
        public float distanceSortTimeInterval = 3f;

        readonly static List<FogVoid> fogVoids = new List<FogVoid>();
        Vector4[] fogVoidPositions;
        Vector4[] fogVoidSizes;
        Matrix4x4[] fogVoidMatrices;
        float distanceSortLastTime;
        static bool requireRefresh;
        int lastFogVoidCount;

        private void OnEnable() {
            if (trackingCenter == null) {
                Camera cam = null;
                Tools.CheckCamera(ref cam);
                if (cam != null) {
                    trackingCenter = cam.transform;
                }
            }
            if (fogVoidPositions == null || fogVoidPositions.Length != MAX_FOG_VOID) {
                fogVoidPositions = new Vector4[MAX_FOG_VOID];
            }
            if (fogVoidSizes == null || fogVoidSizes.Length != MAX_FOG_VOID) {
                fogVoidSizes = new Vector4[MAX_FOG_VOID];
            }
            if (fogVoidMatrices == null || fogVoidMatrices.Length != MAX_FOG_VOID) {
                fogVoidMatrices = new Matrix4x4[MAX_FOG_VOID];
            }
            requireRefresh = true;
        }


        void LateUpdate() {
            if (!usingVoids) return;
            usingVoids = false;

            int fogVoidsCount = fogVoids.Count;
            if (fogVoidsCount != lastFogVoidCount) {
                lastFogVoidCount = fogVoidsCount;
                requireRefresh = true;
            }
            if (requireRefresh) {
                requireRefresh = false;
                TrackFogVoids(true);
            } else {
                if (fogVoidsCount == 0) return;
                TrackFogVoids();
            }
            SubmitFogVoidData();
        }

        void SubmitFogVoidData() {

            bool allowRotation = VolumetricFogManager.allowFogVoidRotation;

            int count = 0;
            int fogVoidsCount = fogVoids.Count;
            for (int i = 0; count < MAX_FOG_VOID && i < fogVoidsCount; i++) {
                FogVoid fogVoid = fogVoids[i];
                if (fogVoid == null || !fogVoid.isActiveAndEnabled) continue;
                Transform t = fogVoid.transform;
                Vector3 pos = t.position;
                Vector3 scale = t.lossyScale;
                if (scale.x < 0.01f) scale.x = 0.01f;
                if (scale.y < 0.01f) scale.y = 0.01f;
                if (scale.z < 0.01f) scale.z = 0.01f;
                scale.x *= 0.5f;
                scale.y *= 0.5f;
                scale.z *= 0.5f;
                fogVoidPositions[count].x = pos.x;
                fogVoidPositions[count].y = pos.y;
                fogVoidPositions[count].z = pos.z;
                fogVoidPositions[count].w = 10f * (1f - fogVoid.falloff) * (1f - fogVoid.falloff);
                if (allowRotation) {
                    fogVoidMatrices[count] = Matrix4x4.TRS(pos, t.rotation, scale).inverse;
                } else {
                    float width = scale.x;
                    float height = scale.y;
                    float depth = scale.z;
                    fogVoidSizes[count].x = 1f / (0.0001f + width * width);
                    fogVoidSizes[count].y = 1f / (0.0001f + height * height);
                    fogVoidSizes[count].z = 1f / (0.0001f + depth * depth);
                }
                fogVoidSizes[count].w = fogVoid.roundness;
                count++;
            }
            Shader.SetGlobalInt(ShaderParams.VoidCount, count);
            if (count > 0) {
                Shader.SetGlobalVectorArray(ShaderParams.VoidPositions, fogVoidPositions);
                if (allowRotation) {
                    Shader.SetGlobalMatrixArray(ShaderParams.VoidMatrices, fogVoidMatrices);
                }
                Shader.SetGlobalVectorArray(ShaderParams.VoidSizes, fogVoidSizes);
            }
        }

        public static void RegisterFogVoid(FogVoid fogVoid) {
            if (fogVoid != null) {
                fogVoids.Add(fogVoid);
                requireRefresh = true;
            }
        }

        public static void UnregisterFogVoid(FogVoid fogVoid) {
            if (fogVoid != null && fogVoids.Contains(fogVoid)) {
                fogVoids.Remove(fogVoid);
                requireRefresh = true;
            }
        }

        /// <summary>
        /// Look for nearest voids
        /// </summary>
        public void TrackFogVoids(bool forceImmediateUpdate = false) {

            // Look for new voids?
            if (fogVoids != null && fogVoids.Count > 0 && (forceImmediateUpdate || !Application.isPlaying || (distanceSortTimeInterval > 0 && Time.time - distanceSortLastTime > distanceSortTimeInterval))) {
                if (trackingCenter != null) {
                    distanceSortLastTime = Time.time;
                    fogVoids.Sort(fogVoidDistanceComparer);
                }
            }
        }

        int fogVoidDistanceComparer(FogVoid v1, FogVoid v2) {
            float dist1 = (v1.transform.position - trackingCenter.position).sqrMagnitude;
            float dist2 = (v2.transform.position - trackingCenter.position).sqrMagnitude;
            if (dist1 < dist2) return -1;
            if (dist1 > dist2) return 1;
            return 0;
        }


        public void Refresh() {
            requireRefresh = true;
        }


    }

}