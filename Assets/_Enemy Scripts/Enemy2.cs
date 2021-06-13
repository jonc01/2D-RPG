using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy2 : MonoBehaviour
{
    //Text Popups
    [SerializeField] private TextPopupsHandler TextPopupsHandler;
    [SerializeField] private TextPopupsHandler AttackIndicator;
    [SerializeField] private HitEffectsHandler HitEffectsHandler;
    [SerializeField] Vector3 TPOffset = new Vector3(0, 0.7f, 0);

    TimeManager timeManager;

    public LayerMask playerLayers;
    public Transform player;
    public PlayerCombat playerCombat;
    public GameObject hitParticlePrefab;
    public GameObject deathParticlePrefab;
    public GameObject stunLParticlePrefab;
    public GameObject stunRParticlePrefab;
    public GameObject initialStunParticle;

    private ScreenShakeListener screenshake;

    public float maxHealth = 200;
    float currentHealth;
    public HealthBar healthBar;
    float maxHeal = 100;

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
    public float enAttackDamage = 10f;
    public float enAttackSpeed = 1.1f; //lower value for lower delays between attacks
    public float enAttackAnimSpeed = .4f; //lower value for shorter animations
    [Range(0f, 1.0f)]
    public float stunResist = 0f; //0f takes full stun duration, 1.0f complete stun resist
    public float allowStun = 0f;
    public float allowStunCD = 1f; //how often enemy can be stunned
    bool allowBreak;
    bool isBroken;

    [Space] //knockback
    public float kbThrust = 3.0f;
    public float kbDuration = 2.0f;

    [SerializeField] bool enCanAttack, isAttacking; //for parry()
    [SerializeField]
    bool playerToRight, aggroStarted;
    bool enIsHurt;
    bool enStunned;
    bool canChase;

    Coroutine IsAttackingCO;

    [Header("Attack variables")]
    bool isShielded = true;
    RaycastHit2D aggroRaycast;

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
        screenshake = GameObject.Find("ScreenShakeManager").GetComponent<ScreenShakeListener>();

        TextPopupsHandler = GameObject.Find("ObjectPool(TextPopups)").GetComponent<TextPopupsHandler>();
        AttackIndicator = GameObject.Find("ObjectPool(Attack/Alert Indicators)").GetComponent<TextPopupsHandler>();
        HitEffectsHandler = GameObject.Find("ObjectPool(HitEffects)").GetComponent<HitEffectsHandler>();

        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();

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
        canChase = true;
        allowBreak = false;
        isBroken = false;

        enController.moveSpeed += Random.Range(-.1f, .1f);
        
    }

    void Update()
    {
        IdleAnimCheck();
        Move();
        WhereIsPlayer();
    }

    void IdleAnimCheck()
    {
        if (rb != null)
        {
            //idle animation is default state
            if (rb.velocity.x == 0)
            {
                enAnimator.SetBool("move", false); //check to make sure enemy isn't playing run anim while not moving
                enAnimator.SetBool("idle", true);
            }
            else
            {
                enAnimator.SetBool("move", true);
                enAnimator.SetBool("idle", false);
            }
        }
    }

    void WhereIsPlayer()
    {
        if (transform.position.x < player.position.x) //player is right
        {
            playerToRight = true;
        }
        else if (transform.position.x > player.position.x) //player is left
        {
            playerToRight = false;
        }
    }

    void Move()
    {
        if (rb != null && enController != null && isAlive && playerCombat.isAlive && !enStunned) //check if object has rigidbody
        {
            if (player != null)
            {
                if (Mathf.Abs(transform.position.x - player.position.x) <= aggroRange && enController.enCanMove)
                {
                    aggroStarted = true;
                    StartChase();
                }
                else if (aggroStarted && enController.enCanMove)
                {
                    StartChase();
                }
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
        if (enController.enCanMove)
        {
            if (transform.position.x < player.position.x) //player is right
            {
                //player is to right, move right
                rb.velocity = new Vector2(enController.moveSpeed, 0); //moves at moveSpeed

                //Facing right, flip sprite to face right
                enController.enFacingRight = true;
                enController.Flip();
            }
            else if (transform.position.x > player.position.x) //player is left
            {
                //player is to left, move left
                rb.velocity = new Vector2(-enController.moveSpeed, 0);

                enController.enFacingRight = false;
                enController.Flip();
            }
            
            if(Mathf.Abs(transform.position.x - player.position.x) <= (enAttackRange+.5f)) //long attack range
            {
                StartAttack(2);
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
        //canChase = false;
        rb.velocity = new Vector2(0, 0);
        enAnimator.SetBool("move", false);
        //enAnimator.SetBool("inCombat", true);
        enController.enCanMove = false;
    }

    void ShowAttackIndicator()
    {
        if (AttackIndicator != null)
        {
            Vector3 tempPos = transform.position;
            tempPos.y += 0.2f;

            AttackIndicator.ShowText(tempPos, "!");
        }
    }

    void StartAttack(int atkVariation)
    {
        switch (atkVariation)
        {
            case 1:
                IsAttackingCO = StartCoroutine(IsAttacking());
                break;
            case 2:
                enController.enCanMove = false;
                IsAttackingCO = StartCoroutine(IsComboAttacking());
                break;
            default:
                break;
        }
    }

    IEnumerator IsAttacking()
    {
        if (enCanAttack && !isAttacking && !enStunned)
        {
            isAttacking = true;
            enStunned = false;
            enCanAttack = false;

            enAnimator.SetTrigger("Attack");

            enAnimator.SetBool("isAttacking", true);
            enAnimator.SetBool("move", false);

            enController.enCanMove = false;
            rb.velocity = new Vector2(0, 0);

            yield return new WaitForSeconds(enAttackAnimSpeed); //time when damage is dealt based on animation

            rb.velocity = new Vector2(0, 0); //stop enemy from moving
            Attack();
            yield return new WaitForSeconds(enAttackSpeed); //delay between attacks
            enAnimator.SetBool("isAttacking", false);
        
            enController.enCanMove = true;
            enCanAttack = true;
            isAttacking = false;
            //canChase = true;
        }
    }

    void Attack(float damageMult = 1f) //default, attack player when in melee range
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, playerLayers);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            //Debug.Log("Enemy Hit " + player.name);
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage * damageMult); //attackDamage + additional damage from parameter
        }
    }

    void Attack2() //default, attack player when in melee range
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, playerLayers);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage); //attackDamage + additional damage from parameter
            player.GetComponent<PlayerCombat>().GetKnockback(playerToRight);
        }
    }

    IEnumerator IsComboAttacking()
    {
        if(enCanAttack && !isAttacking && !enStunned)
        {
            isAttacking = true;
            enStunned = false;
            enCanAttack = false;
            ShowAttackIndicator();

            enController.enCanMove = false;
            rb.velocity = new Vector2(0, 0);
            enAnimator.SetTrigger("StartChargeUp");
            yield return new WaitForSeconds(1.1f);
            rb.velocity = new Vector2(0, 0);

            //DisableShield(); //called in Animation event

            enAnimator.SetTrigger("StartChargedAttack"); //start first attack
            yield return new WaitForSeconds(.2f);

            LungeOnAttack(); //allowing movement during lunge
            yield return new WaitForSeconds(.02f);
            enController.enCanMove = false;

            //EnableShield(); //called in animation

            Attack2();
            
            yield return new WaitForSeconds(.3f); //delay before starting next attack

            enAnimator.SetTrigger("StartChargeUp"); // start second attack
            yield return new WaitForSeconds(.3f);

            enAnimator.SetTrigger("StartChargedAttack");
            yield return new WaitForSeconds(.2f);

            LungeOnAttack();
            yield return new WaitForSeconds(.02f);
            enController.enCanMove = false;
            Attack(1.5f);

            yield return new WaitForSeconds(.4f);
            //enAnimator.SetTrigger("IdleStunnable");
            enAnimator.SetBool("IdleStunnableB", true);

            yield return new WaitForSeconds(.3f);
            //StopChase();
            enAnimator.SetBool("IdleStunnableB", false);

            yield return new WaitForSeconds(.5f);

            //canChase = true;
            enController.enCanMove = true;
            isAttacking = false;
            enCanAttack = true;
        }
    }

    void LungeOnAttack(float lungeThrust = 3f, float lungeDuration = 5f) //defaults
    {
        //lungeThrust - velocity of lunge movement
        //lungeDuration - how long to maintain thrust velocity
        float distToPlayer = transform.position.x - player.transform.position.x;

        Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset

        if (enController.enCanFlip) 
        {
            enController.enCanMove = true; //allow move to face correct direction
            if(distToPlayer < 0)
            {
                tempOffset.x += lungeDuration;
                Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, lungeThrust * Time.fixedDeltaTime);
                transform.position = smoothPosition;
            }
            else
            {
                tempOffset.x -= lungeDuration;
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
        if (enAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRange);
    }

    public void TakeDamage(float damage, float damageMultiplier = 1.0f)
    {
        if (isAlive == true)
        {
            float damageTaken = damage * damageMultiplier;
            if (isBroken)
            {
                damageTaken *= 2f;
            }
            currentHealth -= damageTaken;
            healthBar.SetHealth(currentHealth);

            if (currentHealth > maxHealth) //in case of overheal
                currentHealth = maxHealth;
   
            //show damage/heal numbers
            if (TextPopupsHandler)
            {
                Vector3 tempPos = transform.position;
                tempPos += TPOffset;
                Vector3 particleOffset = tempPos;
                particleOffset.y -= .5f;
                if (isBroken)
                {
                    TextPopupsHandler.ShowDamage(damageTaken, tempPos, true);
                    if(screenshake != null)
                    {
                        screenshake.Shake(1);
                    }

                    if (playerToRight)
                    {
                        Instantiate(stunLParticlePrefab, particleOffset, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(stunRParticlePrefab, particleOffset, Quaternion.identity);
                    }
                }
                else
                {
                    TextPopupsHandler.ShowDamage(damageTaken, tempPos);
                }
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
                //enCanAttack = true;
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
        if (isAlive && maxHeal > 0 && currentHealth < maxHealth)
        {
            maxHeal -= healAmount;
            currentHealth += healAmount;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

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
            //if (Time.time > allowStun && !enStunned) //cooldown timer starts when recovered from stun
            if(allowBreak && !isBroken)
            {
                float fullDuration = 1f;
                fullDuration -= stunResist; //getting percentage of stun based on stunResist
                duration *= fullDuration;
                enAnimator.SetBool("IdleStunnableB", false);
                
                isAttacking = false;

                /*if (timeManager != null)
                {
                    timeManager.CustomSlowMotion(.02f, 5f);
                }*/

                if (IsAttackingCO != null)
                    StopCoroutine(IsAttackingCO); //stopping attack coroutine when attacking

                StartCoroutine(StunEnemy(duration));
            }
        }
    }

    IEnumerator StunEnemy(float stunDuration)
    {
        if (!enStunned)
        {
            isBroken = true;
            yield return new WaitForSeconds(.01f);
            enAnimator.SetTrigger("en2Stunned");

            enStunned = true;
            StopChase();
            enCanAttack = false;
            enController.enCanMove = false;

            if(initialStunParticle != null)
            {
                Vector3 tempLocation = transform.position;
                tempLocation.y += .5f;

                Instantiate(initialStunParticle, tempLocation, Quaternion.identity, transform);
            }

            if (isAlive)
            {
                Vector3 tempPos = transform.position;
                tempPos += TPOffset;
                AttackIndicator.ShowBreak(tempPos);
            }

            yield return new WaitForSeconds(stunDuration);
            EnableShield(); //shield is back, no more increased damage taken
            isBroken = false;
            enAnimator.SetTrigger("en2StunRecover");
            yield return new WaitForSeconds(1f); //time for recover animation

            enController.enCanMove = true;
            enController.EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
            enStunned = false;

            canChase = true;
            enCanAttack = true;
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
        if(player != null)
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
}
