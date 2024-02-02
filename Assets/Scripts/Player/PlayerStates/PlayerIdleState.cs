using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, string animatorBoolName)
        : base(player, stateMachine, animatorBoolName) { }

    public override void Enter()
    {
        base.Enter();

        // Check if the player object is null before accessing its rigidbody component
        if (player?.rb != null)
        {
            player.SetZeroVelocity();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (xInput == player.facingDirection && player.IsWallDetected())
            return;

        if (xInput != 0 && !player.isBusy)
            stateMachine.ChangeState(player.moveState);
    }
}
