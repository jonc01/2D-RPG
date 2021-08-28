using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    //
    [TextArea]
    public string Note = "Input for Enemy object";

    [Header("=== Required References for setup ===")]
    public EnemyRaycast enRaycast;
    public BaseEnemyClass enemyClass;

    [Space]
    [Header("=== Adjustable Variables ===")]
    [SerializeField] float CODurationLower = .3f;
    [SerializeField] float CODurationUpper = 1f;
    //float aggroDuration //TODO: how long to run in last known direction of player until returning to patrolling/idling

    //
    //public 
    [Header("=== Variables ===")]
    [SerializeField] bool aggroStarted;
    public Coroutine IsPatrollingCO;
    public Coroutine IsIdlingCO;
    public bool isPatrolling, isIdling;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        aggroStarted = false;

        bool startDir = (Random.value > 0.5f);
        if(enemyClass != null)
            enemyClass.MoveRight(startDir);

        //if(references == null)
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CoroutineCheck();
        MoveCheck();
        LedgeWallCheck();
        PlayerCheck();
        AttackCheck();
        AttackFlip();
    }

    void CoroutineCheck()
    {
        if (enemyClass.enStunned)
        {
            StopPatrolling();
            StopIdling();
        }
    }

    public virtual void MoveCheck()
    {
        if(enRaycast.groundDetect && !aggroStarted && !enemyClass.isAttacking && !enemyClass.enStunned) //&& enemyClass.enCanMove)
        {
            //Logic for enemy to decide between Patrolling and Idling
            if (isPatrolling)
            {
                if (enemyClass.enFacingRight)
                {
                    enemyClass.MoveRight(true);
                }
                else
                {
                    enemyClass.MoveRight(false);
                }
            }

            //Switch between Patrolling or Idling, and the duration to run the action
            if (!isPatrolling && !isIdling)
            {
                bool switchDir = (Random.value > .5f);
                bool idleSwitch = (Random.value > .5f);
                float coDuration = (Random.Range(CODurationLower, CODurationUpper));

                if (idleSwitch)
                {
                    StartPatrol(coDuration, switchDir);
                }
                else
                {
                    StartIdle(coDuration, switchDir);
                }
            }
        }
    }

    void LedgeWallCheck()
    {
        if(!enRaycast.groundDetect || enRaycast.wallDetect)
        {
            if(enemyClass.rb.velocity.y == 0) //don't flip when falling
            {
                FlipDir();
            }
        }
    }

    void FlipDir() //flip current direction
    {
        bool dir = !enemyClass.enFacingRight;
        enemyClass.MoveRight(dir);
    }

    #region Coroutines - Idle, Patrol
    void StartIdle(float duration, bool switchDir, bool knockbackHitB = false)
    {
        if(!isIdling)
            IsIdlingCO = StartCoroutine(Idling(duration, switchDir, knockbackHitB));
    }

    void StopIdling()
    {
        if (IsIdlingCO != null)
            StopCoroutine(IsIdlingCO);

        isIdling = false;
    }

    IEnumerator Idling(float duration, bool switchDir, bool knockbackHitB)
    {
        isIdling = true;
        enemyClass.enCanMove = false;
        if (switchDir)
            FlipDir();

        yield return new WaitForSeconds(duration);
        isIdling = false;
        enemyClass.enCanMove = true;
    }

    void StartPatrol(float duration, bool switchDir)
    {
        if(!isPatrolling)
            IsPatrollingCO = StartCoroutine(Patrolling(duration, switchDir));
    }

    void StopPatrolling()
    {
        if (IsPatrollingCO != null)
            StopCoroutine(IsPatrollingCO);

        isPatrolling = false;
    }

    IEnumerator Patrolling(float duration, bool switchDir)
    {
        isPatrolling = true;
        if (switchDir)
            FlipDir();

        yield return new WaitForSeconds(duration);
        isPatrolling = false;
    }

    #endregion

    void PlayerCheck()
    {
        if(!enRaycast.wallDetect && enRaycast.groundDetect && !enemyClass.enStunned)
        {
            if(enRaycast.playerDetectFront || enRaycast.playerDetectBack)
            {
                aggroStarted = true;
                StopPatrolling();

                if(isIdling) //TODO: might not need, is a check if Idling is interrupted
                {
                    StopIdling();
                    enemyClass.enCanMove = true;
                }
                enemyClass.MoveRight(enRaycast.playerToRight);
            }
            else
            {
                aggroStarted = false;
            }
        }
        else
        {
            aggroStarted = false; //TODO: if using aggroTimer, will have to set within aggroTimer CO
        }
    }

    void AttackCheck()
    {
        if (enRaycast.playerInRange)
        {
            enemyClass.StartAttackCO();
        }
    }

    void AttackFlip()
    {
        if (enemyClass.isAttacking && enemyClass.enCanFlip)
        {
            if(enemyClass.enFacingRight != enRaycast.playerToRight)
            {
                enemyClass.ManualFlip(enRaycast.playerToRight);
            }
        }
    }
}
