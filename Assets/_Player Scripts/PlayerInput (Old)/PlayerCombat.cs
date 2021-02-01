using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    [SerializeField] bool m_noBlood = false;
    public PlayerMovement movement;
    public CharacterController2D controller;
    public Transform playerLocation;
    [SerializeField] Canvas PlayerHealthBarCanvas;
    [SerializeField] GameObject StatusStunned;

    [Space]
    //Text Popups
    public GameObject TextPopupsPrefab;
    public TextPopupsHandler TextPopupsHandler;
    [SerializeField] Vector3 TPOffset = new Vector3(0, 0f, 0);
    [SerializeField] Transform PlayerHealthBar;

    [Space]
    public float maxHealth = 100;
    public float currentHealth;
    public HealthBar healthBar;
    public HealthBar experienceBar;
    public bool isAlive = true;

    [Space]
    //weapon stats
    public float wepDamage = 10f; //default values, should change based on weapon stats
    public float wepRange = .5f; //
    public float playerAttackSpeed = .3f;

    public Transform attackPoint;
    float attackRange;
    float attackHeavyRange = 0.58f;
    float attackDamageLight;
    float attackDamageHeavy;
    float attackDamageHeavyMultiplier;
    public bool canAttack = true;
    public float stunStrength = 1f;
    [SerializeField] bool playerStunned;

    public float attackTime = 0.25f; //0.25 seems good, give or take .1 seconds
    //bool canMove = true;

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
    private int currentHeavyAttack = 0;
    private float timeSinceHeavyAttack = 0.0f;

    //ability cooldowns
    public float altAttackCD = 3f;
    bool AltAttacking;
    private float allowAltAttack = 0;
    public float altAttackTime = .3f;

    //weapon specific
    public float knockback = 5f;

    SpriteRenderer sr;
    [SerializeField] private Material mWhiteFlash;
    private Material mDefault;
    Coroutine IsLightAttackingCO;
    Coroutine IsHeavyAttackingCO;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mDefault = sr.material;

        //var keyboard = Keyboard.current; //temp workaround
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        isAlive = true;

        //weapons stats

        attackRange = wepRange;
        attackHeavyRange = wepRange*1.15f;

        //attackDamageHeavyMultiplier = weapon.wepDamageHeavyMultiplier;
        attackDamageHeavyMultiplier = 1.5f; //placeholder

        attackDamageLight = wepDamage;
        attackDamageHeavy = wepDamage*attackDamageHeavyMultiplier;

        
        //movement.canMove = true;
        canAttack = true;
        AltAttacking = false;
        playerStunned = false;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLightAttack += Time.deltaTime;
        timeSinceHeavyAttack += Time.deltaTime;

        CheckLightAttack();
        CheckHeavyAttack();

        CheckAltAttack(); //Parry

        //DodgeAttackCancel(); //not currently in-use, allows for either cancelling of attacks with a dodge input, or some alt Dodge attack

        /////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////
        //TODO: testing healing, delete after player respawn is added
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //RevivePlayer(1.0f); //1.0 = 100%, 0.5 = 50%
            controller.RespawnPlayerResetLevel();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            HealPlayer(25f); //how much health to heal
        }
        /////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////
    }

    void CheckLightAttack()
    {
        if (Input.GetButtonDown("Fire1") && timeSinceLightAttack > 0.25f && canAttack) //0.25f attack speed
        {
            currentLightAttack++;

            // Loop back to one after third attack
            if (currentLightAttack > 3)
                currentLightAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (timeSinceLightAttack > 1.0f)
                currentLightAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            animator.SetTrigger("Attack" + currentLightAttack);

            //Coroutine IsAttackingCO;
            IsLightAttackingCO = StartCoroutine(IsLightAttacking(currentLightAttack));

            // Reset timer
            timeSinceLightAttack = 0.0f;
        }
    }

    void CheckHeavyAttack()
    {
        if(Input.GetButtonDown("Fire2") && timeSinceHeavyAttack > 0.4f && canAttack)
        {
            currentHeavyAttack++;

            if (currentHeavyAttack > 3)
                currentHeavyAttack = 1;

            if (timeSinceHeavyAttack > 1.0f)
                currentHeavyAttack = 1;

            // Call one of three attack animations "Attack1Heavy", "Attack2Heavy", "Attack3Heavy"
            animator.SetTrigger("Attack" + currentHeavyAttack + "Heavy");

            IsHeavyAttackingCO = StartCoroutine(IsAttackingHeavy(currentHeavyAttack));

            /*if (currentLightAttack == 2) //if we use combos with mixed Light and Heavy attacks
            {
                Debug.Log("attempt combo");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("combo works");
                }
            }*/
            
            timeSinceHeavyAttack = 0.0f;
        }
    }

    void CheckAltAttack()
    {
        if (Time.time > allowAltAttack && movement.canMove)
        {
            if (Input.GetButtonDown("Fire3") && canAttack && !AltAttacking)
            {
                //currentBlockDuration = Time.timeSinceLevelLoad;
                animator.SetTrigger("Block");
                StartCoroutine(AltAttack());
                movement.canMove = false;
            }
            AltAttacking = false;
        }
    }

    IEnumerator IsLightAttacking(int attackNum)
    {
        if(movement.isGrounded) //should let player attack mid air without stopping movement
            movement.canMove = false;

        animator.SetBool("isAttacking", true);
        movement.rb.velocity = new Vector2(0, movement.rb.velocity.y); //maintaining y velocity, instead of making player float

        switch (attackNum)
        {
            case 1:
                yield return new WaitForSeconds(0.1f);
                Attack(); //Attack functions determine damage and attack hitbox
                //yield return
                break;
            case 2:
                yield return new WaitForSeconds(0.1f);
                Attack();
                //yield return
                break;
            case 3:
                yield return new WaitForSeconds(0.2f);
                AttackHeavy();
                //yield return
                break;
            default:
                yield return new WaitForSeconds(0.01f); //
                break;
        }

        //movement.rb.velocity = new Vector2(0, 0); //stop player from moving
        
        //yield return new WaitForSeconds(attackTime);
        yield return new WaitForSeconds(playerAttackSpeed);
        movement.canMove = true;
        
        animator.SetBool("isAttacking", false);
        //movement.canMove = true;
        //movement.runSpeed = movement.defaultRunSpeed;
    }

    IEnumerator IsAttackingHeavy(int attackNum)
    {
        if (movement.isGrounded) //should let player attack mid air without stopping movement
            movement.canMove = false;

        animator.SetBool("isAttacking", true);
        movement.rb.velocity = new Vector2(0, movement.rb.velocity.y); //maintaining y velocity, instead of making player float

        switch (attackNum)
        {
            case 1:
                yield return new WaitForSeconds(0.4f);
                AttackHeavy(); //Attack functions determine damage and attack hitbox
                yield return new WaitForSeconds(0.3f);
                break;
            case 2:
                yield return new WaitForSeconds(0.3f);
                AttackHeavy();
                yield return new WaitForSeconds(0.5f);
                break;
            case 3:
                yield return new WaitForSeconds(0.3f);
                AttackHeavy();
                yield return new WaitForSeconds(0.5f);
                break;
            default:
                yield return new WaitForSeconds(0.01f); //
                break;
        }

        yield return new WaitForSeconds(playerAttackSpeed);
        movement.canMove = true;

        animator.SetBool("isAttacking", false);
    }

    void Attack()
    {
        //Attack range, detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //damage enemies
        foreach (Collider2D enemy in hitEnemies) //loop through enemies hit
        {
            /*if(enemy.GetComponent<EnemyController>() != null) //after migrating below functions into EnemyController
            {
                enemy.GetComponent<EnemyController>().TakeDamage(attackDamageLight);
                enemy.GetComponent<EnemyController>().GetKnockback(knockback/2);
                enemy.GetComponent<EnemyController>().GetStunned(.3f);
            }*/

            if (enemy.GetComponent<Enemy>() != null) //TODO: ^ add TakeDamage, etc to EnemyController manually updating for each new enemy
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamageLight); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy>().GetKnockback(controller.m_FacingRight);
                //enemy.GetComponent<Enemy>().GetStunned(.3f, false);
            }

            if (enemy.GetComponent<StationaryEnemy>() != null)
                enemy.GetComponent<StationaryEnemy>().TakeDamage(attackDamageLight);

            if (enemy.GetComponent<Enemy2>() != null)
            {
                enemy.GetComponent<Enemy2>().TakeDamage(attackDamageLight); //attackDamage + additional damage from parameter
            }

            if (enemy.GetComponent<EnemyBossBandit>() != null)
                enemy.GetComponent<EnemyBossBandit>().TakeDamage(attackDamageLight);
        }
    }

    void AttackHeavy()
    {
        //animator.SetTrigger("Attack3");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackHeavyRange, enemyLayers);
        //Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(attackPoint.position, attackHeavyRange, enemyLayers);
        
        //LungeOnAttack();

        foreach (Collider2D enemy in hitEnemies) //loop through enemies hit
        {
            if(enemy.GetComponent<EnemyController>() != null)
            {
                //TODO: move common enemy scripting to EnemyController, instead of calling individual TakeDamage scripts
            }

            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamageHeavy); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy>().GetKnockback(controller.m_FacingRight);
                //enemy.GetComponent<Enemy>().GetStunned(stunStrength);
            }
            
            if (enemy.GetComponent<StationaryEnemy>() != null)
                enemy.GetComponent<StationaryEnemy>().TakeDamage(attackDamageHeavy);

            if (enemy.GetComponent<Enemy2>() != null)
            {
                enemy.GetComponent<Enemy2>().TakeDamage(attackDamageHeavy); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy2>().GetStunned(stunStrength*2);
            }

            if (enemy.GetComponent<EnemyBossBandit>() != null)
                enemy.GetComponent<EnemyBossBandit>().TakeDamage(attackDamageHeavy);
        }
    }
    IEnumerator AltAttack() //TODO: adjust hitbox closer to player
    {
        if (movement.isGrounded) //should let player attack mid air without stopping movement
            movement.canMove = false;

        animator.SetBool("isAttacking", true);
        _AltAttack(wepDamage*3f, wepRange*3f); //deals 300% weapon damage and applies knockback to enemies
        movement.rb.velocity = new Vector2(0, 0); //stop player from moving
        AltAttacking = true;

        allowAltAttack = Time.time + altAttackCD;
        yield return new WaitForSeconds(altAttackTime);
        movement.canMove = true;
        animator.SetBool("isAttacking", false);
    }

    void _AltAttack(float altDamage, float altRange)
    {
        //range increase to around 15f-20f
        //hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackHeavyRange, enemyLayers);

        Vector3 newAttackPoint = attackPoint.position;
        //newAttackPoint.x += (wepRange * 3) / 2;
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
                enemy.GetComponent<Enemy>().TakeDamage(altDamage); //attackDamage + additional damage from parameter
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
                enemy.GetComponent<EnemyBossBandit>().CheckParry();
        }
    }

    IEnumerator Parry()
    {
        yield return new WaitForSeconds(.1f);
        //GetEnemy ... TryParry(); //get do the actual check in GetParried
        yield return new WaitForSeconds(1.0f);
    }
    
    void ParryAttack()
    {
        //GetEnemy ... GetParried
    }

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

    void LungeOnAttack(float lungeThrust = 3f, float lungeDuration = 5f, bool lunge = true) //defaults, set "lunge" to false for knockback
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
        Gizmos.DrawWireSphere(attackPoint.position, attackHeavyRange);

        Vector3 newAttackPoint = attackPoint.position; //this could easily get out of hand if weapons have too much range

        //player position and c center of wire cube
        // [    player c           ] //vs  player[          c           ]  //should probably just use a raycast
        //newAttackPoint
        if (controller.m_FacingRight)
        {
            newAttackPoint.x += (wepRange * 3) / 2; //divide in half so that player isn't at center of rectangle
            Gizmos.DrawWireCube(newAttackPoint, new Vector3(wepRange * 3, 1, 0));
        }
        else
        {
            newAttackPoint.x += -(wepRange * 3) / 2; //divide in half so that player isn't at center of rectangle
            Gizmos.DrawWireCube(newAttackPoint, new Vector3(wepRange * 3, 1, 0));
        }
    }
     
    public void GetKnockback(bool pushToRight, float kbThrust = 3f, float kbDuration = 5f) //defaults
    {
        //lungeThrust - velocity of lunge movement
        //lungeDuration - how long to maintain thrust velocity

        if (!animator.GetBool("isRolling"))
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
            StunPlayer(1f);
        }
    }

    public void StunPlayer(float stunDuration)
    {
        if(!playerStunned)
            StartCoroutine(Stun(stunDuration));
    }

    IEnumerator Stun(float stunDuration, bool root = true) //root: player's velocity is set to 0
    {
        playerStunned = true;
        canAttack = false;
        movement.canMove = false;
        animator.SetBool("move", false);
        animator.SetTrigger("Stunned");
        ShowStatusStun(true);
        //FlashMaterial();
        if (root)
            movement.rb.velocity = new Vector2(0, movement.rb.velocity.y); //only rooting player x velocity

        yield return new WaitForSeconds(stunDuration);
        
        playerStunned = false;
        canAttack = true;
        movement.canMove = true;
        ShowStatusStun(false);
    }

    void ShowStatusStun(bool setActive)
    {
        if (StatusStunned != null)
        {
            //PlayerHealthBarCanvas.GetComponentInChildren<Canvas>().enabled = true;
            StatusStunned.SetActive(setActive);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isAlive)
        {
            if (currentHealth > 0) {
                if(animator.GetBool("isRolling")) //damage dodged
                {
                    damage = 0;
                    TextPopupsHandler.ShowDodge(transform.position);
                }
                currentHealth -= (damage);
                healthBar.SetHealth(currentHealth);
                if(damage > 0)
                {
                    Vector3 tempPos = transform.position;
                    tempPos += TPOffset;
                    TextPopupsHandler.ShowDamage(damage, tempPos);
                    //var showDmg = Instantiate(TextPopupsPrefab, PlayerHealthBar.position, Quaternion.identity, PlayerHealthBar.transform);
                    //showDmg.transform.SetParent(PlayerHealthBar.transform);
                    
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

    void ResetMaterial()
    {
        sr.material = mDefault;
    }

    public void HealPlayer(float healAmount)
    {
        if (isAlive && currentHealth > 0)
        {
            currentHealth += healAmount;
            healthBar.SetHealth(currentHealth);
            //animator.SetTrigger("Hurt");
            if (TextPopupsPrefab)
            {
                Vector3 tempPos = transform.position;
                tempPos += TPOffset;
                /*Vector3 tempPos1 = PlayerHealthBar.position;
                tempPos1 += new Vector3(0, -.5f, 0);*/
                TextPopupsHandler.ShowHeal(healAmount, tempPos);
            }
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth; //can't overheal, can implement an overheal/shield later
            }

            //hurt animation
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        //Die animation
        isAlive = false;
        animator.SetBool("noBlood", m_noBlood);
        animator.SetTrigger("Death");
        movement.rb.velocity = new Vector2(0, 0); //prevent player from moving
        movement.canMove = false;
        canAttack = false;
        //kill player
    }

    void RevivePlayer(float spawnHpPercentage) //no use right now
    {
        isAlive = true;
        movement.canMove = true;
        canAttack = true;
        currentHealth = (spawnHpPercentage * maxHealth); //spawnHpPercentage 1.0 = 100%
        if (isAlive && currentHealth > 0)
        {
            healthBar.SetHealth(currentHealth);
            if (TextPopupsPrefab)
            {
                TextPopupsHandler.ShowHeal(spawnHpPercentage, transform.position); //respawn player with x percentage of 
            }
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth; //can't overheal, can implement an overheal/shield later
            }
        }
    }
}
