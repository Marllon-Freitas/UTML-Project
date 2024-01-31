using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry Skill")]
    [SerializeField]
    private SkillTreeSlot_UI parryUnlockButton;
    public bool parryUnlocked { get; private set; }

    [Header("Parry Restore Health")]
    [SerializeField]
    private SkillTreeSlot_UI parryRestoreHealthUnlockButton;

    [Range(0f, 1f)]
    [SerializeField]
    private float parryRestoreHealthPercentage;
    public bool parryRestoreHealthUnlocked { get; private set; }

    [Header("Parry With Mirage")]
    [SerializeField]
    private SkillTreeSlot_UI parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked { get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();

        if (parryRestoreHealthUnlocked)
        {
            int restoreHealthAmount = Mathf.RoundToInt(
                player.characterStats.GetMaxHealthValue() * parryRestoreHealthPercentage
            );
            player.characterStats.IncreaseHealthBy(restoreHealthAmount);
        }
    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        parryRestoreHealthUnlockButton
            .GetComponent<Button>()
            .onClick.AddListener(UnlockParryRestoreHealth);
        parryWithMirageUnlockButton
            .GetComponent<Button>()
            .onClick.AddListener(UnlockParryWithMirage);
    }

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockParryRestoreHealth();
        UnlockParryWithMirage();
    }

    private void UnlockParry()
    {
        if (parryUnlockButton == true)
            parryUnlocked = true;
    }

    private void UnlockParryRestoreHealth()
    {
        if (parryRestoreHealthUnlockButton == true)
            parryRestoreHealthUnlocked = true;
    }

    private void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlockButton == true)
            parryWithMirageUnlocked = true;
    }

    public void MakeMirageOnParry(Transform _respawnPosition)
    {
        if (parryWithMirageUnlocked)
            player.skillManager.cloneSkill.CreateCloneWithDelay(_respawnPosition);
    }
}
