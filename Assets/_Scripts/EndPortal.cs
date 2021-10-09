using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPortal : MonoBehaviour
{
    //public EnemyBossBandit boss;
    public Interactable interactableScript;
    public GameObject portal;

    [SerializeField] private string stage1, stage2;

    void Start()
    {
        portal.SetActive(false);
    }

    /*void Update()
    {
        if (boss && !boss.isAlive)
        {
            portal.SetActive(true);
            if (interactableScript.isInRange)
            {
                textPrompt.SetActive(true);
            }
            else
            {
                textPrompt.SetActive(false);
            }
        }
    }*/
    
    public void NextLevel()
    {
        bool randStage = (Random.value > 0.5f); //50/50 pick random stage from provided

        if (randStage)
        {
            Debug.Log("stage1: " + stage1);
            //SceneManager.LoadScene(stage1);
        }
        else
        {
            Debug.Log("stage2: " + stage2);
            //SceneManager.LoadScene(stage2);
        }
    }

    /*public void ReplayLevel() //PLACEHOLDER: end of level should proceed to next level
    {
        if (boss && !boss.isAlive)
        {
            SceneManager.LoadScene("DemoLevel");
            textPrompt.SetActive(false);
        }
    }*/

}
