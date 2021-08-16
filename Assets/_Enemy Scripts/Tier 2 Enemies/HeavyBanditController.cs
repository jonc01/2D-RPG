using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBanditController : BaseEnemyController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void MoveCheck()
    {
        //base.MoveCheck();
        //Focus on Idle, short durations for Patrol
    }
}
