using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice And Fire Effect", menuName = "Data/Item Effect/Ice And Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private Vector2 newVelocity;

    public override void ExecuteEffect(Transform _respawnPosition)
    {
        Player player = PlayerManager.instance.player;
        bool isThirdAttack = player.primaryAttack.comboCounter == 2;

        if (!isThirdAttack) return;

        GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _respawnPosition.position, player.transform.rotation);
        newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(player.facingDirection * newVelocity.x, newVelocity.y);

        Destroy(newIceAndFire, 10f);
    }
}
