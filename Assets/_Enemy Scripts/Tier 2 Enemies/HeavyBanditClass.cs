using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBanditClass : BaseEnemyClass
{
    [Header("=== HeavyBandit ===")]
    //[SerializeField] ScreenShakeListener screenShake;
    [SerializeField] protected GameObject initialStunParticle;
    [SerializeField] GameObject stunParticleL;
    [SerializeField] GameObject stunParticleR;
    [SerializeField] EnemyRaycast enRaycast;

    [SerializeField] 
    protected bool allowBreak,
        isBroken;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        allowBreak = false;
        isBroken = false;
    }

    // Update is called once per frame
    /*protected override void Update() //TODO: might not be needed unless adding to Update (only updating Idle/Move anims)
    {
        base.Update();
    }*/

    public override void Attack(float damageMult = 1)
    {
        //base.Attack(damageMult); 
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRadius, playerLayer);
        
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            if (player.GetComponent<PlayerCombat>() != null)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage * damageMult); //attackDamage + additional damage from parameter
                player.GetComponent<PlayerCombat>().StunPlayer(.5f); //attackDamage + additional damage from parameter
            }
        }
    }

    protected override IEnumerator Attacking() //attack has two hits
    {
        if(enCanAttack && !isAttacking && !enStunned)
        {
            isAttacking = true;
            enCanAttack = false;
            EnDisableMove();
            //ShowAttackIndicator();
        //First Attack Charge Up
            EnEnableFlip();
            EnAnimator.PlayAttack(); //Play ChargeUp animation
            yield return new WaitForSeconds(1.1f);
            EnAnimator.PlayAnim(1);

            EnDisableFlip();
            yield return new WaitForSeconds(.14f); //.2
            LungeOnAttack();
            yield return new WaitForSeconds(.08f); //.02
            Attack();

        //Second Attack Charge Up
            yield return new WaitForSeconds(.3f); //delay before starting next attack
            EnAnimator.PlayAttack(); //AttackChargeUp animation
            EnEnableFlip();
            //Second Attack Swing
            yield return new WaitForSeconds(.3f);
            EnAnimator.PlayAnim(1); //AttackEnd anim
            EnDisableFlip();
            //Lunge with collider
            yield return new WaitForSeconds(.14f); //.2
            LungeOnAttack();
            yield return new WaitForSeconds(.08f); //.02
            Attack();
            yield return new WaitForSeconds(.4f);
            EnAnimator.PlayIdle2(); //StunnableIdle
            EnEnableFlip();
        
        //End: Stunnable, delay before starting 
            yield return new WaitForSeconds(.3f);
            isAttacking = false;
            EnAnimator.PlayIdle(isAttacking, enStunned, isHurt);

            yield return new WaitForSeconds(enAttackSpeed);

            enCanAttack = true;
            EnEnableMove();
        }
    }

    protected void LungeOnAttack(float force = 15.0f)
    {
        float dir; 
        if (enFacingRight) //Lunge in direction enemy is facing
        {
            dir = 1;
        }
        else
        {
            dir = -1;
        }
        Vector3 targetPos = transform.position;
        RaycastHit2D hitPlayer = Physics2D.Raycast(enAttackPoint.position, transform.right * dir, enRaycast.attackRange, playerLayer);
        if(hitPlayer)
        {
            if (hitPlayer.transform.gameObject.GetComponent<PlayerCombat>() != null)
            {
                targetPos = hitPlayer.transform.gameObject.GetComponent<PlayerCombat>().GetPlayerPosition();
            }
        }
        else
        {
            targetPos.x += 3f * dir;
        }

        var lungeForce = force;
        /*if(Mathf.Abs(targetPos.x - transform.position.x) <= .3f) //if player is too close, lunge can be side-stepped
        {
            lungeForce = 1f; //short range lunge to hit the player within .3f
        }*/
        targetPos.y = transform.position.y; //prevents enemy from jump up to lunge at player
        rb.AddForce((targetPos - transform.position).normalized * lungeForce, ForceMode2D.Impulse);
    }

    public override void GetStunned(float duration = 1f, bool fullStun = true)
    { //increase damage taken when stunned
        if (isAlive)
        {
            if(allowBreak && !isBroken)// && !enStunned)
            {
                StopAttackCO();

                float fullDuration = 1f;
                fullDuration -= stunResist;
                duration *= fullDuration;

                StartCoroutine(StunEnemy(duration));
            }
        }
    }

    protected override IEnumerator StunEnemy(float stunDuration)
    {
        if (!enStunned)
        {
            enStunned = true; //enStunned is referenced in Controller, even if HeavyBandit uses Break instead
            isBroken = true;
            EnDisableMove();
            enCanAttack = false;
            //yield return new WaitForSeconds(.1f);
            EnAnimator.PlayStunned();

            /*if (timeManager != null)
            {
                timeManager.CustomSlowMotion(.02f, .5f);
            }*/

            if (initialStunParticle != null)
            {
                Vector3 tempLocation = transform.position;
                tempLocation.y += .5f;
                Instantiate(initialStunParticle, tempLocation, Quaternion.identity, transform);
            }

            if (isAlive)
            {
                Vector3 tempPos = transform.position;
                tempPos.y += TPOffset;
                AttackIndicator.ShowBreak(tempPos);
            }

            yield return new WaitForSeconds(stunDuration);
            EnableShield();
            isBroken = false;
            EnAnimator.PlayAnim(0); //Stun Recover

            yield return new WaitForSeconds(1f); //time for recover animation, or short delay before moving again
            enStunned = false;
            enCanAttack = true;
            isAttacking = false; //in case of attack interrupted from stun
            EnEnableMove();
            EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
        }
    }

    public override void TakeDamage(float damage, float damageMultiplier = 1, bool isCrit = false)
    {
        if (isBroken && isAlive)
        {
            base.TakeDamage(damage *= 2f, damageMultiplier, true);

            ScreenShakeListener.Instance.Shake();
            
            if(enRaycast && stunParticleL && stunParticleL)
            {
                Vector3 particleOffset = transform.position;
                particleOffset.y += hitEffectOffset;
                if (enRaycast.playerToRight)
                {
                    Instantiate(stunParticleL, particleOffset, Quaternion.identity);
                }
                else
                {
                    Instantiate(stunParticleR, particleOffset, Quaternion.identity);
                }
            }
        }
        else
        {
            base.TakeDamage(damage, damageMultiplier);
        }
    }

    #region Animation Events/Bool Toggles
    protected void DisableShield()
    {
        allowBreak = true;
    }

    protected void EnableShield()
    {
        allowBreak = false;
    }
    #endregion
}
