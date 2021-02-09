﻿using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{

    //[SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;

    
    public Rigidbody2D rb;

    public Animator enAnimator;
    public bool enFacingRight = false;
    public float moveSpeedDefault = 1;
    public float moveSpeed;


    //to prevent flipping with Enemy parent object
    public Transform HealthBar;

    [Space]
    public bool enCanFlip;
    public bool enCanMove;

    public bool enCanParry;


    // Use this for initialization
    public void Start()
    {
        moveSpeed = moveSpeedDefault;
        enCanFlip = true;
        enCanMove = true;
        enCanParry = false;
    }

    // Update is called once per frame
    public void Update()
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
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                HealthBar.localRotation = Quaternion.Euler(0, 0, 0);
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
        moveSpeed = 0;
    }

    public void EnEnableMove()
    {
        enCanMove = true;
        moveSpeed = moveSpeedDefault;
    }

    public void EnEnableParry()
    {
        enCanParry = true;
    }

    public void EnDisableParry()
    {
        enCanParry = false;
    }
}
