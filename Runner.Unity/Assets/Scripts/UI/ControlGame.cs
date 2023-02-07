using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This Script is used for the player to Control the Game. 
/// Currently, press R to reset Scene. 
/// press esc to activate PauseMenu.
/// </summary>
public class ControlGame : MonoBehaviour
{
    #region Key Variables
    public KeyCode resetKey = KeyCode.R;
    public KeyCode menuKey = KeyCode.Escape;
    #endregion

    public GameObject pauseMenu;
    public static bool isPaused = false;
    // global variable to control game pause. 
    // Use this to control inputs when on pause. 

    #region ControlGameFunctions
    public void ResetTheGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        print("Reset Game Scene.");
    }

    // Pause game and Open Menu Scene
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused= true;
    }

    // if Menu Scene is open, close it and resume to game scene.
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    // works only when built. 
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion


    private void Start()
    {
        // Game Starts with pausemenu deactivated. 
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (!isPaused && Input.GetKeyUp(resetKey))
        {
            ResetTheGame();
        }

        if (!isPaused && Input.GetKeyUp(menuKey))
        {
            PauseGame();
        } else if (Input.GetKeyUp(menuKey))
        {
            ResumeGame();
        }

    }
}
