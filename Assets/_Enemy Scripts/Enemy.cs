using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public GameObject TextPopupsPrefab;
    public Transform player;
    public LayerMask playerLayers;
    public PlayerCombat playerCombat;
    public GameObject hitPrefabToRight;
    public GameObject hitPrefabToLeft;
    public GameObject deathParticlePrefab;

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

    [SerializeField]
    //bool enCanMove = true;
    bool enCanAttack = true, isAttacking; //for parry()
    [SerializeField]
    bool playerToRight, aggroStarted;
    bool enIsHurt;
    bool enStunned;
    bool attackStopped;



    void Start()
    {
        //Stats
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
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
        attackStopped = false;
    }

    void Update()
    {
        //Debug.Log("canMove: " + enCanMove + ", canAttack: " + enCanAttack);
        if (rb != null && enController != null && isAlive && playerCombat.isAlive && !enStunned) //check if object has rigidbody
        {
            //checking distance to player for aggro range
            float distToPlayer = Vector2.Distance(transform.position, player.position);

            //range <= 3
            if(distToPlayer <= aggroRange && enController.enCanMove) //how to start aggro
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
            else if(aggroStarted && enController.enCanMove) //now that we have aggro
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
        enAnimator.SetBool("move", true);

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
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, playerLayers);
        
        //damage enemies
        foreach (Collider2D player in hitPlayer) //loop through enemies hit
        {
            //Debug.Log("Enemy Hit " + player.name);
            player.GetComponent<PlayerCombat>().TakeDamage(enAttackDamage); //attackDamage + additional damage from parameter
        }
    }

    IEnumerator IsAttacking()
    {//TODO: this is a mess, lots of variables that can be combined
        if (enCanAttack && !isAttacking)
        {
            attackStopped = false;
            isAttacking = true;
            
            enAnimator.SetTrigger("Attack");
            
            enAnimator.SetBool("inCombat", true);
            enAnimator.SetBool("isAttacking", true);
            enAnimator.SetBool("move", false);
            
            enCanAttack = false;
            enController.enCanMove = false;
            rb.velocity = new Vector2(0, 0);
            yield return new WaitForSeconds(enAttackAnimSpeed); //time when damage is dealt based on animation

            if (attackStopped)
            {
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
        if (isAlive == true)
        {

            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            //show damage/heal numbers
            if (TextPopupsPrefab)
            {
                ShowTextPopup(damage);
            }
            
            //hurt animation
            if (enAnimator != null && damage > 0) //took damage, not heal
            {
                //stopping coroutine
                attackStopped = true;
                
                enIsHurt = true;
                enAnimator.SetTrigger("Hurt");
                StartCoroutine(StunEnemy(1f));
                enCanAttack = true;
                enAnimator.SetBool("isAttacking", false);
                //attackStopped = false;
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void GetKnockback(float knockbackAmount)
    {
        if (rb != null) {
            Vector3 changeLocation = GetComponent<Transform>().position;

            Vector3 tempLocation = changeLocation;
            tempLocation.y += .5f;
            
            //testing
            if(enController.enFacingRight && hitPrefabToRight != null)
            {
                Debug.Log("enFacingRight = true" + enController.enFacingRight);
                Instantiate(hitPrefabToLeft, tempLocation, Quaternion.identity);
            }
            else if (!enController.enFacingRight && hitPrefabToLeft != null)
            {
                Debug.Log("enFacingRight = false" + enController.enFacingRight);
                Instantiate(hitPrefabToRight, tempLocation, Quaternion.identity);
            }
            else
            {
                Debug.Log("we cry");
            }


            if (playerToRight) //player to the right, knockback left
            {
                if (!enController.enFacingRight && hitPrefabToLeft != null) //back facing player
                {
                    Instantiate(hitPrefabToLeft, tempLocation, Quaternion.identity);
                    Debug.Log("particles to the left");
                }
                //rb.AddForce(Vector2.left * knockbackAmount);
                changeLocation.x -= .1f;
            }
            else //player to the left
            {
                if (enController.enFacingRight && hitPrefabToRight != null) //back facing player
                {
                    Instantiate(hitPrefabToRight, tempLocation, Quaternion.identity);
                    Debug.Log("particles to the right");
                }
                changeLocation.x += .1f;
                //rb.AddForce(Vector2.right * knockbackAmount);
            }
            GetComponent<Transform>().position = changeLocation;
            StartCoroutine(StunEnemy(.3f));
        }

        /*var showKnockback = Instantiate(TextPopupsPrefab, transform.position, Quaternion.identity, transform);
        showKnockback.GetComponent<TextMeshPro>().text = "?!";*/
    }

    void ShowTextPopup(float damageAmount)
    {
        Vector3 tempTransform = transform.position; //randomize damage number position
        tempTransform.x += Random.Range(-.1f, .1f);
        tempTransform.y += Random.Range(-.9f, .1f);


        var showDmg = Instantiate(TextPopupsPrefab, tempTransform, Quaternion.identity, transform);
        showDmg.GetComponent<TextMeshPro>().text = Mathf.Abs(damageAmount).ToString();
        if(damageAmount < 0)
            showDmg.GetComponent<TextMeshPro>().color = new Color32(35, 220, 0, 255);
        /*if (enController.enFacingRight) //player facing right by default
            showDmg.transform.Rotate(0f, 0f, 0f);*/
    }

    public void EnIsHurtStart()
    {
        enIsHurt = true;
    }

    public void EnIsHurtEnd()
    {
        enIsHurt = false;
    }

    IEnumerator StunEnemy(float stunDuration)
    {
        if (!enStunned)
        {
            enStunned = true;
            StopChase();
            enCanAttack = false;
            enController.enCanMove = false;

            //var showDmg = Instantiate(TextPopupsPrefab, transform.position, Quaternion.identity, transform);
            //showDmg.GetComponent<TextMeshPro>().text = "?"; 

            yield return new WaitForSeconds(stunDuration);

            enCanAttack = true;
            enController.enCanMove = true;
            enStunned = false;
        }
    }

    public void GiveExperience(int experiencePoints){
        Debug.Log("Give player " + experiencePoints + " XP");
        //give xp
        //
    }

    void Die()
    {
        //Die animation
        if(enAnimator != null)
        {
            enAnimator.SetTrigger("Death");
        }

        //give player exp
        GiveExperience(experiencePoints);

        //hide hp bar


        //disable enemy object
        isAlive = false;

        if(deathParticlePrefab != null)
        {
            Vector3 changeLocation = GetComponent<Transform>().position;
            Vector3 tempLocation = changeLocation;
            tempLocation.y += .5f;
            Instantiate(deathParticlePrefab, tempLocation, Quaternion.identity);
        }

        DeleteEnemyObject();
        //StartCoroutine(DeleteEnemyObject());
    }

    /*IEnumerator DeleteEnemyObject()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }*/

    private void DeleteEnemyObject()
    {
        Destroy(this.gameObject);
    }
}
