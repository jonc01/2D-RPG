using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy2 : MonoBehaviour
{
    public GameObject TextPopupsPrefab;
    [SerializeField]
    private Transform TextPopupOffset;
    private GameObject tempShowDmg; //to flip damage popup as they are created
    
    public LayerMask playerLayers;
    public Transform player;
    public PlayerCombat playerCombat;
    //public GameObject hitPrefabToRight;
    //public GameObject hitPrefabToLeft;
    public GameObject hitParticlePrefab;
    public GameObject deathParticlePrefab;
    public GameObject stunLParticlePrefab;
    public GameObject stunRParticlePrefab;

    public float maxHealth = 100;
    float currentHealth;
    public HealthBar healthBar;

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
    public float enAttackDamage = 5f;
    public float enAttackSpeed = 1.1f; //lower value for lower delays between attacks
    public float enAttackAnimSpeed = .4f; //lower value for shorter animations
    [Range(0f, 1.0f)]
    public float stunResist = .5f; //0f takes full stun duration, 1.0f complete stun resist
    public float allowStun = 0f;
    public float allowStunCD = 5f; //how often enemy can be stunned

    [Space] //knockback
    public float kbThrust = 3.0f;
    public float kbDuration = 2.0f;

    [SerializeField]
    bool enCanAttack = true, isAttacking; //for parry()
    [SerializeField]
    bool playerToRight, aggroStarted;
    bool enIsHurt;
    bool enStunned;

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

    }

    void Update()
    {
        if (rb != null)
        {
            if (rb.velocity.x == 0)
            {
                enAnimator.SetBool("move", false); //check to make sure enemy isn't playing run anim while 
            }
            else
            {
                enAnimator.SetBool("move", true);
            }
        }

        if (rb != null && enController != null && isAlive && playerCombat.isAlive && !enStunned) //check if object has rigidbody
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


    //AI aggro
    void StartChase()
    {
        enAnimator.SetBool("inCombat", false);
        //enController.enCanMove = true;
        //enAnimator.SetBool("move", true);

        if (enController.enCanMove)
        {
            if (transform.position.x < player.position.x) //player is right
            {
                playerToRight = true;
                //player is to right, move right

                rb.velocity = new Vector2(enController.moveSpeed, 0); //moves at moveSpeed


                //Facing right, flip sprite to face right
                enController.enFacingRight = true;
                enController.Flip();
                if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange)
                {
                    //Attack();
                    //enAnimator.SetTrigger("Attack");
                    StartCoroutine(IsAttacking());
                    isAttacking = false;
                }
                //if(enCanMove)
                //enController.Flip();
            }
            else if (transform.position.x > player.position.x) //player is left
            {
                playerToRight = false;
                //player is to left, move left

                rb.velocity = new Vector2(-enController.moveSpeed, 0);


                enController.enFacingRight = false;
                //if (enCanMove)
                enController.Flip();
                if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange)
                {
                    //Attack();
                    StartCoroutine(IsAttacking());
                    isAttacking = false;
                }
                //if(enCanMove)
                // enController.Flip();
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
        rb.velocity = new Vector2(0, 0);
        enAnimator.SetBool("move", false);
        //enAnimator.SetBool("inCombat", true);
        enController.enCanMove = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //
    }

    void Attack()
    {
        Vector3 changeLocation = transform.position;
        if (enController.enFacingRight)
        {
            //go right
            changeLocation.x += .3f;
            transform.position = changeLocation;
            //movement.rb.AddForce(Vector2.right * 10f);
        }
        else
        {
            //go left
            changeLocation.x -= .3f;
            transform.position = changeLocation;
            //movement.rb.AddForce(Vector2.left * 10f);
        }

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, playerLayers);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            //Debug.Log("Enemy Hit " + player.name);
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage); //attackDamage + additional damage from parameter
        }
    }

    IEnumerator IsAttacking()
    {//TODO: combine redundant variables
        if (enCanAttack && !isAttacking)
        {
            enStunned = false;
            isAttacking = true;

            enAnimator.SetTrigger("Attack");

            enAnimator.SetBool("inCombat", true);
            enAnimator.SetBool("isAttacking", true);
            enAnimator.SetBool("move", false);

            enCanAttack = false;
            enController.enCanMove = false;
            rb.velocity = new Vector2(0, 0);

            yield return new WaitForSeconds(enAttackAnimSpeed); //time when damage is dealt based on animation

            if (enStunned)
            {
                isAttacking = false; //prevent enemy from getting stuck on "isAttacking" since it is never set to false
                yield break;
            }

            rb.velocity = new Vector2(0, 0); //stop enemy from moving
            Attack();
            yield return new WaitForSeconds(enAttackSpeed); //delay between attacks
            enAnimator.SetBool("isAttacking", false);
        }
        enController.enCanMove = true;
        enCanAttack = true;
    }


    private void OnDrawGizmosSelected()
    {
        if (enAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRange);
    }

    public void TakeDamage(float damage)
    {
        /*if (TextPopupsPrefab)
        {
            ShowTextPopup(damage);
        }*/

        if (isAlive == true)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;


            //temp knockback
            Vector3 tempLocation = GetComponent<Transform>().position;
            tempLocation.y += .5f;
            //

            Instantiate(hitParticlePrefab, tempLocation, Quaternion.identity);
            //show damage/heal numbers
            if (TextPopupsPrefab)
            {
                ShowTextPopup(damage);
            }


            //hurt animation
            if (enAnimator != null && damage > 0) //took damage, not heal
            {
                //stopping coroutine
                //attackStopped = true;

                enIsHurt = true;
                enAnimator.SetTrigger("Hurt");
                enCanAttack = true;
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

    void ResetMaterial()
    {
        sr.material = mDefault;
    }

    public void GetKnockback(float knockbackAmount)
    {
        if (rb != null)
        {
            Vector3 changeLocation = GetComponent<Transform>().position;
            Vector3 tempLocation = changeLocation; //for particle instantiate
            tempLocation.y += .5f;

            float distToPlayer = transform.position.x - player.transform.position.x; //getting player direction to enemy //if 0 will use last direction

            Vector3 tempOffset = gameObject.transform.position; //can implement knockup with y offset
            //tempOffset2.x += knockbackDist;

            if (distToPlayer > 0) //to right of player
            {
                //knockback to left
                tempOffset.x += kbDuration;
                Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
                transform.position = smoothPosition;
            }
            else //to left of player
            {
                //knockback to right
                tempOffset.x -= kbDuration;
                Vector3 smoothPosition = Vector3.Lerp(transform.position, tempOffset, kbThrust * Time.fixedDeltaTime);
                transform.position = smoothPosition;
            }

            Instantiate(hitParticlePrefab, tempLocation, Quaternion.identity);
        }
    }

    void ShowTextPopup(float damageAmount)
    {
        Vector3 tempTransform = transform.position; //randomize damage number position
        tempTransform.x += Random.Range(-.1f, .1f);
        tempTransform.y += Random.Range(-.9f, .1f);


        var showDmg = Instantiate(TextPopupsPrefab, TextPopupOffset.position, Quaternion.identity, TextPopupOffset);
        showDmg.GetComponent<TextMeshPro>().text = Mathf.Abs(damageAmount).ToString();
        tempShowDmg = showDmg;
        if (damageAmount < 0)
            showDmg.GetComponent<TextMeshPro>().color = new Color32(35, 220, 0, 255);
        /*if (enController.enFacingRight) //player facing right by default
            showDmg.transform.Rotate(0f, 0f, 0f);*/



        if (enController.enFacingRight)
        {
            FlipTextAgain(180);
        }
        else
        {
            FlipTextAgain(0);
        }

    }
    public void FlipTextAgain(float rotateAgain) //gets called in PlayerMovement to flip with player
    {
        tempShowDmg.GetComponent<TextPopups>().FlipText(rotateAgain);
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
        if (Time.time > allowStun && !enStunned) //cooldown timer starts when recovered from stun
        {
            float fullDuration = 1f;
            fullDuration -= stunResist; //getting percentage of stun based on stunResist
            duration *= fullDuration;
            enAnimator.SetTrigger("en2Stunned");
            StartCoroutine(StunEnemy(duration));
        }
    }

    IEnumerator StunEnemy(float stunDuration)
    {
        if (!enStunned)
        {
            enStunned = true;
            StopChase();
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
                var showStunned = Instantiate(TextPopupsPrefab, transform.position, Quaternion.identity, transform);
                showStunned.GetComponent<TextMeshPro>().text = "\n*Stun*"; //temp fix to offset not working (anchors)
            }

            yield return new WaitForSeconds(stunDuration);

            enAnimator.SetTrigger("en2StunRecover");
            yield return new WaitForSeconds(.5f); //time for recover animation
            enCanAttack = true;
            enController.enCanMove = true;
            enController.EnEnableFlip(); //precaution in case enemy is stunned during attack and can't flip
            enStunned = false;
            allowStun = Time.time + allowStunCD;
        }
    }

    public void GiveExperience(int experiencePoints)
    {
        Debug.Log("Give player " + experiencePoints + " XP");
        //give xp
        //
    }

    void Die()
    {
        //Die animation
        if (enAnimator != null)
        {
            enAnimator.SetTrigger("Death");
        }

        //give player exp
        GiveExperience(experiencePoints);

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

    /*private void DeleteEnemyObject()
    {
        Destroy(this.gameObject);
    }*/
}
