using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shock Strike Effect", menuName = "Data/Item Effect/Shock Strike")]
public class ShockStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPosition.position, Quaternion.identity);

        Destroy(newThunderStrike, 1f);
    }
}
