using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPortal : MonoBehaviour
{
    public EnemyBossBandit boss;
    public GameObject textPrompt;
    public Interactable interactableScript;
    public GameObject portal;

    void Start()
    {
        textPrompt.SetActive(false);
        portal.SetActive(false);

        boss = GameObject.Find("BanditBoss").GetComponent<EnemyBossBandit>();
    }

    void Update()
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
    }

    public void ReplayLevel() //PLACEHOLDER: end of level should give option to return to town, or proceed to next level
    {
        if (boss && !boss.isAlive)
        {
            SceneManager.LoadScene("DemoLevel");
            textPrompt.SetActive(false);
        }
    }
}
