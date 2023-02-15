using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VolumetricFogAndMist2 {

    public static class Tools {

        public static Color ColorBlack = Color.black;
        public static void CheckCamera(ref Camera cam) {
            if (cam != null) return;
            cam = Camera.main;
            if (cam == null) {
                Camera[] cameras = Object.FindObjectsOfType<Camera>();
                for (int k = 0; k < cameras.Length; k++) {
                    if (cameras[k].isActiveAndEnabled && cameras[k].gameObject.activeInHierarchy) {
                        cam = cameras[k];
                        return;
                    }

                }
            }
        }

        public static VolumetricFogManager CheckMainManager() {
            VolumetricFogManager fogManager = Object.FindObjectOfType<VolumetricFogManager>();
            if (fogManager == null) {
                GameObject go = new GameObject();
                fogManager = go.AddComponent<VolumetricFogManager>();
                go.name = fogManager.managerName;
#if UNITY_EDITOR
                Undo.RegisterCreatedObjectUndo(go, "Create Volumetric Fog Manager");
#endif
            }
            return fogManager;
        }


        public static void CheckManager<T>(ref T manager) where T : Component {
            if (manager == null) {
                VolumetricFogManager root = CheckMainManager();
                if (root == null) return;
                manager = root.GetComponentInChildren<T>(true);
                if (manager == null) {
                    GameObject o = new GameObject();
                    o.transform.SetParent(root.transform, false);
                    manager = o.AddComponent<T>();
                    o.name = ((IVolumetricFogManager)manager).managerName;
#if UNITY_EDITOR
                        Undo.RegisterCreatedObjectUndo(o, "Create Fog Manager");
#endif

                }
            }
        }
    }

}