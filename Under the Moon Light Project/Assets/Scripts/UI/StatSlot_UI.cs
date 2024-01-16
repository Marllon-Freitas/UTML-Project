using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatSlot_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private StatType statType;
    [SerializeField] private string sateName;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + sateName;

        if (statNameText != null)
            statNameText.text = sateName;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateStateValueUi();

        ui = GetComponentInParent<UI>();
    }

    public void UpdateStateValueUi()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats == null)
            return;

        statValueText.text = playerStats.GetStat(statType).GetValue().ToString();

        if (statType == StatType.health)
            statValueText.text = playerStats.GetMaxHealthValue().ToString();

        if (statType == StatType.damage)
            statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();

        if (statType == StatType.critPower)
            statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();

        if (statType == StatType.critChance)
            statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();

        if (statType == StatType.evasion)
            statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();

        if (statType == StatType.magicRes)
            statValueText.text = (playerStats.magicResistance.GetValue() + playerStats.intelligence.GetValue() * 3).ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowStatToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.HideStatToolTip();
    }
}
