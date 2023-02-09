using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Runner.UI
{
    public class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private SceneLoader sceneLoader;

        private void Awake()
        {
            playButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                fadeCanvasGroup.DOFade(0f, 0.5f);
                sceneLoader.LoadSceneAsync("Demo").Forget();
            });

            optionsButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                Debug.Log("Options Button Pressed.");
            });

            quitButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                Application.Quit();
            });
        }
    }    
}
