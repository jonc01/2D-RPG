using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public EnemyController enController;
    public Animator enAnimator;

    void Start()
    {
        enAnimator.SetBool("move", false);
    }

    void Update()
    {
        IdleAnimCheck();
        MoveAnimCheck();
    }

    void IdleAnimCheck()
    {
        if(enController.rb != null)
        {
            if (enController.rb.velocity.x == 0)
            {
                enAnimator.SetBool("move", false);
                enAnimator.SetBool("idle", true);
                if (enController.aggroStarted)
                {
                    enAnimator.SetBool("inCombat", true);
                }
                else
                {
                    enAnimator.SetBool("inCombat", false);
                }
            }
            else
            {
                if (enController.knockbackHit)
                {
                    enAnimator.SetBool("move", false);
                    enAnimator.SetBool("idle", true);
                }
                enAnimator.SetBool("idle", false);
            }
        }
    }

    void MoveAnimCheck()
    {
        if (enController.rb.velocity.x != 0)
        {
            enAnimator.SetBool("move", true);
        }
        else
        {
            enAnimator.SetBool("move", false);
        }
    }

    
}
