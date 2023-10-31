using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform player;
    private Enemy_Skeleton _enemy;
    private int moveDirection;
    public SkeletonBattleState(Enemy enemyBase, EnemyStateMachine stateMachine, string animatorBoolName, Enemy_Skeleton enemy) : base(enemy, stateMachine, animatorBoolName)
    {
        _enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (_enemy.IsPlayerDetected())
        {
            stateTimer = _enemy.battleTime;
            if (_enemy.IsPlayerDetected().distance < _enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(_enemy.AttackState);
            }
        }
        else
        {
            if (stateTimer <= 0 || Vector2.Distance(player.transform.position, _enemy.transform.position) > 7)
                stateMachine.ChangeState(_enemy.IdleState);
        }

        if (player.position.x < _enemy.transform.position.x)
            moveDirection = -1;
        else
            moveDirection = 1;

        _enemy.SetVelocity(_enemy.moveSpeed * moveDirection, rb.velocity.y);
    }

    private bool CanAttack()
    {
        if (Time.time >= _enemy.lastTimeAttacked + _enemy.attackCooldown)
        {
            _enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }
}
