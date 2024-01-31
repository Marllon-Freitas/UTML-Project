using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;

    public void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public bool HaveEnoughMoney(int _amount)
    {
        if (_amount > currency)
        {
            Debug.Log("Not enough money");
            return false;
        }

        currency -= _amount;
        return true;
    }

    public int CurrentCurrencyAumont()
    {
        return currency;
    }

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}
