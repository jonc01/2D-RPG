using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public PlayerMovement movement; //might not need

    public PlayerCombat combat;


    //Use this bool to check when an enemy is within the trigger collider
    public bool hitEnemy;


    void Update()
    {
        //Debug.Log("hitEnemy: " + hitEnemy);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO: need to check what is hit with a second trigger collider to get the tag of the enemy.
        //  setup second trigger just in front of solid collider 
            // can maybe just reuse second collider


        //OnCollison
            //Get Tag of enemy from Trigger collider    
                //stun/CheckParry of enemy
            //disable solid collider after x seconds maybe?
            
        //need to be able to enable/disable solidCollider from TriggerCollider1
        //and disable solidCollider from overlapping TriggerCollider2

        //TriggerCollider1: enables SolidCollider when enemy is not within hitbox (doesn't need to overlap, please fix)
                        // if enemy is overlapping with solid hitbox, just Stun, don't dash.
        //SolidCollider: is enabled if enemy not overlapping to begin with, disable after end of dash, or on hit (consider hit to be end of dash)
                //maybe only enable when also dashing and TriggerCollider1 is not set
        //TriggerCollider2: disable SolidCollider on hit or end of dash ^^ 


        ApplyCollision(collision.gameObject);
        /*
        if (collision.CompareTag("Boss"))
        {
            //CheckParry
        }*/
        //hitEnemy = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //hitEnemy = false;
    }

    void ApplyCollision(GameObject collider)
    {
        combat.OnSuccessfulBash();

        hitEnemy = true; //??
        movement.CancelDash();
        Debug.Log("Player dash stopped.");


        if(collider.GetComponent<Enemy>() != null)
        {
            collider.GetComponent<Enemy>().TakeDamage(5);
        }

        /*foreach (Collider2D enemy in hitEnemies) //loop through enemies hit
        {
            if (enemy.GetComponent<EnemyController>() != null)
            {
                //TODO: move common enemy scripting to EnemyController, instead of calling individual TakeDamage scripts
            }

            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(5); //attackDamage + additional damage from parameter
                //enemy.GetComponent<Enemy>().GetKnockback(controller.m_FacingRight);
                //enemy.GetComponent<Enemy>().GetStunned(stunStrength);
            }

            if (enemy.GetComponent<Enemy2>() != null)
            {
                enemy.GetComponent<Enemy2>().TakeDamage(attackDamageHeavy * damageMultiplier); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy2>().GetStunned(2);
            }
            if (enemy.GetComponent<EnemyBossBandit>() != null)
                enemy.GetComponent<EnemyBossBandit>().TakeDamage(5);
        }*/


        if (collider.CompareTag("Enemy"))
        {

        }
    }

    
    /*void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) {
            hitEnemy = true;
            movement.CancelDash();
            Debug.Log("Player dash stopped.");
        }

        if (collision.CompareTag("Boss"))
        {
            //CheckParry
        }
        //Debug.Log("hitEnemy true -> " + hitEnemy);
        //movement.CancelDash();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        hitEnemy = false;
        //Debug.Log("hitEnemy false -> " + hitEnemy);
    }*/
}
