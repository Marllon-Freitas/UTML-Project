using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone Skill Info")]
    [SerializeField]
    private GameObject clonePrefab;

    [SerializeField]
    private float cloneDuration;

    [Space]
    [SerializeField]
    private bool canAttack;

    [Space]
    [Header("Clone can duplicate")]
    [SerializeField]
    private bool canDuplicateClone;

    [SerializeField]
    private float chanceToDuplicate;

    [Space]
    [Header("Crystal instead of clone")]
    public bool crystalInsteadOfClone;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystalSkill.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone
            .GetComponent<Clone_Skill_Controller>()
            .SetupClone(
                _clonePosition,
                cloneDuration,
                canAttack,
                _offset,
                FindClosestEnemy(newClone.transform),
                canDuplicateClone,
                chanceToDuplicate,
                player
            );
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(
            CloneDelayCoroutine(_enemyTransform, new Vector3(1 * player.facingDirection, 0))
        );
    }

    private IEnumerator CloneDelayCoroutine(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_transform, _offset);
    }
}
