using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    //
    [TextArea]
    public string Note = "Input for Enemy object";

    [Header("=== Required References for setup ===")]
    public int placeholder; //DELETEME
    public EnemyRaycast enRaycast;

    //!!



    // Start is called before the first frame update
    void Start()
    {


        //if(references == null)
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
     * 
     * BaseEnemyClass.MoveRight(true);
     * 
     * 
     */

    void UpdateCoroutines()
    {
        //TODO: might just update this in BaseEnemyClass
    }
    
    void MoveRight() //move to BaseEnemyClass
    {
        //EnemyClass //(BaseEnemyClass OR new class) //TODO: if using inherited script, can I still use "BaseEnemyClass class" or check if override bool
        
    }

    void Idle()
    {

    }

    void Patrol()
    {

    }

    void ChasePlayer() // ????
    {
        if (enRaycast.playerToRight)
        {
            //
        }
    }

}
