using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basic Timer Class.
/// Is implemented in TimerText.
/// Currently, timer starts when scene activates. 
/// Ends when Player Character Triggers "Goal Space"
/// </summary>
public class Timer : MonoBehaviour
{
    public Text timerText;
    
    #region TimerVariables 
    private float startTime;
    public float endTime;
    public float timerDuration;
    private bool isRunning = true;
    private string secDecimal = "f2";
    #endregion

    #region TimerMethods
    public void EndTimer()
    {
        isRunning= false;   
        timerText.color= Color.yellow;  
    }
    #endregion

    // Start is called before the first frame update
    void Start()    
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
        {
            return;
        }
        
        timerDuration= Time.time - startTime;   

        string minutes = ((int)timerDuration / 60).ToString();
        string seconds = (timerDuration % 60).ToString(secDecimal);

        timerText.text = minutes + "m  " + seconds + "s";
    }
}
