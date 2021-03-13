using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    //Use this bool to check when an enemy is within the trigger collider
    public bool hitEnemy;

    void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: don't need to compareTags, only checking if a solid collider is in trigger
        if (collision.CompareTag("Enemy"))
        {
            hitEnemy = true;
            //movement.CancelDash();
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
    }
}
