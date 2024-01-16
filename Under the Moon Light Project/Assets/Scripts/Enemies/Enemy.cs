using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("Move Info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;

    [Header("Attack Info")]
    public float attackDistance;
    public float attackCooldown;
    [HideInInspector] public float lastTimeAttacked;

    [Header("Screen Shake")]
    [SerializeField]
    private ScreenShakeProfile screenShakeProfile;
    [SerializeField]
    private ScreenShakeProfile screenShakeProfileStunned;
    private CinemachineImpulseSource impulseSource;

    [Header("Stun Info")]
    public float stunDuration = 1f;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;


    public EnemyStateMachine stateMachine { get; private set; }
    public string lastAnimBoolName { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }

    protected override void Start()
    {
        base.Start();

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.CurrentState.Update();
    }

    public override void SlowEntityBy(float _slowPercent, float _slowDuration)
    {
        moveSpeed *= (1 - _slowPercent);
        animator.speed *= (1 - _slowPercent);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
    }

    public virtual void AssignLastAnimName(string _animBoolName) => lastAnimBoolName = _animBoolName;

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, 50, whatIsPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDirection, transform.position.y));
    }
    public virtual void AnimationFinishTrigger() => stateMachine.CurrentState.AnimationFinishTrigger();

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimerCoroutine(_duration));
    public virtual void FreezeTime(bool timeFrozen)
    {
        if (timeFrozen)
        {
            moveSpeed = 0;
            animator.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            animator.speed = 1;
        }
    }

    protected virtual IEnumerator FreezeTimerCoroutine(float seconds)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(seconds);
        FreezeTime(false);
    }

    #region Counter Attack Window
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion

    public override void DamageImpact()
    {
        base.DamageImpact();
        //shake the camera
        CameraShakeManager.Instance.ScreenShakeFromProfile(screenShakeProfile, impulseSource);
    }

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            CameraShakeManager.Instance.ScreenShakeFromProfile(screenShakeProfileStunned, impulseSource);
            return true;
        }
        return false;
    }
}
