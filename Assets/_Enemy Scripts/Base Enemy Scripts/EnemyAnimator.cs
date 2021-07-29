using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("=== Required References for setup ===")]
    public Animator enAnimator;

    [Space]
    private string currentState;


    // Animation States 
    [SerializeField]
    public string EN_IDLE = "Idle", //TODO: customizable through inspector?
        EN_MOVE = "Move",
        EN_HURT = "Hurt",
        EN_DEATH = "Death",
        EN_STUNNED = "Stunned",
        EN_ATTACKING = "Attacking"; //?

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(string newState) //TODO: might not use OR only use for looping animations
    {
        if (newState == currentState) return;

        enAnimator.Play(newState); //TODO: can't use this for Attacking trigger, should override other anim states
        currentState = newState;
    }

    public void PlayAnimation(string anim)
    {
        enAnimator.Play(anim); //or just grab from array of anims? anim[0] = idle...
    }

    public void PlayAttackAnim()
    {
        enAnimator.SetBool("isAttacking", true); //TODO: ALL PLACEHOLDER
        enAnimator.SetTrigger("Attack");
        enAnimator.SetBool("move", false);

    }

    void CheckDeath() //TODO: just manually call from BaseEnemyClass
    {
        
    }
}
