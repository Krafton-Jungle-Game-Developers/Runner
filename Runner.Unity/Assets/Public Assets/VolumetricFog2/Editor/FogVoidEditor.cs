using UnityEngine;
using UnityEditor;

namespace VolumetricFogAndMist2 {

    [CustomEditor(typeof(FogVoid))]
    public class FogVoidEditor : Editor {

        SerializedProperty falloff, roundness;

        private void OnEnable() {
            falloff = serializedObject.FindProperty("falloff");
            roundness = serializedObject.FindProperty("roundness");
        }


        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(roundness);
            EditorGUILayout.PropertyField(falloff);

            serializedObject.ApplyModifiedProperties();

        }
    }

}