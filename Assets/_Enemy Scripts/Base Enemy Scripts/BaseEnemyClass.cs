using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyClass : MonoBehaviour
{
    // REFERENCES GET AT START()
    //public HealthBar healthBar; //TODO: in Start() healthBarTransform = healthBar.GetComponent<Transform>();
    //private Transform healthBarTransform (in flip())

    // HEALTH
    //float maxHealth
    //float currentHealth

    // ATTACK
    //float attackRange
    //float attackSpeed
    //float attackDamage

    // ANIMATION
    //float attackAnimSpeed


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void TakeDamage()
    {
        //state.ChangeState(EN_HURT); //TODO: State Controller should recognize this as the const stored
    }

    public void GetStunned()
    {

    }

    public void Die()
    {

    }
}
