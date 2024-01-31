using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatToolTip_UI : ToolTip_UI
{
    [SerializeField]
    private TextMeshProUGUI description;

    public void ShowStatToolTip(string _text)
    {
        description.text = _text;
        AdjustPosition();
        gameObject.SetActive(true);
    }

    public void HideStatToolTip()
    {
        description.text = "";
        gameObject.SetActive(false);
    }
}
