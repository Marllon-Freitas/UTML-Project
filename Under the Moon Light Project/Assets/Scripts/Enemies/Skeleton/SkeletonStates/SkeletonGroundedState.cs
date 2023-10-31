using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundedState : EnemyState
{
    protected Enemy_Skeleton _enemy;
    private Transform player;

    public SkeletonGroundedState(Enemy enemyBase, EnemyStateMachine stateMachine, string animatorBoolName, Enemy_Skeleton enemy) : base(enemy, stateMachine, animatorBoolName)
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

        if (_enemy != null)
        {
            if (_enemy.IsPlayerDetected() || Vector2.Distance(_enemy.transform.position, player.position) < 2)
                stateMachine.ChangeState(_enemy.BattleState);
        }
    }
}
