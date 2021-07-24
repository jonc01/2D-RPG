using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRaycast : MonoBehaviour
{
    [Header("=== Required reference setup ===")]
    [SerializeField] public LayerMask
        playerLayer,
        groundLayer;

    [SerializeField]
    private Transform groundCheck;
    private Transform
        wallPlayerCheck,
        attackCheck;

    [Header("=== Adjustable Variables ===")]
    [SerializeField] //Raycast variables
    private float
        groundCheckDistance,
        wallCheckDistance,
        playerCheckDistance, //Aggro range
        attackRange; //when to start attacking player, uses a raycast to detect if player is within range

    [Space]
    [Header("=== Raycast Checks ===")]
    public bool playerToRight;
    [SerializeField]
    public bool
        playerDetectFront,
        playerDetectBack,
        playerInRange,
        groundDetect,
        wallDetect;


    public bool enFacingRight; //TODO: remove 

    void Start()
    {
        
    }

    
    void Update()
    {
        AttackCheck();
        MoveCheck();
        UpdatePlayerToRight();
    }

    void AttackCheck()
    {
        playerInRange = Physics2D.Raycast(attackCheck.position, -transform.right, attackRange, playerLayer);

        /*if (playerInRange)
        {
            enCanAttack = true; //redundant
        }
        else
        {
            enCanAttack = false; //redundant
        }*/
        
        //TODO: delete this if Enemy2 call works
        /*if(playerInRange && overrideAttack)
        {
            enemyOverrideScript.StartAttack();
        }*/
    }

    void MoveCheck()
    {

    }

    void UpdatePlayerToRight()
    {
        if (playerDetectFront)
        {
            if (enFacingRight)
            {
                playerToRight = true;
            }
            else
            {
                playerToRight = false;
            }
        }
        else if (playerDetectBack)
        {
            if (enFacingRight)
            {
                playerToRight = false;
            }
            else
            {
                playerToRight = true;
            }
        }
        else
        {
            //playerToRight = false;
        }
    }
}
