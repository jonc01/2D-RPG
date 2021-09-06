using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRaycast : MonoBehaviour
{
    [Header("=== Required reference setup ===")]
    [SerializeField] bool debugging = false;
    [SerializeField] BaseEnemyClass enemyClass;
    public LayerMask playerLayer;
    public LayerMask groundLayer;
    [SerializeField]
    protected Transform groundCheck,
        wallPlayerCheck,
        attackCheck;

    [Space]
    [Header("=== Adjustable Variables ===")] //Raycast variables
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float
        wallCheckDistance = -0.5f, //negative values - enemies are initialized facing left
        playerCheckDistance = -3f; //Aggro range
    public float attackRange = -0.67f; //when to start attacking player, uses a raycast to detect if player is within range

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
    
    void Update()
    {
        AttackCheck();
        GroundWallCheck();
        PlayerDetectCheck();
        UpdatePlayerToRight();

        if (debugging)
        {
            DebugDrawRaycast();
        }
    }

    void DebugDrawRaycast()
    {
        Vector3 down = transform.TransformDirection(Vector3.down) * groundCheckDistance;
        Debug.DrawRay(groundCheck.position, down, Color.green);

        Vector3 right = transform.TransformDirection(Vector3.right) * wallCheckDistance;
        //Debug.DrawRay(wallPlayerCheck.position, right, Color.blue);

        Vector3 attackRight = transform.TransformDirection(Vector3.right) * playerCheckDistance;
        Debug.DrawRay(wallPlayerCheck.position, attackRight, Color.cyan);

        Vector3 attackLeft = transform.TransformDirection(Vector3.left) * playerCheckDistance;
        Debug.DrawRay(wallPlayerCheck.position, attackLeft, Color.red);

        Vector3 playerInAttackRange = transform.TransformDirection(Vector3.left) * attackRange;
        Debug.DrawRay(attackCheck.position, playerInAttackRange, Color.magenta);
    }

    void AttackCheck()
    {
        playerInRange = Physics2D.Raycast(attackCheck.position, -transform.right, attackRange, playerLayer);
    }

    void GroundWallCheck()
    {
        groundDetect = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        wallDetect = Physics2D.Raycast(wallPlayerCheck.position, transform.right, wallCheckDistance, groundLayer);
    }

    void PlayerDetectCheck()
    {
        playerDetectFront = Physics2D.Raycast(wallPlayerCheck.position, transform.right, playerCheckDistance, playerLayer);
        playerDetectBack = Physics2D.Raycast(wallPlayerCheck.position, -transform.right, playerCheckDistance, playerLayer);
    }

    void UpdatePlayerToRight()
    {
        if (playerDetectFront) 
        {
            if (enemyClass.enFacingRight) // E-> P
            {
                playerToRight = true;
            }
            else if (!enemyClass.enFacingRight) // P <-E
            {
                playerToRight = false;
            }
        }
        else if (playerDetectBack) //can't use else in case of player jumping above raycast
        {
            if (enemyClass.enFacingRight) // P E->
            {
                playerToRight = false;
            }
            else if (!enemyClass.enFacingRight) // <-E P
            {
                playerToRight = true;
            }
        }
        //no else; don't want to update playerToRight when player jumps above raycast
    }
}
