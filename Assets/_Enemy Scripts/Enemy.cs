﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    //Text Popups
    public TextPopupsHandler TextPopupsHandler;
    [SerializeField] Vector3 TPOffset = new Vector3(0, .3f, 0);
    public HitEffectsHandler HitEffectsHandler;
    public DeathParticlesHandler DeathParticlesHandler;

    public LayerMask playerLayers;
    public Transform player;
    public PlayerCombat playerCombat;
    //public GameObject hitPrefabToRight;
    //public GameObject hitPrefabToLeft;
    public GameObject hitParticlePrefab;
    public GameObject stunLParticlePrefab;
    public GameObject stunRParticlePrefab;

    public float maxHealth = 100;
    float currentHealth;
    public HealthBar healthBar;

    //experience points based on enemy level
    //public int enLevel

    public int experiencePoints = 10;
    public Animator enAnimator;
    public bool isAlive;

    [SerializeField]
    public Rigidbody2D rb;
    [SerializeField]
    float aggroRange = 3f; //when to start chasing player
                        //might extend to aggro to player before enemy enters screen
    [SerializeField]
    float enAttackRange = .5f; //when to start attacking player, stop enemy from clipping into player
    public Transform enAttackPoint;
    public EnemyController enController;
    [Space]
    public float enAttackDamage = 5f;
    public float enAttackSpeed = 1.0f; //lower value for lower delays between attacks
    public float enAttackAnimSpeed = .4f; //lower value for shorter animations
    [Range(0f, 1.0f)]
    public float stunResist = 0f; //0f takes full stun duration, 1.0f complete stun resist
    public float allowStun = 0f;
    public float allowStunCD = 1f; //how often enemy can be stunned
    
    [Space] //knockback
    public float kbThrust = 3.0f;
    public float kbDuration = 5.0f;


    [SerializeField]
    bool enCanAttack, isAttacking; //for parry()
    [SerializeField]
    bool playerToRight, aggroStarted;
    bool enIsHurt;
    bool enStunned;
    bool enCanChase;
    bool knockbackHit;

    // Stop Coroutines
    Coroutine IsAttackingCO;

    SpriteRenderer sr;
    [SerializeField]
    private Material mWhiteFlash; //material to flash to on hit
    private Material mDefault; //default material to switch back to

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mDefault = sr.material;

        player = GameObject.Find("Player").transform;
        playerCombat = player.GetComponent<PlayerCombat>();

        if (transform.position.x > player.transform.position.x)
        {
            playerToRight = false;
        }
        else
        {
            playerToRight = true;
        }

        //Stats
        currentHealth = maxHealth;
        if(healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        
        isAlive = true;
        enCanAttack = true;
        enCanChase = true;
        //AI aggro
        rb = GetComponent<Rigidbody2D>();
        enAnimator.SetBool("move", false);
        //enController.enAnimator.SetBool("isRunning", false);
        //enController.enFacingRight = false; //start facing left (towards player start)
        isAttacking = false;
        aggroStarted = false;
        enIsHurt = false;
        enStunned = false;
        knockbackHit = false;

        enAttackSpeed += Random.Range(-.4f, .4f);
        enController.moveSpeed += Random.Range(-.4f, .4f);
    }

    void Update()
    {
        IdleAnimCheck();
        MoveAnimCheck();
        Move();
    }

    void IdleAnimCheck()
    {
        if (rb.velocity.x == 0)
        {
            if (!enCanAttack)
            {
                enAnimator.SetBool("idle", true);
            }else if (knockbackHit)
            {
                enAnimator.SetBool("idle", true);
            }
        }
        else
        {
            enAnimator.SetBool("idle", false);
        }
    }

    void MoveAnimCheck()
    {
        if (rb.velocity.x != 0)
        {
            enAnimator.SetBool("move", true);
        }
        else
        {
            enAnimator.SetBool("move", false);
        }
    }

    void Move()
    {
        if (rb != null && enController != null && isAlive && playerCombat.isAlive && !enStunned) //check if object has rigidbody
        {
            //checking distance to player for aggro range
            float distToPlayer = Vector2.Distance(transform.position, player.position);

            //range <= 3
            if (enController.enCanMove)
            {
                if (distToPlayer <= aggroRange) //how to start aggro
                {
                    aggroStarted = true;
                    //chase player
                    if (enCanChase)
                        StartChase();
                }
                else if (aggroStarted && enController.enCanMove) //now that we have aggro
                {
                    if (enCanChase)
                        StartChase();
                }
            }
        }
        else if (!isAlive)
        {
            if (rb != null) //not needed if enemy is just deleted on death
                rb.velocity = new Vector2(0, 0);
        }
        if (!playerCombat.isAlive)
            StopChase();
    }

    //AI aggro
    void StartChase()
    {
        //enAnimator.SetBool("inCombat", false);

        if (enController.enCanMove && enCanChase)
        {
            if (transform.position.x < player.position.x) //player is right
            {
                playerToRight = true;
                //player is to right, move right
                
                rb.velocity = new Vector2(enController.moveSpeed, 0); //moves at moveSpeed
                
                //Facing right, flip sprite to face right
                enController.enFacingRight = true;
                enController.Flip();
                if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange)
                {
                    if(!isAttacking)
                        IsAttackingCO = StartCoroutine(IsAttacking());
                }
            }
            else if (transform.position.x > player.position.x) //player is left
            {
                playerToRight = false;
                //player is to left, move left
                
                rb.velocity = new Vector2(-enController.moveSpeed, 0);

                enController.enFacingRight = false;
                enController.Flip();
                if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange)
                {
                    if(!isAttacking)
                        IsAttackingCO = StartCoroutine(IsAttacking());
                }
            }
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }

        if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange)
        {
            StopChase(); //stop moving, stops enemy from walking into player
                         //Attack //when in attack range
                         //StartCoroutine
        }
    }

    void StopChase(float duration = 1f)
    {
        StartCoroutine(StoppingChase(duration));
    }

    IEnumerator StoppingChase(float duration)
    {
        if (knockbackHit)
            enAnimator.SetTrigger("Hurt");

        enCanChase = false;
        rb.velocity = new Vector2(0, 0);
        enController.EnDisableMove();
        //enAnimator.SetBool("move", false);
        yield return new WaitForSeconds(duration); //1f
        enCanChase = true;
        enController.EnEnableMove();
        knockbackHit = false;
    }

    void Attack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, playerLayers);
        
        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage); //attackDamage + additional damage from parameter
        }
    }

    IEnumerator IsAttacking()
    {//TODO: combine redundant variables
        enStunned = false; //attackStopped = false;

        enAnimator.SetBool("isAttacking", true);
        isAttacking = true;
        enAnimator.SetTrigger("Attack");
        //enAnimator.SetBool("inCombat", true);

        enCanAttack = false;
        StopChase();

        enController.EnDisableMove();
        yield return new WaitForSeconds(enAttackAnimSpeed);
        Attack();

        yield return new WaitForSeconds(enAttackSpeed); //delay between attacks
        enController.EnEnableMove();
        enAnimator.SetBool("isAttacking", false);
        isAttacking = false;
        enCanAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (enAttackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRange);
    }

    public void TakeDamage(float damage)
    {
        if (isAlive)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            //show damage/heal numbers
            if (TextPopupsHandler)
            {
                Vector3 tempPos = transform.position;
                tempPos += TPOffset;
                TextPopupsHandler.ShowDamage(damage, tempPos);
            }
            
            //hurt animation
            if (enAnimator != null && damage > 0) //took damage, not heal 
            {
                //stopping coroutine
                //attackStopped = true;
                Vector3 particleLocation = transform.position;
                Vector3 particleOffset = particleLocation;
                particleOffset.y += .5f;
                //Instantiate(hitParticlePrefab, particleOffset, Quaternion.identity);
                HitEffectsHandler.ShowHitEffect(particleOffset);

                enIsHurt = true;
                enAnimator.SetTrigger("Hurt");

                //GetStunned(1f);
                enCanAttack = true;
                enAnimator.SetBool("isAttacking", false);
                //attackStopped = false;

                sr.material = mWhiteFlash; //flashing enemy sprite
                Invoke("ResetMaterial", .1f);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void TakeHeal(float healAmount)
    {
        if (isAlive)
        {
            currentHealth += healAmount;

            if (TextPopupsHandler)
            {
                Vector3 tempPos = transform.position;
                tempPos += TPOffset;
                TextPopupsHandler.ShowHeal(healAmount, tempPos);
            }
        }
    }

    void ResetMaterial()
    {
        sr.material = mDefault;
    }

    public void GetKnockback(bool playerFacingRight, float kbThrust = 2f, float kbDuration = 5f) //defaults
    {
        //kbThrust - velocity of lunge movement
        //kbDuration - how long to maintain thrust velocity (distance)
        knockbackHit = true;
        
        //float distToPlayer = transform.position.x - player.transform.position.x; //getting player direction to enemy //if 0 will use last direction
        Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset

        if (playerFacingRight) //knockback <- enemy -> player
        {
            tempOffset.x += kbDuration;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
            transform.position = smoothPosition;
        }
        else //player <- enemy -> knockback
        {
            tempOffset.x -= kbDuration;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
            transform.position = smoothPosition;
        }
        enAnimator.SetTrigger("Hurt");
        StopChase(1f);
    }

    public void EnIsHurtStart()
    {
        enIsHurt = true;
    }

    public void EnIsHurtEnd()
    {
        enIsHurt = false;
    }

    public void GetStunned(float duration = 1f, bool fullStun = true) //two animations, full stun and light stun (stagger)
    {
        if(Time.time > allowStun && !enStunned) //cooldown timer starts when recovered from stun
        {
            if (IsAttackingCO != null)
            {
                StopCoroutine(IsAttackingCO);
            }

            if (fullStun)
            {
                float fullDuration = 1f;
                fullDuration -= stunResist; //getting percentage of stun based on stunResist
                duration *= fullDuration;
                enAnimator.SetTrigger("enStunned");
                StartCoroutine(StunEnemy(duration));
            }
            else
            {
                enAnimator.SetTrigger("enLightStun");
                StartCoroutine(LightStunEnemy(.3f));
            }
        }
    }

    IEnumerator LightStunEnemy(float lightStunDuration)
    {
        StopChase();
        enCanAttack = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        yield return new WaitForSeconds(lightStunDuration);
        enCanAttack = true;
        enController.EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
        allowStun = Time.time + allowStunCD;
    }

    IEnumerator StunEnemy(float stunDuration)
    {
        if (!enStunned)
        {
            enStunned = true;
            StopChase();
            enCanAttack = false;
            enController.enCanMove = false;
            if (stunLParticlePrefab != null && stunRParticlePrefab != null)
            {
                Vector3 changeLocation = GetComponent<Transform>().position;
                Vector3 tempLocation = changeLocation;
                tempLocation.y += .5f;

                if (enController.enFacingRight)
                {
                    Instantiate(stunLParticlePrefab, tempLocation, Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(stunRParticlePrefab, tempLocation, Quaternion.identity, transform);
                }
            }

            if (isAlive)
            {
                Vector3 tempPos = transform.position;
                //tempPos += TPOffset;
                tempPos.y += 0f;
                enController.noReScaleTextHandler.ShowText(tempPos, "Stun");
                //TextPopupsHandler.ShowText(tempPos, "Stun");
            }

            yield return new WaitForSeconds(stunDuration);

            enAnimator.SetTrigger("enStunRecover");
            yield return new WaitForSeconds(.5f); //time for recover animation
            isAttacking = false;
            enCanAttack = true;
            enController.EnEnableMove();
            enController.EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
            enStunned = false;
            allowStun = Time.time + allowStunCD;
        }
        
    }

    void Die()
    {
        isAlive = false;
        //Die animation
        if(enAnimator != null)
        {
            enAnimator.SetTrigger("Death");
        }

        //give player exp
        playerCombat.GiveXP(experiencePoints);

        StopAllCoroutines(); //stops attack coroutine if dead

        //playerCombat.HealPlayer(10);
        //hide hp bar


        //disable enemy object
        isAlive = false;

        if(DeathParticlesHandler != null)
        {
            Vector3 changeLocation = GetComponent<Transform>().position;
            Vector3 tempLocation = changeLocation;
            tempLocation.y += .5f;
            DeathParticlesHandler.ShowHitEffect(tempLocation);
        }

        //DeleteEnemyObject();
        StartCoroutine(DeleteEnemyObject());
    }

    IEnumerator DeleteEnemyObject() //only using if we want to use enemy death animation, currently just exploding object
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponentInChildren<Canvas>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    /*private void DeleteEnemyObject()
    {
        Destroy(this.gameObject);
    }*/
}
