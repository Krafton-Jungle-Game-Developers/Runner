using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace VolumetricFogAndMist2 {

    public partial class VolumetricFogEditor {

        bool mouseIsDown;

        private void OnSceneGUI() {

            Event e = Event.current;
            if (fog == null || !fog.enableFogOfWar || !maskEditorEnabled.boolValue || e == null || fog.fogOfWarTexture == null) {
                return;
            }

            Camera sceneCamera = null;
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null) sceneCamera = sceneView.camera;
            if (sceneCamera == null) return;

            Vector2 mousePos = Event.current.mousePosition;
            if (mousePos.x < 0 || mousePos.x > sceneCamera.pixelWidth || mousePos.y < 0 || mousePos.y > sceneCamera.pixelHeight) return;

            Selection.activeGameObject = fog.gameObject;
            fog.UpdateMaterialPropertiesNow();

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            Bounds bounds = new Bounds(fog.transform.position, new Vector3(fog.transform.lossyScale.x, 0.01f, fog.transform.lossyScale.z));
            float distance;
            if (bounds.IntersectRay(ray, out distance)) {
                Vector3 hitPoint = ray.origin + ray.direction * distance;
                float handleSize = HandleUtility.GetHandleSize(hitPoint) * 0.5f;
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.SphereHandleCap(0, hitPoint, Quaternion.identity, handleSize, EventType.Repaint);
                HandleUtility.Repaint();

                Handles.color = new Color(1, 1, 0, 0.85f);
                Handles.DrawWireDisc(hitPoint, Vector3.up, maskBrushWidth.intValue * 0.995f);
                Handles.color = new Color(0, 0, 1, 0.85f);
                Handles.DrawWireDisc(hitPoint, Vector3.up, maskBrushWidth.intValue);

                if (e.isMouse && e.button == 0) {
                    int controlID = GUIUtility.GetControlID(FocusType.Passive);
                    EventType eventType = e.GetTypeForControl(controlID);

                    if (eventType == EventType.MouseDown) {
                        GUIUtility.hotControl = controlID;
                        mouseIsDown = true;
                        PaintOnMaskPosition(hitPoint);
                    } else if (eventType == EventType.MouseUp) {
                        GUIUtility.hotControl = controlID;
                        mouseIsDown = false;
                    }

                    if (mouseIsDown && eventType == EventType.MouseDrag) {
                        GUIUtility.hotControl = controlID;
                        PaintOnMaskPosition(hitPoint);
                    }
                }
            }
        }


        #region Mask Texture support functions

        private void CreateNewMaskTexture() {
            int res = Mathf.Clamp(fog.fogOfWarTextureSize, 256, 8192);
            Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false, true);
            tex.wrapMode = TextureWrapMode.Clamp;
            int len = res * res;
            Color32[] cc = new Color32[len];
            Color32 opaque = new Color32(255, 255, 255, 255);
            for (int k = 0; k < cc.Length; k++) {
                cc[k] = opaque;
            }
            tex.SetPixels32(cc);
            tex.Apply();

            string fileName = AssetDatabase.GenerateUniqueAssetPath("Assets/FogOfWarTexture.asset");
            AssetDatabase.CreateAsset(tex, fileName);
            AssetDatabase.SaveAssets();
            fog.fogOfWarTexture = tex;
            fog.maskBrushMode = MASK_TEXTURE_BRUSH_MODE.RemoveFog;
            EditorUtility.SetDirty(fog);
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            EditorGUIUtility.PingObject(tex);
        }

        void PaintOnMaskPosition(Vector3 pos) {
            if (maskBrushMode.intValue == (int)MASK_TEXTURE_BRUSH_MODE.ColorFog) {
                PaintColorOnMaskPosition(pos);
            } else {
                PaintAlphaOnMaskPosition(pos);
            }
        }

        private void PaintAlphaOnMaskPosition(Vector3 pos) {
            // Get texture location
            Color32[] maskColors = fog.fogOfWarTextureData;
            if (maskColors == null) {
                fog.ReloadFogOfWarTexture();
                maskColors = fog.fogOfWarTextureData;
            }
            if (maskColors == null) {
                EditorUtility.DisplayDialog("Fog Of War Editor", "Re-enable fog of war to create the underline texture.", "Ok");
                return;
            }
            Vector3 fogOfWarCenter = fog.anchoredFogOfWarCenter;
            float x = (pos.x - fogOfWarCenter.x) / fog.fogOfWarSize.x + 0.5f;
            float z = (pos.z - fogOfWarCenter.z) / fog.fogOfWarSize.z + 0.5f;
            int textureSize = fog.fogOfWarTextureSize;
            int tx = Mathf.Clamp((int)(x * textureSize), 0, textureSize - 1);
            int ty = Mathf.Clamp((int)(z * textureSize), 0, textureSize - 1);

            // Prepare brush data
            int brushSize = Mathf.FloorToInt(fog.fogOfWarTextureSize * maskBrushWidth.intValue / fog.fogOfWarSize.x);
            byte color = maskBrushMode.intValue == (int)MASK_TEXTURE_BRUSH_MODE.AddFog ? (byte)255 : (byte)0;
            float brushOpacity = 1f - maskBrushOpacity.floatValue * 0.2f;
            float fuzziness = 1.1f - maskBrushFuzziness.floatValue;
            byte colort = (byte)(color * (1f - brushOpacity));
            float radiusSqr = brushSize * brushSize;
            // Paint!
            for (int j = ty - brushSize; j < ty + brushSize; j++) {
                if (j < 0) continue; else if (j >= textureSize) break;
                int jj = j * textureSize;
                int dj = (j - ty) * (j - ty);
                for (int k = tx - brushSize; k < tx + brushSize; k++) {
                    if (k < 0) continue; else if (k >= textureSize) break;
                    int distSqr = dj + (k - tx) * (k - tx);
                    float op = distSqr / radiusSqr;
                    float threshold = UnityEngine.Random.value;
                    if (op <= 1f && threshold * op < fuzziness) {
                        maskColors[jj + k].a = (byte)(colort + maskColors[jj + k].a * brushOpacity);
                    }
                }
            }
            fog.UpdateFogOfWar(true);
        }


        private void PaintColorOnMaskPosition(Vector3 pos) {
            // Get texture location
            Color32[] maskColors = fog.fogOfWarTextureData;
            if (maskColors == null) {
                fog.ReloadFogOfWarTexture();
                maskColors = fog.fogOfWarTextureData;
            }
            if (maskColors == null) {
                EditorUtility.DisplayDialog("Fog Of War Editor", "Re-enable fog of war to create the underline texture.", "Ok");
                return;
            }
            Vector3 fogOfWarCenter = fog.anchoredFogOfWarCenter;
            float x = (pos.x - fogOfWarCenter.x) / fog.fogOfWarSize.x + 0.5f;
            float z = (pos.z - fogOfWarCenter.z) / fog.fogOfWarSize.z + 0.5f;
            int textureSize = fog.fogOfWarTextureSize;
            int tx = Mathf.Clamp((int)(x * textureSize), 0, textureSize - 1);
            int ty = Mathf.Clamp((int)(z * textureSize), 0, textureSize - 1);

            // Prepare brush data
            int brushSize = Mathf.FloorToInt(fog.fogOfWarTextureSize * maskBrushWidth.intValue / fog.fogOfWarSize.x);
            float brushOpacity = 1f - maskBrushOpacity.floatValue * 0.2f;
            float fuzziness = 1.1f - maskBrushFuzziness.floatValue;
            byte rt = (byte)(maskBrushColor.colorValue.r * (1f - brushOpacity) * 255f);
            byte gt = (byte)(maskBrushColor.colorValue.g * (1f - brushOpacity) * 255f);
            byte bt = (byte)(maskBrushColor.colorValue.b * (1f - brushOpacity) * 255f);
            Color32 colort = new Color32(rt, gt, bt, 255);
            float radiusSqr = brushSize * brushSize;
            // Paint!
            for (int j = ty - brushSize; j < ty + brushSize; j++) {
                if (j < 0) continue; else if (j >= textureSize) break;
                int jj = j * textureSize;
                int dj = (j - ty) * (j - ty);
                for (int k = tx - brushSize; k < tx + brushSize; k++) {
                    if (k < 0) continue; else if (k >= textureSize) break;
                    int distSqr = dj + (k - tx) * (k - tx);
                    float op = distSqr / radiusSqr;
                    float threshold = UnityEngine.Random.value;
                    if (op <= 1f && threshold * op < fuzziness) {
                        maskColors[jj + k].r = (byte)(colort.r + maskColors[jj + k].r * brushOpacity);
                        maskColors[jj + k].g = (byte)(colort.g + maskColors[jj + k].g * brushOpacity);
                        maskColors[jj + k].b = (byte)(colort.b + maskColors[jj + k].b * brushOpacity);
                    }
                }
            }
            fog.UpdateFogOfWar(true);
        }


        #endregion

    }

}