using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    [SerializeField] float[] hpPhases; //health %s to spawn additionals at corresponding index
    [SerializeField] public int currentPhase; //starts at 0, for use with index
    [SerializeField] GameObject[] additionals; //holds Enemy adds to spawn at certain hp phases

    public void UpdateHealth(float currentHP, float maxHP) //TODO: currently only spawns one enemy at a time
    {
        if(currentPhase <= hpPhases.Length-1) //don't bother if no more phases
        {
            float percentHP = currentHP / maxHP;
            if (percentHP <= hpPhases[currentPhase]) //compare health % to damage phases
            {
                SpawnEnemies(currentPhase); //spawn enemies at current phase
                currentPhase++; //progress phase
            }
        }
    }

    private void SpawnEnemies(int phase)
    {
        for(int i=0; i <= currentPhase; i++)
        {
            if(additionals[i] != null)
                additionals[i].SetActive(true);
        }
    }

    public void BossDead()
    {
        foreach(GameObject enemy in additionals)
        {
            if(enemy != null)
            {
                enemy.GetComponent<BaseEnemyClass>().TakeDamage(999, 1, true);
            }
        }
    }
}
