using System.Collections.Generic;
using UnityEngine;

namespace VolumetricFogAndMist2 {

    public class VolumetricFogSubVolume : MonoBehaviour {

        public VolumetricFogProfile profile;
        public float fadeDistance = 1f;

        public static List<VolumetricFogSubVolume> subVolumes = new List<VolumetricFogSubVolume>();

        void OnDrawGizmos() {
            Gizmos.color = new Color(1, 1, 1, 0.35f);
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }

        void OnEnable() {
            if (!subVolumes.Contains(this)) {
                subVolumes.Add(this);
            }
        }

        void OnDisable() {
            if (subVolumes.Contains(this)) {
                subVolumes.Remove(this);
            }
        }

        public Bounds GetBounds() {
            return new Bounds(transform.position, transform.lossyScale);
        }

        public void SetBounds(Bounds bounds) {
            Transform parent = transform.parent;
            Vector3 scale = bounds.size;
            if (parent != null) {
                Vector3 scaleFactor = transform.parent.lossyScale;
                scale.x /= scaleFactor.x;
                scale.y /= scaleFactor.y;
                scale.z /= scaleFactor.z;
            }
            transform.localScale = scale;
            transform.position = bounds.center;
        }

    }

}