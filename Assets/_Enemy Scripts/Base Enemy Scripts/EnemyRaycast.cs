using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRaycast : MonoBehaviour
{
    [Header("=== Required reference setup ===")]
    public LayerMask playerLayer;
    public LayerMask groundLayer;



    [SerializeField]
    private Transform groundCheck,
        wallPlayerCheck,
        attackCheck;

    [Header("=== Adjustable Variables ===")] //Raycast variables
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float
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


    public bool enFacingRight; //TODO: remove, should be in movement script (BaseEnemyClass)


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
            if (enFacingRight) // E-> P
            {
                playerToRight = true;
            }
            else if (!enFacingRight) // P <-E
            {
                playerToRight = false;
            }
        }
        else if (playerDetectBack) //can't use else in case of player jumping above raycast
        {
            if (enFacingRight) // P E->
            {
                playerToRight = false;
            }
            else if (!enFacingRight) // <-E P
            {
                playerToRight = true;
            }
        }
    }
}
