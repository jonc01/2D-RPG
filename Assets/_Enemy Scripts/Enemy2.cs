﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy2 : MonoBehaviour
{
    //Text Popups
    public TextPopupsHandler TextPopupsHandler;
    [SerializeField] Vector3 TPOffset = new Vector3(0, -.5f, 0);
    public HitEffectsHandler HitEffectsHandler;

    public LayerMask playerLayers;
    public Transform player;
    public PlayerCombat playerCombat;
    //public GameObject hitPrefabToRight;
    //public GameObject hitPrefabToLeft;
    public GameObject hitParticlePrefab;
    public GameObject deathParticlePrefab;
    public GameObject stunLParticlePrefab;
    public GameObject stunRParticlePrefab;

    public float maxHealth = 300;
    float currentHealth;
    public HealthBar healthBar;

    //experience points based on enemy level
    //public int enLevel


    public int experiencePoints = 20;
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
    public float enAttackSpeed = 1.1f; //lower value for lower delays between attacks
    public float enAttackAnimSpeed = .4f; //lower value for shorter animations
    [Range(0f, 1.0f)]
    public float stunResist = .5f; //0f takes full stun duration, 1.0f complete stun resist
    public float allowStun = 0f;
    public float allowStunCD = 5f; //how often enemy can be stunned

    [Space] //knockback
    public float kbThrust = 3.0f;
    public float kbDuration = 2.0f;

    [SerializeField]
    bool enCanAttack = true, isAttacking; //for parry()
    [SerializeField]
    bool playerToRight, aggroStarted;
    bool enIsHurt;
    bool enStunned;

    [Header("Attack variables")]
    bool isShielded = true;


    SpriteRenderer sr;
    [SerializeField]
    private Material mWhiteFlash;
    private Material mDefault;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mDefault = sr.material;

        player = GameObject.Find("Player").transform;
        playerCombat = player.GetComponent<PlayerCombat>();

        //Stats
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        isAlive = true;
        //enController.enCanMove = true;
        enCanAttack = true;
        //AI aggro
        rb = GetComponent<Rigidbody2D>();
        enAnimator.SetBool("move", false);
        //enController.enAnimator.SetBool("isRunning", false);
        //enController.enFacingRight = false; //start facing left (towards player start)
        isAttacking = false;
        aggroStarted = false;
        enIsHurt = false;
        enStunned = false;

        enController.moveSpeed += Random.Range(-.1f, .1f);
    }

    void Update()
    {
        IdleAnimCheck();
        Move();
        //GetLungeDistance();
        //CheckLungeAttack();
        //GetLungeDirection();
    }

    void IdleAnimCheck()
    {
        if (rb != null)
        {
            if (rb.velocity.x == 0)
            {
                enAnimator.SetBool("move", false); //check to make sure enemy isn't playing run anim while 
            }
            else
            {
                enAnimator.SetBool("move", true);
            }
        }
    }

    void Move()
    {
        if (rb != null && enController != null && isAlive && playerCombat.isAlive && !enStunned) //check if object has rigidbody
        {
            //checking distance to player for aggro range
            float distToPlayer = Vector2.Distance(transform.position, player.position);

            //range <= 3
            if (distToPlayer <= aggroRange && enController.enCanMove) //how to start aggro
            {
                aggroStarted = true;
                //chase player
                StartChase();
                /*if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange)
                {
                    StopChase(); //stop moving, don't clip into player just to attack
                    //Attack //when in attack range
                    //StartCoroutine
                }*/
            }
            else if (aggroStarted && enController.enCanMove) //now that we have aggro
            {
                StartChase();
                //StopChase(); //if player outruns aggro range stop chase, currently keep chasing forever
            }
        }
        else if (!isAlive)
        {
            if (rb != null)
                rb.velocity = new Vector2(0, 0);
        }
        if (!playerCombat.isAlive)
            StopChase();
    }

    //AI aggro
    void StartChase()
    {
        enAnimator.SetBool("inCombat", false);
        //enController.enCanMove = true;
        //enAnimator.SetBool("move", true);

        if (enController.enCanMove)
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
                    //Attack();
                    //enAnimator.SetTrigger("Attack");
                    StartCoroutine(IsAttacking());
                    isAttacking = false;
                }
                //if(enCanMove)
                //enController.Flip();
            }
            else if (transform.position.x > player.position.x) //player is left
            {
                playerToRight = false;
                //player is to left, move left

                rb.velocity = new Vector2(-enController.moveSpeed, 0);


                enController.enFacingRight = false;
                //if (enCanMove)
                enController.Flip();
                if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange)
                {
                    //Attack();
                    StartCoroutine(IsAttacking());
                    isAttacking = false;
                }
                //if(enCanMove)
                // enController.Flip();
            }
        }

        if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange)
        {
            StopChase(); //stop moving, don't clip into player just to attack
                         //Attack //when in attack range
                         //StartCoroutine
        }
    }

    void StopChase()
    {
        rb.velocity = new Vector2(0, 0);
        enAnimator.SetBool("move", false);
        //enAnimator.SetBool("inCombat", true);
        enController.enCanMove = false;
    }

    IEnumerator IsAttacking()
    {//TODO: combine redundant variables
        if (enCanAttack && !isAttacking)
        {
            isAttacking = true;
            enStunned = false;
            enCanAttack = false;

            enAnimator.SetTrigger("Attack");

            enAnimator.SetBool("inCombat", true);
            enAnimator.SetBool("isAttacking", true);
            enAnimator.SetBool("move", false);

            enController.enCanMove = false;
            rb.velocity = new Vector2(0, 0);

            yield return new WaitForSeconds(enAttackAnimSpeed); //time when damage is dealt based on animation

            if (enStunned)
            {
                isAttacking = false; //prevent enemy from getting stuck on "isAttacking" since it is never set to false
                yield break;
            }

            rb.velocity = new Vector2(0, 0); //stop enemy from moving
            Attack();
            yield return new WaitForSeconds(enAttackSpeed); //delay between attacks
            enAnimator.SetBool("isAttacking", false);
        }
        enController.enCanMove = true;
        enCanAttack = true;
    }

    void Attack() //default, attack player when in melee range
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, playerLayers);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            //Debug.Log("Enemy Hit " + player.name);
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage, true); //attackDamage + additional damage from parameter
        }
    }

    float GetLungeDistance()
    {
        //raycast

        float distance = Mathf.Abs(transform.position.x - player.position.x);
        return distance;
    }

    bool GetLungeDirection()
    {
        if (playerToRight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckLungeAttack() //when to start LungeAttack
    {
        if (GetLungeDistance() <= 5f)
        {
            StartLungeAttack();
        }
    }

    void StartLungeAttack()
    {
        enCanAttack = false;
        StartCoroutine(LungeAttacking());
    }

    IEnumerator LungeAttacking()
    {
        if (enCanAttack && !isAttacking)
        {
            StopChase();
            Debug.Log("LungeAttacking"); //DELETEME
            enStunned = false;
            isAttacking = true;
            enAnimator.SetBool("isAttacking", true);
            enAnimator.SetBool("move", false);

            enAnimator.SetTrigger("StartChargeUpLoop"); 

            yield return new WaitForSeconds(1.0f); //Charge up time

            isShielded = false; //disable shield, allow stun
            enAnimator.SetTrigger("StartChargeUp");

            yield return new WaitForSeconds(1.06f); //or 1.1f
            enController.enCanMove = true;

            //dash to player location, attack
            if(GetLungeDirection() == true)
            {
                rb.AddForce(Vector3.right * 2);
            }
            else
            {
                rb.AddForce(Vector3.left * 2);
            }

            enAnimator.SetTrigger("StartChargedAttack");
            //enCanAttack = false;
            //enController.enCanMove = false;
            //rb.velocity = new Vector2(0, 0);

            yield return new WaitForSeconds(0.1f); //time when damage is dealt based on animation/lunge
            isShielded = true;

            rb.velocity = new Vector2(0, 0); //stop enemy from moving
            LungeAttack();
            yield return new WaitForSeconds(enAttackSpeed); //delay between attacks
            enAnimator.SetBool("isAttacking", false);
            enController.enCanMove = true;
            enCanAttack = true;
        }
        //enController.enCanMove = true;
        //enCanAttack = true;
    }

    void LungeAttack()
    {

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, playerLayers);


        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage*2, true); //attackDamage + additional damage from parameter
        }
        //no longer rooted
        enController.enCanMove = true;
        enCanAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (enAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRange);
    }

    public void TakeDamage(float damage, float damageMultiplier = 1.0f)
    {
        if (isAlive == true)
        {
            float damageTaken = damage * damageMultiplier;
            currentHealth -= damageTaken;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
   
            //show damage/heal numbers
            if (TextPopupsHandler)
            {
                Vector3 tempPos = transform.position;
                tempPos += TPOffset;
                TextPopupsHandler.ShowDamage(damageTaken, tempPos);
            }

            //hurt animation
            if (enAnimator != null && damage > 0) //took damage, not heal
            {
                //stopping coroutine
                //attackStopped = true;

                Vector3 particleLocation = transform.position;
                Vector3 particleOffset = particleLocation;
                particleOffset.y += .5f;
                HitEffectsHandler.ShowHitEffect(particleOffset);

                enIsHurt = true;
                enAnimator.SetTrigger("Hurt");
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
        
        Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset

        if (playerFacingRight) //enemy -> knockback
        {
            tempOffset.x += kbDuration;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
            transform.position = smoothPosition;
        }
        else //knockback <- enemy
        {
            tempOffset.x -= kbDuration;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
            transform.position = smoothPosition;
        }
    }

    public void EnIsHurtStart()
    {
        enIsHurt = true;
    }

    public void EnIsHurtEnd()
    {
        enIsHurt = false;
    }

    public void GetStunned(float duration) //allow player to call this function
    {
        if (isAlive)
        {
            if (Time.time > allowStun && !enStunned) //cooldown timer starts when recovered from stun
            {
                float fullDuration = 1f;
                fullDuration -= stunResist; //getting percentage of stun based on stunResist
                duration *= fullDuration;
                enAnimator.SetTrigger("en2Stunned");
                StartCoroutine(StunEnemy(duration));
            }
        }
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
                tempPos += TPOffset;
                TextPopupsHandler.ShowStun(tempPos);
            }

            yield return new WaitForSeconds(stunDuration);

            enAnimator.SetTrigger("en2StunRecover");
            yield return new WaitForSeconds(.5f); //time for recover animation
            enCanAttack = true;
            enController.enCanMove = true;
            enController.EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
            enStunned = false;
            allowStun = Time.time + allowStunCD;
        }
    }

    void Die()
    {
        isAlive = false;
        //Die animation
        if (enAnimator != null)
        {
            enAnimator.SetTrigger("Death");
        }

        //give player exp
        playerCombat.GiveXP(experiencePoints);

        StopAllCoroutines(); //stops attack coroutine if dead
        //hide hp bar


        //disable enemy object
        isAlive = false;

        if (deathParticlePrefab != null)
        {
            Vector3 changeLocation = GetComponent<Transform>().position;
            Vector3 tempLocation = changeLocation;
            tempLocation.y += .5f;
            Instantiate(deathParticlePrefab, tempLocation, Quaternion.identity);
        }

        //DeleteEnemyObject();
        StartCoroutine(DeleteEnemyObject());
    }

    IEnumerator DeleteEnemyObject()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponentInChildren<Canvas>().enabled = false;
        yield return new WaitForSeconds(.5f);
        Destroy(this.gameObject);
    }

    /*private void DeleteEnemyObject()
    {
        Destroy(this.gameObject);
    }*/
}
