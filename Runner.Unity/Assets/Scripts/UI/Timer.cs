using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Basic Timer Class. 
/// Please keep do not modify this class. 
/// All timer control methods are in class TimerControl.
/// </summary>
public class Timer 
{
    #region TimerVariables 
    private float _startTime;
    private float _endTime;
    //private float _timerDuration;

    #endregion

    #region TimerMethods
    public void StartTimer()
    {
        _startTime = Time.time; 
        
    }

    public void EndTimer()
    {
        _endTime = Time.time;
          
    }

    public void PauseTimer()
    {
        // TODO
    }

    public float Duration()
    {
        return Time.time - _startTime;
        
    }
    #endregion

}
