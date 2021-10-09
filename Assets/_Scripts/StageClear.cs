using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClear : MonoBehaviour
{
    public bool levelCleared; //for reference from Item Selection

    [SerializeField] int enemyCount; //number of enemies in level

    public GameObject EndPortal; //opens portal to move player to next stage
    public GameObject EnemyList; //get all Enemies under this GameObject

    public PlayerCombat playerCombat; //TODO: make new script to manage player inventory

    // Start is called before the first frame update
    void Start()
    {
        levelCleared = false;
        EndPortal.SetActive(false);

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

        if(playerCombat == null)
        {
            playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
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
        }
        //if this breaks, update enemyCount with enemyCount = EnemyList.transform.childCount
    }

    public void UpdatePlayerInventory(float XP, int gold)
    {
        playerCombat.GiveXP(XP);
        //playerInventory.GiveGold(gold); //TODO: need playerInventory script setup
    }
}
