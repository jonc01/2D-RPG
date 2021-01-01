using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyBossBandit : MonoBehaviour
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


    public GameObject HealthBarCanvas;
    public float maxHealth = 1000;
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
    float aggroRange = 5f; //when to start chasing player
                           //might extend to aggro to player before enemy enters screen
    [SerializeField]
    float enAttackRange1 = .5f; //when to start attacking player, stop enemy from clipping into player
    public Transform enAttackPoint;
    float enAttackRange2 = .8f;
    public Transform enAttackPoint2;

    public EnemyController enController;
    [Space]
    public float enAttackDamage = 10f;
    public float enAttackSpeed = 2f; //lower value for lower delays between attacks
    public float enAttackAnimSpeed = .4f; //lower value for shorter animations
    [Range(0f, 1.0f)]
    public float stunResist = .5f; //0f takes full stun duration, 1.0f complete stun resist
    public float allowStun = 0f;
    public float allowStunCD = 5f; //how often enemy can be stunned

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

        //Stats
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        isAlive = true;
        //enController.enCanMove = true;
        enCanAttack = true;
        //AI aggro
        rb = GetComponent<Rigidbody2D>();
        enAnimator.SetBool("Move", false);
        //enController.enFacingRight = false; //start facing left (towards player start)
        isAttacking = false;
        aggroStarted = false;
        enIsHurt = false;
        enStunned = false;

    }

    void Update()
    {
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
        ShowHealthBar();

        enAnimator.SetBool("Move", true);

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
                if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange1)
                {
                    StartCoroutine(IsAttacking());
                    isAttacking = false;
                }
            }
            else if (transform.position.x > player.position.x) //player is left
            {
                playerToRight = false;
                //player is to left, move left

                rb.velocity = new Vector2(-enController.moveSpeed, 0);


                enController.enFacingRight = false;
                //if (enCanMove)
                enController.Flip();
                if (Mathf.Abs(transform.position.x - player.position.x) <= enAttackRange1)
                {
                    StartCoroutine(IsAttacking());
                    isAttacking = false;
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

    void Attack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange1, playerLayers);

        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage); //attackDamage + additional damage from parameter
        }
    }

    IEnumerator IsAttacking()
    {//TODO: combine redundant variables
        if (enCanAttack && !isAttacking)
        {
            //int atkSequence = Random.Range(1, 5);
            int atkSequence = Random.Range(3, 5); //TODO: DEBUGGING revert to above

            Debug.Log("atkSequence rand: " + atkSequence);

            enStunned = false;
            isAttacking = true;

            switch (atkSequence) 
            {
                case 1: //Attack1 //can be parried
                    enAnimator.SetTrigger("Attack1Slow"); //TODO: Alternate between two attacks

                    enAnimator.SetBool("isAttacking", true);
                    enAnimator.SetBool("Move", false);

                    enCanAttack = false;
                    enController.enCanMove = false;
                    rb.velocity = new Vector2(0, 0);

                    yield return new WaitForSeconds(0.7f); //time until damage is dealt based on animation

                    if (enStunned)
                    {
                        isAttacking = false; //prevent enemy from getting stuck on "isAttacking" since it is never set to false
                        yield break;
                    }

                    rb.velocity = new Vector2(0, 0); //stop enemy from moving
                    Attack();
                    yield return new WaitForSeconds(enAttackSpeed); //delay between attacks
                    enAnimator.SetBool("isAttacking", false);
                    break;
                case 2: //Attack2
                    enAnimator.SetTrigger("Attack2Slow");
                    enAnimator.SetBool("isAttacking", true);
                    enAnimator.SetBool("Move", false);
                    rb.velocity = new Vector2(0, 0);

                    yield return new WaitForSeconds(0.6f);
                    Attack();

                    yield return new WaitForSeconds(enAttackSpeed);
                    enAnimator.SetBool("isAttacking", false);
                    break;
                case 3: //Attack1 + Attack2
                    enAnimator.SetBool("isAttacking", true);
                    enAnimator.SetBool("Move", false);
                    rb.velocity = new Vector2(0, 0);

                    enAnimator.SetTrigger("Attack1SlowStartCombo");
                    yield return new WaitForSeconds(0.6f);
                    Attack();

                    enAnimator.SetTrigger("Attack2Slow");
                    yield return new WaitForSeconds(0.6f); //maybe faster start up variation for this combo
                    Attack();

                    yield return new WaitForSeconds(enAttackSpeed*2f);
                    enAnimator.SetBool("isAttacking", false);
                    break;
                case 4: //Attack1+2 x 3 //can not flip, no parry
                    //TODO: finish adding Attack1/2 Combo 1/2/3 

                    yield return new WaitForSeconds(enAttackSpeed * 2f); //long delay before attacking again since we have a long attack sequence
                    break;
                default:
                    yield return new WaitForSeconds(0.01f);
                    break;
            }
        }

        enController.enCanMove = true;
        enCanAttack = true;
    }


    private void OnDrawGizmosSelected()
    {
        if (enAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRange1);

        if (enAttackPoint2 == null)
            return;

        Gizmos.DrawWireSphere(enAttackPoint2.position, enAttackRange2);
    }

    public void TakeDamage(float damage)
    {
        if (isAlive == true)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;


            //temp knockback
            Vector3 tempLocation = GetComponent<Transform>().position;
            tempLocation.y -= .0f;
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

            if (playerToRight)
            {
                changeLocation.x -= .1f; //knockback
                //rb.AddForce(Vector2.left * knockbackAmount);
            }
            else
            {
                changeLocation.x += .1f;
                //rb.AddForce(Vector2.left * knockbackAmount);
            }

            GetComponent<Transform>().position = changeLocation;
            //StartCoroutine(StunEnemy(0f));
        }

        /*var showKnockback = Instantiate(TextPopupsPrefab, transform.position, Quaternion.identity, transform);
        showKnockback.GetComponent<TextMeshPro>().text = "?!";*/
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
            //enAnimator.SetTrigger("en2Stunned");
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
        //give player exp
        GiveExperience(experiencePoints);

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
        Destroy(this.gameObject);
    }
}
