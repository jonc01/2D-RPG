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

    //dodge cooldown
    public float dodgeCD = 1;
    private float allowDodge = 0;
    public float dodgeTime = .5f;

    //private int m_currentAttack = 0;
    //private float m_timeSinceAttack = 0.0f;
    public int m_facingDirection = 1;
    public bool m_rolling = false;
    private bool m_grounded = false;
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    public bool canMove = true;

    //created new separate from CharacterController2D
    public bool isGrounded = true;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        canMove = true;
        m_facingDirection = 1;
        controller.canFlip = true;
    }

    // Update is called once per frame
    void Update()
    {
        //var keyboard = Keyboard.current; //temp workaround
        //timer for attack combo
        //m_timeSinceAttack += Time.deltaTime;
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
            if (playerCombat.tempShowDmg != null)
            {
                playerCombat.FlipTextAgain(0);
            }
        }
        else if(horizontalMove < 0)
        {
            m_facingDirection = -1;
            if (playerCombat.tempShowDmg != null)
            {
                playerCombat.FlipTextAgain(180);
            }
        }

        if (Input.GetButtonDown("Jump") && rb.velocity.y == 0)
        //if (keyboard.spaceKey.isPressed && rb.velocity.y == 0)
        {
            if(canMove)
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
        if (Time.time > allowDodge && canMove)
        {
            if (Input.GetButtonDown("Dodge") && !m_rolling && isGrounded) //prevent dodging midair
            //if (keyboard.leftShiftKey.isPressed && !m_rolling && isGrounded) //prevent dodging midair
            {
                StartCoroutine(DodgeRoll());
                canMove = false;
                //crouch = true; can set later, or just disable hitbox, not collider
            }
        }
        m_rolling = false;

    }

    private void FixedUpdate()
    {
        if (canMove == true)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            //runSpeed = defaultRunSpeed;
        }

        if(canMove == false)
            runSpeed = 0f;

        jump = false;
        //animator.SetBool("Jumping", false);
    }

    IEnumerator DodgeRoll()
    {
        playerCombat.canAttack = false;
        m_rolling = true;
        animator.SetTrigger("Roll");
        animator.SetBool("isRolling", true);
        rb.velocity = new Vector2(m_facingDirection * m_rollForce, rb.velocity.y);
        allowDodge = Time.time + dodgeCD;
        //playerCombat.playerArmor = 100; //temp or dodge invulnerability
        yield return new WaitForSeconds(dodgeTime); //dodge duration
        //playerCombat.playerArmor = 0; //temp
        canMove = true;
        animator.SetBool("isRolling", false);
        playerCombat.canAttack = true;
    }

    void AE_ResetRoll()
    {
        m_rolling = false;
        crouch = false;
    }
}
