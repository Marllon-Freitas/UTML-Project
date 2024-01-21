using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;
    protected Rigidbody2D rb;

    private string animatorBoolName;

    protected float stateTimer;
    protected bool triggersCalled;

    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animatorBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;
        this.animatorBoolName = animatorBoolName;
    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Enter()
    {
        triggersCalled = false;
        rb = enemyBase.rb;
        enemyBase.animator.SetBool(animatorBoolName, true);
    }


    public virtual void Exit()
    {
        enemyBase.animator.SetBool(animatorBoolName, false);
        //enemyBase.AssignLastAnimName(animatorBoolName);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggersCalled = true;
    }
}
