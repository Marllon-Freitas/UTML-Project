using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStunnedState : EnemyState
{
    private Enemy_Skeleton _enemy;

    public SkeletonStunnedState(Enemy enemyBase, EnemyStateMachine stateMachine, string animatorBoolName, Enemy_Skeleton enemy) : base(enemy, stateMachine, animatorBoolName)
    {
        _enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        _enemy.entityFX.InvokeRepeating("RedCollorBlink", 0, 0.1f);
        stateTimer = _enemy.stunDuration;
        rb.velocity = new Vector2(-_enemy.facingDirection * _enemy.stunDirection.x, _enemy.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();
        _enemy.entityFX.Invoke("CancelColorChange", 0);
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(_enemy.IdleState);
        }
    }
}
