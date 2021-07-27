using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyClass : MonoBehaviour
{
    [Space] [Header("=== REQUIRED REFERENCES ===")]
    public EnemyAnimator enAnimator;
    public HealthBar healthBar;

    [Space] [Header("=== References get at Start()")]
    // REFERENCES GET AT START()
    //public HealthBar healthBar; //TODO: in Start() healthBarTransform = healthBar.GetComponent<Transform>();
    //private Transform healthBarTransform (in flip())
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    private Material mDefault;
    [SerializeField] private DeathParticlesHandler deathParticlesHandler;
    private Transform healthBarTransform;

    // HEALTH
    //float maxHealth
    //float currentHealth

    // ATTACK
    //float attackRange
    //float attackSpeed
    //float attackDamage

    [Space] [Header("=== Adjustable Variables ===")]
    public float moveSpeed = 2.5f;

    [Space] [Header("=== Transforms/Offsets ===")]
    [SerializeField] float hitEffectOffset = .5f;

    // ANIMATION
    //float attackAnimSpeed

    [Space] //TODO: might not need to be public
    // COROUTINES
    public Coroutine IsAttackingCO;
    public Coroutine IsPatrollingCO;
    public Coroutine IsIdlingCO;
    public bool
        isPatrolling,
        isIdling;


    // VARIABLES
    [Space] [Header("test")]
    public bool isAlive;
    public bool enStunned;
    public bool enCanMove;

    public bool isAttacking;
    public bool enFacingRight;
    public bool enCanFlip;



    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        mDefault = sr.material;

        healthBarTransform = healthBar.GetComponent<Transform>();



        isAlive = true;
        enStunned = false;
    }

    // Update is called once per frame
    void Update()
    {

        CoroutineCheck();
    }

    #region MOVEMENT

    public void MoveRight(bool moveRight) 
    {
        if(enCanMove && !isAttacking)
        {
            if (moveRight)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                //enFacingRight = true;
                Flip();
            }
            else
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                //enFacingRight = false;
                Flip();
            }
        }
    }



    #endregion

    void CoroutineCheck()
    {
        if (enStunned)
        {
            //Option 1
            if (IsAttackingCO != null)
                StopCoroutine(IsAttackingCO);

            //Option 2
            //StopAttackCO();




            if (IsPatrollingCO != null)
                StopCoroutine(IsPatrollingCO);

            //TODO: isIdlingCO???
        }
    }

    #region Flip Update
    void Flip() //Change direction enemy is facing
    {
        if (enCanMove)
        {
            if(rb.velocity.x > 0) //moving right
            {
                enFacingRight = true;
            }
            else //moving left
            {
                enFacingRight = false;
            }
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


    #endregion

    #region Interaction References
    public void TakeDamage()
    {
        if (isAlive)
        {
            //state.ChangeState(EN_HURT); //TODO: State Controller should recognize this as the const stored
            
        }
    }

    public void GetStunned()
    {

    }
    #endregion

    public void Die()
    {
        isAlive = false;

        /*if(enAnimator != null)
        {
            enAnimator.ChangeState("EN_DEATH");
        }*/

        StopAllCoroutines();

        if(deathParticlesHandler != null)
        {
            //Vector3 changeLocation = GetComponent<Transform>().position; //TODO: if transform.position doesn't work
            //Vector3 tempLocation = changeLocation;
            Vector3 tempLocation = transform.position;
            tempLocation.y += hitEffectOffset;

            deathParticlesHandler.ShowHitEffect(tempLocation);
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
}
