using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Attack Details")]
    public Transform attackCheck;
    public float attackCheckRadius;

    [Header("Collision Info")]
    [SerializeField]
    protected Transform groundCheck;

    [SerializeField]
    protected float groundCheckDistance;

    [SerializeField]
    protected LayerMask whatIsGround;
    public int knockBackDir { get; private set; }

    [Space]
    [SerializeField]
    protected Transform wallCheck;

    [SerializeField]
    protected float wallCheckDistance;

    #region Components
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX entityFX { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public CharacterStats characterStats { get; private set; }
    public CapsuleCollider2D capsuleCollider { get; private set; }
    #endregion

    [Header("Knock back Info")]
    [SerializeField]
    protected Vector2 knockBackDirection;

    [SerializeField]
    protected float knockBackDuration;
    protected bool isKnocked;

    public int facingDirection { get; private set; } = 1;
    public bool isFacingRight = true;

    public System.Action onFlipped;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        entityFX = GetComponent<EntityFX>();
        animator = GetComponentInChildren<Animator>();
        characterStats = GetComponent<CharacterStats>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update() { }

    protected virtual void ReturnDefaultSpeed()
    {
        animator.speed = 1;
    }

    public virtual void SlowEntityBy(float _slowPercent, float _slowDuration) { }

    #region Collision
    public virtual bool IsGroundDetected() =>
        Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    public virtual bool IsWallDetected() =>
        Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * facingDirection,
            wallCheckDistance,
            whatIsGround
        );

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            groundCheck.position,
            new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance)
        );
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            wallCheck.position,
            new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y)
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            isFacingRight ? 0 : 180,
            transform.eulerAngles.z
        );

        onFlipped?.Invoke();
    }

    public void FlipController(float x)
    {
        if (x > 0 && !isFacingRight)
            Flip();
        else if (x < 0 && isFacingRight)
            Flip();
    }
    #endregion

    #region Velocity
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;
        rb.velocity = Vector2.zero;
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;
        rb.velocity = new Vector2(xVelocity, yVelocity);
        FlipController(xVelocity);
    }
    #endregion

    #region Attack

    public virtual void DamageImpact() => StartCoroutine("HitKnockBack");

    public virtual void SetupKnockBackDirection(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockBackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockBackDir = 1;
    }

    protected virtual void SetupZeroKnockBackDirection() { }

    public void SetupKnockBackPower(Vector2 knockBackPower) => knockBackDirection = knockBackPower;

    #endregion

    #region Knock back
    protected virtual IEnumerator HitKnockBack()
    {
        isKnocked = true;
        rb.velocity = new Vector2(knockBackDirection.x * knockBackDir, knockBackDirection.y);
        yield return new WaitForSeconds(knockBackDuration);
        isKnocked = false;
        SetupZeroKnockBackDirection();
    }
    #endregion

    public virtual void Die() { }
}
