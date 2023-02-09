using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Runner.UI
{
    public class SceneLoader : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public async UniTaskVoid LoadSceneAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
