using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player player, PlayerStateMachine stateMachine, string animatorBoolName) : base(player, stateMachine, animatorBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skillManager.swordThrowSkill.DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", 0.2f);
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();
        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (player.transform.position.x > mousePos.x && player.facingDirection == 1)
            player.Flip();
        else if (player.transform.position.x < mousePos.x && player.facingDirection == -1)
            player.Flip();
    }
}
