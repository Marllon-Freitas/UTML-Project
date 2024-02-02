using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currency;
    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentId;
    public SerializableDictionary<string, bool> checkpoints;

    public SerializableDictionary<string, float> volumeSettings;
    public string closestCheckpointId;

    public float lostSoulsXPosition;
    public float lostSoulsYPosition;
    public int lostSoulsAmount;

    public GameData()
    {
        this.lostSoulsXPosition = 0;
        this.lostSoulsYPosition = 0;
        this.lostSoulsAmount = 0;

        this.currency = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        closestCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();
        volumeSettings = new SerializableDictionary<string, float>();
    }
}
