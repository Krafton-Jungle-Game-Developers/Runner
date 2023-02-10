using UnityEngine;

namespace Runner.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private Animator mainMenuScreenAnimator;

        private void OnEnable()
        {
            mainMenuScreen.SetActive(true);
        }
    }
}