using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillToolTip_UI : ToolTip_UI
{
    [SerializeField]
    private TextMeshProUGUI skillText;

    [SerializeField]
    private TextMeshProUGUI skillName;

    [SerializeField]
    private TextMeshProUGUI skillCost;

    [SerializeField]
    private float defaultNameFontSize;

    public void ShowToolTip(string _skillName, string _skillDescription, int _price)
    {
        skillName.text = _skillName;
        skillText.text = _skillDescription;
        skillCost.text = "Cost: " + _price;

        AdjustPosition();

        AdjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    }
}
