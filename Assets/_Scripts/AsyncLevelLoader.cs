using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLevelLoader : MonoBehaviour
{
    public static AsyncLevelLoader asyncLevelLoader;
    bool playerLoaded;

    [SerializeField] private GameObject playerObject;
    [SerializeField] private Vector3 spawnPosition; //default: 

    private void Awake()
    {
        asyncLevelLoader = this;

        if(playerObject == null)
            playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    public void LoadScene(string sceneName, string sceneToUnload) //TODO: manual call from menu Play(), can combine this into one script (LevelLoader)
    {
        //EndPortal script is in the scene we are leaving
        //is passes the name of the scene we want to load, and the current scene to unload after

        //TODO: start loading screen

        StartCoroutine(LoadSceneCO(sceneName, sceneToUnload));

        //SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //TODO: when done, reposition Player object
        
        //TODO: end loading screen

        // Unload prev scene async
        //UnloadScene(sceneToUnload);
    }

    IEnumerator LoadSceneCO(string sceneName, string sceneToUnload)
    {
        AsyncOperation sceneToLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!sceneToLoad.isDone)
        {
            yield return null;
        }
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        if(playerObject != null)
            playerObject.transform.position = spawnPosition;

        UnloadScene(sceneToUnload);
    }

    public void LoadScene(string sceneName) //TODO: manual call from menu Play(), can combine this into one script (LevelLoader)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void UnloadScene(string sceneName)
    {
        StartCoroutine(Unload(sceneName));
    }

    IEnumerator Unload(string sceneName)
    {
        yield return null; //delay before starting scene unload, otherwise crash
        SceneManager.UnloadSceneAsync(sceneName); //UnloadScene()
    }

    public void LoadPlayer()
    {
        StartCoroutine(StartLoadPlayer());
    }

    IEnumerator StartLoadPlayer()
    {
        playerLoaded = false;
        //In case we already have a Player object loaded, this unloads the old Player scene and loads a new one
        //Only call this when starting a game, or restarting.
        //TODO: needs testing for Restart //TODO: can workaround by sending player to MainMenu on death
        for (int i=0; i<SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == "_NeverUnload")
            {
                UnloadScene("_NeverUnload");
            }
        }

        yield return null;
        var playerScene = SceneManager.LoadSceneAsync("_NeverUnload", LoadSceneMode.Additive);

        while (!playerScene.isDone)
        {
            Debug.Log("loading player");
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("_NeverUnload"));

        playerLoaded = true;
    }

    public void StartGame(string startStage, string unloadStage)
    {
        //1) Load Player scene (_NeverUnload)
        //2) When loaded, set Player scene as active scene (allows us to unload MainMenu scene)
        //3) Unload MainMenu
        //4) Load TutorialStage
        
        StartCoroutine(StartGameCO(startStage, unloadStage));
    }

    IEnumerator StartGameCO(string startStage, string unloadStage)
    {
        LoadPlayer();

        while (!playerLoaded)
        {
            Debug.Log("loading player");
            yield return null;
        }

        SceneManager.UnloadSceneAsync(unloadStage);
        LoadScene(startStage);
    }
}
