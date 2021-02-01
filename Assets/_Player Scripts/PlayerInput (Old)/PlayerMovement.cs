using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float m_rollForce = 5.0f;
    public CharacterController2D controller;
    public Animator animator;

    public float defaultRunSpeed = 40f;
    public float runSpeed = 40f;
    public Rigidbody2D rb;
    public PlayerCombat playerCombat;
    public GameObject player;

    //dodge/dash cooldown
    public float dodgeCD = 1;
    private float allowDodge = 0;
    public float dodgeTime = .5f;

    public float dashCD = 2;
    private float allowDash = 0; //delete me if combining dash with dodge CD
    public float dashTime = .1f;
    private float dashTimeLeft;
    public float dashSpeed;
    public float distanceBetweenImages;
    private float lastImageXpos;
    private float lastDash = -100f;


    //private int m_currentAttack = 0;
    //private float m_timeSinceAttack = 0.0f;
    public int m_facingDirection = 1;
    public bool m_rolling;
    public bool isDashing;
    float horizontalMove = 0f;
    bool jump = false;

    public bool canMove = true;

    //created new separate from CharacterController2D
    public bool isGrounded = true;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canMove = true;
        m_facingDirection = 1;
        controller.canFlip = true;
        m_rolling = false;
        isDashing = false;
    }


    void Update()
    {
        if (canMove == true)
        {
            runSpeed = defaultRunSpeed;
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        }
        
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove)); //greater than 0 -> play run anim, less than -> play idle
        //animator.SetBool("Grounded", true);

        if (horizontalMove > 0)
        {
            m_facingDirection = 1;
        }
        else if(horizontalMove < 0)
        {
            m_facingDirection = -1;
        }

        if (Input.GetButtonDown("Jump") && rb.velocity.y == 0)
        //if (keyboard.spaceKey.isPressed && rb.velocity.y == 0)
        {
            if (canMove)
                rb.AddForce(Vector2.up * 100f);
                
            //jump = true;
            //animator.SetBool("Grounded", false);
            //animator.SetBool("Jumping", true);
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

        if(rb.velocity.y < 0)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("isFalling", true);
        }

        //Dodge Roll
        CheckDodge();

        DashInput();
        CheckDash();
    }

    private void FixedUpdate()
    {
        if (canMove == true)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
            //runSpeed = defaultRunSpeed;
        }

        if(canMove == false)
            runSpeed = 0f;

        jump = false;
    }

    void CheckDodge()
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
        //StopAllCoroutines();
            //IsAttacking,
             
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
        m_rolling = false;
    }
    
    void AE_ResetRoll()
    {
        m_rolling = false;
    }

    void DashInput()
    {
        //Dash (mid-air dodge)
        if (Time.time > allowDash && canMove) //TODO: can just move this into Dodge ^^^^ just switching between both depending on "isGrounded"
        {
            if (Input.GetButtonDown("Dodge") && !m_rolling && !isDashing && !isGrounded)
            {
                Dash();
            }
        }
    }

    private void Dash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        allowDash = Time.time + dodgeCD;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    private void CheckDash()
    {
        if(isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                controller.canFlip = false;
                rb.velocity = new Vector2(dashSpeed * m_facingDirection, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;

                if(Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }

            if(dashTimeLeft <= 0)
            {
                isDashing = false;
                canMove = true;
                controller.canFlip = true;
            }
        }
    }

}
