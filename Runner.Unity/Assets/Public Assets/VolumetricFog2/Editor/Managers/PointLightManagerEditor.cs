using UnityEngine;
using UnityEditor;

namespace VolumetricFogAndMist2 {

    [CustomEditor(typeof(PointLightManager))]
    public class PointLightManagerEditor : Editor {

        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox("In order to use fast point lights with Volumetric Fog & Mist 2, add a FogPointLight script to the desired point lights (only required by the point light manager; if you're using the option 'Native Lights' this is not required at all).", MessageType.Info);
            DrawDefaultInspector();
        }
    }

}