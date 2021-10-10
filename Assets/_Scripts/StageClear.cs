using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClear : MonoBehaviour
{
    public bool levelCleared; //for reference from Item Selection

    [SerializeField] int enemyCount; //number of enemies in level

    public GameObject EnemyList; //get all Enemies under this GameObject

    public GameObject ArrowIndicator;
    public GameObject EndPortal; //opens portal to move player to next stage

    public PlayerCombat playerCombat;
    public PlayerInventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        levelCleared = false;
        EndPortal.SetActive(false);

        if (ArrowIndicator == null)
            ArrowIndicator = GameObject.Find("ArrowIndicator");

        ArrowIndicator.SetActive(false);

        //Count enemies *make sure to put NPCs in separate GameObject
        if(EnemyList == null)
        {
            EnemyList = GameObject.Find("Enemies");
        }
        else
        {
            enemyCount = EnemyList.transform.childCount;
            //This only gets the number of children under "Enemies", doesn't count children's children
            //In this case, we don't want to count the raycast transforms, healthbars, etc
        }

        if (playerCombat == null)
        {
            playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
        }

        if(playerInventory == null)
        {
            playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerInventory>();
        }
    }

    public void UpdateEnemyCount(float XP, int gold)
    {
        enemyCount--; //called in Enemy Die()

        UpdatePlayerInventory(XP, gold);

        if (enemyCount <= 0)
        {
            levelCleared = true;
            EndPortal.SetActive(true);
            ArrowIndicator.SetActive(true);
        }
        //if this breaks, update enemyCount with enemyCount = EnemyList.transform.childCount
    }

    public void UpdatePlayerInventory(float XP, int gold)
    {
        if(playerCombat != null)
            playerCombat.GiveXP(XP);
        
        if(playerInventory != null)
            playerInventory.GiveGold(gold); //TODO: need playerInventory script setup
    }
}
