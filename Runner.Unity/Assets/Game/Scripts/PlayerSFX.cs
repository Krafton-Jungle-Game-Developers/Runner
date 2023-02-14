using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public AudioSource Jump;
    public AudioSource Dash;
    public AudioSource Stomp;
    public AudioSource Step;

    PlayerMovementController PMC;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        PMC = GetComponent<PlayerMovementController>();   
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PMC.isGrounded == true && PMC.playerVelocity > 2f && audioSource.isPlaying == false)
        {
            audioSource.volume = Random.Range(0.8f, 1);
            audioSource.pitch = Random.Range(0.8f, 1.1f);
            audioSource.Play();
            Debug.Log("Loading Walksound");
        }
    }
}
