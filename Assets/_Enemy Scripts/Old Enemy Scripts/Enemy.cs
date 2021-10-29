using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("*Need Controller")]
    [SerializeField] private EnemyController enController;
    //

    void Start()
    {
        enController.enAnimator.SetBool("move", false);
        //enController.enAttackAnimSpeed = .....
        enController.stunAnimatorVar = "enStunRecover";
    }

    void Update()
    {
        IdleAnimCheck();
        MoveAnimCheck();
        StunnedAnimCheck();
    }

    void IdleAnimCheck()
    {
        if(enController.rb != null)
        {
            if (enController.rb.velocity.x == 0)
            {
                enController.enAnimator.SetBool("move", false);
                enController.enAnimator.SetBool("idle", true);
                if (enController.aggroStarted)
                {
                    enController.enAnimator.SetBool("inCombat", true);
                }
                else
                {
                    enController.enAnimator.SetBool("inCombat", false);
                }
            }
            else
            {
                if (enController.knockbackHit)
                {
                    enController.enAnimator.SetBool("move", false);
                    enController.enAnimator.SetBool("idle", true);
                }
                enController.enAnimator.SetBool("idle", false);
            }
        }
    }

    void MoveAnimCheck()
    {
        if(enController.rb != null)
        {
            if (enController.rb.velocity.x != 0)
            {
                enController.enAnimator.SetBool("move", true);
            }
            else
            {
                enController.enAnimator.SetBool("move", false);
            }
        }
    }

    void StunnedAnimCheck()
    {
        if(enController.enStunned)
            enController.enAnimator.SetTrigger("enStunned");
    }

    void StunRecoverAnim()
    {
        enController.enAnimator.SetTrigger("enStunRecover");
    }
}
