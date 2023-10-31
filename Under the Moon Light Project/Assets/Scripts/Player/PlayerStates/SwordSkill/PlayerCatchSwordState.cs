using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(Player player, PlayerStateMachine stateMachine, string animatorBoolName) : base(player, stateMachine, animatorBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform;

        if (player.transform.position.x > sword.position.x && player.facingDirection == 1)
            player.Flip();
        else if (player.transform.position.x < sword.position.x && player.facingDirection == -1)
            player.Flip();

        rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDirection, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", 0.1f);
    }

    public override void Update()
    {
        base.Update();

        if (triggersCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
