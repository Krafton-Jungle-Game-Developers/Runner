using UnityEngine;

namespace VolumetricFogAndMist2 {

    [ExecuteInEditMode]
    public class FogVoid : MonoBehaviour {

        [Range(0, 1)] public float roundness = 0.5f;
        [Range(0, 1)] public float falloff = 0.5f;

        private void OnEnable() {
            FogVoidManager.RegisterFogVoid(this);
        }

        private void OnDisable() {
            FogVoidManager.UnregisterFogVoid(this);
        }

        void OnDrawGizmosSelected() {

            Gizmos.color = new Color(1, 1, 0, 0.75F);

            if (VolumetricFogManager.allowFogVoidRotation) {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            } else {
                Gizmos.DrawWireCube(transform.position, transform.lossyScale);
            }
        }
    }
}