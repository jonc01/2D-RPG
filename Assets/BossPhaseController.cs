using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    [SerializeField] int totalDamagePhases;

    [SerializeField] float[] hpThreshold;

    float hpThreshold1 = .25f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateHealth(float currentHP, float maxHP)
    {
        //if currentHP/maxHP
        if((currentHP/maxHP) <= hpThreshold1)
        {

        }
    }

    private void SpawnEnemies()
    {
        //Set active selected enemy object
    }
}
