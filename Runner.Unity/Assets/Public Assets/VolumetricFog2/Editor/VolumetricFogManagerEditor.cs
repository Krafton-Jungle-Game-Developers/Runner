using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

namespace VolumetricFogAndMist2 {

    [CustomEditor(typeof(VolumetricFogManager))]
    public class VolumetricFogManagerEditor : Editor {

        SerializedProperty mainCamera, sun, moon, fogLayer, includeTransparent, includeSemiTransparent, alphaCutOff, flipDepthTexture, mainManager;
        SerializedProperty downscaling, blurPasses, blurDownscaling, blurSpread, blurHDR, blurEdgePreserve, blurEdgeDepthThreshold, ditherStrength;

        bool toggleOptimizeBuild;
        VolumetricFogShaderOptions shaderAdvancedOptionsInfo;
        int maxIterations;

        private void OnEnable() {
            mainCamera = serializedObject.FindProperty("mainCamera");
            sun = serializedObject.FindProperty("sun");
            moon = serializedObject.FindProperty("moon");
            fogLayer = serializedObject.FindProperty("fogLayer");
            includeTransparent = serializedObject.FindProperty("includeTransparent");
            includeSemiTransparent = serializedObject.FindProperty("includeSemiTransparent");
            alphaCutOff = serializedObject.FindProperty("alphaCutOff");
            flipDepthTexture = serializedObject.FindProperty("flipDepthTexture");
            mainManager = serializedObject.FindProperty("mainManager");
            downscaling = serializedObject.FindProperty("downscaling");
            blurPasses = serializedObject.FindProperty("blurPasses");
            blurDownscaling = serializedObject.FindProperty("blurDownscaling");
            blurSpread = serializedObject.FindProperty("blurSpread");
            blurHDR = serializedObject.FindProperty("blurHDR");
            blurEdgePreserve = serializedObject.FindProperty("blurEdgePreserve");
            blurEdgeDepthThreshold = serializedObject.FindProperty("blurEdgeDepthThreshold");
            ditherStrength = serializedObject.FindProperty("ditherStrength");
            ScanAdvancedOptions();
        }


        public override void OnInspectorGUI() {

            EditorGUILayout.Separator();

            UniversalRenderPipelineAsset pipe = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (pipe == null) {
                EditorGUILayout.HelpBox("Please assign the Universal Rendering Pipeline asset (go to Project Settings -> Graphics). You can use the UniversalRenderPipelineAsset included in the demo folder or create a new pipeline asset (check documentation for step by step setup).", MessageType.Error);
                return;
            }

            if (QualitySettings.renderPipeline != null) {
                pipe = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
            }

            if (!pipe.supportsCameraDepthTexture) {
                EditorGUILayout.HelpBox("Depth Texture option is required in Universal Rendering Pipeline asset!", MessageType.Error);
                if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                    Selection.activeObject = pipe;
                }
                EditorGUILayout.Separator();
                GUI.enabled = false;
            }

            if (VolumetricFogEditor.lastEditingFog != null) {
                if (GUILayout.Button("<< Back To Last Volumetric Fog")) {
                    Selection.SetActiveObjectWithContext(VolumetricFogEditor.lastEditingFog, null);
                    GUIUtility.ExitGUI();
                }
            }

            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mainCamera);
            EditorGUILayout.PropertyField(sun);
            EditorGUILayout.PropertyField(moon);
            fogLayer.intValue = EditorGUILayout.LayerField("Fog Layer", fogLayer.intValue);
            EditorGUILayout.PropertyField(flipDepthTexture);
            EditorGUILayout.PropertyField(mainManager);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("Custom Depth Pre-Pass", "Support for transparent or semi-transparent objects or needed custom depth pass."), EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(includeTransparent, new GUIContent("Transparent Objects", "Specify which layers contain transparent objects that should be covered by fog"));
            int transparentLayerMask = includeTransparent.intValue;
            if ((transparentLayerMask & (1 << fogLayer.intValue)) != 0) {
                EditorGUILayout.HelpBox("Do not include the layer used for fog volumes (Fog Layer).", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(includeSemiTransparent, new GUIContent("Alpha Clipping Objects", "Specify which smi-transparent objects (cutout materials) should be covered by fog."));
            int includeSemiTransparentMask = includeSemiTransparent.intValue;
            if (includeSemiTransparentMask != 0) {
                if ((includeSemiTransparentMask & (1 << fogLayer.intValue)) != 0) {
                    EditorGUILayout.HelpBox("Do not include the layer used for fog volumes (Fog Layer).", MessageType.Warning);
                }
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(alphaCutOff, new GUIContent("Alpha CutOff"), true);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent(""), GUIContent.none);
                if (GUILayout.Button("Refresh")) {
                    DepthRenderPrePassFeature.DepthRenderPass.FindAlphaClippingRenderers();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            if (includeTransparent.intValue != 0 || includeSemiTransparent.intValue != 0) {
                if (!DepthRenderPrePassFeature.installed) {
                    EditorGUILayout.HelpBox("Transparent option requires 'DepthRendererPrePass Feature' added to the Forward Renderer of the Universal Rendering Pipeline asset. Check the documentation for instructions.", MessageType.Warning);
                    if (pipe != null && GUILayout.Button("Show Pipeline Asset")) Selection.activeObject = pipe;
                }
            } else if (DepthRenderPrePassFeature.installed) {
                EditorGUILayout.HelpBox("No transparent objects included. Remove 'DepthRendererPrePass Feature' from the Forward Renderer of the Universal Rendering Pipeline asset to save performance.", MessageType.Warning);
                if (pipe != null && GUILayout.Button("Show Pipeline Asset")) Selection.activeObject = pipe;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("Final Composition", "Support for off-screen rendering and composition to screen target. Allows optimizations like downsampling."), EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(downscaling);
            EditorGUILayout.PropertyField(blurPasses);
            if (blurPasses.intValue > 0) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(blurDownscaling, new GUIContent("Downscaling"));
                EditorGUILayout.PropertyField(blurSpread, new GUIContent("Spread"));
                EditorGUILayout.PropertyField(blurHDR, new GUIContent("HDR"));
                EditorGUILayout.PropertyField(blurEdgePreserve, new GUIContent("Preserve Edges"));
                if (blurEdgePreserve.boolValue) {
                    EditorGUILayout.PropertyField(blurEdgeDepthThreshold, new GUIContent("Edge Threshold"));
                }
                EditorGUI.indentLevel--;
            }
            if (EditorGUI.EndChangeCheck()) {
                EditorApplication.delayCall += () => UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            EditorGUILayout.PropertyField(ditherStrength);

            if (blurPasses.intValue > 0 || downscaling.floatValue > 1) {
                if (!VolumetricFogRenderFeature.installed) {
                    EditorGUILayout.HelpBox("These options require 'Volumetric Fog Render Feature' added to the Forward Renderer of the Universal Rendering Pipeline asset. Check the documentation for instructions.", MessageType.Warning);
                    if (pipe != null && GUILayout.Button("Show Pipeline Asset")) Selection.activeObject = pipe;
                }
                EditorGUILayout.HelpBox("When downscaling or blur option is enabled, fog volumes ignore render queue value. Select the render pass event in the Volumetric Fog Render Feature.", MessageType.Info);
            } else if (VolumetricFogRenderFeature.installed) {
                EditorGUILayout.HelpBox("No downscaling/blur used. Remove 'Volumetric Fog Render Feature' from the Forward Renderer of the Universal Rendering Pipeline asset to save performance.", MessageType.Warning);
                if (pipe != null && GUILayout.Button("Show Pipeline Asset")) Selection.activeObject = pipe;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            bool shaderOptionsOpen = toggleOptimizeBuild;
            if (shaderOptionsOpen) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
            }

            if (GUILayout.Button(toggleOptimizeBuild ? "Hide Shader Options" : "Shader Options", GUILayout.Width(150))) {
                toggleOptimizeBuild = !toggleOptimizeBuild;
            }

            if (toggleOptimizeBuild && shaderAdvancedOptionsInfo != null) {

                int optionsCount = shaderAdvancedOptionsInfo.options.Length;
                for (int k = 0; k < optionsCount; k++) {
                    ShaderAdvancedOption option = shaderAdvancedOptionsInfo.options[k];
                    if (option.hasValue) continue;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(10));
                    bool prevState = option.enabled;
                    bool newState = EditorGUILayout.Toggle(prevState, GUILayout.Width(18));
                    if (prevState != newState) {
                        shaderAdvancedOptionsInfo.options[k].enabled = newState;
                        shaderAdvancedOptionsInfo.pendingChanges = true;
                        GUIUtility.ExitGUI();
                        return;
                    }
                    EditorGUILayout.LabelField(option.name);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(10));
                    EditorGUILayout.LabelField("", GUILayout.Width(18));
                    GUIStyle wrapStyle = new GUIStyle(GUI.skin.label);
                    wrapStyle.wordWrap = true;
                    EditorGUILayout.LabelField(option.description, wrapStyle);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(10));
                EditorGUI.BeginChangeCheck();
                float prevLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100;
                maxIterations = EditorGUILayout.IntField(new GUIContent("Max Iterations", "The maximum number of raymarching steps."), maxIterations, GUILayout.Width(175));
                if (EditorGUI.EndChangeCheck()) {
                    shaderAdvancedOptionsInfo.pendingChanges = true;
                }
                EditorGUIUtility.labelWidth = prevLabelWidth;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Refresh", GUILayout.Width(60))) {
                    ScanAdvancedOptions();
                    GUIUtility.ExitGUI();
                    return;
                }
                if (!shaderAdvancedOptionsInfo.pendingChanges)
                    GUI.enabled = false;
                if (GUILayout.Button("Save Changes", GUILayout.Width(110))) {
                    shaderAdvancedOptionsInfo.SetOptionValue("MAX_ITERATIONS", maxIterations);
                    shaderAdvancedOptionsInfo.UpdateAdvancedOptionsFile();
                    GUIUtility.ExitGUI();
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }

            if (shaderOptionsOpen) {
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Managers", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Point Light Manager", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (GUILayout.Button("Open", GUILayout.Width(150))) {
                Selection.activeGameObject = VolumetricFogManager.pointLightManager.gameObject;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Fog Void Manager", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (GUILayout.Button("Open", GUILayout.Width(150))) {
                Selection.activeGameObject = VolumetricFogManager.fogVoidManager.gameObject;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Noise Generator", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (GUILayout.Button("Open", GUILayout.Width(150))) {
                NoiseGenerator window = EditorWindow.GetWindow<NoiseGenerator>("Noise Generator", true);
                window.minSize = new Vector2(400, 400);
                window.Show();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        void ScanAdvancedOptions() {
            if (shaderAdvancedOptionsInfo == null) {
                shaderAdvancedOptionsInfo = new VolumetricFogShaderOptions();
            }
            shaderAdvancedOptionsInfo.ReadOptions();
            maxIterations = shaderAdvancedOptionsInfo.GetOptionValue("MAX_ITERATIONS");
        }

    }
}