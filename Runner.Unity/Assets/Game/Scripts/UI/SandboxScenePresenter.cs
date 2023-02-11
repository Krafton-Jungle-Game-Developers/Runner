using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Runner.UI
{
    public class SandboxScenePresenter : MonoBehaviour
    {
        [SerializeField] private Image fadePanelImage;

        private async UniTaskVoid Awake()
        {
            await fadePanelImage.DOFade(0f, 0.5f);
        }
    }
}
