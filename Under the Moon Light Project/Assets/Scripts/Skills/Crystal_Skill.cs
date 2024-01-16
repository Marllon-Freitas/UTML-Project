using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField]
    private float crystalDuration;

    [SerializeField]
    private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal")]
    [SerializeField]
    private SkillTreeSlot_UI unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Crystal mirage")]
    [SerializeField]
    private bool cloneInsteadOfCrystal;

    [SerializeField]
    private SkillTreeSlot_UI unlockCrystalMirageButton;
    public bool crystalMirageUnlocked { get; private set; }

    [Header("Explosive crystal")]
    [SerializeField]
    private bool canExplode;

    [SerializeField]
    private SkillTreeSlot_UI unlockExplosiveCrystalButton;
    public bool explosiveCrystalUnlocked { get; private set; }

    [Header("Moving crystal")]
    [SerializeField]
    private bool canMoveToEnemy;

    [SerializeField]
    private SkillTreeSlot_UI unlockMovingCrystalButton;
    public bool movingCrystalUnlocked { get; private set; }

    [SerializeField]
    private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField]
    private SkillTreeSlot_UI unlockMultiStackingCrystalButton;
    public bool multiStackingCrystalUnlocked { get; private set; }

    [SerializeField]
    private bool canUseMultiStacks;

    [SerializeField]
    private int amountOfStacks;

    [SerializeField]
    private float multiStackCooldown;

    [SerializeField]
    private float useTimeWindow;

    [SerializeField]
    private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCrystalMirageButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveCrystalButton
            .GetComponent<Button>()
            .onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackingCrystalButton
            .GetComponent<Button>()
            .onClick.AddListener(UnlockMultiStackingCrystal);
    }

    #region  Unlocking skills
    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if (unlockCrystalMirageButton.unlocked)
            crystalMirageUnlocked = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveCrystalButton.unlocked)
            explosiveCrystalUnlocked = true;
    }

    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
            movingCrystalUnlocked = true;
    }

    private void UnlockMultiStackingCrystal()
    {
        if (unlockMultiStackingCrystalButton.unlocked)
            multiStackingCrystalUnlocked = true;
    }

    #endregion
    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.cloneSkill.CreateClone(
                    currentCrystal.transform,
                    Vector3.zero
                );
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript =
            currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(
            crystalDuration,
            canExplode,
            canMoveToEnemy,
            moveSpeed,
            FindClosestEnemy(currentCrystal.transform),
            player
        );
    }

    public void CurrentCrystalChooseRandomTarget() =>
        currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWindow);

                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(
                    crystalToSpawn,
                    player.transform.position,
                    Quaternion.identity
                );

                crystalLeft.Remove(crystalToSpawn);

                newCrystal
                    .GetComponent<Crystal_Skill_Controller>()
                    .SetupCrystal(
                        crystalDuration,
                        canExplode,
                        canMoveToEnemy,
                        moveSpeed,
                        FindClosestEnemy(newCrystal.transform),
                        player
                    );

                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }

                return true;
            }
        }

        return false;
    }

    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }
}
