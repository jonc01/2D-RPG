using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject settingsMenuUI;
    public SettingsMenu settingsMenu;

    public GameObject creditsMenuUI;

    //public LevelLoader levelLoader; //TODO: delete
    
    public void PlayDemo()
    {
        mainMenuUI.SetActive(false);
        //levelLoader.LoadSelectIndexLevel(1); //TODO: clean up
        //levelLoader.LoadSelectLevel("TutorialStage");

        //AsyncLevelLoader.asyncLevelLoader.LoadPlayer();

        //AsyncLevelLoader.asyncLevelLoader.UnloadScene("MainMenu");
        AsyncLevelLoader.asyncLevelLoader.StartGame("TutorialStage", "MainMenu");
        //AsyncLevelLoader.asyncLevelLoader.LoadScene("TutorialStage", "MainMenu");
        //SceneManager.LoadScene("TutorialStage");

        //SceneManager.LoadScene("DemoLevel");
    }

    /*public void Play()
    {
        //load next scene in build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }*/

    public void OpenSettings()
    {
        mainMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
        settingsMenu.UpdateSettings();
    }
    public void SettingsToMainMenu()
    {
        mainMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }


    public void OpenCredits()
    {
        mainMenuUI.SetActive(false);
        creditsMenuUI.SetActive(true);
    }

    public void CreditsToMainMenu()
    {
        mainMenuUI.SetActive(true);
        creditsMenuUI.SetActive(false);
    }
}
