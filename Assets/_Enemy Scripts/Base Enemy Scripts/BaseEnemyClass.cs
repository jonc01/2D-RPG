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

    [Space]
    // COROUTINES //TODO: might not need to be public
    public Coroutine IsAttackingCO;
    public Coroutine IsPatrollingCO;
    public Coroutine IsIdlingCO;
    public bool
        isPatrolling,
        isIdling;


    // VARIABLES
    public bool isAlive;


    void Start()
    {





        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {

        CoroutineCheck();
    }

    void CoroutineCheck()
    {
        if (enStunned)
        {
            //Option 1
            if (IsAttackingCO != null)
                StopCoroutine(IsAttackingCO);

            //Option 2
            //StopAttackCO();




            if (IsPatrollingCO != null)
                StopCoroutine(IsPatrollingCO);

            //TODO: isIdlingCO???
        }
    }


    public void TakeDamage()
    {
        if (isAlive)
        {
            //state.ChangeState(EN_HURT); //TODO: State Controller should recognize this as the const stored

        }
    }

    public void GetStunned()
    {

    }

    public void Die()
    {
        isAlive = false;
    }
}
