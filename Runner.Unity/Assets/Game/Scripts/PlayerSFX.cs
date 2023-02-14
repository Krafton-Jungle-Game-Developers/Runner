using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public AudioSource stepAudioSource;
    public AudioSource jumpAudioSource;
    public AudioSource groundAudioSource;
    public AudioSource stompAudioSource;
    public AudioSource dashAudioSource;

    PlayerMovementController PMC;
    [SerializeField] MovementState playerStateBuff;

    void Awake()
    {
        PMC = GetComponent<PlayerMovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PMC.isGrounded == true && PMC.playerVelocity > 2f && stepAudioSource.isPlaying == false)
        {
            //Running Audio Play
            stepAudioSource.volume = Random.Range(0.8f, 1);
            stepAudioSource.pitch = Random.Range(0.8f, 1.1f);
            stepAudioSource.Play();
            Debug.Log("Loading Running sound");

        }
        if (PMC.state != playerStateBuff)
        {
            if (PMC.state == MovementState.Dashing)
            {
                //Dashing Audio Play
                dashAudioSource.volume = Random.Range(0.8f, 1);
                dashAudioSource.pitch = Random.Range(0.8f, 1.1f);
                dashAudioSource.Play();
            }
            else if (PMC.state == MovementState.Stomping)
            {
                Debug.Log("Loading Stomp sound");
                //TODO: Stmop Audio Source Place
            }
            else if (PMC.state == MovementState.Running)
            {
                Debug.Log("Loading Ground sound");
                //On Ground Audio Play
                groundAudioSource.volume = Random.Range(0.8f, 1);
                groundAudioSource.pitch = Random.Range(0.8f, 1.1f);
                groundAudioSource.Play();
            }
            else if (PMC.state == MovementState.Air && playerStateBuff != MovementState.Dashing)
            {
                Debug.Log("Loading Ground sound");
                //Jump Audio Play
                jumpAudioSource.volume = Random.Range(0.8f, 1);
                jumpAudioSource.pitch = Random.Range(0.8f, 1.1f);
                jumpAudioSource.Play();
            }

        }
        playerStateBuff = PMC.state;
    }
}
