using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// !!!!!!!!!! This Script is Temporary. !!!!!!!!!!
/// Used to test Enemy Counter UI. 
/// Please merge functions to REAL enemy script later. 
/// </summary>
public class TEMPEnemyScript : MonoBehaviour
{
    private string killmethod = "KillCountUp";
    private void OnCollisionEnter(Collision collision)
    {
        // Update Enemy Counter
        GameObject.FindGameObjectWithTag("EnemyCounter").SendMessage(killmethod);

        // Change to DEAD State
        Death();
    }

    // Script to Change Enemy Object to DEAD State. 
    private void Death()
    {
        Destroy(gameObject);
    }
}
