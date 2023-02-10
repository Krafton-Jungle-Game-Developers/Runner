using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Runner.UI
{
    public class DemoScenePresenter : MonoBehaviour
    {
        [SerializeField] private KeyCode resetKey = KeyCode.R;
        [SerializeField] private KeyCode menuKey = KeyCode.Escape;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Image fadePanelImage;
        [SerializeField] private bool isPaused;

        private async UniTaskVoid Awake()
        {
            pauseMenu.SetActive(false);
            await fadePanelImage.DOFade(0f, 0.5f);
        }

        private void Update()
        {
            if (!isPaused && Input.GetKeyUp(resetKey))
            {
                ResetGame();
            }

            if (!isPaused && Input.GetKeyUp(menuKey))
            {
                PauseGame();
            }
            else if (Input.GetKeyUp(menuKey))
            {
                ResumeGame();
            }
        }

        private void UnlockMouse()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        private void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void ResetGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            print("Reset Game Scene.");
        }
        
        private void PauseGame()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            UnlockMouse();
        }
        
        private void ResumeGame()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
            isPaused = false;
            LockMouse();
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
