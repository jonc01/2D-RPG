using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{
    public bool CanDash { get; private set; }

    private float lastDashTime;

    private Vector2 dashDirection;
    private Vector2 lastAfterImagePos;

    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        CanDash = false; //just dashed  //TODO: can replace with startCooldown //Time.time
        player.InputHandler.UseDashInput();

        dashDirection = Vector2.right * player.FacingDirection;

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)    //only adding dash velocity while dashing, prevents sliding
        {
            PlaceAfterImage();

            player.SetVelocity(playerData.dashVelocity, dashDirection);
            player.SetVelocityY(0f); //while dashing, y velocity stays at 0
            CheckIfShouldPlaceAfterImage();

            if (Time.time >= startTime + playerData.dashTime)
            {
                isAbilityDone = true;
                lastDashTime = Time.time; //cooldown: comparing time since last dash
            }
        }
    }

    private void CheckIfShouldPlaceAfterImage()
    {
        if(Vector2.Distance(player.transform.position, lastAfterImagePos) >= playerData.distBetweenAfterImages)
        {
            PlaceAfterImage();
        }
    }

    private void PlaceAfterImage()
    {
        //TODO: 10/29 finish after image
        PlayerAfterImagePool.Instance.GetFromPool();
        lastAfterImagePos = player.transform.position;
    }

    public bool CheckIfCanDash() //checking cooldown
    {
        return CanDash && Time.time >= lastDashTime + playerData.dashCooldown;
    }

    public void ResetCanDash() => CanDash = true;

}
