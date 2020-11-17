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
    public GameObject TextPopupsPrefab;
    public Transform playerLocation;

    //textPopups for referencce in PlayerMovement
    public GameObject tempShowDmg;

    public float maxHealth = 100;
    public float currentHealth;
    public HealthBar healthBar;
    public HealthBar experienceBar;
    public bool isAlive = true;

    //weapon stats
    public float wepDamage = 10f; //default values, should change based on weapon stats
    public float wepRange = .5f; //

    public Transform attackPoint;
    float attackRange;
    float attackHeavyRange = 0.58f;
    float attackDamageLight;
    float attackDamageHeavy;
    float attackDamageHeavyMultiplier;
    public bool canAttack = true;
    public float stunStrength = 1f;

    public float attackTime = 0.25f; //0.25 seems good, give or take .1 seconds
    //bool canMove = true;

    //Block check
    //private const float minBlockDuration = 0.25f;
    //private float currentBlockDuration = 0f;
    //private bool blockIsHeld = false;

    public LayerMask enemyLayers;
    public LayerMask chestLayers;

    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;

    //ability cooldown
    //public float dodgeCD = 1;
    public float altAttackCD = 3f;
    bool AltAttacking;
    private float allowAltAttack = 0;
    public float altAttackTime = .3f;

    //weapon specific
    public float knockback = 50f;

    void Start()
    {
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
    }
    // Update is called once per frame
    void Update()
    {

        m_timeSinceAttack += Time.deltaTime;
        //Attack Animations //&& (blockIsHeld == false) 
        if (Input.GetButtonDown("Fire1") && m_timeSinceAttack > 0.25f && canAttack)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            animator.SetTrigger("Attack" + m_currentAttack);

            if (m_currentAttack == 3) {
                AttackHeavy(); //can add parameter to Attack(10), for additional 10 damage on top of player damage
                StartCoroutine(IsAttacking());
            }
            else
            {
                //movement.canMove = false;
                Attack();
                StartCoroutine(IsAttacking());
                //Attack();
            }
                // Reset timer
                m_timeSinceAttack = 0.0f;
                //
        }

        //Block/Alt attack
        if (Time.time > allowAltAttack && movement.canMove)
        {
            if (Input.GetButtonDown("Fire2") && canAttack && !AltAttacking)
            {
                /*private const float minBlockDuration = 0.25f;
                private float currentBlockDuration = 0f;
                */
                //currentBlockDuration = Time.timeSinceLevelLoad;
                //blockIsHeld = true;

                /*m_rolling = true;
                animator.SetTrigger("Roll");
                rb.velocity = new Vector2(m_facingDirection * m_rollForce, rb.velocity.y);
                allowDodge = Time.time + dodgeCD;*/
                animator.SetTrigger("Block");
                StartCoroutine(AltAttack());
                movement.canMove = false;
                //crouch = true; can set later, or just disable hitbox, not collider

                //AltAttacking = false;

                /*if (blockCounter < 100)
                {
                    animator.SetTrigger("Block");
                }
                else
                {
                    animator.ResetTrigger("Block");
                    animator.SetBool("IdleBlock", true);
                    Debug.Log("IdleBlock: " + animator.GetBool("IdleBlock"));

                }*/
                //blockCounter++;
                /*if (blockIsHeld == true)
                {
                    Debug.Log("HOLDING block");
                    movement.runSpeed = 0f;
                    animator.SetBool("IdleBlock", true);
                }*/
                /*else
                {
                    Debug.Log("NOT HOLDING block");
                    animator.ResetTrigger("Block");
                    animator.SetBool("IdleBlock", false);
                }*/
                //Blocking(true);
            }
            AltAttacking = false;
        }
        /*if (blockIsHeld == false)
        {
            //Debug.Log("NOT HOLDING block");
            movement.runSpeed = movement.defaultRunSpeed;
            animator.ResetTrigger("Block");
            animator.SetBool("IdleBlock", false);
        }*/
        //else if (Input.GetButtonUp("Fire2"))
        //{
        //blockIsHeld = false;
        //Blocking(false);

        //animator.SetBool("IdleBlock", false);
        //}


        /////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////
        //TODO: testing healing and death animations, delete after player respawn is added
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

    IEnumerator IsAttacking()
    {
        if(movement.isGrounded) //should let player attack mid air without stopping movement
            movement.canMove = false;

        animator.SetBool("isAttacking", true);
        //Attack();
        movement.rb.velocity = new Vector2(0, 0); //stop player from moving
        //yield return new WaitForSeconds(attackTime);
        yield return new WaitForSeconds(0.3f);
        movement.canMove = true;
        animator.SetBool("isAttacking", false);
        //movement.canMove = true;
        //movement.runSpeed = movement.defaultRunSpeed;
    }

    void Attack()
    {
        //Attack range, detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //movement.rb.AddForce(Vector2.right * 10f);
        //damage enemies
        foreach (Collider2D enemy in hitEnemies) //loop through enemies hit
        {
            //Debug.Log("We Hit " + enemy.name);
            if (enemy.GetComponent<Enemy>() != null) //TODO: better way to do this? have to manually update for each new enemy
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamageLight); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy>().GetKnockback(knockback/2);
            }

            if (enemy.GetComponent<StationaryEnemy>() != null)
                enemy.GetComponent<StationaryEnemy>().TakeDamage(attackDamageLight);


            if (enemy.GetComponent<Enemy2>() != null)
            {
                enemy.GetComponent<Enemy2>().TakeDamage(attackDamageLight); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy2>().GetKnockback(knockback/3);
            }
            
        }
    }

    void AttackHeavy()
    {
        //animator.SetTrigger("Attack3");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackHeavyRange, enemyLayers);
        //Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(attackPoint.position, attackHeavyRange, enemyLayers);
        
        Vector3 changeLocation = playerLocation.position;
        if (controller.m_FacingRight)
        {
            //go right
            changeLocation.x += .1f;
            playerLocation.position = changeLocation;
            //movement.rb.AddForce(Vector2.right * 10f);
        }
        else
        {
            //go left
            changeLocation.x -= .1f;
            playerLocation.position = changeLocation;
            //movement.rb.AddForce(Vector2.left * 10f);
        }
        foreach (Collider2D enemy in hitEnemies) //loop through enemies hit
        {
            //Debug.Log("We Hit " + enemy.name);
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamageHeavy); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy>().GetKnockback(knockback*2);
                enemy.GetComponent<Enemy>().GetStunned(stunStrength);
            }
            
            if (enemy.GetComponent<StationaryEnemy>() != null)
                enemy.GetComponent<StationaryEnemy>().TakeDamage(attackDamageHeavy);

            if (enemy.GetComponent<Enemy2>() != null)
            {
                enemy.GetComponent<Enemy2>().TakeDamage(attackDamageHeavy); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy2>().GetKnockback(knockback * 2);
                enemy.GetComponent<Enemy2>().GetStunned(stunStrength);
            }
        }
    }
    IEnumerator AltAttack()
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

        Vector3 newAttackPoint = attackPoint.position; //this could easily get out of hand if weapons have too much range
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

        /*if (altHitEnemies.Length > 1)
        {
            Debug.Log("altHitEnemies: " + altHitEnemies.Length);
        }
        else
        {
            Debug.Log("no enemies hit");
            Debug.Log("altHitEnemies: " + altHitEnemies.Length);
        }*/
        

        //might have to manually flip //attackPoint.position or -attackPoint.position
            //might not need to, it might just initiate Collider2D when we attack, and face in correct direction
        
        
        Vector2 altRangeSize = new Vector2(2, altRange);
        //Collider2D[] altHitEnemies = Physics2D.OverlapBoxAll(newAttackPoint, new Vector3(wepRange * 3, 1, 0), 180, enemyLayers);



        //Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, altRangeSize, 180, enemyLayers);
        //Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(attackPoint.position, attackHeavyRange, enemyLayers);
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
            //Debug.Log("We Hit " + enemy.name);
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(altDamage); //attackDamage + additional damage from parameter
                enemy.GetComponent<Enemy>().GetKnockback(knockback*4f);
                enemy.GetComponent<Enemy>().GetStunned(stunStrength);
            }

            if (enemy.GetComponent<StationaryEnemy>() != null)
                enemy.GetComponent<StationaryEnemy>().TakeDamage(altDamage);

            if(enemy.GetComponent<Enemy2>() != null)
            {
                enemy.GetComponent<Enemy2>().TakeDamage(altDamage);
                enemy.GetComponent<Enemy2>().GetStunned(stunStrength);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(attackPoint.position, attackHeavyRange);

        Vector3 newAttackPoint = attackPoint.position; //this could easily get out of hand if weapons have too much range

        //player position and c center of "cube"
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

    public void TakeDamage(float damage)
    {
        if (currentHealth > 0) {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            animator.SetTrigger("Hurt");
            if (TextPopupsPrefab) {
                ShowTextPopup(damage);
            }
        }

        //hurt animation
        if (currentHealth <= 0){
            Die();
        }
    }
    void ShowTextPopup(float damageAmount)
    {

        //Vector3 tempTransform = transform.position; //randomize damage number position
        Vector3 tempPos = transform.position;
        tempPos.x += Random.Range(-.1f, .1f);
        tempPos.y += Random.Range(-.9f, .1f);
        //tempTransform = screenPosition; //have numbers float in place, don't follow object

        

        var showDmg = Instantiate(TextPopupsPrefab, tempPos, Quaternion.identity, transform);
        showDmg.GetComponent<TextMeshPro>().text = damageAmount.ToString();
        tempShowDmg = showDmg;

        if (controller.m_FacingRight)
        {
            FlipTextAgain(0);
        }
        else
        {
            FlipTextAgain(180);
        }
    }

    public void FlipTextAgain(float rotateAgain) //gets called in PlayerMovement to flip with player
    {
        tempShowDmg.GetComponent<TextPopups>().FlipText(rotateAgain);
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
                ShowTextPopupHeal(healAmount);
            }
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth; //can't overheal, can implement an overheal/shield later
            }
        }

        //hurt animation
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void ShowTextPopupHeal(float healAmount)
    {
        Vector3 tempTransform = transform.position; //randomize damage number position
        tempTransform.x += Random.Range(-.1f, .1f);
        tempTransform.y += Random.Range(-.9f, .2f);

        var showHeal = Instantiate(TextPopupsPrefab, tempTransform, Quaternion.identity, transform);
        showHeal.GetComponent<TextMeshPro>().text = healAmount.ToString();
        showHeal.GetComponent<TextMeshPro>().color = new Color32(35, 220, 0, 255);
        tempShowDmg = showHeal;

        if (controller.m_FacingRight)
        {
            FlipTextAgain(0);
        }
        else
        {
            FlipTextAgain(180);
        }

    }

    /*void Blocking(bool isBlocking)
    {
        if(isBlocking == true)
        {
            Debug.Log("HOLDING");
            //movement.runSpeed = 0f;
            movement.rb.velocity = new Vector2(0, 0);
            animator.SetBool("IdleBlock", true);
        }
        else
        {
            //Debug.Log("NOT HOLDING");
            //movement.runSpeed = movement.defaultRunSpeed;
            animator.ResetTrigger("Block");
            animator.SetBool("IdleBlock", false);
        }
    }*/

    void Die()
    {
        Debug.Log("Player died.");
        //Die animation
        isAlive = false;
        animator.SetBool("noBlood", m_noBlood);
        animator.SetTrigger("Death");
        movement.rb.velocity = new Vector2(0, 0); //stop player from moving
        movement.canMove = false;
        canAttack = false;
        //disable player object
        //kill player
    }

    void RevivePlayer(float spawnHpPercentage) //no use right now, 
    {
        isAlive = true;
        movement.canMove = true;
        canAttack = true;
        currentHealth = (spawnHpPercentage * maxHealth);
        if (isAlive && currentHealth > 0)
        {
            healthBar.SetHealth(currentHealth);
            if (TextPopupsPrefab)
            {
                ShowTextPopupHeal(spawnHpPercentage);
            }
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth; //can't overheal, can implement an overheal/shield later
            }
        }
    }
}
