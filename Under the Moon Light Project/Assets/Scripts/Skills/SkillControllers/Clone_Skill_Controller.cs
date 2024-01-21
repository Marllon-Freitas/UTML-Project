using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField]
    private float colorLosingSpeed;

    private float cloneTimer;

    [SerializeField]
    private float attackMultiplier;

    [SerializeField]
    private Transform attackCheck;

    [SerializeField]
    private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private int facingDir = 1;

    private bool canDuplicateClone;
    private float chanceToDuplicate;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            spriteRenderer.color = new Color(
                1,
                1,
                1,
                spriteRenderer.color.a - (Time.deltaTime * colorLosingSpeed)
            );

            if (spriteRenderer.color.a <= 0)
                Destroy(gameObject);
        }
    }

    public void SetupClone(
        Transform _newTransform,
        float _cloneDuration,
        bool _canAttack,
        Vector3 _offset,
        Transform _closestEnemy,
        bool _canDuplicate,
        float _chanceToDuplicate,
        Player _player,
        float _attackMultiplier
    )
    {
        if (_canAttack)
            animator.SetInteger("AttackNumber", Random.Range(1, 3));

        attackMultiplier = _attackMultiplier;
        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        closestEnemy = _closestEnemy;
        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -0.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            attackCheck.position,
            attackCheckRadius
        );

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();
                PlayerStats playerStats = player.GetComponent<PlayerStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier);

                if (player.skillManager.cloneSkill.canApplyOnHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipment(
                        EquipmentType.Weapon
                    );

                    if (weaponData != null)
                        weaponData.Effect(hit.transform);
                }

                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.cloneSkill.CreateClone(
                            hit.transform,
                            new Vector3(.5f * facingDir, 0)
                        );
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()
    {
        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
