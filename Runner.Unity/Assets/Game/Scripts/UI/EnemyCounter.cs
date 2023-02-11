using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is used to control Enemy Counter UI. 
/// Is implemented in EnemyCounter.
/// If player kills Enemy, it should call function KillCountUp(). 
/// </summary>
public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text enemyCounter;

    // ============= Enemy Counter Variables ==============
    private int allEnemyNumInScene;
    private int killedEnemyNum;
    
    // ====================================================
    
    // Start is called before the first frame update
    void Start()
    {
        killedEnemyNum= 0;
        allEnemyNumInScene = GameObject.FindGameObjectsWithTag("Enemy").Length;
        enemyCounter.text = killedEnemyNum.ToString() 
                            + "/" 
                            + allEnemyNumInScene.ToString(); 
    }

    // Update is called once per frame
    void Update()
    {
        /* if (killedEnemyNum == allEnemyNumInScene)
        {
            AllEnemyKill();
        }
        */
    }

    // ============== Enemy Counter Methods ================
    
    // Method is called when player kills Tag : Enemy
    private void KillCountUp() 
    {
        killedEnemyNum++;
        enemyCounter.text = killedEnemyNum.ToString()
                            + "/"
                            + allEnemyNumInScene.ToString();
    }

    private void AllEnemyKill()
    {

    }


    // --- End of Enemy Counter Methods ---
}
