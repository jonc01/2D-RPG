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

    public LevelLoader levelLoader;
    
    public void PlayDemo()
    {
        mainMenuUI.SetActive(false);
        levelLoader.LoadSelectLevel(1);

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
