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
    public TimeManager timeManager;

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
            timeManager.DoFreezeTime();
            combat.OnSuccessfulBash();
            movement.CancelDash();

            if (collider.GetComponent<BaseEnemyClass>() != null)
            {
                collider.GetComponent<BaseEnemyClass>().TakeDamage(5);
                collider.GetComponent<BaseEnemyClass>().GetStunned();
                //collider.GetComponent<EnemyController>().CheckParry();
            }

            ////////////////////////////////////////////////////////////

            if (collider.GetComponent<EnemyController>() != null)
            {
                collider.GetComponent<EnemyController>().TakeDamage(5);
                collider.GetComponent<EnemyController>().GetStunned();
                //collider.GetComponent<EnemyController>().CheckParry();
            }

            if (collider.GetComponent<Enemy2>() != null) //TODO: delete all but EnemyController ref
            {
                //collider.GetComponent<Enemy2>().TakeDamage(5); //attackDamage + additional damage from parameter
                //collider.GetComponent<Enemy2>().GetStunned(2);
            }

            if (collider.GetComponent<StationaryEnemy>() != null)
            {
                collider.GetComponent<StationaryEnemy>().TakeDamage(10);
            }

            if (collider.GetComponent<EnemyBossBandit>() != null)
            {
                collider.GetComponent<EnemyBossBandit>().TakeDamage(10);
                collider.GetComponent<EnemyBossBandit>().CheckParry();
            }
        }
    }
}
