using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindow_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;

    [SerializeField] private Image[] materialImage;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {

        Debug.Log("SetupCraftWindow " + _data);
        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        for (int i = 0; i < _data.craftingMaterials.Count; i++)
        {
            if (_data.craftingMaterials.Count > materialImage.Length)
                Debug.LogWarning("You have more materials amount than you have material slots in craft window");


            materialImage[i].sprite = _data.craftingMaterials[i].itemData.itemIcon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.text = _data.craftingMaterials[i].stackSize.ToString();
            materialSlotText.color = Color.white;
        }

        itemIcon.sprite = _data.itemIcon;
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription();

        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(_data, _data.craftingMaterials));
    }
}
