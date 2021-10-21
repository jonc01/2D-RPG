using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLevelLoader : MonoBehaviour
{
    public static AsyncLevelLoader asyncLevelLoader;

    private void Awake()
    {
        asyncLevelLoader = this;
    }

    public void LoadScene(string sceneName, string sceneToUnload) //TODO: manual call from menu Play(), can combine this into one script (LevelLoader)
    {
        //EndPortal script is in the scene we are leaving
        //is passes the name of the scene we want to load, and the current scene to unload after
        Debug.Log("scene to UNLOAD: " + sceneToUnload);

        //TODO: start loading screen

        Debug.Log("scene to LOAD: " + sceneName);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //Debug.Log("LOADING Scene: " + sceneName);
        //TODO: when done, reposition Player object

        //TODO: end loading screen

        // Unload prev scene async
        UnloadScene(sceneToUnload);
    }

    public void UnloadScene(string sceneName)
    {
        Debug.Log("UNLOADING SCENE: " + sceneName);
        StartCoroutine(Unload(sceneName));
    }

    IEnumerator Unload(string sceneName)
    {
        yield return null; //delay before starting scene unload, otherwise crash
        SceneManager.UnloadSceneAsync(sceneName); //UnloadScene()
        Debug.Log("UNLOADED: " + sceneName);
    }

    public void LoadScene(string sceneName) //TODO: manual call from menu Play(), can combine this into one script (LevelLoader)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Debug.Log("LOADING Scene: " + sceneName);
    }

    public void LoadPlayer()
    {
        StartCoroutine(StartLoadPlayer());
    }

    IEnumerator StartLoadPlayer()
    {
        for(int i=0; i<SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == "_NeverUnload")
            {
                UnloadScene("_NeverUnload");
            }
        }

        yield return null;
        LoadScene("_NeverUnload");
    }

    public void StartGame(string startStage)
    {
        //TODO: MainMenu gets unloaded while trying to load rest of scenes
    }
}
