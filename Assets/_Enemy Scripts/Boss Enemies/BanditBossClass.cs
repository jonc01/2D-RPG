using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditBossClass : HeavyBanditClass
{
    [Header("=== Bandit Boss ===")]
    [SerializeField] EffectAnimatorManager effectAnimator;
    [SerializeField] Transform enAttackPoint2;
    public GameObject HealthBarCanvas;
    [SerializeField] Transform TPOffsetObj;
    [SerializeField] TimeManager timeManager;


    [SerializeField] Vector2 enAttackRange2 = new Vector2(1.85f, 0.4f);

    float breakDuration;
    int atkSequence;
    //


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //get Boss HP bar (1 per level) should be disabled by default, enables when player starts aggro
        //aggro should be permanent? or HP bar never goes away until boss is dead

        //BossHealthBarCanvas 
        if(HealthBarCanvas == null)
            HealthBarCanvas = GameObject.Find("BossHealthBar");

        if(HealthBarCanvas != null)
            healthBarTransform = healthBar.GetComponent<Transform>();

        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        atkSequence = 1;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        HPPhaseCheck();
    }
    
    public void ShowHealthBar(bool displayHP = true)
    {
        if (healthBar != null && HealthBarCanvas != null)
        {
            if (displayHP)
            {
                HealthBarCanvas.GetComponentInChildren<Canvas>().enabled = true;
                healthBar.SetHealth(currentHealth);
            }
            else if (!displayHP)
            {
                HealthBarCanvas.GetComponentInChildren<Canvas>().enabled = false;
            }
        }
    }

    void Attack(float damageMultiplier, bool knockback = false) //knockback has short stun
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRadius, playerLayer);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            if (player.GetComponent<PlayerCombat>() != null)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage * damageMultiplier); //attackDamage + additional damage from parameter
                float distToPlayer = transform.position.x - player.transform.position.x; //getting player direction to enemy //if 0 will use last direction
                if (knockback)
                {
                    if (distToPlayer > 0)
                    {
                        player.GetComponent<PlayerCombat>().GetKnockback(false); //knockback to left
                    }
                    else
                    {
                        player.GetComponent<PlayerCombat>().GetKnockback(true); //knockback to right
                    }
                }
            }
        }
    }

    void Attack2(float damageMultiplier, bool knockback = true) //knockback has short stun
    {
        Collider2D[] hitPlayer = Physics2D.OverlapBoxAll(enAttackPoint2.position, enAttackRange2, 180, playerLayer);

        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            if (player.GetComponent<PlayerCombat>() != null)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage * damageMultiplier); //attackDamage + additional damage from parameter
                float distToPlayer = transform.position.x - player.transform.position.x; //getting player direction to enemy //if 0 will use last direction
                if (knockback)
                {
                    if (distToPlayer > 0)
                    {
                        player.GetComponent<PlayerCombat>().GetKnockback(false); //knockback to left
                    }
                    else
                    {
                        player.GetComponent<PlayerCombat>().GetKnockback(true); //knockback to right
                    }
                }
            }
        }
    }

    public override void StartAttackCO()
    {
        if (!isAttacking) //check needed, or else IsAttackingCO doesn't get stopped by StopCoroutine
            IsAttackingCO = StartCoroutine(Attacking());
    }

    protected override IEnumerator Attacking()
    {
        if(enCanAttack && !isAttacking && !enStunned)
        {
            int phase;
            if(currentHealth >= (maxHealth / 2)) //phase 1: 50%+ hp
            {
                phase = 1;
                breakDuration = .5f;
            }
            else //phase 2
            {
                phase = 2;
                breakDuration = .75f;
            }

            enCanAttack = false;
            isAttacking = true;

            switch (atkSequence)
            {
                case 1:
                    atkSequence = 2; //next attack
                    EnAnimator.AnimTrigger("Attack1SlowStart"); //0.7f total anim time
                    EnDisableMove();
                    yield return new WaitForSeconds(0.3f); //0.4f to hit player running past
                    EnDisableFlip();
                    yield return new WaitForSeconds(0.4f);
                    Attack(1.5f);
                    yield return new WaitForSeconds(enAttackSpeed);

                    break;
                case 2:
                    if(phase == 1)
                    {
                        atkSequence = 3;
                    }
                    else
                    {
                        atkSequence = 4;
                    }

                    EnAnimator.AnimTrigger("Attack2SlowStart"); //0.62f total anim time
                    EnDisableMove();
                    yield return new WaitForSeconds(0.20f); //.54f
                    EnDisableFlip();
                    yield return new WaitForSeconds(0.34f);
                    LungeOnAttack(30f);
                    yield return new WaitForSeconds(0.08f); //Don't change: is the time it takes to lunge and hit attack
                    Attack2(1f);
                    yield return new WaitForSeconds(enAttackSpeed);

                    break;
                case 3: //Attack1 + Attack2
                    atkSequence = 1; //sequence loops back to 1
                    ShowAttackIndicator();

                    EnDisableMove();
                    EnAnimator.AnimTrigger("Attack1SlowStartCombo"); //0.6f anim
                    yield return new WaitForSeconds(0.3f);
                    EnDisableFlip();
                    yield return new WaitForSeconds(0.3f);
                    Attack(1.5f);
                    yield return new WaitForSeconds(.2f);
                    EnEnableFlip();
                    EnAnimator.AnimTrigger("Attack2SlowStart"); //0.52f
                    yield return new WaitForSeconds(0.24f); //.44
                    EnDisableFlip();
                    yield return new WaitForSeconds(0.2f);
                    LungeOnAttack(30f);
                    yield return new WaitForSeconds(0.08f); //.08 for lunge
                    //EnDisableMove();
                    Attack2(1f);
                    yield return new WaitForSeconds(0.4f);
                    EnAnimator.AnimBool("IdleLongStunnable", true);
                    EnAnimator.PlayAnim(1);
                    yield return new WaitForSeconds(.6f); //TODO: needs testing on duration
                    EnAnimator.AnimBool("IdleLongStunnable", false);

                    break;
                case 4:
                    atkSequence = 1;
                    breakDuration = .85f; //TODO: test between 1.5f and 1.0f (set at hp phase check)
                    ShowAttackIndicator2();

                    EnDisableMove(); 
                    EnAnimator.AnimTrigger("Attack1SlowStartCombo"); //.6 total
                    yield return new WaitForSeconds(.34f); //.54
                    EnDisableFlip();
                    yield return new WaitForSeconds(.2f); //TODO: needs testing
                    LungeOnAttack(20f);
                    yield return new WaitForSeconds(0.08f);
                    Attack(1f);
                    yield return new WaitForSeconds(.4f);
                    EnEnableFlip();

                    EnAnimator.AnimTrigger("Attack2SlowStartCombo"); 
                    yield return new WaitForSeconds(.34f); //.4 ?
                    EnDisableFlip();
                    LungeOnAttack(30f);
                    yield return new WaitForSeconds(0.08f);
                    Attack2(1f);
                    yield return new WaitForSeconds(.3f);
                    EnEnableFlip();

                    EnAnimator.AnimTrigger("Attack1SlowStartCombo2");
                    yield return new WaitForSeconds(.24f); //.3 total
                    EnDisableFlip();
                    LungeOnAttack(20f);
                    yield return new WaitForSeconds(0.08f);
                    Attack(1.5f);
                    yield return new WaitForSeconds(.2f);
                    EnEnableFlip();

                    EnAnimator.AnimTrigger("Attack2SlowStartCombo2");
                    yield return new WaitForSeconds(.14f); //.2 total
                    EnDisableFlip();
                    LungeOnAttack(30f);
                    yield return new WaitForSeconds(0.08f);
                    Attack2(1.5f);
                    yield return new WaitForSeconds(.2f);
                    EnEnableFlip();

                    EnAnimator.AnimTrigger("Attack1SlowStartCombo3");
                    yield return new WaitForSeconds(.14f); //.2 total
                    EnDisableFlip();
                    LungeOnAttack(20f);
                    yield return new WaitForSeconds(0.08f);
                    Attack(2f);
                    yield return new WaitForSeconds(.2f);
                    EnEnableFlip();

                    EnAnimator.AnimTrigger("Attack2SlowStartCombo3");
                    yield return new WaitForSeconds(.14f); //.2 total
                    EnDisableFlip();
                    LungeOnAttack(30f);
                    yield return new WaitForSeconds(0.08f);
                    Attack2(2f);

                    yield return new WaitForSeconds(.4f); //short delay so the Attack2 anim doesn't get cut off
                    EnAnimator.AnimBool("IdleLongStunnable", true);
                    EnAnimator.PlayAnim(1);  //enAnimator.SetBool("IdleLongStunnable", true);
                    yield return new WaitForSeconds(.5f);
                    EnAnimator.AnimBool("IdleLongStunnable", false);
                    //moving from Stunnable idle anim IdleLong anim
                    EnAnimator.AnimBool("IdleLong", true);
                    EnAnimator.PlayAnim(0); 
                    yield return new WaitForSeconds(.5f);
                    EnAnimator.AnimBool("IdleLong", false);

                    yield return new WaitForSeconds(enAttackSpeed); //TODO: increase length on this, make sure stun is still encouraged
                    break;
                default:
                    yield return new WaitForSeconds(0.01f);
                    break;
            }
            EnEnableFlip();
            EnEnableMove();
            isAttacking = false;
            enCanAttack = true;
        }
    }

    void ShowAttackIndicator()
    {
        Vector3 tempOffset = TPOffsetObj.transform.position;
        tempOffset.y -= 0.5f;

        if(AttackIndicator != null)
        {
            AttackIndicator.ShowText(tempOffset, "!");
        }
    }

    void ShowAttackIndicator2()
    {
        Vector3 tempOffset = TPOffsetObj.transform.position;
        tempOffset.y -= 0.4f;

        if(AttackIndicator != null)
        {
            AttackIndicator.ShowText(tempOffset, "!!", 1.4f);
        }
    }

    void HPPhaseCheck() //TODO: animation doesn't loop when bool is set
    {
        if(currentHealth > 0 && currentHealth <= (maxHealth / 2))
        {
            //effectAnimator.Vortex(true);
            effectAnimator.Vortex();
        }
        else
        {
            effectAnimator.Vortex(false, false);
        }
    }

    public override void GetStunned(float duration = 1, bool fullStun = true)
    {
        //TODO: SLOWMO 
        //TODO: PARTICLES
        base.GetStunned(duration, fullStun);
    }

    protected override IEnumerator StunEnemy(float stunDuration)
    {
        //return base.StunEnemy(stunDuration);

        if (!enStunned && isAlive)
        {
            enStunned = true; //enStunned is referenced in Controller, even if HeavyBandit uses Break instead
            isBroken = true;
            EnDisableMove();
            enCanAttack = false;
            isAttacking = false;

            if (initialStunParticle != null)
            {
                Instantiate(initialStunParticle, transform.position, Quaternion.identity, transform);
            }

            yield return new WaitForSeconds(.1f); //short delay before starting slow motion

            if (timeManager != null) //
            {
                timeManager.CustomSlowMotion(.02f, .5f);
                //timeManager.DoSlowMotion();
            }

            Vector3 tempPos = transform.position; 
            tempPos.y += TPOffset;
            AttackIndicator.ShowBreak(tempPos);

            EnAnimator.PlayStunned(); 
            yield return new WaitForSeconds(.5f); //1.1f
            EnAnimator.PlayAnim(2);
            yield return new WaitForSeconds(breakDuration); //1.3f //recoverDuration, is based on current phase
            //EnAnimator.PlayIdle(isAttacking, enStunned, isHurt); //TODO: play StunRecover animation NOT IDLE

            EnableShield();
            isBroken = false;
            yield return new WaitForSeconds(.2f);
            ShowAttackIndicator(); 
            yield return new WaitForSeconds(.5f);
            enCanAttack = true;
            isAttacking = false; //in case of attack interrupted from stun //TODO: might not need, is called earlier ^
            enStunned = false;
            EnEnableMove();
            EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
        }
    }

    public override void TakeDamage(float damage, float damageMultiplier = 1, bool isCrit = false)
    {
        if (isAlive && isBroken)
        {
            //EnAnimator.PlayAnim(13); //TODO: need number //*just need this, rest is handled in HeavyBanditClass
            //EnAnimator.AnimTrigger("Hit");

                //*PlayHurt() doesn't work, left it null to prevent playing anim when hurt normally
            //HeavyBanditClass should instantiate particles
        }
        //base has isAlive check
        base.TakeDamage(damage, damageMultiplier, isCrit);
    }

    public override void Die() //TODO: test
    {
        //base.Die();
        StopAllCoroutines(); //stops attack coroutine if dead
        EnDisableMove();

        isAlive = false;
        enCanAttack = false;

        ShowHealthBar(false);
        
        //give player exp //TODO: removed, reimplement at some point

        if (DeathParticlesHandler != null)
        {
            Vector3 deathParticleLocation = transform.position;
            deathParticleLocation.y += hitEffectOffset;

            DeathParticlesHandler.ShowHitEffect(deathParticleLocation);
        }

        StartCoroutine(DeleteEnemyObject());
    }

    IEnumerator DeleteEnemyObject()
    {
        EnDisableFlip();
        yield return new WaitForSeconds(.2f);
        EnAnimator.PlayDeathAnim();
        yield return new WaitForSeconds(1.0f);

        if (DeathParticlesHandler != null)
        {
            Vector3 deathParticleLocation = transform.position;
            deathParticleLocation.y -= .4f;

            DeathParticlesHandler.ShowHitEffect(deathParticleLocation);
        }

        //Destroy(this.gameObject);
        gameObject.SetActive(false); //TODO: set to false so that EndPortal can reference if isAlive
    }

    private void OnDrawGizmosSelected()
    {
        if (enAttackPoint == null)
            return;

        //Attack1
        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRadius);

        if (enAttackPoint2 == null)
            return;

        //Attack2
        Gizmos.DrawWireCube(enAttackPoint2.position, new Vector3(enAttackRange2.x, enAttackRange2.y, 0f));

    }


    #region Flip()
    protected override void Flip() //Change direction enemy is facing
    {
        if (enCanMove && !isHurt) //prevent flipping if receiving knockback
        {
            if (rb.velocity.x > 0) //moving right
            {
                enFacingRight = true;
            }
            else if (rb.velocity.x < 0) //moving left
            {
                enFacingRight = false;
            }
            //else was making !enFacingRight default when not moving, should only update when moving
        }

        if (enCanFlip) //separate from velocity/enFacingRight check, because enemy would flip during knockback
        {
            if (enFacingRight)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                //healthBarTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                //healthBarTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public override void ManualFlip(bool faceRight) //used in Controller if isAttacking and needs to flip to player direction
    {
        if (enCanFlip)
        {
            enFacingRight = faceRight;
            if (faceRight)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                //healthBarTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                //healthBarTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
    #endregion
}
