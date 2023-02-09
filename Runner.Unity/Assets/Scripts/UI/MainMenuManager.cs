using UnityEngine;

namespace Runner.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private Animator mainMenuScreenAnimator;
        [SerializeField] private TimedEvent timedEvent;

        private void OnEnable()
        {
            mainMenuScreen.SetActive(true);
        }
    }
}