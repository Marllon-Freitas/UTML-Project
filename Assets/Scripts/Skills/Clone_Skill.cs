using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone Skill Info")]
    [SerializeField]
    private float attackMultiplier;

    [SerializeField]
    private GameObject clonePrefab;

    [SerializeField]
    private float cloneDuration;

    [Space]
    [Header("Clone Attack")]
    [SerializeField]
    private SkillTreeSlot_UI cloneAttackUnlockButton;

    [SerializeField]
    private float cloneAttackMultiplier;

    [SerializeField]
    private bool canAttack;

    [Space]
    [Header("Agressive Clone")]
    [SerializeField]
    private SkillTreeSlot_UI agressiveCloneUnlockButton;

    [SerializeField]
    private float agressiveCloneMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Space]
    [Header("Multiple Clones")]
    [SerializeField]
    private SkillTreeSlot_UI multipleClonesUnlockButton;

    [SerializeField]
    private float multipleClonesAttackMultiplier;

    [SerializeField]
    private bool canDuplicateClone;

    [SerializeField]
    private float chanceToDuplicate;

    [Space]
    [Header("Crystal instead of clone")]
    [SerializeField]
    private SkillTreeSlot_UI crystalInsteadOfCloneUnlockButton;
    public bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        agressiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAgressiveClone);
        multipleClonesUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultipleClones);
        crystalInsteadOfCloneUnlockButton
            .GetComponent<Button>()
            .onClick.AddListener(UnlockCrystalInsteadOfClone);
    }

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAgressiveClone();
        UnlockMultipleClones();
        UnlockCrystalInsteadOfClone();
    }

    #region Unlock region

    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAgressiveClone()
    {
        if (agressiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = agressiveCloneMultiplier;
        }
    }

    private void UnlockMultipleClones()
    {
        if (multipleClonesUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multipleClonesAttackMultiplier;
        }
    }

    private void UnlockCrystalInsteadOfClone()
    {
        if (crystalInsteadOfCloneUnlockButton.unlocked)
            crystalInsteadOfClone = true;
    }

    #endregion

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
                player,
                attackMultiplier
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
