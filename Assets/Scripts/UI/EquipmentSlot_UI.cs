using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot_UI : ItemSlot_UI
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "Equipment Slot - " + slotType.ToString();
    }

    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (item == null || item.itemData == null)
            return;

        Inventory.instance.UnEquipItem(item.itemData as ItemData_Equipment);
        Inventory.instance.AddItem(item.itemData as ItemData_Equipment);

        ui.itemToolTip.HideToolTip();

        CleanUpSlot();
    }
}
