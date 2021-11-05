using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    [SerializeField] float[] hpPhases; //health %s to spawn additionals at corresponding index
    [SerializeField] public int currentPhase; //starts at 0, for use with index
    [SerializeField] GameObject[] additionals; //holds Enemy adds to spawn at certain hp phases

    public void UpdateHealth(float currentHP, float maxHP) //TODO: needs testing
    {
        float percentHP = currentHP / maxHP;
        if (percentHP <= hpPhases[currentPhase]) //compare health % to damage phases
        {
            SpawnEnemies(currentPhase); //spawn enemies at current phase
            Debug.Log("Spawning Enemies at Phase " + currentPhase);
            currentPhase++; //progress phase
            Debug.Log("currentPhase moving to: " + currentPhase);
        }
    }

    private void SpawnEnemies(int phase)
    {
        //Set active selected enemy object in that corresponding phase
        //foreach(enemy in additionals[phase]) ... 
            //GameObject.SetActive(true); //enabled = true
        //TODO: need null check since enemies are enabled, not instantiated
        //TODO: make sure both .5 damagePhases correctly enable enemies 1 and 2
    }
}
