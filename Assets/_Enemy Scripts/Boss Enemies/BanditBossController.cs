using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditBossController : BaseEnemyController
{
    [Header("=== Boss References ===")]
    [SerializeField] BanditBossClass banditBossClass;



    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        //only Idle until aggroStarted

        /*
        CoroutineCheck();
        MoveCheck();
        LedgeWallCheck();
        PlayerCheck();
        AttackCheck();
        AttackFlip();
        */

        PlayerCheck();
    }

    protected override void PlayerCheck()
    {
        //base.PlayerCheck();

        if (!enRaycast.wallDetect && enRaycast.groundDetect && !enemyClass.enStunned)
        {
            if (enRaycast.playerDetectFront || enRaycast.playerDetectBack)
            {
                aggroStarted = true;
                if(banditBossClass != null)
                    banditBossClass.ShowHealthBar(true); //TODO: test, should stay displayed even if player leaves aggro

                StopPatrolling();

                if (isIdling)
                {
                    StopIdling();
                    enemyClass.enCanMove = true;
                }
                enemyClass.MoveRight(enRaycast.playerToRight);
            }
        }
        else
        {
            aggroStarted = false; //TODO: if using aggroTimer, will have to set within aggroTimer CO
        }
    }

    protected override void AttackCheck()
    {
        if (enRaycast.playerInRange)
        {
            banditBossClass.StartAttackCO();
        }
    }
}
