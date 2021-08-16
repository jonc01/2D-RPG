using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBanditClass : BaseEnemyClass
{
    [Header("=== Particle Reference ===")]
    [SerializeField] GameObject initialStunParticle;

    [SerializeField] 
    bool allowBreak,
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
                player.GetComponent<PlayerCombat>().StunPlayer(1); //attackDamage + additional damage from parameter
                //knockback
            }
        }
    }

    protected override IEnumerator Attacking() //attack has two hits
    {
        if(enCanAttack && !isAttacking && !enStunned)
        {
            isAttacking = true;
            //enStunned = false; //TODO: not sure why this is here, test
            enCanAttack = false;
            EnDisableMove();
            //ShowAttackIndicator();
            EnAnimator.PlayAttack(); //Play ChargeUp animation
            yield return new WaitForSeconds(1.1f);
            EnDisableFlip(); //TODO: needs testing

            EnAnimator.PlayAnim(1); //TODO: needs more setup
            yield return new WaitForSeconds(.2f);
            LungeOnAttack();
            yield return new WaitForSeconds(.02f);
            Attack();

            //Second Attack
            yield return new WaitForSeconds(.3f); //delay before starting next attack
            EnAnimator.PlayAttack(); //AttackChargeUp animation
            EnEnableFlip();
            yield return new WaitForSeconds(.3f);
            EnDisableFlip(); //TODO: needs testing
            EnAnimator.PlayAnim(1); //AttackEnd anim
            yield return new WaitForSeconds(.2f);
            LungeOnAttack();
            yield return new WaitForSeconds(.02f);
            Attack();

            yield return new WaitForSeconds(.4f);
            EnAnimator.PlayIdle2(); //StunnableIdle
            EnEnableFlip();

            yield return new WaitForSeconds(.3f);
            isAttacking = false;
            EnAnimator.PlayIdle(isAttacking, enStunned, isHurt);

            yield return new WaitForSeconds(enAttackSpeed);

            enCanAttack = true;
            EnEnableMove();
        }
    }

    void LungeOnAttack(float lungeThrust = 5f, float lungeDuration = 3f) //defaults //TODO: update this with raycast
    {
        //lungeThrust - velocity of lunge movement
        //lungeDuration - how long to maintain thrust velocity
        //float distToPlayer = transform.position.x - player.transform.position.x; //TODO: update with raycast

        //Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset
        //TODO: needs testing vvvvvvvvvvvvvvvvvvvvvvv otherwise use this^^
        Vector3 tempOffset = transform.position; //can implement knockup with y offset

        if (enFacingRight)
        {
            tempOffset.x += lungeThrust; //lunge to right //TODO: these variables might be mixed up
        }
        else
        {
            tempOffset.x -= lungeThrust; //lunge to left
        }

        Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, lungeDuration * Time.fixedDeltaTime);
        transform.position = smoothPosition;
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
            enStunned = true;
            isBroken = true;
            EnDisableMove();
            enCanAttack = false;
            yield return new WaitForSeconds(.01f);
            EnAnimator.PlayStunned();

            if(initialStunParticle != null)
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
        if (isBroken)
        {
            base.TakeDamage(damage *= 2f, damageMultiplier, true);
        }
        else
        {
            base.TakeDamage(damage, damageMultiplier);
        }
    }

    #region Animation Events/Bool Toggles
    void DisableShield()
    {
        allowBreak = true;
    }

    void EnableShield()
    {
        allowBreak = false;
    }
    #endregion
}
