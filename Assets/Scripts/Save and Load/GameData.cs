using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currency;
    public SerializableDictionary<string, int> inventory;

    public GameData()
    {
        this.currency = 0;
        this.inventory = new SerializableDictionary<string, int>();
    }
}
