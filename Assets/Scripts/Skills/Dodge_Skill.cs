using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("Dodge Skill Settings")]
    [SerializeField]
    private SkillTreeSlot_UI unlockDodgeButton;

    [SerializeField]
    private int evasionAmount;
    public bool dodgeUnlocked;

    [Header("Mirage Dodge")]
    [SerializeField]
    private SkillTreeSlot_UI unlockMirageDodgeButton;
    public bool mirageDodgeUnlocked;

    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
    }

    private void UnlockDodge()
    {
        if (unlockDodgeButton.unlocked && !dodgeUnlocked)
        {
            player.characterStats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpsateStasUI();
            dodgeUnlocked = true;
        }
    }

    private void UnlockMirageDodge()
    {
        if (unlockMirageDodgeButton.unlocked)
            mirageDodgeUnlocked = true;
    }

    public void CreateMirageOnDodge()
    {
        if (mirageDodgeUnlocked)
            SkillManager.instance.cloneSkill.CreateClone(
                player.transform,
                new Vector3(2 * player.facingDirection, 0, 0)
            );
    }
}
