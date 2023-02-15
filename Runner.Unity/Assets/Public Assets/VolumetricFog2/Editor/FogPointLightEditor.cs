using UnityEngine;
using UnityEditor;

namespace VolumetricFogAndMist2 {

    [CustomEditor(typeof(FogPointLight))]
    public class FogPointLightEditor : Editor {


        SerializedProperty inscattering, intensity;

        private void OnEnable() {
            inscattering = serializedObject.FindProperty("inscattering");
            intensity = serializedObject.FindProperty("intensity");
        }


        public override void OnInspectorGUI() {

            EditorGUILayout.HelpBox("Custom multipliers for this point light only. Manage global settings using the Point Light Manager.", MessageType.Info);
            if (GUILayout.Button("Open Point Light Manager")) {
                Selection.activeGameObject = VolumetricFogManager.pointLightManager.gameObject;
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.Separator();

            serializedObject.Update();
            EditorGUILayout.PropertyField(inscattering);
            EditorGUILayout.PropertyField(intensity);
            serializedObject.ApplyModifiedProperties();
        }
    }

}