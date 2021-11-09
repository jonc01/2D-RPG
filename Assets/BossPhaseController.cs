using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    [SerializeField] float[] hpPhases; //health %s to spawn additionals at corresponding index
    [SerializeField] public int currentPhase; //starts at 0, for use with index
    [SerializeField] GameObject[] additionals; //holds Enemy adds to spawn at certain hp phases

    public void UpdateHealth(float currentHP, float maxHP)
    {
        if(currentPhase <= hpPhases.Length-1) //stop if no more phases
        {
            float percentHP = currentHP / maxHP;
            if (percentHP <= hpPhases[currentPhase]) //compare health % to damage phases
            {
                SpawnEnemies(currentPhase); //spawn enemies at current phase
                currentPhase++; //progress phase
                UpdateHealth(currentHP, maxHP); //check again
            }
        }
    }

    private void SpawnEnemies(int phase)
    {
        for (int i=0; i <= phase; i++)
        {
            if(additionals[i] != null)
                additionals[i].SetActive(true);
        }
    }

    public void BossDead()
    {
        foreach(GameObject enemySpawner in additionals)
        {
            if(enemySpawner != null)
            {
                //if(enemySpawner.transform.childCount > 0)
                if(enemySpawner.GetComponentInChildren<BaseEnemyClass>() != null)
                {
                    enemySpawner.GetComponentInChildren<BaseEnemyClass>().TakeDamage(999, 1, true);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
