using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition; //TODO: placeholder, separate fade-out and fade-in
    public float transitionTime = 1f; //TODO: placeholder, use async

    private AsyncOperation sceneAsync;

    //GameObjects to transfer between scenes //TODO: combine most of these under one GameObject
    /*public GameObject MainCameraObject;
    public GameObject CMvcamObject;
    public GameObject PlayerObject;
    public GameObject PlayerUIObject;*/

    //public string sceneName; //TODO: not using
    bool loaded;

    private void Start()
    {
        /*if(MainCameraObject == null)
            MainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");

        if (CMvcamObject == null)
            CMvcamObject = GameObject.FindGameObjectWithTag("CineMachineCam");

        if (PlayerObject == null)
        {
            PlayerObject = GameObject.FindGameObjectWithTag("Player");
            //PlayerObject = GameObjct.Find("PlayerMainObject"); //Character (Knight, Archer, etc) should be child
        }

        if (PlayerUIObject == null)
            PlayerUIObject = GameObject.Find("PlayerUICanvas");*/

    }

    public void LoadSelectIndexLevel(int buildIndex, bool bringPlayer = true) //TODO: not using??
    {
        //StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        //StartCoroutine(LoadLevelIndex(buildIndex, bringPlayer));
    }

    public void LoadSelectLevel(string sceneName) //TODO: needs testing
    {
        //bringPlayer is only false when restarting, or reloading Tutorial
        //StartCoroutine(LoadLevel(level, bringPlayer));
        //StartCoroutine(LoadTEST(level, bringPlayer));
        //StartCoroutine(LoadTESTasync(level, bringPlayer));

        if (!loaded)
        {

            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            loaded = true;
        }
    }
    
    public void UnloadScene(string sceneName)
    {
        StartCoroutine(UnloadSceneCO(sceneName));
    }

    IEnumerator UnloadSceneCO(string sceneName)
    {
        yield return null;
        SceneManager.UnloadScene(sceneName);    //TODO: this is deprecated
        //SceneManager.UnloadSceneAsync(sceneName); //TODO: needs testing
    }

    /*IEnumerator LoadTESTasync(string sceneName)
    {

        yield return null;
    }*/

    //============================
    /*IEnumerator LoadTEST(string sceneName, bool bringPlayer)
    {
        Scene activeScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        Scene nextScene = SceneManager.GetSceneAt(1); //! Not build index, this is from currently loaded scenes

        if (bringPlayer)
        {
            if (PlayerObject && PlayerUIObject && MainCameraObject && CMvcamObject)
            {
                SceneManager.MoveGameObjectToScene(MainCameraObject, nextScene);
                SceneManager.MoveGameObjectToScene(CMvcamObject, nextScene);
                SceneManager.MoveGameObjectToScene(PlayerObject, nextScene);
                SceneManager.MoveGameObjectToScene(PlayerUIObject, nextScene);
            }
        }
        yield return null;

        SceneManager.UnloadSceneAsync(activeScene);

    }*/


    //==================== Loading Scenes with Scene Name ==========================

    /*IEnumerator LoadLevel(string sceneName, bool bringPlayer)
    {
        transition.SetTrigger("Start");

        sceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        sceneAsync.allowSceneActivation = false;
        //sceneAsync = scene;

        /*while(scene.progress < 0.9f)
        {
            
            //yield return null;
        }*/
    /*
        while (!sceneAsync.isDone)
        {
            Debug.Log("Loading: " + sceneAsync.progress);
            yield return null;
        }

        EnableScene(sceneName, bringPlayer);
    }

    void EnableScene(string sceneName, bool bringPlayer)
    {
        Debug.Log("Done loading scene.");
        sceneAsync.allowSceneActivation = true;
        Scene sceneToLoad = SceneManager.GetSceneByName(sceneName);

        if (sceneToLoad.IsValid())
        {
            Debug.Log("Scene is valid");
            if (bringPlayer)
            {
                if(PlayerObject && PlayerUIObject && MainCameraObject && CMvcamObject)
                {
                    SceneManager.MoveGameObjectToScene(MainCameraObject, sceneToLoad);
                    SceneManager.MoveGameObjectToScene(CMvcamObject, sceneToLoad);
                    SceneManager.MoveGameObjectToScene(PlayerObject, sceneToLoad);
                    SceneManager.MoveGameObjectToScene(PlayerUIObject, sceneToLoad);
                }
            }
            SceneManager.SetActiveScene(sceneToLoad);
        }
    }*/

    //==================== Loading Scenes with Build Index ==========================

    /*IEnumerator LoadLevelIndex(int levelIndex, bool bringPlayer)
    {
        transition.SetTrigger("Start");

        AsyncOperation scene = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Additive);
        scene.allowSceneActivation = false;
        sceneAsync = scene;

        while (scene.progress < 0.9f)
        {
            Debug.Log("Loading: " + scene.progress);
            yield return null;
        }
        EnableSceneIndex(levelIndex, bringPlayer);
    }

    void EnableSceneIndex(int sceneIndex, bool bringPlayer)
    {
        Debug.Log("Done loading scene.");
        sceneAsync.allowSceneActivation = true;
        Scene sceneToLoad = SceneManager.GetSceneByBuildIndex(sceneIndex);

        if (sceneToLoad.IsValid())
        {
            Debug.Log("Scene is valid");
            if (bringPlayer)
            {
                if (PlayerObject && PlayerUIObject)
                {
                    SceneManager.MoveGameObjectToScene(PlayerObject, sceneToLoad);
                    SceneManager.MoveGameObjectToScene(PlayerUIObject, sceneToLoad);
                }
            }
            SceneManager.SetActiveScene(sceneToLoad);
        }
    }*/

    /*IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
        PauseMenu.GameIsPaused = false;
    }*/
}
