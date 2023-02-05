using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for Triggering when playercharacter enters "Goal Space"
/// Sends trigger to Timer.
/// </summary>
public class GoalSpace : MonoBehaviour
{
    // name variables 
    private string objectName = "FirstPersonController";
    private string message = "EndTimer";

    
    private void OnTriggerEnter(Collider other)
    {
        GameObject.Find(objectName).SendMessage(message);
    }
}
