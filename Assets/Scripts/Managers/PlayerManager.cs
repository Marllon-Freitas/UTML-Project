using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
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
}
