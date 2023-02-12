using UnityEngine;
using TMPro;
using UnityEngine.UI;

// ViewWrapper?
namespace Runner.UI
{
    public class HUDIndicator : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

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
    }
}