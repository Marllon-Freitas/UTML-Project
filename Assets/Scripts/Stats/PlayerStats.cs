using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (_damage > GetMaxHealthValue() * 0.3f)
        {
            player.SetupKnockBackPower(new Vector2(10, 6));
            CameraShakeManager.Instance.ScreenShakeFromProfile(
                player.playersTakesALotOfDamageScreenShakeProfile,
                player.impulseSource
            );
            AudioManager.instance.PlaySoundEffect(47, null);
            Debug.Log("Player is taking a lot of damage");
        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        Debug.Log("Current armor: " + currentArmor);

        if (currentArmor != null)
            currentArmor.Effect(player.transform);
    }

    public override void OnEvasion()
    {
        player.skillManager.dodgeSkill.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _attackMultiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();
        if (_attackMultiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _attackMultiplier);

        if (CanCrit())
        {
            totalDamage = CalculateCritDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicDamage(_targetStats); //remove if you don't want magic damage
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        GameManager.instance.lostSoulsAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;

        // make the  camerashake stop
        CameraShakeManager.Instance.StopShake();

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }
}
