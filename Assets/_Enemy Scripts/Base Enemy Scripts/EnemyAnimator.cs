using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("=== Required References for setup ===")]
    public Animator enAnimController;

    // Animation Clips 
    [SerializeField]
    private string EN_IDLE = "Idle", //TODO: customizable through inspector?
        EN_MOVE = "Move",
        EN_ATTACKING = "Attacking", //?
        EN_HURT = "Hurt",
        EN_STUNNED = "Stunned",
        EN_DEATH = "Death",
        EN_IDLE_2 = "Combat Idle";


    [Space] [Header("=== (optional) New Animations ===")]
    [SerializeField] private string[] newClips = {"New Anim"};
    //^if using this, need null checks with PlayX functions

    public void PlayAnim(int anim)
    {
        enAnimController.Play(newClips[anim]); //or just grab from array of anims? anim[0] = idle...
    }

    ///////////////////////////////////////////////

    public void PlayIdle(bool isAttacking, bool enStunned, bool isHurt)
    {
        if (!isAttacking && !enStunned && !isHurt)
            enAnimController.Play(EN_IDLE);
    }

    public void PlayMove(bool isAttacking)
    {
        if(!isAttacking) //attacking check prevents Move being played when hit by shieldBash knockback
            enAnimController.Play(EN_MOVE);
    }

    public void PlayAttack()
    {
        enAnimController.Play(EN_ATTACKING);
    }

    public void PlayHurt()
    {
        if(EN_HURT != null) //null check for certain enemies without hurt animations until stunned
            enAnimController.Play(EN_HURT);
    }

    public void PlayStunned()
    {
        enAnimController.Play(EN_STUNNED);
    }

    public void PlayIdle2()
    {
        enAnimController.Play(EN_IDLE_2);
    }

    public void PlayDeathAnim()
    {
        if(EN_DEATH != null)
            enAnimController.Play(EN_DEATH);
    }
}
