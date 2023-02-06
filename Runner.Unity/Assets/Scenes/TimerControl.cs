using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used for controlling class Timer.
/// if you need to Modify TIMER, please use this class.
/// Is implemented in TimerText.
/// Currently, timer starts when scene activates.
/// Ends when Player Character Triggers "Goal Space"
/// </summary>
public class TimerControl : MonoBehaviour
{
    public Text timerText;
    Timer timer = new Timer();
    //public Timer timer;

    // Change these Variables for Timer
    #region TimerControlVariables
    public bool timerRunBool = false;
    public string secDecimal = "f2";
    public Color initColor = Color.white;
    public Color finishColor = Color.yellow;
    #endregion

    #region TimerControlMethods
    public void StartProcess()
    {
        // Timer Init 
        timerRunBool = true;
        timerText.color = initColor;
        timer.StartTimer();
    }
    
    public void EndProcess()
    {
        timerRunBool = false;
        timerText.color = finishColor;
        timer.EndTimer();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {        
        // timer = FindObjectOfType<Timer>();
        StartProcess();   
    }

    // Update is called once per frame
    void Update()
    {
        if (!timerRunBool)
        {
            return;
        }

        float timerDuration = timer.Duration();
        string minutes = ((int)timerDuration / 60).ToString();
        string seconds = (timerDuration % 60).ToString(secDecimal);

        timerText.text = minutes + "m  " + seconds + "s";
    }
}
