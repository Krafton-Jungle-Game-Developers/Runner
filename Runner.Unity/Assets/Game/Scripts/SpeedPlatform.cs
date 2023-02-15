using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPlatform : MonoBehaviour
{
    [HideInInspector] public PlayerMovementController playerController;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        playerController.acceleration *= 8f;
        playerController.deceleration /= 10f;
        playerController.isBoosting = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerController.acceleration /= 8f;
        playerController.deceleration *= 10f;
        playerController.isBoosting = false;
    }
}
