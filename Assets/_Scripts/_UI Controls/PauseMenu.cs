﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public SettingsMenu settingsMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void RestartLevel() //For use with Demo level only
    {
        SceneManager.LoadScene("DemoLevel");
        Resume();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        //SceneManager.LoadScene(0);
    }

    public void OpenSettingsMenu()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
        settingsMenu.UpdateSettings();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }



    public void SettingsToPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
    }
}
