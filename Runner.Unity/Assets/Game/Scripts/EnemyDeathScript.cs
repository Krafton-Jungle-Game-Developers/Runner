using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// !!!!!!!!!! This Script is Temporary. !!!!!!!!!!
/// Used to test Enemy Counter UI. 
/// Please merge functions to REAL enemy script later. 
/// Currently Implemented in Enemy > DeathCollider
/// </summary>
public class EnemyDeathScript : MonoBehaviour
{
    [SerializeField] private GameObject Enemyobject;
    private string killmethod = "KillCountUp";

    /*private void Start()
    {
        Enemyobject = gameObject.transform.parent.gameObject;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        // Update Enemy Counter
        GameObject.FindGameObjectWithTag("EnemyCounter").SendMessage(killmethod);

        // Change to DEAD State
        Death();
    }
    

    // Script to Change Enemy Object to DEAD State. 
    private void Death()
    {
        Destroy(Enemyobject);
    }
}
