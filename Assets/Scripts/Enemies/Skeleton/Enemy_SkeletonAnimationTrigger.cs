using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SkeletonAnimationTrigger : MonoBehaviour
{
    private Enemy_Skeleton enemy => GetComponentInParent<Enemy_Skeleton>();

    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                enemy.characterStats.DoDamage(target);
            }
        }
    }

    protected void OpenCounterWidow() => enemy.OpenCounterAttackWindow();
    protected void CloseCounterWidow() => enemy.CloseCounterAttackWindow();
}
