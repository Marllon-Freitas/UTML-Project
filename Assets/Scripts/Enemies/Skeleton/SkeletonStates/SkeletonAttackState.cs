using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackState : EnemyState
{
    private Enemy_Skeleton _enemy;
    public SkeletonAttackState(Enemy enemyBase, EnemyStateMachine stateMachine, string animatorBoolName, Enemy_Skeleton enemy) : base(enemy, stateMachine, animatorBoolName)
    {
        _enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        _enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        _enemy.SetZeroVelocity();

        if (triggersCalled)
            stateMachine.ChangeState(_enemy.BattleState);
    }
}
