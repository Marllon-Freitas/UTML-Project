using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, string animatorBoolName) : base(player, stateMachine, animatorBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //create a clone if the player has the clone skill
        if (SkillManager.instance.cloneSkill != null)
            player.skillManager.cloneSkill.CreateCloneOnDashStart();

        stateTimer = player.dashDuration;
        if (player.IsGroundDetected())
            player.CreateDust();
    }

    public override void Exit()
    {
        base.Exit();

        player.skillManager.cloneSkill.CreateCloneOnDashOver();
        player.SetVelocity(0f, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        //change to wall slide state only if player try to dash on the same direction of the wall
        if (player.IsWallDetected() && player.dashDirection == player.facingDirection)
            stateMachine.ChangeState(player.wallSlideState);

        player.SetVelocity(player.dashSpeed * player.dashDirection, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
