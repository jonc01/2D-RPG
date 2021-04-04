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

    bool TEST;

    private void Update()
    {
        TEST = combat.IsShieldBashing;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        hitEnemy = true; //? why is this here
                         //movement.CancelDash //should get cancelled if isDashing is false

        //might need NULL check
        //solidCollider.GetComponent<Collider2D>().isTrigger = true; //disable solid collider
        //bool placeholder = false;
        if (TEST)
        {
            //check for ShieldBash input, either in PlayerCombat after .2f charge up delay in ShieldBashStart()
            collisionCheck.ApplyCollision(collision.gameObject, true);

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

    }
}
