using UnityEngine;
using UnityEngine.UI;

namespace Runner.UI
{
    public class MainMenuCanvasManager : MonoBehaviour
    {
        [SerializeField] private CanvasScaler canvasScaler;

        private void ScaleCanvas(int scale = 1080)
        {
            canvasScaler.referenceResolution = new Vector2(canvasScaler.referenceResolution.x, scale);
        }
    }
}
