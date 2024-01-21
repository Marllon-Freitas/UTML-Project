using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider2D;
    private Player player;

    private bool canRotate = true;
    private bool isReturning = false;

    private float freezeTimeDuration;
    private float returnSpeed;

    #region Bounce Sword
    [Header("Bounce Sword Info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;
    #endregion

    #region Pierce Sword
    [Header("Pierce Sword Info")]
    private int pierceAmount;
    #endregion

    #region Spin Sword
    [Header("Spin Sword Info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;
    #endregion

    #region Damage
    [Header("Damage Info")]
    private float hitTimer;
    private float hitCooldown;
    #endregion

    private float spinDirection;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    public void SetupSword(
        Vector2 direction,
        float gravityScale,
        Player _player,
        float _freezeTimeDuration,
        float _returnSpeed
    )
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        rb.velocity = direction;
        rb.gravityScale = gravityScale;

        if (pierceAmount <= 0)
            animator.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.SetBool("Rotation", true);
        transform.parent = null;
        isReturning = true;
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounces, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(
        bool _isSpinning,
        float _maxTravelDistance,
        float _spinDuration,
        float _hitCooldown
    )
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                returnSpeed * Time.deltaTime
            );
            if (Vector2.Distance(transform.position, player.transform.position) < 0.2f)
                player.CatchTheSword();
        }

        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (
                Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance
                && !wasStopped
            )
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    new Vector2(transform.position.x + spinDirection, transform.position.y),
                    .5f * Time.deltaTime
                );

                if (spinTimer < 0)
                {
                    isSpinning = false;
                    isReturning = true;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                enemyTarget[targetIndex].position,
                bounceSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
                targetIndex++;
                bounceAmount--;
                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
                if (targetIndex >= enemyTarget.Count)
                {
                    targetIndex = 0;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }
        //collision.GetComponent<Enemy>()?.DamageEffect();
        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        player.characterStats.DoDamage(enemyStats);

        if (player.skillManager.swordThrowSkill.timeStopUnlocked)
            enemy.FreezeTimeFor(freezeTimeDuration);

        if (player.skillManager.swordThrowSkill.vulnerableUnlocked)
            enemyStats.MakeVulnerableFor(freezeTimeDuration);

        ItemData_Equipment equipAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

        if (equipAmulet != null)
            equipAmulet.Effect(enemy.transform);
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        circleCollider2D.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
            return;
        animator.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
