using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPlatform : MonoBehaviour
{
    public PlayerMovementController playerController; 

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        playerController.acceleration *= 1.5f;
        playerController.maxSpeed *= 2f;
        playerController.deceleration = 0.05f;
        playerController.isBoosting = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerController.acceleration /= 1.5f;
        playerController.maxSpeed /= 2f;
        playerController.deceleration = 10f;
        playerController._keepMomentum = true;
        playerController._speedChangeFactor = 1.5f;
        playerController.isBoosting = false;
    }
}
