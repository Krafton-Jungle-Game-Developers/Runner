using UnityEngine;
using UniRx;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private AudioClip stepAudioClip;
    [SerializeField] private AudioClip dashAudioClip;
    [SerializeField] private AudioClip landAudioClip;
    [SerializeField] private AudioClip stompAudioClip;
    [SerializeField] private AudioClip jumpAudioClip;

    private PlayerMovementController _movementController;

    private void Start()
    {
        _movementController = GetComponent<PlayerMovementController>();
        audioSource.volume = Random.Range(0.8f, 1);
        audioSource.pitch = Random.Range(0.8f, 1.1f);

        _movementController.State.Subscribe(state => 
        {
            PlayMatchingClip(state);
        }).AddTo(this);
    }

    private void PlayMatchingClip(MovementState state)
    {
        switch (state) 
        {
            case MovementState.Running:
                audioSource.loop = true;
                audioSource.clip = stepAudioClip;
                audioSource.Play();
                break;
            case MovementState.Dashing:
                audioSource.loop = false;
                audioSource.PlayOneShot(dashAudioClip);
                break;
            case MovementState.Stomping:
                audioSource.loop = false;
                Debug.Log($"상태 변경 발생: {state}");
                break;
            case MovementState.Air:
                audioSource.loop = false;
                Debug.Log($"상태 변경 발생: {state}");
                break;
            default:
                break;
        }
    }
}
