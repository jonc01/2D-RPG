using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyClass : MonoBehaviour
{
    [Space]
    [Header("=== Required References for setup ===")]
    public EnemyAnimator EnAnimator;
    public HealthBar healthBar;
    public Transform enAttackPoint;
    [SerializeField] private Material mWhiteFlash;
    public LayerMask playerLayer;

    [Space] [Header("=== References get at Start()")]
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    private Material mDefault;
    private Transform healthBarTransform;

    // PREFAB HANDLERS (Start())
    [SerializeField] protected TextPopupsHandler TextPopupsHandler;
    [SerializeField] protected TextPopupsHandler AttackIndicator;
    [SerializeField] protected HitEffectsHandler HitEffectsHandler;
    [SerializeField] protected DeathParticlesHandler DeathParticlesHandler;


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
    [SerializeField] protected float
        enAttackDamage = 5f,
        enAttackSpeed = 1.0f,
        enAttackAnimSpeed = .4f,
        enAttackRadius = .3f;

    public float XP = 10f;
    

    [Space]
    [Range(0f, 1.0f)]
    [SerializeField] protected float stunResist = 0f; //0f takes full stun duration, 1.0f complete stun resist
    [SerializeField] float allowStunCD = 0.1f; //how often an enemy can be stunned
    float allowStun = 0f;

    [Space]
    [SerializeField] float kbThrust = 1.0f;
    [SerializeField] float kbDuration = 5.0f;


    [Space] [Header("=== Adjustable Transforms/Offsets ===")]
    [SerializeField] protected float hitEffectOffset = .5f;
    [SerializeField] protected float TPOffset = .4f;

    [Space] [Header("=== Optional Prefabs/References ===")]
    public GameObject stunLParticlePrefab;
    public GameObject stunRParticlePrefab;
    public TextPopupsHandler noReScaleTextHandler; //use for text to float above enemy, no re-scaling
    [SerializeField] protected bool useScreenshake;
    protected ScreenShakeListener screenshake;

    // === ANIMATION ===
    //float attackAnimSpeed

    [Space] [Header("=== Variables ===")]
    public bool isAlive;
    private float currentHealth;
    public bool enStunned;
    public bool isHurt;
    public bool enCanMove;
    public bool enCanAttack;
    public bool isAttacking;
    public bool enFacingRight;
    public bool enCanFlip;
    public bool enCanParry;
    private float moveSpeed;
    Coroutine IsAttackingCO;


    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        mDefault = sr.material;
        healthBarTransform = healthBar.GetComponent<Transform>();        

        TextPopupsHandler = GameObject.Find("ObjectPool(TextPopups)").GetComponent<TextPopupsHandler>();
        AttackIndicator = GameObject.Find("ObjectPool(Attack/Alert Indicators)").GetComponent<TextPopupsHandler>();
        HitEffectsHandler = GameObject.Find("ObjectPool(HitEffects)").GetComponent<HitEffectsHandler>();
        DeathParticlesHandler = GameObject.Find("ObjectPool(DeathParticles)").GetComponent<DeathParticlesHandler>();

        if(useScreenshake)
            screenshake = GameObject.Find("ScreenShakeManager").GetComponent<ScreenShakeListener>();

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
        isHurt = false;
        enStunned = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CheckMove();
    }

    #region MOVEMENT

    public void MoveRight(bool moveRight)
    {
        if(enCanMove && !isAttacking)
        {
            if (moveRight)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                enFacingRight = true;
                Flip();
            }
            else
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                enFacingRight = false;
                Flip();
            }
        }
    }

    void CheckMove() //Idle/Move animations
    {
        if(EnAnimator != null)
        {
            if(rb.velocity.x == 0)
            {
                EnAnimator.PlayIdle(isAttacking, enStunned, isHurt);
            }
            else
            {
                EnAnimator.PlayMove(isAttacking);
            }
        }
    }

    public void KnockbackRecover()
    {
        EnDisableMove();
        StartCoroutine(KnockbackRecovering());
    }

    IEnumerator KnockbackRecovering()
    {
        yield return new WaitForSeconds(.2f);
        EnEnableMove();
    }

    #endregion

    #region Attack functions

    public virtual void Attack(float damageMult = 1f)
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRadius, playerLayer);

        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            if (player.GetComponent<PlayerCombat>() != null)
                player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage * damageMult); //attackDamage + additional damage from parameter
        }
    }

    protected virtual IEnumerator Attacking()
    {
        if(enCanAttack && !isAttacking && !enStunned)
        {
            isAttacking = true;
            enCanAttack = false;
            EnDisableMove();

            EnAnimator.PlayAttack();
            EnDisableFlip();
            yield return new WaitForSeconds(enAttackAnimSpeed);

            Attack();
            yield return new WaitForSeconds(enAttackSpeed); //delay before starting attack again

            EnEnableFlip();
            isAttacking = false;
            enCanAttack = true;
            EnEnableMove();
        }
    }

    public virtual void StartAttackCO()
    {
        if(!isAttacking) //check needed, or else IsAttackingCO doesn't get stopped by StopCoroutine
            IsAttackingCO = StartCoroutine(Attacking());
    }

    public void StopAttackCO()
    {
        if (IsAttackingCO != null)
        {
            isAttacking = false;
            enCanAttack = true;
            StopCoroutine(IsAttackingCO);
        }
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
        // Switch the way the enemy is labelled as facing.
        //enFacingRight = !enFacingRight;

        // Multiply the enemy's x local scale by -1
        //.Vector3 theScale = transform.localScale;
        if (enCanMove && !isHurt) //prevent flipping if receiving knockback
        {
            if(rb.velocity.x > 0) //moving right
            {
                enFacingRight = true;
            }
            else if(rb.velocity.x < 0) //moving left
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

    public void ManualFlip(bool faceRight) //TODO: delete if not used
    {
        if (enCanFlip)
        {
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
    }

    #endregion

    #region Interaction References
    public virtual void TakeDamage(float damage, float damageMultiplier = 1.0f, bool isCrit = false)
    {
        if (isAlive)
        {
            float damageTaken = damage * damageMultiplier;
            currentHealth -= damageTaken;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            if (TextPopupsHandler)
            {
                Vector3 tempPos = transform.position;
                tempPos.y += TPOffset;
                TextPopupsHandler.ShowDamage(damageTaken, tempPos, isCrit);
                if (isCrit && screenshake != null)
                {
                    screenshake.Shake(1);
                }
            }

            if(damage > 0)
            {
                Vector3 particleLocation = transform.position;
                particleLocation.y += hitEffectOffset;
                HitEffectsHandler.ShowHitEffect(particleLocation);

                if (!isAttacking)
                    EnAnimator.PlayHurt();

                sr.material = mWhiteFlash;
                isHurt = true;
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
        isHurt = false;
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

    public void GetKnockback(bool playerFacingRight, float thrustMult = 1f)//, float kbDuration = 5f) //defaults
    {
        //playerFacingRight - passed from PlayerCombat when damage is applied
        //kbThrust - velocity of lunge movement
        //kbDuration - how long to maintain thrust velocity (distance)

        Vector3 tempOffset = gameObject.transform.position;
        if (playerFacingRight) //knockback <- enemy -- player //knockback to direction player is facing
        {
            tempOffset.x += kbDuration;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, (kbThrust * thrustMult) * Time.fixedDeltaTime);
            transform.position = smoothPosition;
        }
        else //player -- enemy -> knockback
        {
            tempOffset.x -= kbDuration;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, (kbThrust * thrustMult) * Time.fixedDeltaTime);
            transform.position = smoothPosition;
        }
        KnockbackRecover();
    }

    public virtual void GetStunned(float duration = 1f, bool fullStun = true)
    {
        if (isAlive)
        {
            if(Time.time > allowStun && !enStunned)
            {
                StopAttackCO();

                if (fullStun)
                {
                    float fullDuration = 1f; //0.0 - 1.0 for % of stun duration reduction
                    fullDuration -= stunResist;
                    duration *= fullDuration; //get adjusted duration with stunResist % reduced
                    StartCoroutine(StunEnemy(duration));
                }
                else
                {
                    //
                }
            }
        }
    }

    protected virtual IEnumerator StunEnemy(float stunDuration)
    {
        if (!enStunned)
        {
            enStunned = true;
            enCanAttack = false;
            EnDisableMove();

            EnAnimator.PlayStunned();

            if (stunLParticlePrefab != null && stunRParticlePrefab != null)
            {
                //Vector3 changeLocation = GetComponent<Transform>().position;
                //Vector3 tempLocation = changeLocation;
                Vector3 tempLocation = transform.position;
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

        StopAllCoroutines();
        EnAnimator.PlayDeathAnim();

        if (DeathParticlesHandler != null)
        {
            Vector3 tempLocation = transform.position;
            tempLocation.y += hitEffectOffset;

            DeathParticlesHandler.ShowHitEffect(tempLocation);
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
        moveSpeed = 0; //
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void EnEnableMove()
    {
        enCanMove = true;
        moveSpeed = defaultMoveSpeed; //remove if not lowering movespeed
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
