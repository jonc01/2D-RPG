using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy2 : MonoBehaviour
{

    public EnemyController enController;

    [SerializeField] Vector3 TPOffset = new Vector3(0, 0.7f, 0);
    //public float enAttackDamage = 10f;
    //public float enAttackSpeed = 1.1f; //lower value for lower delays between attacks
    //public float enAttackAnimSpeed = .4f; //lower value for shorter animations

    TimeManager timeManager;
    private ScreenShakeListener screenshake;

    //Text Popups
    [SerializeField] private TextPopupsHandler AttackIndicator; //TODO: should be animation or no handler, manual instantiate


    public GameObject initialStunParticle;


    //experience points based on enemy level
    //public int enLevel


    //public int experiencePoints = 20;
    //public Animator enAnimator;
    //public bool isAlive;
    bool allowBreak;
    bool isBroken;
    bool isShielded = true;

    //bool enStunned;

    Coroutine IsComboAttackingCO;
    
    //TODO: this should work??
    //float aggroRange = 3f; //when to start chasing player
                           //might extend to aggro to player before enemy enters screen
    
    //float enAttackRange = .5f; //when to start attacking player, stop enemy from clipping into player
    //public Transform enAttackPoint

    //Coroutine IsAttackingCO;

    //[Header("Attack variables")]
    //RaycastHit2D aggroRaycast;

    //SpriteRenderer sr;
    //[SerializeField]
    //private Material mWhiteFlash;
    //private Material mDefault;
    

    void Start()
    {
        screenshake = GameObject.Find("ScreenShakeManager").GetComponent<ScreenShakeListener>();

        AttackIndicator = GameObject.Find("ObjectPool(Attack/Alert Indicators)").GetComponent<TextPopupsHandler>();

        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();

       

        //enController.enCanMove = true;
        //enCanAttack = true;
        //AI aggro
       // rb = GetComponent<Rigidbody2D>();
        enController.enAnimator.SetBool("move", false); //TODO: unnecessary
        enController.stunAnimatorVar = "en2StunRecover";
        //enController.enAnimator.SetBool("isRunning", false);
        //enController.enFacingRight = false; //start facing left (towards player start)
        //isAttacking = false;
        //aggroStarted = false;
        //enIsHurt = false;
        //enStunned = false;
        //canChase = true;
        allowBreak = false;
        isBroken = false;
    }

    void Update()
    {
        IdleAnimCheck();
        //WhereIsPlayer();
        MoveCheck();
        StartAttack();
        DeathCheck();
    }

    void IdleAnimCheck()
    {
        if (enController.rb != null)
        {
            //idle animation is default state
            if (enController.rb.velocity.x == 0)
            {
                enController.enAnimator.SetBool("move", false); //check to make sure enemy isn't playing run anim while not moving
                enController.enAnimator.SetBool("idle", true);
            }
            else
            {
                enController.enAnimator.SetBool("move", true);
                enController.enAnimator.SetBool("idle", false);
            }
        }
    }

    public void MoveCheck() //RaycastChecks
    {
        //groundDetect
        //wallDetect
        //playerDetectFront
        //playerDetectBack

        if (enController.groundDetect && !enController.aggroStarted && !enController.isAttacking && !enController.enStunned)
        {
            if (enController.isPatrolling)
            {
                if (enController.enFacingRight) //move in direction enemy is facing
                {
                    enController.MoveRight(true);
                }
                else
                {
                    enController.MoveRight(false);
                }
            }

            if (!enController.isPatrolling) //when patrolling is ended, random value to idle or move again, and duration of selected action
            {
                bool switchDir = (Random.value > 0.5f);
                bool idleSwitch = (Random.value > 0.5f);
                float coDuration = (Random.Range(0.3f, 1f));

                if (idleSwitch)
                {
                    enController.StartPatrolling(coDuration, switchDir);
                }
                else
                {
                    enController.StartIdling(coDuration, switchDir);
                }
            }
        }

        if (!enController.groundDetect || enController.wallDetect) //if ledge is found or wall is hit or player is behind enemy
        {
            //turn around
            if (enController.rb.velocity.y == 0)
                enController.FlipDir();
        }

        //this will hit player through wall and enemy will keep flipping until player leaves range
        //wallDetect logic is met, but is aggro'ing to player
        if (!enController.wallDetect && enController.groundDetect && !enController.enStunned)
        {
            if (enController.playerDetectFront || enController.playerDetectBack)
            {
                enController.StopPatrolling();

                if (enController.IsIdlingCO != null && !enController.knockbackHit)
                {
                    enController.StopIdling();
                    if (enController.isIdling)
                    {
                        enController.enCanMove = true;
                        enController.isIdling = false;
                    }
                }

                enController.isPatrolling = false;
                enController.aggroStarted = true;

                if (enController.playerDetectFront)
                {
                    enController.MoveRight(enController.enFacingRight);
                }
                else if(enController.playerDetectBack)
                {
                    enController.MoveRight(!enController.enFacingRight);
                }
            }
            else
            {
                //player leaves aggro range, patrol again
                enController.aggroStarted = false;
            }
        }
    }

    void ShowAttackIndicator() //TODO: fix reference
    {
        if (AttackIndicator != null)
        {
            Vector3 tempPos = transform.position;
            tempPos.y += 0.2f;

            AttackIndicator.ShowText(tempPos, "!");
        }
    }

    public void StartAttack(int atkVariation = 2) //TODO: delete 
    {
        if (enController.playerInRange)
        {
            switch (atkVariation)
            {
                case 1:
                    enController.IsAttackingCO = StartCoroutine(IsAttacking());
                    break;
                case 2:
                    enController.enCanMove = false;
                    IsComboAttackingCO = StartCoroutine(IsComboAttacking());
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator IsAttacking()
    {
        if (enController.enCanAttack && !enController.isAttacking && !enController.enStunned)
        {
            enController.isAttacking = true;
            enController.enStunned = false;
            enController.enCanAttack = false;

            enController.enAnimator.SetTrigger("Attack");

            enController.enAnimator.SetBool("isAttacking", true);
            enController.enAnimator.SetBool("move", false);

            enController.enCanMove = false;
            enController.rb.velocity = new Vector2(0, enController.rb.velocity.y);

            yield return new WaitForSeconds(enController.enAttackAnimSpeed); //time when damage is dealt based on animation

            enController.rb.velocity = new Vector2(0, enController.rb.velocity.y); //stop enemy from moving
            Attack2();
            yield return new WaitForSeconds(enController.enAttackSpeed); //delay between attacks
            enController.enAnimator.SetBool("isAttacking", false);
        
            enController.enCanMove = true;
            enController.enCanAttack = true;
            enController.isAttacking = false;
            //canChase = true;
        }
    }

    void Attack2() //default, attack player when in melee range
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enController.enAttackPoint.position, enController.enAttackRadius, enController.playerLayer);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            if (player.GetComponent<PlayerCombat>() != null)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(enController.enAttackDamage); //attackDamage + additional damage from parameter
                player.GetComponent<PlayerCombat>().GetKnockback(enController.playerToRight);
            }
        }
    }

    IEnumerator IsComboAttacking()
    {
        if(enController.enCanAttack && !enController.isAttacking && !enController.enStunned)
        {
            enController.isAttacking = true;
            enController.enStunned = false;
            enController.enCanAttack = false;
            ShowAttackIndicator();

            enController.enCanMove = false;
            enController.rb.velocity = new Vector2(0, 0);
            enController.enAnimator.SetTrigger("StartChargeUp");
            yield return new WaitForSeconds(1.1f);
            enController.rb.velocity = new Vector2(0, 0);

            //DisableShield(); //called in Animation event

            enController.enAnimator.SetTrigger("StartChargedAttack"); //start first attack
            yield return new WaitForSeconds(.2f);

            LungeOnAttack(); //allowing movement during lunge
            yield return new WaitForSeconds(.02f);
            enController.enCanMove = false;

            //EnableShield(); //called in animation

            Attack2();
            
            yield return new WaitForSeconds(.3f); //delay before starting next attack

            enController.enAnimator.SetTrigger("StartChargeUp"); // start second attack
            yield return new WaitForSeconds(.3f);

            enController.enAnimator.SetTrigger("StartChargedAttack");
            yield return new WaitForSeconds(.2f);

            LungeOnAttack();
            yield return new WaitForSeconds(.02f);
            enController.enCanMove = false;
            Attack2();

            yield return new WaitForSeconds(.4f);
            //enAnimator.SetTrigger("IdleStunnable");
            enController.enAnimator.SetBool("IdleStunnableB", true);

            yield return new WaitForSeconds(.3f);
            //StopChase();
            enController.enAnimator.SetBool("IdleStunnableB", false);

            yield return new WaitForSeconds(.5f);

            //canChase = true;
            enController.enCanMove = true;
            enController.isAttacking = false;
            enController.enCanAttack = true;
        }
    }

    void LungeOnAttack(float lungeThrust = 3f, float lungeDuration = 5f) //defaults //TODO: update this with raycast
    {
        //lungeThrust - velocity of lunge movement
        //lungeDuration - how long to maintain thrust velocity
        //float distToPlayer = transform.position.x - player.transform.position.x; //TODO: update with raycast

        Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset

        if (enController.enCanFlip)
        {
            enController.enCanMove = true; //allow move to face correct direction
            if(!enController.playerToRight)
            {
                if (enController.enFacingRight)
                {
                    tempOffset.x += lungeDuration; //lunge to left
                }
                else
                {
                    tempOffset.x -= lungeDuration; //lunge to right
                }
                Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, lungeThrust * Time.fixedDeltaTime);
                transform.position = smoothPosition;
            }
            else //!playerToRight, lunge towards player
            {
                if (enController.enFacingRight)
                {
                    tempOffset.x -= lungeDuration; //lunge to left
                }
                else
                {
                    tempOffset.x += lungeDuration; //lunge to right
                }
                Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, lungeThrust * Time.fixedDeltaTime);
                transform.position = smoothPosition;
            }
        }
    }

    void DisableShield() //for use in animation events
    {
        allowBreak = true;
    }
    
    void EnableShield()
    {
        allowBreak = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (enController.enAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enController.enAttackPoint.position, enController.enAttackRadius);
    }

    public void GetStunned(float duration) //allow player to call this function //TODO: this might be more robust than enController's
    {
        if (enController.isAlive)
        {
            //if (Time.time > allowStun && !enStunned) //cooldown timer starts when recovered from stun
            if(allowBreak && !isBroken)
            {
                if (IsComboAttackingCO != null)
                {
                    StopCoroutine(IsComboAttackingCO);
                    enController.isAttacking = false;
                }

                float fullDuration = 1f;
                fullDuration -= enController.stunResist; //getting percentage of stun based on stunResist
                duration *= fullDuration;
                enController.enAnimator.SetBool("IdleStunnableB", false);

                enController.isAttacking = false;

                StartCoroutine(StunEnemy(duration));
            }
        }
    }

    IEnumerator StunEnemy(float stunDuration)
    {
        if (!enController.enStunned)
        {
            isBroken = true;
            yield return new WaitForSeconds(.01f);
            enController.enAnimator.SetTrigger("en2Stunned");

            enController.enStunned = true;
            //StopChase(); //TODO: delete
            enController.enCanAttack = false;
            enController.enCanMove = false;

            if(initialStunParticle != null)
            {
                Vector3 tempLocation = transform.position;
                tempLocation.y += .5f;

                Instantiate(initialStunParticle, tempLocation, Quaternion.identity, transform);
            }

            if (enController.isAlive)
            {
                Vector3 tempPos = transform.position;
                tempPos += TPOffset;
                AttackIndicator.ShowBreak(tempPos);
            }

            yield return new WaitForSeconds(stunDuration);
            EnableShield(); //shield is back, no more increased damage taken
            isBroken = false;
            enController.enAnimator.SetTrigger("en2StunRecover");
            yield return new WaitForSeconds(1f); //time for recover animation

            enController.enCanMove = true;
            enController.EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
            enController.enStunned = false;

            //canChase = true;
            enController.enCanAttack = true;
        }
    }

    void DeathCheck()
    {
        //isAlive = false;
        //Die animation
        /*if (enAnimator != null)
        {
            enAnimator.SetTrigger("Death");
        }*/

        //give player exp
        //if(player != null)
        //    playerCombat.GiveXP(experiencePoints);

        //disable enemy object
        //isAlive = false;

        /*if (deathParticlePrefab != null)
        {
            Vector3 changeLocation = GetComponent<Transform>().position;
            Vector3 tempLocation = changeLocation;
            tempLocation.y += .5f;
            Instantiate(deathParticlePrefab, tempLocation, Quaternion.identity);
        }*/

        //DeleteEnemyObject();
        if (!enController.isAlive)
            StartCoroutine(DeleteEnemyObject());
    }

    IEnumerator DeleteEnemyObject()
    {
        enController.sr.enabled = false;
        GetComponentInChildren<Canvas>().enabled = false;
        yield return new WaitForSeconds(.5f);
        Destroy(this.gameObject);
    }
}
