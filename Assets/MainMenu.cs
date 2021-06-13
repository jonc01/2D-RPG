using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
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

    /*public void OpenSettings()
    {
        //mainMenuUI.SetActive(false);
        //settingsUI.SetActive(true);
        //...
    }*/

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
