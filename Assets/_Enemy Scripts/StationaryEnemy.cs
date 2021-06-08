using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StationaryEnemy : MonoBehaviour
{
    //Text Popups
    public GameObject TextPopupsPrefab;
    public TextPopupsHandler TextPopupsHandler;
    [SerializeField] Vector3 TPOffset = new Vector3(0, .2f, 0);
    public HitEffectsHandler HitEffectsHandler;

    //this is for support stationary enemies, can make non-support stationary separate
    //this script only targets enemies that are "friendly" to this object, and heals them


    public LayerMask playerLayers;
    public Transform player;
    public PlayerCombat playerCombat;
    public LayerMask enemyLayers;
    public GameObject HitToRight;
    public GameObject HitToLeft;
    //public GameObject HitEffect;
    public GameObject deathParticlePrefab;

    public float maxHealth = 100;
    float currentHealth;
    public HealthBar healthBar;

    public int experiencePoints = 20;
    public Animator enAnimator;
    public bool isAlive;


    [SerializeField]
    public Rigidbody2D rb;
    float aggroRange = 3f; //when to start chasing player //might extend to aggro to player before enemy enters screen
    float enAttackRange = 1.5f; //when to start attacking player, stop enemy from clipping into player
    public Transform enAttackPoint;
    public EnemyController enController;

    [Space]
    public float enAttackDamage = 10f; //can just heal off of this value
    public float enAttackSpeed = .4f; //lower value for lower delays between attacks
    public float enAttackAnimSpeed = .7f; //lower value for shorter animations
    bool enCanAttack = true;

    SpriteRenderer sr;
    [SerializeField]
    private Material mWhiteFlash;
    private Material mDefault;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mDefault = sr.material;

        player = GameObject.Find("Player").transform;

        //Stats
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        isAlive = true;
        enCanAttack = true;
        //AI aggro
        /*if(rb != null)
            rb = GetComponent<Rigidbody2D>();*/
    }

    void Update()
    {
        if (rb != null && enController != null && isAlive) //check if object has rigidbody
        {

        }
        else if (!isAlive)
        {
            //moveSpeed = 0;
            if (rb != null)
                rb.velocity = new Vector2(0, 0);
        }
    }

    void Attack() //for Stationary, can be buffing abilities, ex: totem
    {
        //Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(enAttackPoint.position, enAttackRange, enemyLayers);
        //                                                                                  targetting enemy allies only
        //damage enemies
        /*foreach (Collider2D enemy in hitEnemies) //loop through enemies hit
        {
            //if(enemy != null && enemy.GetComponent<Enemy>() != null)
            //enemy.GetComponent<Enemy>().TakeDamage(-enAttackDamage); //negative damage for healing don't need Heal() function
        }*/
    }

    IEnumerator IsAttacking()
    {
        if (enCanAttack)
        {
            //rb.velocity = new Vector2(0, 0); //stop moving
            /*enAnimator.SetBool("isAttacking", true);
            enAnimator.SetTrigger("Attack");
            //enAnimator.SetBool("inCombat", true);
            enAnimator.SetBool("isRunning", false);*/
            //
            enCanAttack = false;
            yield return new WaitForSeconds(enAttackAnimSpeed); //time when damage actually registers

            Attack();
            //enCanMove = true; //might cause enemy to slide while attacking is player moves
            //yield return new WaitForSeconds(.5f);
            yield return new WaitForSeconds(enAttackSpeed); //cooldown between attacks
            //enAnimator.SetBool("isAttacking", false);
            enCanAttack = true;
            //enAnimator.SetBool("isAttacking", false);
            //enAnimator.SetTrigger("Attack");
        }
        //enCanAttack = false;
        //enCanAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        /*if (enAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enAttackPoint.position, enAttackRange);*/
    }

    public void TakeDamage(float damage, float damageMultiplier = 1.0f)
    {
        if (isAlive == true)
        {
            float damageTaken = damage * damageMultiplier;
            currentHealth -= damageTaken;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            Vector3 changeLocation = GetComponent<Transform>().position;

            Vector3 tempLocation = changeLocation;
            //tempLocation.y += .0f; //y offset
            if(player.position.x > transform.position.x) //player to right of enemy
            {
                Instantiate(HitToLeft, tempLocation, Quaternion.identity);
            }
            else
            {
                Instantiate(HitToRight, tempLocation, Quaternion.identity);
            }

            //Instantiate(HitEffect, tempLocation, Quaternion.identity);
            HitEffectsHandler.ShowHitEffect(tempLocation);

            sr.material = mWhiteFlash; //flashing enemy sprite
            Invoke("ResetMaterial", .1f);

            //show damage/heal numbers
            if (TextPopupsPrefab)
            {
                Vector3 tempPos = transform.position;
                tempPos += TPOffset;
                TextPopupsHandler.ShowDamage(damageTaken, tempPos);
            }
            
            //hurt animation
            if (enAnimator != null)
            {
                enAnimator.SetTrigger("Hurt");
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

    void Die()
    {
        isAlive = false;
        //Die animation
        if (enAnimator != null)
        {
            enAnimator.SetTrigger("Death");
        }

        //give player exp
        playerCombat.GiveXP(experiencePoints);

        //hide hp bar


        //disable enemy object
        isAlive = false;

        if (deathParticlePrefab != null)
        {
            Vector3 changeLocation = GetComponent<Transform>().position;
            Vector3 tempLocation = changeLocation;
            tempLocation.y -= .1f;
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
