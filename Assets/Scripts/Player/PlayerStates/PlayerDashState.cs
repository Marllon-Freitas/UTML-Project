using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, string animatorBoolName)
        : base(player, stateMachine, animatorBoolName) { }

    public override void Enter()
    {
        base.Enter();

        //create a clone if the player has the clone skill
        if (SkillManager.instance.dashSkill != null)
            player.skillManager.dashSkill.CloneOnDash();

        stateTimer = player.dashDuration;
        AudioManager.instance.PlaySoundEffect(39, null);

        if (player.IsGroundDetected())
            player.CreateDust();

        player.characterStats.MakeInvencible(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.skillManager.dashSkill.CloneOnArrival();
        player.SetVelocity(0f, rb.velocity.y);
        player.characterStats.MakeInvencible(false);
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
