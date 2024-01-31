using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    [Header("Inventory UI")]
    [SerializeField]
    private Transform inventorySlotParent;

    [SerializeField]
    private Transform stashSlotParent;

    [SerializeField]
    private Transform equipmentSlotParent;

    [SerializeField]
    private Transform statSlotParent;

    private ItemSlot_UI[] inventoryItemSlots;
    private ItemSlot_UI[] stashItemSlots;
    private EquipmentSlot_UI[] equipmentItemSlots;
    private StatSlot_UI[] statSlots;

    [Header("Items cooldown")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    public float flaskCooldown;
    private float armorCooldown;

    [Header("Data base")]
    public List<InventoryItem> loadedItems;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlots = inventorySlotParent.GetComponentsInChildren<ItemSlot_UI>();
        stashItemSlots = stashSlotParent.GetComponentsInChildren<ItemSlot_UI>();
        equipmentItemSlots = equipmentSlotParent.GetComponentsInChildren<EquipmentSlot_UI>();
        statSlots = statSlotParent.GetComponentsInChildren<StatSlot_UI>();
        AddStartingItems();
    }

    private void AddStartingItems()
    {
        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.itemData);
                }
            }

            return;
        }

        for (int i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
                AddItem(startingItems[i]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ItemData newItem = inventory[inventory.Count - 1].itemData;
            RemoveItem(newItem);
        }
    }

    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = item.Key;
        }

        if (oldEquipment != null)
        {
            UnEquipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);

        newEquipment.AddModifiers();

        RemoveItem(_item);

        UpdateSlotUI();
    }

    public void UnEquipItem(ItemData_Equipment _itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(_itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(_itemToRemove);
            _itemToRemove.RemoveModifiers();
        }
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < equipmentItemSlots.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentItemSlots[i].slotType)
                    equipmentItemSlots[i].UpdateSlot(item.Value);
            }
        }

        for (int i = 0; i < inventoryItemSlots.Length; i++)
        {
            inventoryItemSlots[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlots.Length; i++)
        {
            stashItemSlots[i].CleanUpSlot();
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlots[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlots[i].UpdateSlot(stash[i]);
        }

        UpsateStasUI();
    }

    public void UpsateStasUI()
    {
        for (int i = 0; i < statSlots.Length; i++)
        {
            statSlots[i].UpdateStateValueUi();
        }
    }

    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlots.Length)
        {
            Debug.Log("Inventory is full");
            return false;
        }
        return true;
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);
        else
            Debug.LogError("Item type not found");

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
                value.RemoveStack();
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI();
    }

    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (
                stashDictionary.TryGetValue(
                    _requiredMaterials[i].itemData,
                    out InventoryItem stashValue
                )
            )
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("not enough materials");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("not enough materials");
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].itemData);
        }

        AddItem(_itemToCraft);
        Debug.Log("Here is your item " + _itemToCraft.name);

        return true;
    }

    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
                equipedItem = item.Key;
        }

        return equipedItem;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
            Debug.Log("Flask on cooldown;");
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        Debug.Log("Armor on cooldown");
        return false;
    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach (var item in GetItemDataBase())
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        Debug.Log("Saving inventory data");
        _data.inventory.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }
    }

    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetsName = AssetDatabase.FindAssets(
            "",
            new[] { "Assets/DataItens/Equipements" }
        );
        foreach (string SOName in assetsName)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
}
