using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    //Use this bool to check when an enemy is within the trigger collider
    public bool hitEnemy;
    public CollisionCheck collisionCheck;
    public Collider2D solidCollider;
    public PlayerMovement movement;
    public PlayerCombat combat;

    void OnTriggerEnter2D(Collider2D collision)
    {
        hitEnemy = true; //? why is this here
                         //movement.CancelDash //should get cancelled if isDashing is false

        //might need NULL check
        //solidCollider.GetComponent<Collider2D>().isTrigger = true; //disable solid collider
        //bool placeholder = false;
        if (combat.IsShieldBashing)
        {
            //check for ShieldBash input, either in PlayerCombat after .2f charge up delay in ShieldBashStart()
            ApplyCollision(collision.gameObject, true);

            //! doesn't work, doesn't get re-set back to bool, doesn't prevent dash from happening.
            //TRY: use trigger being past collider to just GetComponent and Stun Enemy anyway, no need to disable dash
            // or stop player from moving, just continue dash, push enemy, as long as Enemy is stunned
        }
        else
        {
            //do nothing
        }
        //movement.CancelDash(); //need to prevent dashing completely if enemy is in collider
        //movement.DisableDash();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        hitEnemy = false;
        //solidCollider.GetComponent<Collider2D>().isTrigger = false; //solid collider
        if(movement.isDashing) //might need a delay after IsShieldBashing is set to false
            ApplyCollision(collision.gameObject, true);
    }

    public void ApplyCollision(GameObject collider, bool overrideDash = false)
    {
        if (movement.isDashing || overrideDash)
        {
            combat.OnSuccessfulBash();
            hitEnemy = true; //??
            movement.CancelDash();

            if (collider.GetComponent<Enemy>() != null)
            {
                collider.GetComponent<Enemy>().TakeDamage(5);
                collider.GetComponent<Enemy>().GetStunned();
            }

            if (collider.GetComponent<Enemy2>() != null)
            {
                collider.GetComponent<Enemy2>().TakeDamage(5); //attackDamage + additional damage from parameter
                collider.GetComponent<Enemy2>().GetStunned(2);
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
