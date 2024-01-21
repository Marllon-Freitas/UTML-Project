using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{
    #region State Machine
    public SkeletonMoveState MoveState { get; private set; }
    public SkeletonIdleState IdleState { get; private set; }
    public SkeletonBattleState BattleState { get; private set; }
    public SkeletonAttackState AttackState { get; private set; }
    public SkeletonStunnedState SkeletonStunnedState { get; private set; }
    public SkeletonDeadState SkeletonDeadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        IdleState = new SkeletonIdleState(this, stateMachine, "Idle", this);
        MoveState = new SkeletonMoveState(this, stateMachine, "Move", this);
        BattleState = new SkeletonBattleState(this, stateMachine, "Move", this);
        AttackState = new SkeletonAttackState(this, stateMachine, "Attack", this);
        SkeletonStunnedState = new SkeletonStunnedState(this, stateMachine, "Stunned", this);
        SkeletonDeadState = new SkeletonDeadState(this, stateMachine, "Die", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(SkeletonStunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(SkeletonDeadState);
    }
}
