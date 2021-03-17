using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    //Use this bool to check when an enemy is within the trigger collider
    public bool hitEnemy;



    void OnTriggerEnter2D(Collider2D collision)
    {
        hitEnemy = true;
        //disable collider

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        hitEnemy = false;
    }
}
