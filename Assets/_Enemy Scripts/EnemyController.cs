using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{

    //[SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;

    
    public Rigidbody2D rb;

    public Animator enAnimator;
    public bool enFacingRight = false;
    public float moveSpeed = 1;


    //to prevent flipping with Enemy parent object
    public Transform HealthBar;
    public Transform TextPopups;

    [SerializeField]
    private bool enCanFlip;
    public bool enCanMove;

    // Use this for initialization
    void Start()
    {
        enCanFlip = true;
        enCanMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        //child = GameObject.Find("HealthBarCanvas");
        if (enCanMove)
        {
            if (rb.velocity.x > 0) //moving right
            {
                enFacingRight = true;
            }
            else //moving left
            {
                enFacingRight = false;
            }
        }

    }

    public void Flip()
    {
        // Switch the way the enemy is labelled as facing.
        //enFacingRight = !enFacingRight;

        // Multiply the enemy's x local scale by -1
        //.Vector3 theScale = transform.localScale;
        if (enCanFlip) {
            if (enFacingRight)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                HealthBar.localRotation = Quaternion.Euler(0, 180, 0);
                //theScale.y = 180; //negative x now faces
                //transform.localScale = theScale;
                /*Vector3 childScale = child.transform.localScale;
                childScale.x *= -1;
                child.transform.localScale = childScale;*/
                //TextPopups.localRotation = Quaternion.Euler(0, 0, 0); //works without
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                HealthBar.localRotation = Quaternion.Euler(0, 0, 0);

                //theScale.y = 0; //faces left by default
                //TextPopups.localRotation = Quaternion.Euler(0, 180, 0); //
            }
        }
    }

    public void EnDisableFlip()
    {
        enCanFlip = false;
    }

    public void EnEnableFlip()
    {
        enCanFlip = true;
    }

    public void EnDisableMove()
    {
        enCanMove = false;
    }

    public void EnEnableMove()
    {
        enCanMove = true;
    }
}
