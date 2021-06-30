using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    //Text Popups]
    [Header("Start() references")]
    [SerializeField] private TextPopupsHandler TextPopupsHandler;
    [SerializeField] private TextPopupsHandler AttackIndicator;
    [SerializeField] Vector3 TPOffset = new Vector3(0, .4f, 0);
    [SerializeField] private HitEffectsHandler HitEffectsHandler;
    public DeathParticlesHandler DeathParticlesHandler;

    [Space]
    [Header("References")] //Player References
    [SerializeField]
    public LayerMask
        playerLayer,
        groundLayer;


    public Transform player;
    public PlayerCombat playerCombat;

    //public GameObject hitPrefabToRight;
    //public GameObject hitPrefabToLeft;
    public GameObject stunLParticlePrefab;
    public GameObject stunRParticlePrefab;

    public float maxHealth = 100;
    float currentHealth;
    public HealthBar healthBar;
    float maxHeal = 100;

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
    public float 
        enAttackDamage = 5f,
        enAttackSpeed = 1.0f, //lower value for lower delays between attacks
        enAttackAnimSpeed = .4f; //lower value for shorter animations
    [Range(0f, 1.0f)]
    public float 
        stunResist = 0f, //0f takes full stun duration, 1.0f complete stun resist
        allowStun = 0f,
        allowStunCD = 0.1f; //how often enemy can be stunned
    
    [Space] //knockback
    public float 
        kbThrust = 3.0f,
        kbDuration = 5.0f;


    [SerializeField]
    private bool 
        enCanAttack, 
        isAttacking,
        aggroStarted,
        enIsHurt,
        enStunned,
        enCanChase,
        knockbackHit;

    //Raycast checks
    [SerializeField]
    private Transform
        groundCheck,
        wallPlayerCheck,
        attackCheck;

    [SerializeField]
    private bool 
        playerToRight,
        playerDetectFront,
        playerDetectBack,
        playerInRange,
        groundDetect,
        wallDetect;

    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        playerCheckDistance, //Aggro range
        attackRange;



    // Stop Coroutines
    Coroutine IsAttackingCO;
    Coroutine IsPatrollingCO;
    Coroutine IsIdlingCO;
    bool isPatrolling;

    SpriteRenderer sr;
    [SerializeField]
    private Material mWhiteFlash; //material to flash to on hit
    private Material mDefault; //default material to switch back to

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mDefault = sr.material;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCombat = player.GetComponent<PlayerCombat>();

        TextPopupsHandler = GameObject.Find("ObjectPool(TextPopups)").GetComponent<TextPopupsHandler>();
        AttackIndicator = GameObject.Find("ObjectPool(Attack/Alert Indicators)").GetComponent<TextPopupsHandler>();
        HitEffectsHandler = GameObject.Find("ObjectPool(HitEffects)").GetComponent<HitEffectsHandler>();
        DeathParticlesHandler = GameObject.Find("ObjectPool(DeathParticles)").GetComponent<DeathParticlesHandler>();

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
        isAttacking = false;
        aggroStarted = false;
        enIsHurt = false;
        enStunned = false;
        knockbackHit = false;

        enAttackSpeed += Random.Range(-.1f, .1f);
        enController.moveSpeed += Random.Range(-.1f, .1f);


        //TODO: everything here vvv
        //bool startDir = (Random.Range(0, 2) == 0); //should return 0 or 1
        //supposedly this works 
        bool startDir = (Random.value > 0.5f);

        MoveRight(startDir);
        //MoveRight(true);
    }

    void Update()
    {
        IdleAnimCheck();
        MoveAnimCheck();
        //Move(); //replace this
        MoveCheck();
        AttackCheck();
    }

    void IdleAnimCheck()
    {
        if(rb != null)
        {
            if (rb.velocity.x == 0)
            {
                enAnimator.SetBool("move", false);
                enAnimator.SetBool("idle", true);
                if (aggroStarted)
                {
                    enAnimator.SetBool("inCombat", true);
                }
                else
                {
                    enAnimator.SetBool("inCombat", false);
                }
            }
            else
            {
                if (knockbackHit)
                {
                    enAnimator.SetBool("move", false);
                    enAnimator.SetBool("idle", true);
                }
                enAnimator.SetBool("idle", false);
            }
        }
    }

    void MoveCheck() //RaycastChecks
    {
        //TODO: move this to EnemyController once setup
        groundDetect = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        playerDetectFront = Physics2D.Raycast(wallPlayerCheck.position, transform.right, playerCheckDistance, playerLayer);
        playerDetectBack = Physics2D.Raycast(wallPlayerCheck.position, -transform.right, playerCheckDistance, playerLayer);
        wallDetect = Physics2D.Raycast(wallPlayerCheck.position, transform.right, wallCheckDistance, groundLayer);

        //TODO: shouldn't be needed, is set by default, changes when no ground, or wall, or player is detected
        if (groundDetect && !aggroStarted && !isAttacking)
        {
            if (isPatrolling)
            {
                if (enController.enFacingRight) //move in direction enemy is facing
                {
                    MoveRight(true);
                }
                else
                {
                    MoveRight(false);
                }
            }

            if (!isPatrolling) //when patrolling is ended, random value to idle or move again, and duration of selected action
            {
                bool switchDir = (Random.value > 0.5f);
                bool idleSwitch = (Random.value > 0.5f);
                float coDuration = (Random.Range(0.3f, 1f));

                if (idleSwitch)
                {
                    IsPatrollingCO = StartCoroutine(Patrolling(coDuration, switchDir));
                }
                else
                {
                    StartIdling(coDuration, switchDir);
                }
            }
        }

        if(!groundDetect || wallDetect) //if ledge is found or wall is hit or player is behind enemy
        {
            //turn around
            FlipDir();
        }

        if (!wallDetect && groundDetect) //prevent raycast hitting player through wall
        {
            if (playerDetectFront || playerDetectBack)
            {
                if (IsPatrollingCO != null)
                    StopCoroutine(IsPatrollingCO);

                if (IsIdlingCO != null)
                    StopCoroutine(IsIdlingCO);

                isPatrolling = false;
                aggroStarted = true;
                if (playerDetectFront)
                {
                    MoveRight(enController.enFacingRight);
                }
                else
                {
                    MoveRight(!enController.enFacingRight);
                }
            }
            else
            {
                //player leaves aggro range, patrol again
                aggroStarted = false;
            }
        }
    }

    void MoveRight(bool moveRight)
    {
        if (enController.enCanMove)
        {
            if (moveRight)
            {
                rb.velocity = new Vector2(enController.moveSpeed, 0);
                enController.enFacingRight = true;
                enController.Flip();
            }
            else
            {
                rb.velocity = new Vector2(-enController.moveSpeed, 0);
                enController.enFacingRight = false;
                enController.Flip();
            }
        }
    }

    void FlipDir() //flips current direction
    {
        bool dir = !enController.enFacingRight;
        MoveRight(dir);
    }

    IEnumerator Patrolling(float duration, bool switchDir)
    {
        isPatrolling = true;
        if (switchDir)
            FlipDir();

        yield return new WaitForSeconds(duration);
        isPatrolling = false;
    }

    void StartIdling(float duration, bool switchDir)
    {
        IsIdlingCO = StartCoroutine(Idling(duration, switchDir));
    }

    IEnumerator Idling(float duration, bool switchDir)
    {
        enController.enCanMove = false;
        if (switchDir)
            FlipDir();

        yield return new WaitForSeconds(duration);
        enController.enCanMove = true;
    }

    void AttackCheck()
    {
        if (enController.enFacingRight)
        {
            playerInRange = Physics2D.Raycast(attackCheck.position, transform.right, enAttackRange, playerLayer);
        }
        else
        {
            playerInRange = Physics2D.Raycast(attackCheck.position, -transform.right, enAttackRange, playerLayer);
        }

        if (playerInRange)
        {
            IsAttackingCO = StartCoroutine(IsAttacking());
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
    
    void Attack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, playerLayer);
        
        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage); //attackDamage + additional damage from parameter
        }
    }

    IEnumerator IsAttacking()
    {//TODO: combine redundant variables
        if (enCanAttack && !isAttacking && !enStunned)
        {
            isAttacking = true;
            enStunned = false; //attackStopped = false;
            enCanAttack = false;

            enAnimator.SetBool("isAttacking", true);
            enAnimator.SetTrigger("Attack");
            enAnimator.SetBool("move", false);

            //StopChase(.3f);

            //enController.EnDisableMove();
            enController.enCanMove = false;
            rb.velocity = new Vector2(0, 0);

            yield return new WaitForSeconds(enAttackAnimSpeed);
            Attack();

            yield return new WaitForSeconds(enAttackSpeed); //delay between attacks
            //enController.EnEnableMove();
            enAnimator.SetBool("isAttacking", false);

            enCanChase = true;
            enController.enCanMove = true;
            isAttacking = false;
            enCanAttack = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (enAttackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRange);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

        if (enController.enFacingRight)
        {
            Gizmos.DrawLine(wallPlayerCheck.position, new Vector2(wallPlayerCheck.position.x - wallCheckDistance, wallPlayerCheck.position.y));
            //Gizmos.DrawLine(wallPlayerCheck.position, new Vector2(wallPlayerCheck.position.x - playerCheckDistance, wallPlayerCheck.position.y));
        }
        else
        {
            Gizmos.DrawLine(wallPlayerCheck.position, new Vector2(wallPlayerCheck.position.x + wallCheckDistance, wallPlayerCheck.position.y));
            //Gizmos.DrawLine(wallPlayerCheck.position, new Vector2(wallPlayerCheck.position.x + playerCheckDistance, wallPlayerCheck.position.y));
        }
        
        //Attack range
        //Gizmos.DrawLine(attackCheck.position, new Vector2(attackCheck.position.x + attackRange, attackCheck.position.y));
    }

    public void TakeDamage(float damage, float damageMultiplier = 1.0f)
    {
        if (isAlive)
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

                //enIsHurt = true;
                //enAnimator.SetTrigger("Hurt");
                if(isAttacking == false)
                {
                    enAnimator.SetTrigger("enLightStun");
                }

                //GetStunned(1f);
                //enCanAttack = true;
                //enAnimator.SetBool("isAttacking", false);
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
            maxHeal -= healAmount; //enemy can only heal this amount total
            currentHealth += healAmount;
            healthBar.SetHealth(currentHealth); //update health bar
            if(currentHealth > maxHealth) //prevent overhealing
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
        knockbackHit = true;
        
        //float distToPlayer = transform.position.x - player.transform.position.x; //getting player direction to enemy //if 0 will use last direction
        Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset
        
        if (playerFacingRight) //knockback <- enemy -- player //knockback to direction player is facing
        {
            tempOffset.x += kbDuration;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
            transform.position = smoothPosition;
        }
        else //player -- enemy -> knockback
        {
            tempOffset.x -= kbDuration;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
            transform.position = smoothPosition;
        }
        IsIdlingCO = StartCoroutine(Idling(.3f, false));
        //StopChase(.7f);
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
        if (isAlive)
        {
            if(Time.time > allowStun && !enStunned) //cooldown timer starts when recovered from stun
            {
                if (IsAttackingCO != null)
                {
                    isAttacking = false;
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
    }

    IEnumerator LightStunEnemy(float lightStunDuration) //used in knockback
    {
        StartIdling(.6f, false);
        //enCanAttack = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        yield return new WaitForSeconds(lightStunDuration);
        //enCanAttack = true;
        enController.EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
        allowStun = Time.time + allowStunCD;
    }

    IEnumerator StunEnemy(float stunDuration)
    {
        if (!enStunned)
        {
            enStunned = true;
            StartIdling(.3f, false);
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
                tempPos.y -= .4f;
                AttackIndicator.ShowStun(tempPos, .8f);
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
        if(playerCombat != null)
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
}
