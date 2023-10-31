using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMoveState : SkeletonGroundedState
{
    public SkeletonMoveState(Enemy enemyBase, EnemyStateMachine stateMachine, string animatorBoolName, Enemy_Skeleton enemy) : base(enemyBase, stateMachine, animatorBoolName, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        _enemy.SetVelocity(_enemy.moveSpeed * _enemy.facingDirection, rb.velocity.y);

        if (!_enemy.IsGroundDetected() || _enemy.IsWallDetected())
        {
            _enemy.Flip();
            stateMachine.ChangeState(_enemy.IdleState);
        }
    }
}
