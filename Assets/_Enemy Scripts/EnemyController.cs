using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("!If using separate script to override")]
    [SerializeField] private bool overrideAttack = false;
    [SerializeField] private bool overrideMove = false;

    [Space]
    [Header("=== Required reference setup ===")]
    public Animator enAnimator;
    [SerializeField] private Material mWhiteFlash; //material to flash to on hit
    [SerializeField] private LayerMask
        playerLayer,
        groundLayer;
    public Transform enAttackPoint;
    public HealthBar healthBar;

    [Space]
    [Header("=== Adjustable Variables ===")]
    [SerializeField] Vector3 TPOffset = new Vector3(0, .4f, 0);
    public int experiencePoints = 10;
    public float 
        maxHealth = 100,
        maxHeal = 100;
    [SerializeField] float 
        aggroRange = 3f, //when to start chasing player
        enAttackRange = .3f; //TODO: .5f //when to start attacking player, stop enemy from clipping into player
    public float moveSpeedDefault = 2f;
    float m_jumpForce = 7.5f; //not in-use
    public float
        enAttackDamage = 5f,
        enAttackSpeed = 1.0f, //lower value for lower delays between attacks
        enAttackAnimSpeed = .4f; //lower value for shorter animations

    [Space]
    [Header("=== Optional prefabs ===")]
    public GameObject stunLParticlePrefab;
    public GameObject stunRParticlePrefab;
    public TextPopupsHandler noReScaleTextHandler; //use for text to float above enemy, no re-scaling

    [Space]
    [Header("=== Prefab Handlers referenced at Start() ===")]
    [SerializeField] private TextPopupsHandler TextPopupsHandler;
    [SerializeField] private TextPopupsHandler AttackIndicator;
    [SerializeField] private HitEffectsHandler HitEffectsHandler;
    [SerializeField] private DeathParticlesHandler DeathParticlesHandler;
    private SpriteRenderer sr;
    public Rigidbody2D rb;
    private Material mDefault; //grabbed at start

    [Space]
    [Header("=== Variables ===")]
    public bool enFacingRight = false;
    public float moveSpeed = 2f; //current movespeed
    public bool isAlive;
    public float currentHealth;
    private Transform healthBarTransform; //to prevent flipping with Enemy parent object - Referenced at Start()

    [Space]
    [Range(0f, 1.0f)]
    public float
        stunResist = 0f, //0f takes full stun duration, 1.0f complete stun resist
        allowStun = 0f,
        allowStunCD = 0.1f; //how often enemy can be stunned

    [Space] //knockback
    public float
        kbThrust = 3.0f,
        kbDuration = 5.0f;

    [Header("=== Raycast Checks ===")]
    [SerializeField] //Raycast checks
    private Transform groundCheck;
    [SerializeField] private Transform
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

    [SerializeField] //Raycast variables
    private float
        groundCheckDistance,
        wallCheckDistance,
        playerCheckDistance, //Aggro range
        attackRange;

    [SerializeField]
    public bool //TODO: private after inheritance
        enCanAttack,
        isAttacking,
        aggroStarted,
        enIsHurt,
        enStunned,
        knockbackHit;

    [Space]
    public bool
        enCanFlip,
        enCanMove,
        enCanParry;

    public Coroutine IsAttackingCO;
    public Coroutine IsPatrollingCO;
    public Coroutine IsIdlingCO;
    public bool isPatrolling,
        isIdling;


    // Use this for initialization
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        mDefault = sr.material;

        healthBarTransform = healthBar.GetComponent<Transform>();

        TextPopupsHandler = GameObject.Find("ObjectPool(TextPopups)").GetComponent<TextPopupsHandler>();
        AttackIndicator = GameObject.Find("ObjectPool(Attack/Alert Indicators)").GetComponent<TextPopupsHandler>();
        HitEffectsHandler = GameObject.Find("ObjectPool(HitEffects)").GetComponent<HitEffectsHandler>();
        DeathParticlesHandler = GameObject.Find("ObjectPool(DeathParticles)").GetComponent<DeathParticlesHandler>();

        moveSpeed = moveSpeedDefault;
        enCanFlip = true;
        enCanMove = true;
        enCanParry = false;

        isAttacking = false;
        aggroStarted = false;
        enIsHurt = false;
        enStunned = false;
        knockbackHit = false;

        isAlive = true;
        enCanAttack = true;

        //Stats
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        moveSpeed += Random.Range(-.1f, .1f);

        bool startDir = (Random.value > 0.5f);
        MoveRight(startDir);
    }

    // Update is called once per frame
    public void Update()
    {
        //IdleAnimCheck();
        //MoveAnimCheck();
        
        Flip();
        
        if(!overrideMove)
            MoveCheck();

        if(!overrideAttack)
            AttackCheck();
    }

    protected virtual void IdleAnimCheck()
    {
        //not setup
    }
    protected virtual void MoveAnimCheck()
    {
        //not setup
    }

    #region Raycast - Move, Patrol, Idle
    public void MoveCheck() //RaycastChecks
    {
        //Vector2 lineRange = wallPlayerCheck.position + Vector3.right * playerCheckDistance;
        //Vector2 lineRangeBack = wallPlayerCheck.position + Vector3.right * -playerCheckDistance;
        //playerDetectFront = Physics2D.Linecast(wallPlayerCheck.position, lineRange, playerLayer);
        //playerDetectBack = Physics2D.Linecast(wallPlayerCheck.position, lineRangeBack, playerLayer);

        groundDetect = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        wallDetect = Physics2D.Raycast(wallPlayerCheck.position, transform.right, wallCheckDistance, groundLayer);
        playerDetectFront = Physics2D.Raycast(wallPlayerCheck.position, transform.right, playerCheckDistance, playerLayer);
        playerDetectBack = Physics2D.Raycast(wallPlayerCheck.position, -transform.right, playerCheckDistance, playerLayer);

        if (groundDetect && !aggroStarted && !isAttacking)
        {
            if (isPatrolling)
            {
                if (enFacingRight) //move in direction enemy is facing
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

        if (!groundDetect || wallDetect) //if ledge is found or wall is hit or player is behind enemy
        {
            //turn around
            if (rb.velocity.y == 0)
                FlipDir();
        }

        //this will hit player through wall and enemy will keep flipping until player leaves range
        //wallDetect logic is met, but is aggro'ing to player
        if (!wallDetect && groundDetect && !enStunned)
        {
            if (playerDetectFront || playerDetectBack)
            {
                if (IsPatrollingCO != null)
                    StopCoroutine(IsPatrollingCO);

                if (IsIdlingCO != null && !knockbackHit)
                {
                    StopCoroutine(IsIdlingCO);
                    if (isIdling)
                    {
                        enCanMove = true;
                        isIdling = false;
                    }
                }

                isPatrolling = false;
                aggroStarted = true;

                if (playerDetectFront)
                {
                    MoveRight(enFacingRight);
                }
                else
                {
                    MoveRight(!enFacingRight);
                }
            }
            else
            {
                //player leaves aggro range, patrol again
                aggroStarted = false;
            }
        }
    }

    public void MoveRight(bool moveRight)
    {
        if (enCanMove && !isAttacking)
        {
            if (moveRight)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                enFacingRight = true;
                Flip(); //TODO: might not be needed
            }
            else
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                enFacingRight = false;
                Flip(); //TODO: might not be needed
            }
        }
    }

    void FlipDir() //flips current direction
    {
        bool dir = !enFacingRight;
        MoveRight(dir);
    }

    void StartIdling(float duration, bool switchDir, bool knockbackHitB = false)
    {
        IsIdlingCO = StartCoroutine(Idling(duration, switchDir, knockbackHitB));
    }

    IEnumerator Idling(float duration, bool switchDir, bool knockbackHitB)
    {
        if (knockbackHitB)
            StartCoroutine(KnockbackRecover(duration));

        isIdling = true;
        enCanMove = false;
        if (switchDir)
            FlipDir();

        yield return new WaitForSeconds(duration);
        isIdling = false;
        enCanMove = true;
    }

    IEnumerator KnockbackRecover(float duration = .3f)
    {
        yield return new WaitForSeconds(duration);
        knockbackHit = false;
    }

    IEnumerator Patrolling(float duration, bool switchDir)
    {
        isPatrolling = true;
        if (switchDir)
            FlipDir();

        yield return new WaitForSeconds(duration);
        isPatrolling = false;
    }

    #endregion


    public void AttackCheck()
    {
        playerInRange = Physics2D.Raycast(attackCheck.position, -transform.right, enAttackRange, playerLayer);

        if (playerInRange)
        {
            IsAttackingCO = StartCoroutine(IsAttacking());
        }
    }

    void Attack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, playerLayer);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            if (player.GetComponent<PlayerCombat>() != null)
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
            enCanMove = false; //TODO: replace these with enController.EnDisableMove();
            rb.velocity = new Vector2(0, rb.velocity.y);

            yield return new WaitForSeconds(enAttackAnimSpeed);
            Attack();

            yield return new WaitForSeconds(enAttackSpeed); //delay between attacks
            //enController.EnEnableMove();
            enAnimator.SetBool("isAttacking", false);

            enCanMove = true;
            isAttacking = false; //TODO: add controller bool toggle function
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

        if (enFacingRight)
        {
            Gizmos.DrawLine(wallPlayerCheck.position, new Vector2(wallPlayerCheck.position.x - wallCheckDistance, wallPlayerCheck.position.y));
            Gizmos.DrawLine(wallPlayerCheck.position, new Vector2(wallPlayerCheck.position.x - playerCheckDistance, wallPlayerCheck.position.y));
        }
        else
        {
            Gizmos.DrawLine(wallPlayerCheck.position, new Vector2(wallPlayerCheck.position.x + wallCheckDistance, wallPlayerCheck.position.y));
            Gizmos.DrawLine(wallPlayerCheck.position, new Vector2(wallPlayerCheck.position.x + playerCheckDistance, wallPlayerCheck.position.y));
        }

        //Attack range
        //Gizmos.DrawLine(attackCheck.position, new Vector2(attackCheck.position.x + attackRange, attackCheck.position.y));
    }

    public virtual void TakeDamage(float damage, float damageMultiplier = 1.0f) //TODO: move to controller
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
                Vector3 particleLocation = transform.position;
                Vector3 particleOffset = particleLocation;
                particleOffset.y += .5f;
                HitEffectsHandler.ShowHitEffect(particleOffset);

                //enIsHurt = true;
                //enAnimator.SetTrigger("Hurt");
                if (!isAttacking)
                {
                    enAnimator.SetTrigger("enLightStun");
                }

                sr.material = mWhiteFlash; //flashing enemy sprite
                Invoke("ResetMaterial", .1f);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public virtual void TakeHeal(float healAmount) //TODO: move to controller
    {
        if (isAlive && maxHeal > 0 && currentHealth < maxHealth)
        {
            maxHeal -= healAmount; //enemy can only heal this amount total
            currentHealth += healAmount;
            healthBar.SetHealth(currentHealth); //update health bar
            if (currentHealth > maxHealth) //prevent overhealing
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

    public virtual void GetKnockback(bool playerFacingRight, float kbThrust = 2f, float kbDuration = 5f) //defaults
    {
        //playerFacingRight - passed from PlayerCombat when damage is applied
        //kbThrust - velocity of lunge movement
        //kbDuration - how long to maintain thrust velocity (distance)
        knockbackHit = true;

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
        StartIdling(.3f, false);
    }

    public virtual void GetStunned(float duration = 1f, bool fullStun = true) //TODO: move all of this to EnemyController
    {
        //two animations, full stun and light stun (stagger)
        if (isAlive)
        {
            if (Time.time > allowStun && !enStunned) //cooldown timer starts when recovered from stun
            {
                knockbackHit = true;
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
                }
            }
        }
    }

    IEnumerator StunEnemy(float stunDuration) //TODO: move to Controller
    {
        if (!enStunned)
        {
            enStunned = true;
            StartIdling(stunDuration + .5f, false, true);
            enCanAttack = false;
            enCanMove = false;
            if (stunLParticlePrefab != null && stunRParticlePrefab != null)
            {
                Vector3 changeLocation = GetComponent<Transform>().position;
                Vector3 tempLocation = changeLocation;
                tempLocation.y += .5f;

                if (enFacingRight)
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
            EnEnableMove();
            EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
            enStunned = false;
            allowStun = Time.time + allowStunCD;
        }
    }

    protected virtual void Die()
    {
        isAlive = false;
        //Die animation
        if (enAnimator != null)
        {
            enAnimator.SetTrigger("Death");
        }

        //give player exp
        //if(playerCombat != null)
        //    playerCombat.GiveXP(experiencePoints);

        StopAllCoroutines(); //stops attack coroutine if dead //TODO: make sure coroutines are saved in this script if started in Enemy.cs

        //playerCombat.HealPlayer(10);
        //hide hp bar


        //disable enemy object
        //enController.isAlive = false;

        if (DeathParticlesHandler != null)
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
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    #region Flip Update
    public void Flip()
    {
        // Switch the way the enemy is labelled as facing.
        //enFacingRight = !enFacingRight;

        // Multiply the enemy's x local scale by -1
        //.Vector3 theScale = transform.localScale;
        if (enCanMove) //automatically flipping based on velocity
        {
            if (rb.velocity.x > 0) //moving right
            {
                enFacingRight = true;
            }
            else //moving left
            {
                enFacingRight = false;
            }
        }

        if (enCanFlip) {
            if (enFacingRight)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                healthBarTransform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                healthBarTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public void ManualFlip(bool faceRight){ //manually flip
        if (faceRight)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            healthBarTransform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            healthBarTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    #endregion

    #region TakeDamage, GetKnockback, GetStunned
    
    void ResetMaterial()
    {
        sr.material = mDefault;
    }

    #endregion

    #region toggle bools: Flip, Move, Parry, Hurt
    public void EnDisableFlip()
    {
        enCanFlip = false;
    }

    public void EnEnableFlip()
    {
        enCanFlip = true;
    }

    public void EnDisableMove()
    {
        enCanMove = false;
        moveSpeed = 0;
    }

    public void EnEnableMove()
    {
        enCanMove = true;
        moveSpeed = moveSpeedDefault;
    }

    public void EnEnableParry()
    {
        enCanParry = true;
    }

    public void EnDisableParry()
    {
        enCanParry = false;
    }

    public void EnIsHurtStart()
    {
        enIsHurt = true;
    }

    public void EnIsHurtEnd()
    {
        enIsHurt = false;
    }
    #endregion
}
