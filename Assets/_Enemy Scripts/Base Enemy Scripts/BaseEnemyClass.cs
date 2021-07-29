using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyClass : MonoBehaviour
{
    [Space] [Header("=== Required References for setup ===")]
    public EnemyAnimator enAnimator; //Animator manager script
    public HealthBar healthBar;
    public Transform enAttackPoint;
    [SerializeField] private Material mWhiteFlash;
    public LayerMask playerLayer;

    [Space] [Header("=== References get at Start()")]
    // REFERENCES GET AT START()
    //public HealthBar healthBar; //TODO: in Start() healthBarTransform = healthBar.GetComponent<Transform>();
    //private Transform healthBarTransform (in flip())
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    private Material mDefault;
    [SerializeField] private DeathParticlesHandler deathParticlesHandler;
    private Transform healthBarTransform;

    // PREFAB HANDLERS (Start())
    [SerializeField] private TextPopupsHandler TextPopupsHandler;
    [SerializeField] private TextPopupsHandler AttackIndicator;
    [SerializeField] private HitEffectsHandler HitEffectsHandler;
    [SerializeField] private DeathParticlesHandler DeathParticlesHandler;


    // HEALTH
    //float maxHealth
    //float currentHealth

    // ATTACK
    //float attackRange
    //float attackSpeed
    //float attackDamage

    [Space] [Header("=== Adjustable Variables ===")]
    [SerializeField] float defaultMoveSpeed = 2.5f;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] float
        enAttackDamage = 5f,
        enAttackSpeed = 1.0f,
        enAttackAnimSpeed = .4f,
        enAttackRadius = .3f;

    //TODO: maxHeal //rework, no longer needed
    public float XP = 10f;
    

    [Space]
    [Range(0f, 1.0f)]
    [SerializeField] float stunResist = 0f; //0f takes full stun duration, 1.0f complete stun resist
    [SerializeField] float allowStun = 0f;
    [SerializeField] float allowStunCD = 0.1f; //how often an enemy can be stunned

    [Space]
    [SerializeField] float kbThrust = 3.0f;
    [SerializeField] float kbDuration = 5.0f;


    [Space] [Header("=== Adjustable Transforms/Offsets ===")]
    [SerializeField] float hitEffectOffset = .5f;
    [SerializeField] float TPOffset = .4f;

    [Space] [Header("=== Optional Prefabs ===")]
    public GameObject stunLParticlePrefab;
    public GameObject stunRParticlePrefab;
    public TextPopupsHandler noReScaleTextHandler; //use for text to float above enemy, no re-scaling

    // === ANIMATION ===
    //float attackAnimSpeed

    [Space] //TODO: might not need to be public
    // === COROUTINES ===
    public Coroutine IsAttackingCO;

    [Space] [Header("=== Variables ===")]
    public bool isAlive;
    private float currentHealth;
    public bool enStunned;
    public bool enCanMove;
    public bool enCanAttack;
    public bool isAttacking;
    public bool enFacingRight;
    public bool enCanFlip;
    public bool enCanParry;

    private float moveSpeed;


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

        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);

        moveSpeed = defaultMoveSpeed;
        moveSpeed += Random.Range(-.2f, .1f);
        enCanFlip = true;
        enCanMove = true;

        isAttacking = false;
        enCanAttack = true;
        enStunned = false;
        enCanParry = false;

        isAlive = true;
        enStunned = false;
    }

    // Update is called once per frame
    void Update()
    {
        Flip(); //TODO: needs testing use here or only with MoveRight()
        CoroutineCheck();
    }

    #region MOVEMENT

    public void MoveRight(bool moveRight) 
    {
        if(enCanMove && !isAttacking)
        {
            if (moveRight)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                //enFacingRight = true;
                //Flip();
            }
            else
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                //enFacingRight = false;
                //Flip();
            }
        }
    }

    public void KnockbackRecover()
    {
        enCanMove = false;
        StartCoroutine(KnockbackRecovering());
    }

    IEnumerator KnockbackRecovering()
    {
        yield return new WaitForSeconds(.3f);
        enCanMove = true;
    }

    #endregion

    #region Attack functions
    void CoroutineCheck()
    {
        if (enStunned)
        {
            //Option 1
            StopAttackCO();


            //TODO: isIdlingCO???
        }
    }

    void Attack(float damageMult = 1f)
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRadius, playerLayer);

        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            if (player.GetComponent<PlayerCombat>() != null)
                player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage * damageMult); //attackDamage + additional damage from parameter
        }
    }

    IEnumerator Attacking()
    {
        if(enCanAttack && !isAttacking && !enStunned)
        {
            isAttacking = true;
            enCanAttack = false;
            EnDisableMove();

            //TODO: enAnimator.PlayerAttackAnim

            yield return new WaitForSeconds(enAttackAnimSpeed);
            Attack();
            yield return new WaitForSeconds(enAttackSpeed); //delay before starting attack again

            //TODO: enAnimator.SetBool("isAttacking", false);

            isAttacking = false;
            enCanAttack = true;
            EnEnableMove();
        }
    }

    public void StopAttackCO()
    {
        if (IsAttackingCO != null)
            StopCoroutine(IsAttackingCO);
    }

    private void OnDrawGizmosSelected()
    {
        if (enAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRadius);
    }

    #endregion

    #region Flip Update
    void Flip() //Change direction enemy is facing
    {
        if (enCanMove)
        {
            if(rb.velocity.x > 0) //moving right
            {
                enFacingRight = true;
            }
            else if(rb.velocity.x < 0)//moving left
            {
                enFacingRight = false;
            }
            //else was making !enFacingRight default when not moving, should only update when moving
        }

        if (enCanFlip)
        {
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


    #endregion

    #region Interaction References
    public void TakeDamage(float damage, float damageMultiplier = 1.0f)
    {
        if (isAlive)
        {
            float damageTaken = damage * damageMultiplier;
            currentHealth -= damageTaken;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            //state.ChangeState(EN_HURT); //TODO: State Controller should recognize this as the const stored
            //state.PlayAnim(HURT);

            if (TextPopupsHandler)
            {
                Vector3 tempPos = transform.position;
                tempPos.y += TPOffset;
                TextPopupsHandler.ShowDamage(damageTaken, tempPos);
            }

            if(enAnimator != null && damage > 0)
            {
                Vector3 particleLocation = transform.position;
                particleLocation.y += hitEffectOffset;
                HitEffectsHandler.ShowHitEffect(particleLocation);

                if (!isAttacking)
                    enAnimator.PlayAnimation(enAnimator.EN_HURT); //TODO: placeholder 

                sr.material = mWhiteFlash;
                Invoke("ResetMaterial", .1f);
            }

            if(currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void ResetMaterial()
    {
        sr.material = mDefault;
    }

    public void GetHealed(float healAmount)
    {
        if (isAlive && currentHealth < maxHealth)
        {
            currentHealth += healAmount;
            healthBar.SetHealth(currentHealth); //update health bar
            if (currentHealth > maxHealth) //prevent overhealing
            {
                currentHealth = maxHealth;
            }

            if (TextPopupsHandler)
            {
                Vector3 tempPos = transform.position;
                tempPos.y += TPOffset;
                TextPopupsHandler.ShowHeal(healAmount, tempPos);
            }
        }
    }

    public void GetKnockback(bool playerFacingRight, float kbThrust = 2f, float kbDuration = 5f) //defaults
    {
        //playerFacingRight - passed from PlayerCombat when damage is applied
        //kbThrust - velocity of lunge movement
        //kbDuration - how long to maintain thrust velocity (distance)

        //knockbackHit = true; //TODO: ??
        Vector3 tempOffset = gameObject.transform.position;

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
        KnockbackRecover(); //TODO: needs testing
    }

    public void GetStunned(float duration = 1f, bool fullStun = true)
    {
        if (isAlive)
        {
            if(Time.time > allowStun && !enStunned)
            {
                //TODO: knockbackHit = true;
                StopAttackCO();

                if (fullStun)
                {
                    float fullDuration = 1f;
                    fullDuration -= stunResist;
                    duration *= fullDuration;
                    StartCoroutine(StunEnemy(duration));
                }
            }
        }
    }

    IEnumerator StunEnemy(float stunDuration)
    {
        if (!enStunned)
        {
            enStunned = true;
            enCanAttack = false;
            EnDisableMove();

            if (stunLParticlePrefab != null && stunRParticlePrefab != null)
            {
                //Vector3 changeLocation = GetComponent<Transform>().position; //TODO: remove if not needed
                //Vector3 tempLocation = changeLocation;
                Vector3 tempLocation = transform.position; //TODO: needs testing
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
                tempPos.y -= TPOffset;
                AttackIndicator.ShowStun(tempPos, .8f);
            }

            yield return new WaitForSeconds(stunDuration);
            yield return new WaitForSeconds(.3f); //time for recover animation, or short delay before moving again
            enStunned = false;
            enCanAttack = true;
            isAttacking = false; //in case of attack interrupted from stun
            EnEnableMove();
            EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
            allowStun = Time.time + allowStunCD;
        }
    }
    #endregion

    public void Die()
    {
        isAlive = false;

        /*if(enAnimator != null)
        {
            enAnimator.ChangeState("EN_DEATH");
        }*/

        StopAllCoroutines();

        if(deathParticlesHandler != null)
        {
            //Vector3 changeLocation = GetComponent<Transform>().position; //TODO: if transform.position doesn't work
            //Vector3 tempLocation = changeLocation;
            Vector3 tempLocation = transform.position;
            tempLocation.y += hitEffectOffset;

            deathParticlesHandler.ShowHitEffect(tempLocation);
        }
        StartCoroutine(DeleteEnemyObject());
    }

    IEnumerator DeleteEnemyObject()
    {
        sr.enabled = false;
        GetComponentInChildren<Canvas>().enabled = false; //removes health bar
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    #region Toggle Bools: Flip, Move, Parry
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
        moveSpeed = 0; //TODO: not needed?
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void EnEnableMove()
    {
        enCanMove = true;
        moveSpeed = defaultMoveSpeed; //TODO: not needed?
    }

    public void EnEnableParry()
    {
        enCanParry = true;
    }

    public void EnDisableParry()
    {
        enCanParry = false;
    }

    #endregion
}
