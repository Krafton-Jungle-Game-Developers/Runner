using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace VolumetricFogAndMist2
{

    [CustomEditor(typeof(VolumetricFogSubVolume))]
    public class VolumetricFogSubVolumeEditor : Editor
    {

        SerializedProperty profile, fadeDistance;

        private void OnEnable()
        {
            profile = serializedObject.FindProperty("profile");
            fadeDistance = serializedObject.FindProperty("fadeDistance");
        }


        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            EditorGUILayout.PropertyField(profile);
            EditorGUILayout.PropertyField(fadeDistance);

            serializedObject.ApplyModifiedProperties();

        }


        private readonly BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();

        protected virtual void OnSceneGUI() {
            VolumetricFogSubVolume vl = (VolumetricFogSubVolume)target;

            Bounds bounds = vl.GetBounds();
            m_BoundsHandle.center = bounds.center;
            m_BoundsHandle.size = bounds.size;

            // draw the handle
            EditorGUI.BeginChangeCheck();
            m_BoundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck()) {
                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(vl, "Change Bounds");

                // copy the handle's updated data back to the target object
                Bounds newBounds = new Bounds();
                newBounds.center = m_BoundsHandle.center;
                newBounds.size = m_BoundsHandle.size;
                vl.SetBounds(newBounds);
            }
        }
    }

}