using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateController : MonoBehaviour
{

    private string currentState;

    public Animator enAnimator;
    //States
    public const string EN_IDLE = ""; //customizable through inspector?
    public const string EN_MOVE = "";
    public const string EN_HURT = "";
    public const string EN_STUNNED = "";
    public const string EN_ATTACKING = ""; //?

    //!!

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(string newState)
    {
        if (newState == currentState) return;

        enAnimator.Play(newState); //TODO: can't use this for Attacking trigger, should override other anim states
        currentState = newState;
    }
}
