using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftList_UI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> craftListEquipment;
    void Start()
    {
        transform.parent.GetChild(0).GetComponent<CraftList_UI>().SetupCraftList();
        transform.parent.GetChild(0).GetComponent<CraftList_UI>().SetupDefaultCraftWindow();
        //GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftListEquipment[0]);
    }

    public void SetupCraftList()
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < craftListEquipment.Count; i++)
        {
            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
            newSlot.GetComponent<CraftSlot_UI>().SetupCraftSlot(craftListEquipment[i]);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }

    public void SetupDefaultCraftWindow()
    {
        Debug.Log("craftListEquipment " + craftListEquipment[0]);

        if (craftListEquipment[0] != null)
            GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftListEquipment[0]);
    }
}
