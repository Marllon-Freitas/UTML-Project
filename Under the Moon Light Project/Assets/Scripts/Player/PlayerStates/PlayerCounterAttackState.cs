using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;
    public PlayerCounterAttackState(Player player, PlayerStateMachine stateMachine, string animatorBoolName) : base(player, stateMachine, animatorBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
        player.animator.SetBool("SuccessfulCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10;
                    player.animator.SetBool("SuccessfulCounterAttack", true);
                    player.GetComponent<TimeStopWhenHit>().StopTime(0.07f, 11, 0.2f);

                    if (canCreateClone)
                    {
                        canCreateClone = false;
                        player.skillManager.cloneSkill.CreateCloneOnCounterAttack(hit.transform);
                    }
                }
            }
        }

        if (stateTimer < 0 || triggersCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
