using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, string animatorBoolName)
        : base(player, stateMachine, animatorBoolName) { }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySoundEffect(40, null);
        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
        player.CreateDust();
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSoundEffect(40);
    }

    public override void Update()
    {
        base.Update();

        if (rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
