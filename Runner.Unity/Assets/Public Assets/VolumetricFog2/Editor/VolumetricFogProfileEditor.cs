//#define FOG_BORDER

using UnityEngine;
using UnityEditor;

namespace VolumetricFogAndMist2 {

    [CustomEditor(typeof(VolumetricFogProfile))]
    public class VolumetricFogProfileEditor : Editor {

        SerializedProperty raymarchQuality, raymarchMinStep, jittering, dithering;
        SerializedProperty renderQueue, sortingLayerID, sortingOrder;
        SerializedProperty noiseTexture, noiseStrength, noiseScale, noiseFinalMultiplier;
        SerializedProperty useDetailNoise, detailTexture, detailScale, detailStrength, detailOffset;
        SerializedProperty density;
        SerializedProperty shape, border, verticalOffset, distance, distanceFallOff, maxDistance, maxDistanceFallOff;
        SerializedProperty terrainFit, terrainFitResolution, terrainLayerMask, terrainFogHeight, terrainFogMinAltitude, terrainFogMaxAltitude;

        SerializedProperty albedo, enableDepthGradient, depthGradient, depthGradientMaxDistance, enableHeightGradient, heightGradient;
        SerializedProperty brightness, deepObscurance, specularColor, specularThreshold, specularIntensity, ambientLightMultiplier;

        SerializedProperty turbulence, windDirection, useCustomDetailNoiseWindDirection, detailNoiseWindDirection;

        SerializedProperty dayNightCycle, sunDirection, lightDiffusionPower, lightDiffusionIntensity;
        SerializedProperty receiveShadows, shadowIntensity;
        SerializedProperty cookie;

        private void OnEnable() {
            raymarchQuality = serializedObject.FindProperty("raymarchQuality");
            raymarchMinStep = serializedObject.FindProperty("raymarchMinStep");
            jittering = serializedObject.FindProperty("jittering");
            dithering = serializedObject.FindProperty("dithering");

            renderQueue = serializedObject.FindProperty("renderQueue");
            sortingLayerID = serializedObject.FindProperty("sortingLayerID");
            sortingOrder = serializedObject.FindProperty("sortingOrder");

            noiseTexture = serializedObject.FindProperty("noiseTexture");
            noiseStrength = serializedObject.FindProperty("noiseStrength");
            noiseScale = serializedObject.FindProperty("noiseScale");
            noiseFinalMultiplier = serializedObject.FindProperty("noiseFinalMultiplier");

            useDetailNoise = serializedObject.FindProperty("useDetailNoise");
            detailTexture = serializedObject.FindProperty("detailTexture");
            detailScale = serializedObject.FindProperty("detailScale");
            detailStrength = serializedObject.FindProperty("detailStrength");
            detailOffset = serializedObject.FindProperty("detailOffset");

            density = serializedObject.FindProperty("density");
            shape = serializedObject.FindProperty("shape");
            border = serializedObject.FindProperty("border");
            verticalOffset = serializedObject.FindProperty("verticalOffset");

            distance = serializedObject.FindProperty("distance");
            distanceFallOff = serializedObject.FindProperty("distanceFallOff");
            maxDistance = serializedObject.FindProperty("maxDistance");
            maxDistanceFallOff = serializedObject.FindProperty("maxDistanceFallOff");
            terrainFit = serializedObject.FindProperty("terrainFit");
            terrainFitResolution = serializedObject.FindProperty("terrainFitResolution");
            terrainLayerMask = serializedObject.FindProperty("terrainLayerMask");
            terrainFogHeight = serializedObject.FindProperty("terrainFogHeight");
            terrainFogMinAltitude = serializedObject.FindProperty("terrainFogMinAltitude");
            terrainFogMaxAltitude = serializedObject.FindProperty("terrainFogMaxAltitude");

            albedo = serializedObject.FindProperty("albedo");
            enableDepthGradient = serializedObject.FindProperty("enableDepthGradient");
            depthGradient = serializedObject.FindProperty("depthGradient");
            depthGradientMaxDistance = serializedObject.FindProperty("depthGradientMaxDistance");
            enableHeightGradient = serializedObject.FindProperty("enableHeightGradient");
            heightGradient = serializedObject.FindProperty("heightGradient");

            brightness = serializedObject.FindProperty("brightness");
            deepObscurance = serializedObject.FindProperty("deepObscurance");
            specularColor = serializedObject.FindProperty("specularColor");
            specularThreshold = serializedObject.FindProperty("specularThreshold");
            specularIntensity = serializedObject.FindProperty("specularIntensity");
            ambientLightMultiplier = serializedObject.FindProperty("ambientLightMultiplier");

            turbulence = serializedObject.FindProperty("turbulence");
            windDirection = serializedObject.FindProperty("windDirection");
            useCustomDetailNoiseWindDirection = serializedObject.FindProperty("useCustomDetailNoiseWindDirection");
            detailNoiseWindDirection = serializedObject.FindProperty("detailNoiseWindDirection");

            dayNightCycle = serializedObject.FindProperty("dayNightCycle");
            sunDirection = serializedObject.FindProperty("sunDirection");
            lightDiffusionPower = serializedObject.FindProperty("lightDiffusionPower");
            lightDiffusionIntensity = serializedObject.FindProperty("lightDiffusionIntensity");

            receiveShadows = serializedObject.FindProperty("receiveShadows");
            shadowIntensity = serializedObject.FindProperty("shadowIntensity");
            cookie = serializedObject.FindProperty("cookie");
        }


        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(raymarchQuality);
            EditorGUILayout.PropertyField(raymarchMinStep);
            EditorGUILayout.PropertyField(jittering);
            EditorGUILayout.PropertyField(dithering);
            EditorGUILayout.PropertyField(renderQueue);
            EditorGUILayout.PropertyField(sortingLayerID);
            EditorGUILayout.PropertyField(sortingOrder);
            EditorGUILayout.PropertyField(noiseTexture);
            EditorGUILayout.PropertyField(noiseStrength);
            EditorGUILayout.PropertyField(noiseScale);
            EditorGUILayout.PropertyField(noiseFinalMultiplier);
            EditorGUILayout.PropertyField(useDetailNoise);
            if (useDetailNoise.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(detailTexture);
                EditorGUILayout.PropertyField(detailScale);
                EditorGUILayout.PropertyField(detailStrength);
                EditorGUILayout.PropertyField(detailOffset);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(density);
            EditorGUILayout.PropertyField(shape);
#if FOG_BORDER
            EditorGUILayout.PropertyField(border);
#else
            GUI.enabled = false;
            EditorGUILayout.LabelField("Border", "(Disabled in Volumetric Fog Manager)");
            GUI.enabled = true;
#endif
            EditorGUILayout.PropertyField(verticalOffset);
            EditorGUILayout.PropertyField(distance);
            EditorGUILayout.PropertyField(distanceFallOff);
            EditorGUILayout.PropertyField(maxDistance);
            EditorGUILayout.PropertyField(maxDistanceFallOff);

            EditorGUILayout.PropertyField(terrainFit);
            if (terrainFit.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(terrainFitResolution, new GUIContent("Resolution"));
                EditorGUILayout.PropertyField(terrainLayerMask, new GUIContent("Layer Mask"));
                EditorGUILayout.PropertyField(terrainFogHeight, new GUIContent("Fog Height"));
                EditorGUILayout.PropertyField(terrainFogMinAltitude, new GUIContent("Min Altitude"));
                EditorGUILayout.PropertyField(terrainFogMaxAltitude, new GUIContent("Max Altitude"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(albedo);
            EditorGUILayout.PropertyField(enableDepthGradient);
            if (enableDepthGradient.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(depthGradient);
                EditorGUILayout.PropertyField(depthGradientMaxDistance, new GUIContent("Max Distance"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(enableHeightGradient);
            if (enableHeightGradient.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(heightGradient);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(brightness);
            EditorGUILayout.PropertyField(deepObscurance);
            EditorGUILayout.PropertyField(specularColor);
            EditorGUILayout.PropertyField(specularThreshold);
            EditorGUILayout.PropertyField(specularIntensity);

            EditorGUILayout.PropertyField(turbulence);
            EditorGUILayout.PropertyField(windDirection);
            EditorGUILayout.PropertyField(useCustomDetailNoiseWindDirection, new GUIContent("Custom Detail Noise Wind"));
            if (useCustomDetailNoiseWindDirection.boolValue) {
                EditorGUILayout.PropertyField(detailNoiseWindDirection);
            }

            EditorGUILayout.PropertyField(dayNightCycle);
            if (dayNightCycle.boolValue) {
                VolumetricFogManager manager = VolumetricFogManager.GetManagerIfExists();
                if (manager != null && manager.sun == null) {
                    EditorGUILayout.HelpBox("You must assign a directional light to the Sun property of the Volumetric Fog Manager.", MessageType.Warning);
                    if (GUILayout.Button("Go to Volumetric Fog Manager")) {
                        Selection.activeGameObject = manager.gameObject;
                        EditorGUIUtility.ExitGUI();
                        return;
                    }
                }
            } else { 
                EditorGUILayout.PropertyField(sunDirection);
            }
            EditorGUILayout.PropertyField(ambientLightMultiplier, new GUIContent("Ambient Light", "Amount of ambient light that influences fog colors"));
            EditorGUILayout.PropertyField(lightDiffusionPower);
            EditorGUILayout.PropertyField(lightDiffusionIntensity);
#if UNITY_2021_3_OR_NEWER
                EditorGUILayout.PropertyField(cookie);
#endif

            EditorGUILayout.PropertyField(receiveShadows);
            if (receiveShadows.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(shadowIntensity);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();

        }
    }

}
