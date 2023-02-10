using UnityEngine;

/// <summary>
/// Basic Timer Class. 
/// Please keep do not modify this class. 
/// All timer control methods are in class TimerControl.
/// </summary>
public class Timer 
{
    // ============== Timer Variables ==============
    private float _startTime;
    private float _endTime;
    //private float _timerDuration;
    // =============================================

    // ============== Timer Methods ================
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

    public float GetDuration()
    {
        return Time.time - _startTime;
        
    }
    // --- end of Timer Methods. ---

}
