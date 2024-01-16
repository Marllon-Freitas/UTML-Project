using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item Effect/Heal effect")]
public class Heal_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float healPercent;
    public override void ExecuteEffect(Transform _enemyPosition)
    {
        //get player Stats
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //how much is to heal
        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);

        //heal
        playerStats.IncreaseHealthBy(healAmount);
    }
}
