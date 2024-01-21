using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSlot_UI : ItemSlot_UI
{

    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
            return;

        item.itemData = _data;
        itemImage.sprite = _data.itemIcon;
        itemText.text = _data.itemName;

        if (itemText.text.Length > 12)
            itemText.fontSize = itemText.fontSize * 0.7f;
        else
            itemText.fontSize = 24;

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(item.itemData as ItemData_Equipment);
    }
}
