using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrops;
    private List<ItemData> dropsList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrops.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrops[i].dropChance)
            {
                dropsList.Add(possibleDrops[i]);
            }
        }

        for (int i = 0; i < possibleItemDrop; i++)
        {
            int randomIndex = Random.Range(0, dropsList.Count - 1);
            ItemData randomItem = dropsList[randomIndex];

            dropsList.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        Vector2 randomVelocity = new Vector2(Random.Range(-5f, 5f), Random.Range(15, 20));
        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
}
