using UnityEngine;


/// <summary>
/// Script for Triggering when playercharacter enters "Goal Space"
/// Sends trigger to Timer.
/// </summary>
public class GoalSpace : MonoBehaviour
{
    private string message = "EndProcess";

    private void OnTriggerEnter(Collider other)
    { 
        GameObject.FindGameObjectWithTag("Player").SendMessage(message); 
    }
}
