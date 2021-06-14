using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public AbilityUI abilityUI;

    public float defaultRunSpeed = 40f;
    public float runSpeed = 40f;
    public Rigidbody2D rb;
    public PlayerCombat playerCombat;
    public GameObject player;

    //dodge/dash cooldown
    [Header ("DodgeRoll")]
    [SerializeField] float m_rollForce = 4.0f; //default 5.0f
    public float dodgeTime = .5f;
    public float dodgeCD = 2;
    private float allowDodge = 0;

    public bool canDash;
    public float dashCD = 2;
    private float allowDash = 0; //delete me if combining dash with dodge CD
    public float dashTime = .1f;
    private float dashTimeLeft;
    public float dashSpeed;
    public float distanceBetweenImages;
    private float lastImageXpos;
    private float lastDash = -100f;

    private bool cancelDash = false;

    //private int m_currentAttack = 0;
    //private float m_timeSinceAttack = 0.0f;
    public int m_facingDirection = 1;
    public bool m_rolling;
    public bool isDashing;
    float horizontalMove = 0f;
    bool jump = false;

    public bool canMove;

    //created new separate from CharacterController2D
    public bool isGrounded = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canMove = true;
        m_facingDirection = 1;
        controller.canFlip = true;
        m_rolling = false;
        isDashing = false;
        canDash = true;
    }


    public void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
            if (canMove == true)
            {
                runSpeed = defaultRunSpeed;
                horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
                animator.SetBool("Stunned", false);
            }
        
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove)); //greater than 0 -> play run anim, less than -> play idle

            FacingDirection();
            JumpCheck();
            CheckDodge();
        }
    }

    public void FixedUpdate()
    {
        if (canMove == true)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
            //runSpeed = defaultRunSpeed;
        }

        if(canMove == false)
            runSpeed = 0f;

        jump = false;

        //DashInput(); //calling through PlayerCombat ShieldBash()
        CheckDash();
    }

    void FacingDirection()
    {
        if (horizontalMove > 0)
        {
            m_facingDirection = 1;
        }
        else if(horizontalMove< 0)
        {
            m_facingDirection = -1;
        }
    }

    void JumpCheck()
    {
        if (Input.GetButtonDown("Jump") && rb.velocity.y == 0)
        //if (keyboard.spaceKey.isPressed && rb.velocity.y == 0)
        {
            if (canMove)
                rb.AddForce(Vector2.up * 100f);

        }

        //checking if player is jumping or falling
        //if (Mathf.Abs(horizontalMove) > 0 && rb.velocity.y == 0)
        if (rb.velocity.y == 0) //player is grounded
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("isFalling", false);
            isGrounded = true;
        }

        //bool check on Jumping to prevent launching when colliding with wall
        if (rb.velocity.y > 0 && !animator.GetBool("Jumping"))
        {
            animator.SetBool("Jumping", true);
            jump = true;
            isGrounded = false;
        }

        if (rb.velocity.y < 0)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("isFalling", true);
        }
    }

    #region DodgeRoll
    void CheckDodge() //player is immune to stun while rolling, check in PlayerCombat during knockback application
    {
        if (Time.time > allowDodge && canMove)
        {
            if (Input.GetButtonDown("Dodge") && !m_rolling && !isDashing && isGrounded) //prevent dodging midair
            {
                StartCoroutine(DodgeRoll());
                canMove = false;
            }
        }
    }

    IEnumerator DodgeRoll()
    {
        //StopAllCoroutines(); //cancel attacks with dodgeRoll
            //IsAttacking,
             
        abilityUI.StartCooldown(dodgeCD);
        playerCombat.canAttack = false;
        m_rolling = true;
        animator.SetBool("isRolling", true);
        animator.SetTrigger("Roll");
        rb.velocity = new Vector2(m_facingDirection * m_rollForce, rb.velocity.y);
        allowDodge = Time.time + dodgeCD;
        yield return new WaitForSeconds(dodgeTime); //dodge duration
        canMove = true;
        animator.SetBool("isRolling", false);
        playerCombat.canAttack = true;
        m_rolling = false; //DELETE: if we're still using AE_ResetRoll in animation event
    }
    
    void AE_ResetRoll() // called in animation event
    {
        m_rolling = false;
        //calling in animation event can cause issues if roll is cancelled with stun
        //^ this is why animation is frozen after stunned, m_rolling is still true
    }
#endregion

    #region Dash
    //void DashInput()
    //{
    //    //Dash (mid-air dodge) //allowDash
    //    {
    //    if (Time.time > allowDash && canMove) //not in-use, repurposed Dash for ShieldBash
    //        if (Input.GetButtonDown("Dodge") && !m_rolling && !isDashing && !isGrounded)
    //        {
    //            Dash();
    //        }
    //    }
    //}

    public void Dash()
    {
        isDashing = true;
        dashTimeLeft = dashTime; //starts dash in CheckDash()
        lastDash = Time.time; //
        allowDash = Time.time + dashCD;

        cancelDash = false;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    private void CheckDash()
    {
        if(isDashing && canDash)
        {
            if (dashTimeLeft > 0)
            {
                DisableMove(); //no movement inputs
                controller.canFlip = false;
                rb.velocity = new Vector2(dashSpeed * m_facingDirection, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;

                if(Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool(); //display image
                    lastImageXpos = transform.position.x; //track position of last image
                }
            }

            if(dashTimeLeft <= 0)
            {
                CancelDash();
            }
        }
    }

    IEnumerator EndDash()
    {
        DisableMove(); //root player in place, stops sliding if colliders intercept
        isDashing = false;
        canDash = true;
        yield return new WaitForSeconds(.5f);
        controller.canFlip = true;
        EnableMove();
    }

    public void CancelDash()
    {
        if (isDashing)
        {
            playerCombat.EndShieldBash();
            dashTimeLeft = 0; //only needed when CancelDash is called elsewhere
            isDashing = false;
            StartCoroutine(EndDash());
        }
        isDashing = false; //REPLACE if not needed
    }

    public void DisableDash()
    {
        canDash = false; //is re-enabled in EndDash()
    }

    private void ResetDash()
    {
        //cancelDash = false
    }

    #endregion

    public void CheckMove()
    {
        //
        if (canMove && rb.velocity.x > 0f) //attempt to fix stun animation playing while moving
        {
            animator.SetBool("Stunned", false);
            playerCombat.ResetMaterial();
        }
    }

    public void EnableMove()
    {
        canMove = true;
    }

    public void DisableMove()
    {
        rb.velocity = new Vector2(0, 0);
        canMove = false;
    }

    public void StopCO()
    {
        StopAllCoroutines();
    }
}
