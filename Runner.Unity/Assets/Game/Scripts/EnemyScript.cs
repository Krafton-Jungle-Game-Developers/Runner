using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for Enemy Prefab.
/// </summary>
public class EnemyScript : MonoBehaviour
{
    private RaycastHit hit;

    

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Debug.Log("hit point : " + hit.point + 
                      ", distance : " + hit.distance + 
                      ", name : " + hit.collider.name);
            
            Debug.DrawRay(transform.position, 
                          transform.forward * hit.distance, 
                          Color.red);

        } else
        {
            Debug.DrawRay(transform.position,
                          transform.forward * 1000f,
                          Color.red);
        }
    }


}
