using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    // Called by ShieldBash() in PlayerCombat
    // Using when trigger collider is enabled, used to apply the ShieldBash collision

    //public CollisionCheck collisionCheck;
    public Collider2D solidCollider;
    public PlayerMovement movement;
    public PlayerCombat combat;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (combat.IsShieldBashing)
        {
            //check for ShieldBash input, either in PlayerCombat after .2f charge up delay in ShieldBashStart()
            ApplyCollision(collision.gameObject); 
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
    }

    public void ApplyCollision(GameObject collider, bool overrideDash = false) //overrideDash - if an enemy is already in the trigger, don't dash at all
    {
        if (movement.isDashing || overrideDash)
        {
            TimeManager.Instance.DoFreezeTime();
            combat.OnSuccessfulBash();
            movement.CancelDash();

            combat.Attack(); //apply damage/aoe to enemies near stunned

            if (collider.GetComponent<BaseEnemyClass>() != null)
            {
                collider.GetComponent<BaseEnemyClass>().GetStunned();
                //collider.GetComponent<EnemyController>().CheckParry();
            }


            ////////////////////////////////////////////////
            ///////////// vvv DELETE ALL vvv ///////////////

            //if (collider.GetComponent<EnemyBossBandit>() != null)
            //{
            //    collider.GetComponent<EnemyBossBandit>().TakeDamage(10);
            //    collider.GetComponent<EnemyBossBandit>().CheckParry();
            //}
        }
    }
}
