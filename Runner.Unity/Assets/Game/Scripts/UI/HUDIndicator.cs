using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

// ViewWrapper?
namespace Runner.UI
{
    public class HUDIndicator : MonoBehaviour
    {
        [SerializeField] public RectTransform rectTransform;
        [SerializeField] public CanvasGroup canvasGroup;

        [Header("Onscreen")]
        [SerializeField] private RectTransform onScreenRect;
        [SerializeField] private Image onScreenIcon;

        [Header("Offscreen")]
        [SerializeField] private RectTransform offScreenRect;
        [SerializeField] private RectTransform offScreenPointer;
        [SerializeField] private Image offScreenIcon;

        [Header("Distance Text")]
        [SerializeField] private TMP_Text onScreenDistanceText;
        [SerializeField] private TMP_Text offScreenDistanceText;
        private bool _isInIndicatorRadius;
        public bool IsInIndicatorRadius { get; set; }

        public class Factory : PlaceholderFactory<HUDIndicator> { }

        //TODO: implement functions to update view. those will be called from presenter.
        public bool IsVisibleOnScreen(Vector3 screenPos)
        {
            return screenPos.z > 0 
                && screenPos.x > 0 
                && screenPos.x < Screen.width 
                && screenPos.y > 0 
                && screenPos.y < Screen.height;
        }

        public void SetIndicatorOffscreenRotation(Quaternion rotation)
        {
            offScreenPointer.transform.rotation = rotation;
        }

        public void ShowIndicatorDistance(bool onScreen, int distance = 0)
        {
            TMP_Text distanceText = (onScreen) ? onScreenDistanceText : offScreenDistanceText;
            distanceText.text = $"{distance}m";
        }

        public void SetIndicatorOnOffscreen(bool value)
        {
            if (value != onScreenRect.gameObject.activeSelf) { onScreenRect.gameObject.SetActive(value); }
            if (!value != offScreenRect.gameObject.activeSelf) { offScreenRect.gameObject.SetActive(!value); }
        }

        public void SetIndicatorPosition(Vector3 indicatorPos, RectTransform parentRect = null)
        {
            transform.position = (parentRect != null) ? new Vector3(indicatorPos.x + parentRect.localPosition.x, indicatorPos.y + parentRect.localPosition.y, 0f) 
                                                      : indicatorPos;
        }

        public void SetMarkerScale(float distance, float scaleDistance, float minScale, RectTransform prefabRect)
        {
            if (prefabRect == null) { return; }
            
            float scale = (distance - 1f) / (scaleDistance - 1f);
            prefabRect.localScale = Vector2.Lerp(Vector2.one * minScale, Vector2.one, Mathf.Clamp01(scale));
        }

        public void SetMarkerFade(float distance, float fadeDistance, float minFade, CanvasGroup canvasGroup)
        {
            if (canvasGroup == null) { return; }
            
            float fade = (distance - 1f) / (fadeDistance - 1f);
            canvasGroup.alpha = Mathf.Lerp(1f * minFade, 1f, Mathf.Clamp01(fade));
        }
    }
}