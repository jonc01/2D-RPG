﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] string currentStage;

    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public SettingsMenu settingsMenu;
    public GameObject settingsMenuObj;
    public GameObject gameSettings;
    public GameObject videoSettings;

    private void Start()
    {
        currentStage = gameObject.scene.name;
    }

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
        gameSettings.SetActive(false);
        videoSettings.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void RestartLevel() //For use with Demo level only //TODO: remove once stages are added
    {
        //TODO: needs more testing

        //Load TutorialStage or FirstStage instead, Restart Run
        AsyncLevelLoader.asyncLevelLoader.StartGame("TutorialStage", currentStage);
        //!TODO: doesn't work 

        //!!TODO: use same approach as LoadPlayer, isn't working now because it doesn't load the duplicate stage
        

        Resume();
    }

    public static void LoadMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
        //SceneManager.LoadScene(0);
    }

    public void OpenSettingsMenu()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);

        if(settingsMenu != null)
            settingsMenu.UpdateSettings();
    }

    public void SettingsToGameSettings()
    {
        settingsMenuObj.SetActive(false);
        gameSettings.SetActive(true);
    }

    public void SettingsToVideoSettings()
    {
        settingsMenuObj.SetActive(false);
        videoSettings.SetActive(true);
    }

    public void GameOrVideoSettingsToSettingsMenu()
    {
        settingsMenuObj.SetActive(true);
        videoSettings.SetActive(false);
        gameSettings.SetActive(false);
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
