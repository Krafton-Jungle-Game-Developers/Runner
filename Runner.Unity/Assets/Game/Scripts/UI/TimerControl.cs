using UnityEngine;
using TMPro;

/// <summary>
/// This class is used for controlling class Timer.
/// if you need to Modify TIMER, please use this class.
/// Is implemented in TimerText.
/// Currently, timer starts after pressing any button.
/// Ends when Player Character Triggers "Goal Space"
/// </summary>
public class TimerControl : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    private Timer _timer = new();

    // ========= Timer Control Variables =============
    // Change these Variables for Timer
    [SerializeField] private bool timerRunBool = false;
    [SerializeField] private bool isGameFinish = false;
    [SerializeField] private string secDecimal = "f2";
    [SerializeField] private Color initColor = Color.white;
    [SerializeField] private Color finishColor = Color.yellow;
    // ===============================================

    private void Start()
    {        
        // TODO : Prepare Timer GUI 
    }

    private void Update()
    {
        if (!timerRunBool)
        {
            // Timer Starts when player first press anykey. 
            if (!isGameFinish && Input.anyKey)
            {
                StartProcess();
            } 
            return;
        }

        float timerDuration = _timer.GetDuration();
        string minutes = ((int)timerDuration / 60).ToString();
        string seconds = (timerDuration % 60).ToString(secDecimal);

        timerText.text = minutes + "m  " + seconds + "s";
    }

    // ============ Timer Control Methods ===============
    private void StartProcess()
    {
        // Timer Init 
        timerRunBool = true;
        timerText.color = initColor;
        _timer.StartTimer();
    }

    // This Method is currently triggered colliding with "Goal Space". 
    private void EndProcess()
    {
        timerRunBool = false;
        isGameFinish = true;
        timerText.color = finishColor;
        _timer.EndTimer();
    }

    // --- End of Timer Control Methods. ---
}
