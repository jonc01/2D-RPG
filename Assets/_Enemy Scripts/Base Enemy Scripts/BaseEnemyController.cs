using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    //
    [TextArea]
    public string Note = "Input for Enemy object";

    [Header("=== Required References for setup")]
    //public Animator enAnimator;
    public int placeholder;


    //!!



    // Start is called before the first frame update
    void Start()
    {
        //TODO: get RB


        //if(references == null)
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateCoroutines()
    {
        //TODO: might just update this in BaseEnemyClass
    }
    
    void MoveRight()
    {
        //EnemyClass //(BaseEnemyClass OR new class) //TODO: if using inherited script, can I still use "BaseEnemyClass class" or check if override bool
        
    }
}
