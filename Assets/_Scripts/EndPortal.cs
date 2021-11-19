using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPortal : MonoBehaviour
{
    public GameObject portal;
    [SerializeField] string currentScene;
    [SerializeField] private string stage1, stage2;

    void Start() //Note: this is only called when EndPortal is enabled
    {
        //portal.SetActive(false);
        //Scene currentScene = SceneManager.GetActiveScene();
        //string currentSceneName = currentScene.name;

        currentScene = gameObject.scene.name;
        Debug.Log("Current scene: " + currentScene);
    }
    
    public void NextLevel()
    {
        bool randStage = (Random.value > 0.5f); //50/50 pick random stage from provided

        //Scene currentScene = SceneManager.GetActiveScene(); //TODO: make sure this doesn't pick up NeverUnload
        //string currentSceneName = currentScene.name;

        if (randStage)
        {
            //Debug.Log("stage1: " + stage1);
            AsyncLevelLoader.asyncLevelLoader.LoadScene(stage1, currentScene);
        }
        else
        {
            //Debug.Log("stage2: " + stage2);
            AsyncLevelLoader.asyncLevelLoader.LoadScene(stage2, currentScene);
        }
    }
}
