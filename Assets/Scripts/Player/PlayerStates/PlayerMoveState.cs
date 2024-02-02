using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, string animatorBoolName)
        : base(player, stateMachine, animatorBoolName) { }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySoundEffect(14, null);
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSoundEffect(14);
    }

    public override void Update()
    {
        base.Update();

        

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if (xInput == 0 || player.IsWallDetected())
            stateMachine.ChangeState(player.idleState);
    }
}
