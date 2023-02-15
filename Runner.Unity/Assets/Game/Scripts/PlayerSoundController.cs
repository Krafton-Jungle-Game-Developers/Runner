using UnityEngine;
using UniRx;
using DG.Tweening;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private AudioClip stepAudioClip;
    [SerializeField] private AudioClip dashAudioClip;
    [SerializeField] private AudioClip landAudioClip;
    [SerializeField] private AudioClip stompAudioClip;
    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] private AudioClip boostAudioClip;

    MovementState playerStateBuff;

    private PlayerMovementController _movementController;

    private void Start()
    {
        _movementController = GetComponent<PlayerMovementController>();
        audioSource.volume = Random.Range(0.8f, 1);
        audioSource.pitch = Random.Range(0.8f, 1.1f);

        _movementController.State.Subscribe(state => 
        {
            PlayMatchingClip(state);
            playerStateBuff = state;
        }).AddTo(this);
    }

    private void PlayMatchingClip(MovementState state)
    {
        switch (state) 
        {
            //jump는 signal 받아서 처리
            case MovementState.Running:
                if(playerStateBuff == MovementState.Stomping)
                {
                    audioSource.PlayOneShot(stompAudioClip);
                }
                else if(playerStateBuff == MovementState.Air)
                {
                    audioSource.PlayOneShot(landAudioClip);
                }
                audioSource.loop = true;
                audioSource.clip = stepAudioClip;
                audioSource.Play();
                break;
            case MovementState.Dashing:
                audioSource.loop = false;
                audioSource.PlayOneShot(dashAudioClip);
                break;
            //TODO :Delete(처리내용 X)
            case MovementState.Stomping://(->Running, Idle시, Stompsound)
                audioSource.loop = false;
                Debug.Log($"상태 변경 발생: {state}");
                break;
            //TODO :Delete(처리내용 X)
            case MovementState.Air: //(->Running/Idle시, Landsound), Running/Idle ->시, JumpsoundX (signal로 처리) 
                audioSource.loop = false;
                Debug.Log($"상태 변경 발생: {state}");
                break;
            case MovementState.Boosting: //(->Running/Idle시, Landsound), Running/Idle ->시, JumpsoundX (signal로 처리)
                audioSource.loop = true;
                audioSource.clip = boostAudioClip; ;
                audioSource.Play();
                break;
            //case MovementState.Idle:
            //    if (playerStateBuff == MovementState.Stomping)
            //    {
            //        audioSource.PlayOneShot(stompAudioClip);
            //    }
            //    else if (playerStateBuff == MovementState.Air)
            //    {
            //        audioSource.PlayOneShot(landAudioClip);
            //    }
            //    audioSource.loop = false;
            //    Debug.Log($"상태 변경 발생: {state}");
            //    break;
            default:
                break;
        }
    }
}
