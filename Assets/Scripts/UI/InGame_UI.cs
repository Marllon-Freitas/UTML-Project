using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGame_UI : MonoBehaviour
{
    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private Slider healthBarSlider;

    [SerializeField]
    private Image dashImage;

    [SerializeField]
    private Image cristalImage;

    [SerializeField]
    private Image swordThrowImage;

    [SerializeField]
    private Image parryImage;

    [SerializeField]
    private Image blackHoleImage;

    [SerializeField]
    private Image flaskImage;

    [SerializeField]
    private TextMeshProUGUI currentSouls;

    private SkillManager skills;

    // Start is called before the first frame update
    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

        skills = SkillManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        currentSouls.text = PlayerManager.instance.CurrentCurrencyAumont().ToString("#.#");

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dashSkill.dashUnlocked)
            SetColldownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.E) && skills.parrySkill.parryUnlocked)
            SetColldownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.F) && skills.crystalSkill.crystalUnlocked)
            SetColldownOf(cristalImage);

        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.swordThrowSkill.swordUnlockd)
            SetColldownOf(swordThrowImage);

        if (Input.GetKeyDown(KeyCode.R) && skills.blackHoleSkill.blackHoleUnlocked)
            SetColldownOf(blackHoleImage);

        if (Input.GetKeyDown(KeyCode.H) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetColldownOf(flaskImage);

        CheckCollDownOf(dashImage, skills.dashSkill.cooldown);
        CheckCollDownOf(parryImage, skills.parrySkill.cooldown);
        CheckCollDownOf(cristalImage, skills.crystalSkill.cooldown);
        CheckCollDownOf(swordThrowImage, skills.swordThrowSkill.cooldown);
        CheckCollDownOf(blackHoleImage, skills.blackHoleSkill.cooldown);

        CheckCollDownOf(flaskImage, Inventory.instance.flaskCooldown);
    }

    private void UpdateHealthUI()
    {
        healthBarSlider.maxValue = playerStats.GetMaxHealthValue();
        healthBarSlider.value = playerStats.currentHealth;
    }

    private void SetColldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCollDownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }
}
