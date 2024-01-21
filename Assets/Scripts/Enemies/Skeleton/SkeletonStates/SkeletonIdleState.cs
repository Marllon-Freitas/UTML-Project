using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonIdleState : SkeletonGroundedState
{
    public SkeletonIdleState(Enemy enemyBase, EnemyStateMachine stateMachine, string animatorBoolName, Enemy_Skeleton enemy) : base(enemyBase, stateMachine, animatorBoolName, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = _enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(_enemy.MoveState);
        }
    }
}
