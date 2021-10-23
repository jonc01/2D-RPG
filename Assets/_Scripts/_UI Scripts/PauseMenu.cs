using System.Collections;
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

    public void RestartLevel() //For use with Demo level only //TODO: remove once stages are added
    {
        //TODO: load TutorialStage or FirstStage instead, Restart Run
        AsyncLevelLoader.asyncLevelLoader.LoadPlayer();

        string[] sceneToUnload = null;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name != "TutorialStage" || scene.name != "_NeverUnload")
            {
                sceneToUnload[i] = scene.name;
            }
        }
        if(sceneToUnload != null)
        {
            for(int i=0; i<3; i++)
            {
                AsyncLevelLoader.asyncLevelLoader.UnloadScene(sceneToUnload[i]);
            } //TODO: this is mostly a disaster, it doesn't crash, but it breaks all the dependencies
        }

        AsyncLevelLoader.asyncLevelLoader.LoadScene("TutorialStage");

        //!-This will load Tutorial stage with the Player Object, don't bring an additional Player object

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
