using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy2 : MonoBehaviour
{

    public EnemyController enController;

    [SerializeField] Vector3 TPOffset = new Vector3(0, 0.7f, 0);

    TimeManager timeManager;
    private ScreenShakeListener screenshake;

    //Text Popups
    [SerializeField] private TextPopupsHandler AttackIndicator; //TODO: should be animation or no handler, manual instantiate

    //[SerializeField] private HitEffectsHandler HitEffectsHandler;
    //[SerializeField] private TextPopupsHandler TextPopupsHandler;

    //public LayerMask playerLayers;
    //public Transform player;
    //public PlayerCombat playerCombat;

    //public GameObject hitParticlePrefab;
    //public GameObject deathParticlePrefab;
    //public GameObject stunLParticlePrefab;
    //public GameObject stunRParticlePrefab;
    public GameObject initialStunParticle;


    //public float maxHealth = 200;
    //float currentHealth;
    //public HealthBar healthBar;
   // float maxHeal = 100;

    //experience points based on enemy level
    //public int enLevel


    //public int experiencePoints = 20;
    //public Animator enAnimator;
    //public bool isAlive;
    bool allowBreak;
    bool isBroken;
    bool isShielded = true;

    
    //public Rigidbody2D rb;
    [SerializeField]
    float aggroRange = 3f; //when to start chasing player
                           //might extend to aggro to player before enemy enters screen
    [SerializeField]
    float enAttackRange = .5f; //when to start attacking player, stop enemy from clipping into player
    //public Transform enAttackPoint;
    [Space]
    public float enAttackDamage = 10f;
    public float enAttackSpeed = 1.1f; //lower value for lower delays between attacks
    public float enAttackAnimSpeed = .4f; //lower value for shorter animations
    [Range(0f, 1.0f)]
    public float stunResist = 0f; //0f takes full stun duration, 1.0f complete stun resist
    public float allowStun = 0f;
    public float allowStunCD = 1f; //how often enemy can be stunned

    [SerializeField] bool enCanAttack, isAttacking; //for parry()
    [SerializeField]
    //bool playerToRight, aggroStarted;
    //bool enIsHurt;
    bool enStunned;
    //bool canChase;

    //Coroutine IsAttackingCO;

    //[Header("Attack variables")]
    //RaycastHit2D aggroRaycast;

    //SpriteRenderer sr;
    //[SerializeField]
    //private Material mWhiteFlash;
    //private Material mDefault;
    

    void Start()
    {
        //sr = GetComponent<SpriteRenderer>();
        //mDefault = sr.material;

        //player = GameObject.FindGameObjectWithTag("Player").transform;
        //playerCombat = player.GetComponent<PlayerCombat>();
        screenshake = GameObject.Find("ScreenShakeManager").GetComponent<ScreenShakeListener>();

        AttackIndicator = GameObject.Find("ObjectPool(Attack/Alert Indicators)").GetComponent<TextPopupsHandler>();
        //TextPopupsHandler = GameObject.Find("ObjectPool(TextPopups)").GetComponent<TextPopupsHandler>();
        //HitEffectsHandler = GameObject.Find("ObjectPool(HitEffects)").GetComponent<HitEffectsHandler>();

        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        //Stats
        //currentHealth = maxHealth;
        //if (healthBar != null)
       // {
         //   healthBar.SetMaxHealth(maxHealth);
       // }

        //isAlive = true;
        //enController.enCanMove = true;
        //enCanAttack = true;
        //AI aggro
       // rb = GetComponent<Rigidbody2D>();
        enController.enAnimator.SetBool("move", false); //TODO: unnecessary
        //enController.enAnimator.SetBool("isRunning", false);
        //enController.enFacingRight = false; //start facing left (towards player start)
        //isAttacking = false;
        //aggroStarted = false;
        //enIsHurt = false;
        //enStunned = false;
        //canChase = true;
        allowBreak = false;
        isBroken = false;

        //enController.moveSpeed += Random.Range(-.1f, .1f);
    }

    void Update()
    {
        IdleAnimCheck();
        //Move();
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

    /*void WhereIsPlayer()
    {
        if (transform.position.x < player.position.x) //player is right
        {
            playerToRight = true;
        }
        else if (transform.position.x > player.position.x) //player is left
        {
            playerToRight = false;
        }
    }*/

    /*void Move()
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

    }*/

    //AI aggro
    /*void StartChase()
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
    }*/

    public void MoveCheck() //RaycastChecks
    {
        //groundDetect 
        //wallDetect 
        //playerDetectFront
        //playerDetectBack

        if (enController.groundDetect && !enController.aggroStarted && !isAttacking)
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
        if (!enController.wallDetect && enController.groundDetect && !enStunned)
        {
            if (enController.playerDetectFront || enController.playerDetectBack)
            {
                if (enController.IsPatrollingCO != null)
                    StopCoroutine(enController.IsPatrollingCO);

                if (enController.IsIdlingCO != null && !enController.knockbackHit)
                {
                    StopCoroutine(enController.IsIdlingCO);
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
                else
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

    void StopChase() //TODO: might not need 
    {
        //canChase = false;
        enController.rb.velocity = new Vector2(0, 0);
        enController.enAnimator.SetBool("move", false);
        //enAnimator.SetBool("inCombat", true);
        enController.enCanMove = false;
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

    void StartAttack(int atkVariation = 2)
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
                    enController.IsAttackingCO = StartCoroutine(IsComboAttacking());
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator IsAttacking()
    {
        if (enCanAttack && !isAttacking && !enStunned)
        {
            isAttacking = true;
            enStunned = false;
            enCanAttack = false;

            enController.enAnimator.SetTrigger("Attack");

            enController.enAnimator.SetBool("isAttacking", true);
            enController.enAnimator.SetBool("move", false);

            enController.enCanMove = false;
            enController.rb.velocity = new Vector2(0, enController.rb.velocity.y);

            yield return new WaitForSeconds(enAttackAnimSpeed); //time when damage is dealt based on animation

            enController.rb.velocity = new Vector2(0, enController.rb.velocity.y); //stop enemy from moving
            enController.Attack();
            yield return new WaitForSeconds(enAttackSpeed); //delay between attacks
            enController.enAnimator.SetBool("isAttacking", false);
        
            enController.enCanMove = true;
            enCanAttack = true;
            isAttacking = false;
            //canChase = true;
        }
    }

    /*void Attack(float damageMult = 1f) //default, attack player when in melee range
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, enController.playerLayer);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            if (player.GetComponent<PlayerCombat>() != null)
                player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage * damageMult); //attackDamage + additional damage from parameter
        }
    }*/

    void Attack2() //default, attack player when in melee range
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enController.enAttackPoint.position, enAttackRange, enController.playerLayer);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            if (player.GetComponent<PlayerCombat>() != null)
            {
                player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage); //attackDamage + additional damage from parameter
                player.GetComponent<PlayerCombat>().GetKnockback(enController.playerToRight);
            }
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
            enController.Attack(1.5f);

            yield return new WaitForSeconds(.4f);
            //enAnimator.SetTrigger("IdleStunnable");
            enController.enAnimator.SetBool("IdleStunnableB", true);

            yield return new WaitForSeconds(.3f);
            //StopChase();
            enController.enAnimator.SetBool("IdleStunnableB", false);

            yield return new WaitForSeconds(.5f);

            //canChase = true;
            enController.enCanMove = true;
            isAttacking = false;
            enCanAttack = true;
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
            if(enController.playerDetectFront)
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
            else //playerDetectBack, lunge towards player
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

        Gizmos.DrawWireSphere(enController.enAttackPoint.position, enAttackRange);
    }

    /*public void TakeDamage(float damage, float damageMultiplier = 1.0f)
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
    }*/

    /*public void TakeHeal(float healAmount)
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
    }*/

    /*void ResetMaterial()
    {
        sr.material = mDefault;
    }*/

    /*public void GetKnockback(bool playerFacingRight, float kbThrust = 2f, float kbDuration = 5f) //defaults
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
    }*/

    /*public void EnIsHurtStart()
    {
        enIsHurt = true;
    }

    public void EnIsHurtEnd()
    {
        enIsHurt = false;
    }*/

    public void GetStunned(float duration) //allow player to call this function //TODO: this might be more robust than enController's
    {
        if (enController.isAlive)
        {
            //if (Time.time > allowStun && !enStunned) //cooldown timer starts when recovered from stun
            if(allowBreak && !isBroken)
            {
                float fullDuration = 1f;
                fullDuration -= stunResist; //getting percentage of stun based on stunResist
                duration *= fullDuration;
                enController.enAnimator.SetBool("IdleStunnableB", false);
                
                isAttacking = false;

                /*if (timeManager != null)
                {
                    timeManager.CustomSlowMotion(.02f, 5f);
                }*/

                if (enController.IsAttackingCO != null)
                    StopCoroutine(enController.IsAttackingCO); //stopping attack coroutine when attacking

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
            enController.enAnimator.SetTrigger("en2Stunned");

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
            enStunned = false;

            //canChase = true;
            enCanAttack = true;
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

        //StopAllCoroutines(); //stops attack coroutine if dead



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
