using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyBossBandit : MonoBehaviour
{
    [Header("Text Popups")]
    public GameObject TextPopupsPrefab;
    private TextPopupsHandler TextPopupsHandler;
    private TextPopupsHandler AttackIndicator;
    [SerializeField] Vector3 TPOffset = new Vector3 (0, .1f, 0);
    public Transform TPOffsetObj;
    public HitEffectsHandler HitEffectsHandler;

    [Space]
    [Header("References")]
    public LayerMask playerLayers;
    public Transform player;
    public PlayerCombat playerCombat;
    public TimeManager timeManager;
    public ScreenShakeListener screenShake;
    public GameObject hitParticlePrefab;
    public GameObject deathParticlePrefab;
    public GameObject stunLParticlePrefab;
    public GameObject stunRParticlePrefab;
    public GameObject parriedParticlePrefab;


    public GameObject HealthBarCanvas;
    public float maxHealth = 1000;
    float currentHealth;
    public HealthBar healthBar;

    //experience points based on enemy level
    //public int enLevel


    public int experiencePoints = 80;
    public Animator enAnimator;
    public bool isAlive;

    public Rigidbody2D rb;
    [SerializeField]
    float aggroRange = 5f; //when to start chasing player
                           //might extend to aggro to player before enemy enters screen
    [SerializeField]
    float enAttackRange1 = .5f; //when to start attacking player, stop enemy from clipping into player
    public Transform enAttackPoint;
    public Transform enAttackPoint2; //used as PointA for OverlapAreaAll
    [SerializeField]
    Vector2 enAttackRange2 = new Vector2(1.85f, 0.4f);
    public EnemyController enController;
    [Space]
    public float enAttackDamage = 10f;
    public float enAttackSpeed = 2f; //lower value for lower delays between attacks
    public float enAttackAnimSpeed = .4f; //lower value for shorter animations
    [Range(0f, 1.0f)]
    public float stunResist = .5f; //0f takes full stun duration, 1.0f complete stun resist
    public float allowStun = 0f;
    public float allowStunCD = 5f; //how often enemy can be stunned
    public float damageTakenMultiplier = 1f;
    float breakDuration;
    int atkSequence;

    [SerializeField]
    bool enCanAttack = true, isAttacking; //for parry()
    [SerializeField]
    bool playerToRight, aggroStarted;
    bool enIsHurt;
    bool enStunned;
    bool particleHits;

    bool allowBreak;
    bool isBroken;

    Coroutine IsAttackingCO;

    SpriteRenderer sr;
    [SerializeField]
    private Material mWhiteFlash;
    private Material mDefault;

    //Animation timers
    private AnimationClip clip;

    public float stunnedAnimTime;
    public float combo3Attack1AnimTime;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mDefault = sr.material;
        //sr.material.SetFloat(2f);

        player = GameObject.Find("Player").transform;
        playerCombat = GameObject.Find("Player").GetComponent<PlayerCombat>();

        TextPopupsHandler = GameObject.Find("ObjectPool(TextPopups)").GetComponent<TextPopupsHandler>();
        AttackIndicator = GameObject.Find("ObjectPool(Attack/Alert Indicators)").GetComponent<TextPopupsHandler>();

        //Stats
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        isAlive = true;
        //enController.enCanMove = true;
        enCanAttack = true;
        allowBreak = false;
        isBroken = false;
        //AI aggro
        rb = GetComponent<Rigidbody2D>();
        enAnimator.SetBool("Move", false);
        isAttacking = false;
        aggroStarted = false;
        enIsHurt = false;
        enStunned = false;
        particleHits = false;
        atkSequence = 1;

    }   

    void Update()
    {
        WhereIsPlayer();
        Move();
        MoveAnimCheck();
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
        if (rb != null && enController != null && isAlive && playerCombat.isAlive) //check if object has rigidbody
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

    void MoveAnimCheck()
    {
        if(rb.velocity.x == 0)
        {
            enAnimator.SetBool("Move", false);
            enAnimator.SetBool("Idle", true);
        }
        else
        {
            enAnimator.SetBool("Move", true);
            enAnimator.SetBool("Idle", false);
        }
    }

    //AI aggro
    void StartChase()
    {
        ShowHealthBar();


        if (enController.enCanMove)
        {
            enAnimator.SetBool("Move", true);
            if (transform.position.x < player.position.x) //player is right
            {
                //player is to right, move right

                rb.velocity = new Vector2(enController.moveSpeed, 0); //moves at moveSpeed


                //Facing right, flip sprite to face right
                enController.enFacingRight = true;
                enController.Flip();
                if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange1)
                {
                    IsAttackingCO = StartCoroutine(IsAttacking());
                    //isAttacking = false;
                }
            }
            else if (transform.position.x > player.position.x) //player is left
            {
                //player is to left, move left

                rb.velocity = new Vector2(-enController.moveSpeed, 0);


                enController.enFacingRight = false;
                //if (enCanMove)
                enController.Flip();
                if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange1)
                {
                    IsAttackingCO = StartCoroutine(IsAttacking());
                    //isAttacking = false;
                }
            }
        }

        if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange1)
        {
            StopChase(); //stop moving, don't clip into player just to attack
        }
    }

    void StopChase()
    {
        rb.velocity = new Vector2(0, 0);
        enAnimator.SetBool("Move", false);
        enController.enCanMove = false;
    }

    void ShowHealthBar()
    {
        if(healthBar != null && HealthBarCanvas != null)
        {
            HealthBarCanvas.GetComponentInChildren<Canvas>().enabled = true;
            //healthBar.GetComponentInChildren<SpriteRenderer>().enabled = true;
            healthBar.SetHealth(currentHealth);
        }
    }

    void ShowAttackIndicator()
    {
        Vector3 tempOffset = TPOffsetObj.transform.position;
        tempOffset.y -= 0.5f;

        if (AttackIndicator != null)
        {
            AttackIndicator.ShowText(tempOffset, "!");
        }
    }

    void ShowAttackIndicator2()
    {
        Vector3 tempOffset = TPOffsetObj.transform.position;
        tempOffset.y -= 0.4f;

        if (AttackIndicator != null)
        {
            AttackIndicator.ShowText(tempOffset, "!!", 1.4f);
        }
    }

    void Attack(float damageMultiplier, bool knockback = false) //knockback has short stun
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange1, playerLayers);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            playerCombat.TakeDamage(enAttackDamage * damageMultiplier); //attackDamage + additional damage from parameter
            float distToPlayer = transform.position.x - player.transform.position.x; //getting player direction to enemy //if 0 will use last direction
            if (knockback)
            {
                if (distToPlayer > 0)
                {
                    playerCombat.GetKnockback(false); //knockback to left
                }
                else
                {
                    playerCombat.GetKnockback(true); //knockback to right
                }
            }
        }
    }

    void Attack2(float damageMultiplier, bool knockback = true) //knockback has short stun
    {
        Collider2D[] hitPlayer = Physics2D.OverlapBoxAll(enAttackPoint2.position, enAttackRange2, 180, playerLayers);

        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage * damageMultiplier); //attackDamage + additional damage from parameter
            float distToPlayer = transform.position.x - player.transform.position.x; //getting player direction to enemy //if 0 will use last direction
            if (knockback)
            {
                if (distToPlayer > 0)
                {
                    playerCombat.GetKnockback(false); //knockback to left
                }
                else
                {
                    playerCombat.GetKnockback(true); //knockback to right
                }
            }
        }
    }

    IEnumerator IsAttacking()
    {//TODO: combine redundant variables
        if (enCanAttack && !isAttacking && !isBroken)
        {
            int phase;
            if (currentHealth >= (maxHealth / 2)) //phase 1: 50%+ hp
            {
                phase = 1;
                breakDuration = .5f; //stun duration when broken
            }
            else //phase 2
            {
                phase = 2;
                breakDuration = 1.0f;
            }

            Vector3 tempPos = TPOffsetObj.transform.position;
            tempPos.y -= .5f; //TPOffset;

            enCanAttack = false;
            isAttacking = true;

            switch (atkSequence)
            {
                case 1: //Attack1 //can be parried
                    //enStunned = false;
                    atkSequence = 2;

                    isAttacking = true;
                    enAnimator.SetTrigger("Attack1Slow");

                    enAnimator.SetBool("Move", false);

                    enCanAttack = false;
                    enController.enCanMove = false;
                    rb.velocity = new Vector2(0, 0);

                    yield return new WaitForSeconds(0.7f); //time until damage is dealt based on animation

                    rb.velocity = new Vector2(0, 0); //stop enemy from moving
                    Attack(1.5f);
                    yield return new WaitForSeconds(enAttackSpeed); //delay between attacks

                    break;
                case 2: //Attack2 //can be parried
                    //enStunned = false;
                    if(phase == 1)
                    {
                        atkSequence = 3;
                    }
                    else
                    {
                        atkSequence = 4;
                    }

                    isAttacking = true;
                    enAnimator.SetTrigger("Attack2Slow");
                    enAnimator.SetBool("Move", false);
                    rb.velocity = new Vector2(0, 0);

                    yield return new WaitForSeconds(0.6f);
                    LungeOnAttack();
                    yield return new WaitForSeconds(0.02f);
                    enController.enCanMove = false;
                    Attack2(1f);

                    yield return new WaitForSeconds(enAttackSpeed);

                    break;
                case 3: //Attack1 + Attack2 //Attack1 can be parried
                    atkSequence = 1;
                    ShowAttackIndicator();

                    //enStunned = false;
                    isAttacking = true;
                    enAnimator.SetBool("Move", false);
                    rb.velocity = new Vector2(0, 0);

                    enAnimator.SetTrigger("Attack1SlowStartCombo");
                    yield return new WaitForSeconds(0.6f);
                    Attack(1.5f);

                    enAnimator.SetTrigger("Attack2Slow");
                    yield return new WaitForSeconds(0.5f); //maybe faster start up variation for this combo
                    LungeOnAttack();
                    yield return new WaitForSeconds(0.02f);
                    enController.enCanMove = false;
                    Attack2(1f);
                    yield return new WaitForSeconds(0.4f);
                    enAnimator.SetBool("IdleLongStunnable", true);
                    yield return new WaitForSeconds(1.0f); //enAttackSpeed * 2f
                    enAnimator.SetBool("IdleLongStunnable", false);


                    break;
                case 4: //Attack1+2 x 3 //can flip, can parry first attack //has knockback
                    //canParry is set in animationEvent
                    atkSequence = 1;
                    breakDuration = 1.5f;
                    ShowAttackIndicator2();

                    //enStunned = false;
                    isAttacking = true;
                    enAnimator.SetBool("Move", false);
                    rb.velocity = new Vector2(0, 0);

                    enAnimator.SetTrigger("Attack1SlowStartCombo"); //Attack1 can be stunned
                    yield return new WaitForSeconds(.6f);
                    LungeOnAttack();
                    yield return new WaitForSeconds(0.02f);
                    enController.enCanMove = false;
                    Attack(1f);
                    yield return new WaitForSeconds(.4f);

                    enAnimator.SetTrigger("Attack2SlowStartCombo"); //Attack2
                    yield return new WaitForSeconds(.4f);
                    LungeOnAttack();
                    yield return new WaitForSeconds(0.02f);
                    enController.enCanMove = false;
                    Attack2(1f);
                    yield return new WaitForSeconds(.3f);

                    enAnimator.SetTrigger("Attack1SlowStartCombo2"); //Attack1
                    yield return new WaitForSeconds(.3f);
                    LungeOnAttack();
                    yield return new WaitForSeconds(0.02f);
                    enController.enCanMove = false;
                    Attack(1.5f);
                    yield return new WaitForSeconds(.2f);

                    enAnimator.SetTrigger("Attack2SlowStartCombo2"); //Attack2
                    yield return new WaitForSeconds(.2f);
                    LungeOnAttack();
                    yield return new WaitForSeconds(0.02f);
                    enController.enCanMove = false;
                    Attack2(1.5f);
                    yield return new WaitForSeconds(.2f);

                    enAnimator.SetTrigger("Attack1SlowStartCombo3"); //Attack1
                    yield return new WaitForSeconds(.2f);
                    LungeOnAttack();
                    yield return new WaitForSeconds(0.02f);
                    enController.enCanMove = false;
                    Attack(2f);
                    yield return new WaitForSeconds(.2f);

                    enAnimator.SetTrigger("Attack2SlowStartCombo3"); //Attack2
                    yield return new WaitForSeconds(.2f);
                    LungeOnAttack();
                    yield return new WaitForSeconds(0.02f);
                    enController.enCanMove = false;
                    Attack2(2f);

                    yield return new WaitForSeconds(.4f); //short delay so the Attack2 anim doesn't get cut off
                    enAnimator.SetBool("IdleLongStunnable", true);
                    yield return new WaitForSeconds(.5f);
                    enAnimator.SetBool("IdleLongStunnable", false);
                    //moving from Stunnable idle anim IdleLong anim
                    enAnimator.SetBool("IdleLong", true);
                    yield return new WaitForSeconds(.5f);
                    enAnimator.SetBool("IdleLong", false);

                    yield return new WaitForSeconds(enAttackSpeed); //long delay before attacking again since we have a long attack sequence
                    break;
                default:
                    yield return new WaitForSeconds(0.01f); //
                    break;
            }
            enController.enCanMove = true;
            isAttacking = false;
            enCanAttack = true;
        }
    }

    void LungeOnAttack(float lungeThrust = 3f, float lungeDuration = 5f) //defaults
    {
        //lungeThrust - velocity of lunge movement
        //lungeDuration - how long to maintain thrust velocity

        float distToPlayer = transform.position.x - player.transform.position.x; //getting player direction to enemy //if 0 will use last direction

        Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset

        if (enController.enCanFlip)
        {
            enController.enCanMove = true;
            if (distToPlayer < 0) //to right of player //swapped > to < from knockback, moving towards player instead of away
            {
                //lunge to right
                tempOffset.x += lungeDuration;
                Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, lungeThrust * Time.fixedDeltaTime);
                transform.position = smoothPosition;
            }
            else //to left of player
            {
                //lunge to left
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
        
        //Attack1
        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRange1);
        
        if (enAttackPoint2 == null)
            return;

        //Attack2
        Gizmos.DrawWireCube(enAttackPoint2.position, new Vector3(enAttackRange2.x, enAttackRange2.y, 0f));
        
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
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;


            //temp knockback
            Vector3 tempLocation = TPOffsetObj.transform.position;

            //
            HitEffectsHandler.ShowHitEffect(tempLocation);
            //show damage/heal numbers
            if (TextPopupsHandler)
            {
                Vector3 tempPos = TPOffsetObj.transform.position;
                tempPos += TPOffset;
                if(isBroken) //crit damage, damage was multiplied, this is set when Boss is parried
                {
                    //
                    timeManager.DoFreezeTime(.1f);
                    TextPopupsHandler.ShowDamage(damageTaken, tempPos, true);
                }
                else
                {
                    TextPopupsHandler.ShowDamage(damageTaken, tempPos);
                }
            }

            //hurt animation
            if (enAnimator != null && damage > 0) //took damage, not heal
            {
                enIsHurt = true;
                enAnimator.SetTrigger("Hurt");

                sr.material = mWhiteFlash; //flashing enemy sprite
                Invoke("ResetMaterial", .1f);
            }

            if(isBroken)
            {
                screenShake.Shake();
                enAnimator.SetTrigger("StunHits");

                if (particleHits)
                {
                    if (playerToRight)
                    {
                        Instantiate(stunLParticlePrefab, tempLocation, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(stunRParticlePrefab, tempLocation, Quaternion.identity);
                    }
                }
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void ResetMaterial()
    {
        sr.material = mDefault;
    }

    public void GetKnockback(float knockbackAmount)
    {
        if (rb != null)
        {
            Vector3 changeLocation = GetComponent<Transform>().position;

            if (playerToRight)
            {
                changeLocation.x -= .1f; //knockback
            }
            else
            {
                changeLocation.x += .1f;
            }

            GetComponent<Transform>().position = changeLocation;
            //StartCoroutine(StunEnemy(0.1f));
        }
    }

    public void CheckParry()
    {
        if (allowBreak && !isBroken && isAlive)
        {
            StartCoroutine(GetParried());
        }
    }

    IEnumerator GetParried()
    {
        enAnimator.SetBool("IdleLongStunnable", false); //need to reset animation bools since attack is interrupted
        enAnimator.SetBool("IdleLong", false);

        if (IsAttackingCO != null)
            StopCoroutine(IsAttackingCO);

        enController.enCanMove = false;
        isAttacking = false;
        enCanAttack = false;
        Instantiate(parriedParticlePrefab, transform.position, Quaternion.identity);

        isBroken = true;

        Vector3 tempPos = TPOffsetObj.position;
        tempPos += new Vector3(0, .1f, 0);
        AttackIndicator.ShowBreak(tempPos);

        yield return new WaitForSeconds(.1f);
        if(timeManager != null)
            timeManager.DoSlowMotion();

        particleHits = true;
        //damageTakenMultiplier = 2f; //update to only be called TakeDamage when isBroken is true
        enAnimator.SetTrigger("Stunned");
        yield return new WaitForSeconds(.5f); //1.1f //keep this static, transition from initial Stunned to StunnedLoop
        enAnimator.SetBool("IdleLoopStunned", true);

        yield return new WaitForSeconds(breakDuration); //1.3f //recoverDuration //change based on phase
        enAnimator.SetBool("IdleLoopStunned", false);
        isBroken = false;
        yield return new WaitForSeconds(.2f);
        ShowAttackIndicator();
        yield return new WaitForSeconds(.5f);

        enController.enCanMove = true;
        enCanAttack = true;
    }

    public void EnIsHurtStart()
    {
        enIsHurt = true;
    }

    public void EnIsHurtEnd()
    {
        enIsHurt = false;
    }

    void Die()
    {
        enAnimator.SetBool("IdleLongStunnable", false);
        enAnimator.SetBool("IdleLong", false);
        enAnimator.SetBool("Move", false);
        enAnimator.SetBool("Idle", false);

        isAlive = false;
        //give player exp
        playerCombat.GiveXP(experiencePoints);

        StopAllCoroutines(); //stops attack coroutine if dead

        //disable enemy object
        isAlive = false;

        if (deathParticlePrefab != null)
        {
            Vector3 changeLocation = GetComponent<Transform>().position;
            Vector3 tempLocation = changeLocation;
            //tempLocation.y += .5f;
            Instantiate(deathParticlePrefab, tempLocation, Quaternion.identity);
        }

        StartCoroutine(DeleteEnemyObject());
    }

    IEnumerator DeleteEnemyObject()
    {
        HealthBarCanvas.GetComponent<Canvas>().enabled = false;

        Vector3 changeLocation = GetComponent<Transform>().position;
        Vector3 tempLocation = changeLocation;
        //tempLocation.y += .5f;

        int numLoops = 3;

        for(int i=0; i<numLoops; i++)
        {
            enAnimator.SetTrigger("startDeath");
            Instantiate(deathParticlePrefab, tempLocation, Quaternion.identity);
            yield return new WaitForSeconds(.5f);
        }

        if (enAnimator != null)
        {
            //enAnimator.SetTrigger("Death");
        }
        yield return new WaitForSeconds(.5f);
        
        Instantiate(deathParticlePrefab, tempLocation, Quaternion.identity);

        //yield return new WaitForSeconds(.1f);

        //Destroy(this.gameObject);
        gameObject.SetActive(false);
    }
}
