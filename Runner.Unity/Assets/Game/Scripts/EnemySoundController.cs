using UnityEngine;

namespace Runner.Game
{
    public class EnemySoundController : MonoBehaviour
    {
        [SerializeField] private AudioClip deathAudioClip;
        private AudioSource _audioSource;
        public AudioSource EnemyAudioSource => _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayDeathAudio() => _audioSource.PlayOneShot(deathAudioClip);
    }
}

