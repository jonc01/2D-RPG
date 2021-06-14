using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Object/Script References")]
    public Animator animator;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] bool enableScreenshake = true;
    public PlayerMovement movement;
    public CharacterController2D controller;
    public Transform playerLocation;
    [SerializeField] Canvas PlayerHealthBarCanvas;
    [SerializeField] GameObject StatusStunned;
    public TimeManager timeManager;
    public ScreenShakeListener screenShake;
    public GameObject respawnPrompt;

    [Header("Ability UI")]
    [Space]
    public AbilityUI abilityUI;
    public AbilityCycle ability1Cycle;
    public AbilityCycle ability2Cycle;
    bool canSwitch = false;

    [Header("Text Popups")]
    [Space]
    public TextPopupsHandler xpPopups;
    public TextPopupsHandler TextPopupsHandler;
    [SerializeField] Vector3 TPOffset = new Vector3(0, 0f, 0);

    [Space]
    public float playerLevel = 1; //PLACEHOLDER, use save file
    public float maxHealth = 100;
    public float currentHealth;
    public HealthBar healthBar;
    public ExperienceBar experienceBar;
    public bool isAlive = true;
    bool canRespawn = false;
    float allowStun;
    float allowStunCD = 1f;

    [Space]
    //weapon stats
    public float wepDamage = 10f; //default values, should change based on weapon stats
    public float wepRange = .5f; //
    public float playerAttackSpeed = .1f; //.3f
    private float playerHeavyAttackSpeed;

    public Transform attackPoint;
    public Transform heavyAttackPoint;
    public Transform heavyAttackPointWide;
    public Transform parryPoint;
    [SerializeField] float attackRange = .5f;
    [SerializeField] float attackHeavyRange = 0.58f;
    float attackDamageLight;
    float attackDamageHeavy;
    [SerializeField] float attackDamageHeavyMultiplier = 2.0f;
    public bool canAttack = true;
    public float stunStrength = 1f;
    [SerializeField] bool playerStunned;

    public float attackTime = 0.25f; //0.25 seems good, give or take .1 seconds

    //armor stats
    public float playerArmor; //temp

    //Block check
    //private const float minBlockDuration = 0.25f;
    //private float currentBlockDuration = 0f;
    //private bool blockIsHeld = false;

    public LayerMask enemyLayers;
    public LayerMask chestLayers;

    private int currentLightAttack = 0;
    private float timeSinceLightAttack = 0.0f;
    private int currentHeavyAttack = 0; //sharing currentAttack with Light and Heavy
    private float timeSinceHeavyAttack = 0.0f;

    //ability cooldowns
    [Header("Alt Attack")]
    public Collider2D shieldBashCollider;
    public Collider2D shieldBashTrigger;
    public float altAttackCD = 3f; //shieldBash cooldown
    bool AltAttacking;
    private float allowAltAttack = 0; //shieldBash
    public float altAttackTime = .3f;
    bool IsParrying;
    public bool IsShieldBashing;
    bool canStunPlayer;

    //Consumables
    public PlayerHealthPotion healthPotion;

    //weapon specific
    public float knockback = 5f;

    SpriteRenderer sr;
    [SerializeField] private Material mWhiteFlash;
    private Material mDefault;
    Coroutine IsLightAttackingCO;
    Coroutine IsHeavyAttackingCO;
    Coroutine PlayerStunnedCO;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mDefault = sr.material;

        maxHealth = 100 + ((playerLevel - 1) * 10);
        experienceBar.SetXP(0, playerLevel);

        //var keyboard = Keyboard.current; //temp workaround
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        isAlive = true;
        allowStun = 0; //stun immunity CD

        //weapons stats
        //attackRange = wepRange; // would have to tie separate animations to weapons
        //attackHeavyRange = wepRange*1.15f;

        playerHeavyAttackSpeed = (playerAttackSpeed * 1.25f);
        //attackDamageHeavyMultiplier = weapon.wepDamageHeavyMultiplier;
        //attackDamageHeavyMultiplier = 2.0f; //placeholder

        attackDamageLight = wepDamage;
        attackDamageHeavy = wepDamage*attackDamageHeavyMultiplier;

        canAttack = true;
        AltAttacking = false;
        IsParrying = false;
        IsShieldBashing = false;
        playerStunned = false;
        canStunPlayer = true;

        shieldBashCollider.enabled = false;
        shieldBashTrigger.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLightAttack += Time.deltaTime;
        timeSinceHeavyAttack += Time.deltaTime;

        if (!PauseMenu.GameIsPaused)
        {
            if (movement.isGrounded)
            {
                CheckLightAttack();
                CheckHeavyAttack();
            }
            ShieldBashInput();

            //DodgeAttackCancel(); //not currently in-use, allows for either cancelling of attacks with a dodge input, or some alt Dodge attack
            if (!isAlive && canRespawn)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if(respawnPrompt != null)
                    {
                        respawnPrompt.GetComponent<TextMeshProUGUI>().enabled = false;
                    }
                    //RevivePlayer(1.0f); //1.0 = 100%, 0.5 = 50%
                    controller.RespawnPlayerResetLevel();
                    timeManager.ResetTimeScale();
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                healthPotion.UsePotionCharge();
            }
        }
        UpdateAbilityDisplay();
    }
    void UpdateAbilityDisplay()
    {
        if (currentLightAttack > 0 && currentLightAttack <= 3 && canSwitch == true)
        {
            int displayAbility; //ability index to display, [0] -> 1, [1] -> 2, [2] -> 3
            displayAbility = currentLightAttack; //set to currentAttack number to display next attack
            if(currentLightAttack >= 3)
            {
                displayAbility = 0; //exceeded max attack number/index, display first ability
            }
            ability1Cycle.ShowAbility(displayAbility);
            ability2Cycle.ShowAbility(displayAbility);
        }

        if (timeSinceLightAttack > 2.0f) // 1.5f
        {
            ability1Cycle.ShowAbility(0);
            ability2Cycle.ShowAbility(0);
        }
        if (timeSinceHeavyAttack > 2.0f) // if separating heavy attack reset
        {
            //not in use
        }
    }

    #region Light Attack
    void CheckLightAttack()
    {
        if (Input.GetButtonDown("Fire1") && timeSinceLightAttack > playerAttackSpeed && canAttack) //0.25f attack speed
        {
            currentLightAttack++;

            // Loop back to one after third attack
            if (currentLightAttack > 3)
                currentLightAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (timeSinceLightAttack > 2.0f) //if using separate timeSince...Attack for Light only use 1.0f
                currentLightAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            animator.SetTrigger("Attack" + currentLightAttack);

            //Coroutine IsAttackingCO;
            IsLightAttackingCO = StartCoroutine(IsLightAttacking(currentLightAttack));

            // Reset timer
            timeSinceLightAttack = 0.0f;
        }
    }

    IEnumerator IsLightAttacking(int attackNum) // Light Attack Coroutine
    {
        if(movement.isGrounded) //should let player attack mid air without stopping movement
            movement.canMove = false;

        animator.SetBool("isAttacking", true);
        movement.rb.velocity = new Vector2(0, movement.rb.velocity.y); //maintaining y velocity, instead of making player float
        canAttack = false;

        switch (attackNum)
        {
            case 1:
                yield return new WaitForSeconds(0.1f);
                Attack();
                canSwitch = true;
                yield return new WaitForSeconds(0.1f);
                break;
            case 2:
                yield return new WaitForSeconds(0.05f);
                Attack();
                canSwitch = true;
                yield return new WaitForSeconds(0.1f);
                break;
            case 3:
                yield return new WaitForSeconds(0.2f);
                Attack(1.5f); // damage multiplier
                canSwitch = true;
                yield return new WaitForSeconds(0.1f);
                break;
            default:
                yield return new WaitForSeconds(0.01f); //
                break;
        }

        canSwitch = false;

        //yield return new WaitForSeconds(playerAttackSpeed);
        movement.canMove = true;
        canAttack = true;
        
        animator.SetBool("isAttacking", false);
        //movement.runSpeed = movement.defaultRunSpeed;
    }

    void Attack(float damageMultiplier = 1.0f) //applying damage, ...
    {
        //Attack range, detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //damage enemies
        foreach (Collider2D enemy in hitEnemies) //loop through enemies hit
        {
            if(enemy.GetComponent<EnemyController>() != null) //after migrating below functions into EnemyController
            {
                timeManager.DoFreezeTime(.15f, .05f); //freezeDuration, delayToFreeze
                //screenShake.Shake();
                /*enemy.GetComponent<EnemyController>().TakeDamage(attackDamageLight);
                enemy.GetComponent<EnemyController>().GetKnockback(knockback/2);
                enemy.GetComponent<EnemyController>().GetStunned(.3f);*/
            }

            if (enemy.GetComponent<Enemy>() != null) //TODO: ^ add TakeDamage, etc to EnemyController manually updating for each new enemy
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamageLight, damageMultiplier); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy>().GetKnockback(controller.m_FacingRight, 1f);
                //enemy.GetComponent<Enemy>().GetStunned(.3f, false);
            }

            if (enemy.GetComponent<StationaryEnemy>() != null)
                enemy.GetComponent<StationaryEnemy>().TakeDamage(attackDamageLight, damageMultiplier);

            if (enemy.GetComponent<Enemy2>() != null)
            {
                enemy.GetComponent<Enemy2>().TakeDamage(attackDamageLight, damageMultiplier); //attackDamage + additional damage from parameter
            }

            if (enemy.GetComponent<EnemyBossBandit>() != null)
                enemy.GetComponent<EnemyBossBandit>().TakeDamage(attackDamageLight, damageMultiplier);
        }
    }
    #endregion

    #region Heavy Attack
    void CheckHeavyAttack()
    {
        if(Input.GetButtonDown("Fire2") && timeSinceLightAttack > playerAttackSpeed && canAttack) //timeSinceHeavyAttack
        {
            //currentHeavyAttack++; //sharing attack counter
            currentLightAttack++;

            if (currentLightAttack > 3)
                currentLightAttack = 1;

            //if (timeSinceHeavyAttack > 2.0f) // How long to wait before reseting attack chain

            if (timeSinceLightAttack > 2.0f)
                currentLightAttack = 1; //currentHeavyAttack

            // Call one of three attack animations "Attack1Heavy", "Attack2Heavy", "Attack3Heavy"
            animator.SetTrigger("Attack" + currentLightAttack + "Heavy"); //currentHeavyAttack

            IsHeavyAttackingCO = StartCoroutine(IsAttackingHeavy(currentLightAttack)); //currentHeavyAttack

            /*if (currentLightAttack == 2) //if we use combos with mixed Light and Heavy attacks
            {
                Debug.Log("attempt combo");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("combo works");
                }
            }*/

            //timeSinceHeavyAttack = 0.0f;
            timeSinceLightAttack = 0.0f;
        }
    }

    IEnumerator IsAttackingHeavy(int attackNum) // Heavy Attack Coroutine
    {
        if (movement.isGrounded) //should let player attack mid air without stopping movement
            movement.canMove = false;

        animator.SetBool("isAttacking", true);
        movement.rb.velocity = new Vector2(0, movement.rb.velocity.y); //maintaining y velocity, instead of making player float
        canAttack = false;

        switch (attackNum)
        {
            case 1:
                yield return new WaitForSeconds(0.35f);
                AttackHeavy();
                canSwitch = true;
                yield return new WaitForSeconds(0.2f);
                break;
            case 2:
                yield return new WaitForSeconds(0.25f); //Attack functions determine damage and attack hitbox
                AttackHeavy(2); //using hitbox 2
                canSwitch = true;
                yield return new WaitForSeconds(0.1f);
                break;
            case 3:
                yield return new WaitForSeconds(0.25f);
                AttackHeavy(1, 1.5f); //default 1, 1.5x damage
                canSwitch = true;
                yield return new WaitForSeconds(0.2f);
                break;
            default:
                yield return new WaitForSeconds(0.01f); //
                break;
        }
        canSwitch = false;

        //yield return new WaitForSeconds(playerAttackSpeed);
        movement.canMove = true;
        canAttack = true;

        animator.SetBool("isAttacking", false);
    }

    void AttackHeavy(int attackPointVar = 1, float damageMultiplier = 1.0f)
    {
        //Collider2D[] hitEnemiesWide = Physics2D.OverlapAreaAll(heavyAttackPointWide.position, (heavyAttackPointWide.position.y+attackHeavyRange), enemyLayers);
        // .58f,          
        //LungeOnAttack();

        switch (attackPointVar)
        {
            case 1:
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(heavyAttackPoint.position, attackHeavyRange, enemyLayers);
                foreach (Collider2D enemy in hitEnemies) //loop through enemies hit
                {
                    if (enemy.GetComponent<EnemyController>() != null)
                    {
                        timeManager.DoFreezeTime(.15f, .05f); //.1, .05

                        if(enableScreenshake)
                            screenShake.Shake();

                        //TODO: move common enemy scripting to EnemyController, instead of calling individual TakeDamage scripts
                    }

                    if (enemy.GetComponent<Enemy>() != null)
                    {
                        enemy.GetComponent<Enemy>().TakeDamage(attackDamageHeavy, damageMultiplier); //attackDamage + additional damage from parameter
                        enemy.GetComponent<Enemy>().GetKnockback(controller.m_FacingRight);
                    }
            
                    if (enemy.GetComponent<StationaryEnemy>() != null)
                        enemy.GetComponent<StationaryEnemy>().TakeDamage(attackDamageHeavy, damageMultiplier);

                    if (enemy.GetComponent<Enemy2>() != null)
                    {
                        enemy.GetComponent<Enemy2>().TakeDamage(attackDamageHeavy, damageMultiplier); //attackDamage + additional damage from parameter
                    }

                    if (enemy.GetComponent<EnemyBossBandit>() != null)
                        enemy.GetComponent<EnemyBossBandit>().TakeDamage(attackDamageHeavy, damageMultiplier);
                }
                break;
            case 2:
                Collider2D[] hitEnemiesWide = Physics2D.OverlapBoxAll(heavyAttackPointWide.position, new Vector2(attackHeavyRange*2.7f, .8f), enemyLayers);
                foreach (Collider2D enemy in hitEnemiesWide) //loop through enemies hit
                {
                    if (enemy.GetComponent<EnemyController>() != null)
                    {
                        if(enableScreenshake)
                            screenShake.Shake();

                        timeManager.DoFreezeTime(.15f, .05f);
                        //TODO: move common enemy scripting to EnemyController, instead of calling individual TakeDamage scripts
                    }

                    if (enemy.GetComponent<Enemy>() != null)
                    {
                        enemy.GetComponent<Enemy>().TakeDamage(attackDamageHeavy, damageMultiplier); //attackDamage + additional damage from parameter
                        enemy.GetComponent<Enemy>().GetKnockback(controller.m_FacingRight);
                    }

                    if (enemy.GetComponent<StationaryEnemy>() != null)
                        enemy.GetComponent<StationaryEnemy>().TakeDamage(attackDamageHeavy, damageMultiplier);

                    if (enemy.GetComponent<Enemy2>() != null)
                    {
                        enemy.GetComponent<Enemy2>().TakeDamage(attackDamageHeavy, damageMultiplier); //attackDamage + additional damage from parameter
                    }

                    if (enemy.GetComponent<EnemyBossBandit>() != null)
                        enemy.GetComponent<EnemyBossBandit>().TakeDamage(attackDamageHeavy, damageMultiplier);
                }
                break;
            default:
                break;
        }
    }
    #endregion

    IEnumerator StartAltAttack() // ! not being used
    {
        if (movement.isGrounded) //should let player attack mid air without stopping movement
            movement.canMove = false;

        animator.SetBool("isAttacking", true);
        AltAttack(wepDamage*3f, wepRange*3f); //deals 300% weapon damage and applies knockback to enemies
        movement.rb.velocity = new Vector2(0, 0); //stop player from moving
        AltAttacking = true;

        allowAltAttack = Time.time + altAttackCD;
        yield return new WaitForSeconds(altAttackTime);
        movement.canMove = true;
        animator.SetBool("isAttacking", false);
    }

    void AltAttack(float altDamage, float altRange) // ! not being used, calling ShieldBash in TriggerCheck script
    {
        //range increase to around 15f-20f
        //hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackHeavyRange, enemyLayers);

        Vector3 newAttackPoint = attackPoint.position;
        Collider2D[] altHitEnemies;

        if (controller.m_FacingRight)
        {
            newAttackPoint.x += (wepRange * 3) / 2;
            altHitEnemies = Physics2D.OverlapBoxAll(newAttackPoint, new Vector3(wepRange * 3, 1, 0), 180, enemyLayers);
        }
        else
        {
            newAttackPoint.x += -(wepRange * 3) / 2;
            altHitEnemies = Physics2D.OverlapBoxAll(newAttackPoint, new Vector3(wepRange * 3, 1, 0), 0, enemyLayers);
        }
        
        Vector2 altRangeSize = new Vector2(2, altRange);
        //Collider2D[] altHitEnemies = Physics2D.OverlapBoxAll(newAttackPoint, new Vector3(wepRange * 3, 1, 0), 180, enemyLayers);

        //damage enemies
        Vector3 changeLocation = playerLocation.position; //for movement with attack use
        if (controller.m_FacingRight)
        {
            //push backwards to left
            movement.rb.AddForce(Vector2.left * 30f);
        }
        else
        {
            movement.rb.AddForce(Vector2.right * 30f);
        }
        foreach (Collider2D enemy in altHitEnemies) //loop through enemies hit
        {
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(altDamage, 1.0f); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy>().GetKnockback(controller.m_FacingRight);
                enemy.GetComponent<Enemy>().GetStunned(stunStrength);
            }

            if (enemy.GetComponent<StationaryEnemy>() != null)
                enemy.GetComponent<StationaryEnemy>().TakeDamage(altDamage);

            if(enemy.GetComponent<Enemy2>() != null)
            {
                enemy.GetComponent<Enemy2>().TakeDamage(altDamage);
                enemy.GetComponent<Enemy2>().GetStunned(stunStrength);
            }

            if (enemy.GetComponent<EnemyBossBandit>() != null)
            {
                enemy.GetComponent<EnemyBossBandit>().CheckParry();
            }
        }
    }

    IEnumerator Parry() // stop player movement during animation
    {
        if (movement.isGrounded)
            movement.canMove = false;

        animator.SetBool("isAttacking", true);
        ParryAttack(); // hit enemies and check if they can be parried
        movement.rb.velocity = new Vector2(0, 0);
        AltAttacking = true;

        allowAltAttack = Time.time + altAttackCD;
        abilityUI.StartCooldown(altAttackCD);
        yield return new WaitForSeconds(altAttackTime); // parry attack time
        movement.canMove = true;
        animator.SetBool("isAttacking", false);
    }
    
    void ParryAttack() //not currently in-use
    {
        Vector3 parryAttackPoint = parryPoint.position;
        Collider2D[] parriedEnemies = Physics2D.OverlapCircleAll(parryAttackPoint, .3f);

        foreach (Collider2D enemy in parriedEnemies) // loop through enemies hit
        {
            if (enemy.GetComponent<Enemy>() != null)
            {
                /*if (enableScreenshake)
                    screenShake.Shake();*/

                enemy.GetComponent<Enemy>().GetStunned(1);

                //movement.CancelDash();
                animator.SetTrigger("Block");
                //screenShake.startShake();
            }

            if (enemy.GetComponent<EnemyBossBandit>() != null)
            {
                enemy.GetComponent<EnemyBossBandit>().CheckParry();
                //movement.CancelDash();
                //animator.SetTrigger("Block");
            }
        }
    }

    #region ShieldBash
    void ShieldBashInput() 
    {
        if (Time.time > allowAltAttack && movement.canMove)
        {
            if (Input.GetButtonDown("Fire3") && canAttack && !IsShieldBashing)
            {
                ShieldBash();
            }
        }
    }

    void ShieldBash()
    {
        StartCoroutine(ShieldBashStart());
        allowAltAttack = Time.time + altAttackCD; //get cooldown time for ShieldBash
        abilityUI.StartCooldown(altAttackCD);
    }

    IEnumerator ShieldBashStart()
    {
        IsShieldBashing = true; //starting to block damage
        canStunPlayer = false;
        movement.DisableMove();
        animator.SetTrigger("StartBlock");
        yield return new WaitForSeconds(.2f);
        shieldBashCollider.enabled = true; //
        shieldBashTrigger.enabled = true;
        movement.Dash(); //start Dash in movement script
    }

    public void OnSuccessfulBash() //called from CollisionCheck //also called at end of Dash()
    {
        //disable collider on hit
        if (enableScreenshake)
            screenShake.Shake(2);

        timeManager.DoFreezeTime(.1f, .05f);

        StartCoroutine(ShieldBashEnd());
    }

    public void EndShieldBash() //calling if Bash doesn't connect
    {
        StartCoroutine(ShieldBashEnd());
    }

    IEnumerator ShieldBashEnd()
    {
        movement.DisableMove();
        animator.SetTrigger("Block");
        shieldBashCollider.enabled = false;
        shieldBashTrigger.enabled = false;
        IsShieldBashing = false; //stop blocking damage

        yield return new WaitForSeconds(.2f);
        canStunPlayer = true;
        //Instantiate
        movement.EnableMove();
    }    
    #endregion

    void DodgeAttackCancel()
    {
        if (IsLightAttackingCO != null) //allow dodge to cancel attack
        {
            if (movement.m_rolling) //!!! dodge roll and dash can't be started while attacking because attacking sets canMove to false
                StopCoroutine(IsLightAttackingCO); //replace canMove as a condition for dodge/dash for this to work

            if (movement.isDashing)
                StopCoroutine(IsLightAttackingCO);

            /*if (Input.GetButtonDown("Dodge") && animator.GetBool("isAttacking")){
                StopCoroutine(IsAttackingCO);
                Debug.Log("Stopping Attack CO");
            }*/
        }

        //if(IsHeavyAttackingCO != null)
    }

    void LungeOnAttack(float lungeThrust = 3f, float lungeDuration = 5f, bool lunge = true) //defaults, set "lunge" to false for knockback (recoil)
    {
        //lungeThrust - velocity of lunge movement
        //lungeDuration - how long to maintain thrust velocity

        //float distToPlayer = transform.position.x - transform.position.x; //getting player direction to enemy

        Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset

        if (lunge)
        {
            if (controller.m_FacingRight) //lunge towards facing direction
            {
                tempOffset.x += lungeDuration; //lunge to right
            }
            else //to left of player
            {
                tempOffset.x -= lungeDuration; //lunge to left
            }
        }
        else
        {
            if (controller.m_FacingRight) //knockback away from facing direction
            {
                tempOffset.x -= lungeDuration; //knockback to left
            }
            else //to left of player
            {
                tempOffset.x += lungeDuration; //knockback to right
            }
        }
        Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, lungeThrust * Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }

    private void OnDrawGizmos() //OnDrawGizmosSelected
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(heavyAttackPoint.position, attackHeavyRange);
        //Gizmos.DrawWireSphere(parryPoint.position, .3f);

        //player position and c center of wire cube
        // [    player c           ] //vs  player[          c           ]  //should probably just use a raycast
        //newAttackPoint.x += -(wepRange * 3) / 2; //divide in half so that player isn't at center of rectangle
        //Gizmos.DrawWireCube(newAttackPoint, new Vector3(wepRange * 3, 1, 0));

        Vector3 newAttackPoint = heavyAttackPointWide.position;
        Gizmos.DrawWireCube(newAttackPoint, new Vector3(attackHeavyRange*2.7f, .8f, 0)); //* 2.7f, Collider[] BoxOverlap is 2.3f
    }
     
    public void GetKnockback(bool pushToRight, float kbThrust = 3f, float kbDuration = 5f) //defaults //short stun on knockback
    {
        //lungeThrust - velocity of lunge movement
        //lungeDuration - how long to maintain thrust velocity

        if (!animator.GetBool("isRolling") && isAlive) //player is immune to stun while rolling
        {
            Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset
            if (pushToRight) //knockback to right
            {
                tempOffset.x += kbDuration;
                Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
                transform.position = smoothPosition;
            }
            else //knockback to left
            {
                tempOffset.x -= kbDuration;
                Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
                transform.position = smoothPosition;
            }
            StunPlayer(.8f); //stunDuration //.8f for stun lock on 3rd attack
        }
    }

    public void StunPlayer(float stunDuration = 1f)
    {
        if (Time.time > allowStun && canStunPlayer && !playerStunned)
        {
            if(IsLightAttackingCO != null)
                StopCoroutine(IsLightAttackingCO); // Stop Attacking coroutines

            if(IsHeavyAttackingCO != null)
                StopCoroutine(IsHeavyAttackingCO);

            if (!playerStunned) // If player is already stunned, don't stun again
            {
                PlayerStunnedCO = StartCoroutine(Stun(stunDuration));
            }
        }
    }

    IEnumerator Stun(float stunDuration, bool root = true) // root: player's velocity is set to 0
    {
        playerStunned = true;
        canAttack = false;
        movement.canMove = false;
        animator.SetBool("move", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("Stunned", true);
        ShowStatusStun(true);
        //FlashMaterial();
        if (root)
            movement.rb.velocity = new Vector2(0, movement.rb.velocity.y); //only rooting player x velocity

        yield return new WaitForSeconds(stunDuration);

        allowStun = Time.time + allowStunCD;
        playerStunned = false;
        canAttack = true;
        animator.SetBool("Stunned", false);
        movement.canMove = true;
        ResetMaterial();
        ShowStatusStun(false);
    }

    void ShowStatusStun(bool setActive)
    {
        if (StatusStunned != null)
        {
            StatusStunned.SetActive(setActive);
        }
    }

    public void TakeDamage(float damage, bool unBlockable = false)
    {
        if (isAlive)
        {
            if (currentHealth > 0) {
                if(animator.GetBool("isRolling")) //damage dodged
                {
                    damage = 0;
                    //xpPopups.ShowDodge(transform.position); //xpPopups uses a static animation instead of the number animation
                }
                if (!unBlockable)
                {
                    if (IsShieldBashing)
                    {
                        damage = 0;
                        xpPopups.ShowText(transform.position, "Blocked");
                    }
                }
                currentHealth -= (damage);
                healthBar.SetHealth(currentHealth);
                if(damage > 0)
                {
                    Vector3 tempPos = transform.position;
                    tempPos += TPOffset; //damage pop up
                    TextPopupsHandler.ShowDamage(damage, tempPos);
                    
                    if(animator.GetBool("isAttacking") == false)
                        animator.SetTrigger("Hurt");
                    
                    FlashMaterial();
                    //GetKnockback(true); //
                    Invoke("ResetMaterial", .1f);
                }
            }
            //hurt animation
            if (currentHealth <= 0){
                Die();
            }
        }

    }

    void FlashMaterial()
    {
        sr.material = mWhiteFlash; //change sprite to white material
    }

    public void ResetMaterial()
    {
        sr.material = mDefault;
    }

    public void GiveXP(float xp)
    {
        experienceBar.AddXP(xp);
        timeManager.DoFreezeTime(.05f); //short freeze on kill
        xpPopups.ShowText(transform.position, "+" + xp + "xp");
        healthPotion.GetChargeFromKill(xp);
    }

    public void HealPlayer(float healAmount)
    {
        if (isAlive && currentHealth > 0)
        {
            currentHealth += healAmount;
            healthBar.SetHealth(currentHealth);
            if (TextPopupsHandler)
            {
                Vector3 tempPos = transform.position;
                tempPos += TPOffset;
                /*Vector3 tempPos1 = PlayerHealthBar.position;
                tempPos1 += new Vector3(0, -.5f, 0);*/
                TextPopupsHandler.ShowHeal(healAmount, tempPos);
            }
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth; //don't overheal
            }
        }
    }

    IEnumerator DelayRespawn()
    {
        //short delay before respawn input is allowed
        yield return new WaitForSeconds(1.0f);
        if (respawnPrompt != null)
        {
            respawnPrompt.GetComponent<TextMeshProUGUI>().enabled = true;
        }
        canRespawn = true;
    }

    void Die()
    {
        StopAllCoroutines(); //
        movement.StopCO(); // Stop coroutines in movement script

        if (playerStunned && PlayerStunnedCO != null)
        {
            playerStunned = false;
            animator.SetBool("Stunned", false);
            ResetMaterial();
            ShowStatusStun(false);
        }

        //Die animation
        isAlive = false;
        animator.SetBool("noBlood", m_noBlood);
        animator.SetTrigger("Death");
        movement.rb.velocity = new Vector2(0, 0); //prevent player from moving
        movement.DisableMove();
        canAttack = false;
        timeManager.DoFreezeTime(.05f, .5f);
        timeManager.DoSlowMotion();
        //kill player

        StartCoroutine(DelayRespawn());
    }

    void RevivePlayer(float spawnHpPercentage) //no use right now
    {
        isAlive = true;
        movement.EnableMove();
        canAttack = true;
        currentHealth = (spawnHpPercentage * maxHealth); //spawnHpPercentage 1.0 = 100%
        if (isAlive && currentHealth > 0)
        {
            healthBar.SetHealth(currentHealth);
            if (TextPopupsHandler)
            {
                TextPopupsHandler.ShowHeal(spawnHpPercentage, transform.position); //respawn player with x percentage of maxHealth
            }
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth; //can't overheal, can implement an overheal/shield later
            }
        }
    }
}
