using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine, string animatorBoolName)
        : base(player, stateMachine, animatorBoolName) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlideState);

        if (player.IsGroundDetected())
        {
            if (rb.velocity.y < 0.01)
                stateMachine.ChangeState(player.landingState);
            stateMachine.ChangeState(player.idleState);
        }

        if (xInput != 0)
            player.SetVelocity(player.moveSpeed * 1f * xInput, rb.velocity.y);
    }
}
