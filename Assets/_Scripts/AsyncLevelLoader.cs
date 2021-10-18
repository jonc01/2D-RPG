using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLevelLoader : MonoBehaviour
{
    public static AsyncLevelLoader asyncLevelLoader;

    bool gameStart; //TODO: might not use

    //private void Awake() //TODO: !this is what video uses to start game, would be similar to loading Main menu
    //{
    //    //we don't use this because we start on main menu (1 scene, then Play loads Scene1(Player), and Scene2(Tutorial)
    //    if (!gameStart)
    //    {
    //        asyncLevelLoader = this;
    //        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    //        gameStart = true;
    //    }
    //}

    private void Awake()
    {
        asyncLevelLoader = this;
    }

    public void LoadScene(string sceneName, string sceneToUnload) //TODO: manual call from menu Play(), can combine this into one script (LevelLoader)
    {
        //Scene sceneToUnload = SceneManager.GetActiveScene(); //TODO: make sure this doesn't pick up NeverUnload
        //string sceneNameToUnload = sceneToUnload.name;
        Debug.Log("scene to UNLOAD: " + sceneToUnload);

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Debug.Log("scene to LOAD: " + sceneName);
        
        gameStart = true; //TODO: ??????

        // Unload prev scene async
        UnloadScene(sceneToUnload);
    }

    public void UnloadScene(string sceneName)
    {
        StartCoroutine(Unload(sceneName));
    }

    //public void UnloadPrevScene()
    //{
    //    //int scene1 = SceneManager.GetActiveScene().buildIndex;
        
    //}

    IEnumerator Unload(string sceneName) //TODO: needs testing, get current stage
    {
        
        yield return null;

        SceneManager.UnloadSceneAsync(sceneName); //UnloadScene()
        //SceneManager.UnloadSceneAsync(sceneName); //TODO: above is deprecated
    }
    //////////////////////////////////////////////////
    ///

    //public static LevelLoader levelLoader;
    //public string sceneName;

    

}
