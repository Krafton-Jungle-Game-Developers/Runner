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
    [SerializeField] private TMP_Text SlayCounterText;
    [SerializeField] private TMP_Text TotalEnemyText;
    

    // ============= Enemy Counter Variables ==============
    private int allEnemyNumInScene;
    private int killedEnemyNum;

    // ====================================================

    // Start is called before the first frame update
    void Start()
    {
        killedEnemyNum = 0;
        allEnemyNumInScene = GameObject.FindGameObjectsWithTag("Enemy").Length;
        SlayCounterText.text = killedEnemyNum.ToString();
        TotalEnemyText.text = allEnemyNumInScene.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (killedEnemyNum == allEnemyNumInScene)
        {
            AllEnemyKill();
        }*/

    }

    // ============== Enemy Counter Methods ================

    // Method is called when player kills Tag : Enemy
    private void KillCountUp()
    {
        killedEnemyNum++;
        SlayCounterText.text = killedEnemyNum.ToString();
        TotalEnemyText.text = allEnemyNumInScene.ToString();
    }

    /*private void AllEnemyKill()
    {
        enemyCounter.text = "ALL KILL!";
        enemyCounter.color = Color.green;
    }*/


    // --- End of Enemy Counter Methods ---
}
