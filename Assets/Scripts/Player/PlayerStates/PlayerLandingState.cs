using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingState : PlayerState
{
    public PlayerLandingState(
        Player player,
        PlayerStateMachine stateMachine,
        string animatorBoolName
    )
        : base(player, stateMachine, animatorBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.CreateDust();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
