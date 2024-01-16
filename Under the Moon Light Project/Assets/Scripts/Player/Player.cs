using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    //[Header("Input Info")]
    //private PlayerControls controls;
    //private InputAction movementInputAction;

    [Header("Attack Details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = 0.2f;
    public float counterAttackCooldown = 0.5f;
    public float counterAttackCooldownTimer;

    [Header("Move Info")]
    public float moveSpeed = 5f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash Info")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDirection { get; private set; }

    [Header("WallSlide Info")]
    public float wallSlideSpeed;

    [Header("Dust Particle System")]
    public ParticleSystem dustParticle;

    #region Scarf

    [Header("Scarf Offset")]
    [SerializeField]
    private Vector2 idleScarfOffset = new Vector2(-0.01f, -0.1f);

    [SerializeField]
    private Vector2 moveScarfOffset = new Vector2(-0.1f, -0.01f);

    [SerializeField]
    private Vector2 jumpScarfOffset = new Vector2(-0.01f, -0.1f);

    [SerializeField]
    private Vector2 fallScarfOffset = new Vector2(-0.01f, 0.1f);

    [SerializeField]
    private Vector2 wallSlideScarfOffset = new Vector2(-0.01f, -0.1f);

    [SerializeField]
    private Vector2 dashScarfOffset = new Vector2(-0.01f, -0.1f);

    [SerializeField]
    private Vector2 attack01ScarfOffset = new Vector2(-0.01f, -0.1f);

    [Header("Scarf Anchor")]
    [SerializeField]
    private ScarfAnchor scarfAnchor;

    #endregion

    #region Camera
    [Header("Camera Stuff")]
    [SerializeField]
    private GameObject cameraFollowGO;
    private CameraFollowObject cameraFollowObject;
    private float fallYSpeedYDampingChangeThreshold;
    private CinemachineImpulseSource impulseSource;

    [Header("Screen Shake")]
    [SerializeField]
    private ScreenShakeProfile screenShakeProfile;
    public bool isBusy { get; private set; }

    #endregion

    #region State Machine
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerLandingState landingState { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerBlackHoleState blackHoleState { get; private set; }
    public PlayerDeadState deadState { get; private set; }

    #endregion

    #region Components

    public SkillManager skillManager { get; private set; }
    public GameObject sword { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        //State Machine -> initialize the state machine and the states that the player will have
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        landingState = new PlayerLandingState(this, stateMachine, "Idle");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");

        blackHoleState = new PlayerBlackHoleState(this, stateMachine, "Jump");

        deadState = new PlayerDeadState(this, stateMachine, "Die");

        //Input System -> initialize the input system and the actions that the player will have
        //controls = new PlayerControls();
    }

    protected override void Start()
    {
        base.Start();

        skillManager = SkillManager.instance;

        //State Machine -> initialize the state machine and the states that the player will have
        stateMachine.Initialize(idleState);

        cameraFollowObject = cameraFollowGO.GetComponent<CameraFollowObject>();

        fallYSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;

        impulseSource = GetComponent<CinemachineImpulseSource>();

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        base.Update();

        counterAttackCooldownTimer -= Time.deltaTime;

        //State Machine -> update the state machine and the states that the player will have
        stateMachine.CurrentState.Update();

        CheckForDashInput();
        UpdateScarfOffset();
        LerpYCamera();

        if (Input.GetKeyDown(KeyCode.F) && skillManager.crystalSkill.crystalUnlocked)
            skillManager.crystalSkill.CanUseSkill();

        //button to heal
        if (Input.GetKeyDown(KeyCode.H))
            Inventory.instance.UseFlask();
    }

    public override void SlowEntityBy(float _slowPercent, float _slowDuration)
    {
        moveSpeed *= (1 - _slowPercent);
        jumpForce *= (1 - _slowPercent);
        dashSpeed *= (1 - _slowPercent);
        animator.speed *= (1 - _slowPercent);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    public override void Flip()
    {
        base.Flip();
        if (IsGroundDetected())
            CreateDust();
        cameraFollowObject.CallTurn();
    }

    public override void DamageImpact()
    {
        base.DamageImpact();
        //shake the camera
        CameraShakeManager.Instance.ScreenShakeFromProfile(screenShakeProfile, impulseSource);
        //get player component from whatIsPlayer
        GetComponent<TimeStopWhenHit>()
            .StopTime(0.05f, 10, 0.1f);
        //not let the player move for a while
        StartCoroutine(BusyFor(0.2f));
    }

    private void LerpYCamera()
    {
        //if the player is falling, lerp the YDamping
        if (
            rb.velocity.y < fallYSpeedYDampingChangeThreshold
            && !CameraManager.instance.IsLerpingYDamping
            && !CameraManager.instance.LerpedFromPlayerFalling
        )
        {
            CameraManager.instance.LerpYDamping(true);
        }
        //if the player is standing still or moving up
        if (
            rb.velocity.y >= 0f
            && !CameraManager.instance.IsLerpingYDamping
            && CameraManager.instance.LerpedFromPlayerFalling
        )
        {
            //reset the YDamping to the normal amount, so it can be called again
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    }

    public IEnumerator BusyFor(float seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(seconds);
        isBusy = false;
    }

    private void CheckForDashInput()
    {
        if (IsWallDetected())
            return;

        if (skillManager.dashSkill.dashUnlocked == false)
            return;

        if (dashState != null)
        {
            if (
                (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.C))
                && SkillManager.instance.dashSkill.CanUseSkill()
            )
            {
                dashDirection = Input.GetAxisRaw("Horizontal");

                if (dashDirection == 0)
                    dashDirection = facingDirection;
                stateMachine.ChangeState(dashState);
            }
        }
    }

    private void UpdateScarfOffset()
    {
        Vector2 currentOffset = Vector2.zero;

        // idle
        if (stateMachine.CurrentState == idleState)
        {
            currentOffset = idleScarfOffset;
        }
        // jump
        else if (stateMachine.CurrentState == jumpState)
        {
            currentOffset = jumpScarfOffset;
        }
        // fall
        else if (rb.velocity.y < 0)
        {
            currentOffset = fallScarfOffset;
        }
        // run
        else if (rb.velocity.x != 0)
        {
            currentOffset = moveScarfOffset;
        }
        else if (
            stateMachine.CurrentState == wallSlideState && stateMachine.CurrentState != airState
        )
        {
            currentOffset = wallSlideScarfOffset;
        }
        else if (stateMachine.CurrentState == dashState)
        {
            currentOffset = dashScarfOffset;
        }
        else if (stateMachine.CurrentState == primaryAttack)
        {
            currentOffset = attack01ScarfOffset;
        }

        // flip x offset direction if we're facing left
        if (!isFacingRight)
        {
            currentOffset.x *= -1;
        }

        scarfAnchor.partOffset = currentOffset;
    }

    public void AnimationTrigger() => stateMachine.CurrentState.AnimationFinishTrigger();

    public void CreateDust() => dustParticle.Play();

    public void AssignNewSword(GameObject newSword) => sword = newSword;

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    #region Input System
    //private void OnEnable()
    //{
    //    //Input System -> Movement
    //    movementInputAction = controls.Player.Move;
    //    movementInputAction.Enable();
    //    //Input System -> Jump
    //    controls.Player.Jump.performed += Jump;
    //    controls.Player.Jump.Enable();
    //    //Input System -> Dash
    //    controls.Player.Dash.performed += Dash;
    //    controls.Player.Dash.Enable();
    //}

    //private void Dash(InputAction.CallbackContext context)
    //{
    //    //make the user dash
    //    if (dashState != null)
    //    {
    //        dashUsageTimer -= Time.deltaTime;
    //        if (dashUsageTimer < 0)
    //        {
    //            dashUsageTimer = dashCooldown;
    //            dashDirection = Input.GetAxisRaw("Horizontal");

    //            if (dashDirection == 0)
    //                dashDirection = facingDirection;
    //            stateMachine.ChangeState(dashState);
    //        }
    //    }
    //}

    //private void Jump(InputAction.CallbackContext context)
    //{
    //    //make the user jump
    //    if (IsGroundDetected())
    //        stateMachine.ChangeState(jumpState);
    //    else if (IsWallDetected() && !IsGroundDetected())
    //        stateMachine.ChangeState(wallSlideState);
    //}

    //private void OnDisable()
    //{
    //    movementInputAction.Disable();

    //    controls.Player.Jump.performed -= Jump;
    //    controls.Player.Jump.Disable();

    //    controls.Player.Dash.performed -= Dash;
    //    controls.Player.Dash.Disable();
    //}

    //private void FixedUpdate()
    //{


    //}

    #endregion
}
