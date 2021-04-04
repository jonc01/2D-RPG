using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public PlayerMovement movement; //might not need
    public PlayerCombat combat;

    //Use this bool to check when an enemy is within the trigger collider
    public bool hitEnemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("from Trigger ENTER");
        //ApplyCollision(collision.gameObject); //REPLACE

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("from Trigger EXIT");
        //REPLACE if this doesn't get called by default
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("from Collider");
        ApplyCollision(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //hitEnemy = false;
    }

    public void ApplyCollision(GameObject collider, bool overrideDash = false)
    {
        if (movement.isDashing || overrideDash)
        {
            combat.OnSuccessfulBash();
            hitEnemy = true; //??
            movement.CancelDash();

            if(collider.GetComponent<Enemy>() != null)
            {
                collider.GetComponent<Enemy>().TakeDamage(5);
                collider.GetComponent<Enemy>().GetStunned();
            }

            if (collider.GetComponent<Enemy2>() != null)
            {
                collider.GetComponent<Enemy2>().TakeDamage(5); //attackDamage + additional damage from parameter
                collider.GetComponent<Enemy2>().GetStunned(2);
            }

            if(collider.GetComponent<StationaryEnemy>() != null)
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
