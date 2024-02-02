using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }
    private float lastTimeAttack;
    private float comboWindow = 2f;

    public PlayerPrimaryAttackState(
        Player player,
        PlayerStateMachine stateMachine,
        string animatorBoolName
    )
        : base(player, stateMachine, animatorBoolName) { }

    public override void Enter()
    {
        base.Enter();

        xInput = 0;

        if (comboCounter > 2 || Time.time >= lastTimeAttack + comboWindow)
            comboCounter = 0;

        player.animator.SetInteger("ComboCounter", comboCounter);

        float attackDirection = player.facingDirection;

        if (xInput != 0)
            attackDirection = xInput;

        player.SetVelocity(
            player.attackMovement[comboCounter].x * attackDirection,
            player.attackMovement[comboCounter].y
        );

        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", 0.15f);

        comboCounter++;
        lastTimeAttack = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            player.SetZeroVelocity();
        }

        if (triggersCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
