using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major Stats")]
    public Stat strength; //1 point increase damage by 1 and crit damage by 1%
    public Stat agility; //1 point increase crit chance by 1% and dodge chance by 1%
    public Stat intelligence; //1 point increase magic damage and magic resistance by 3
    public Stat vitality; //1 point increase health by 3 or 5

    [Header("Offensive Stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower; //150% by default

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    //magic stuff
    public Stat magicResistance;

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited; //does damage over time
    public bool isFrozen; //reduce armor by 20%
    public bool isShocked; //reduce accuracy by 20%

    //timers
    private float ignitedTimer;
    private float frozenTimer;
    private float shockedTimer;

    [SerializeField] private float effectDuration = 3f;


    //ignited
    private float igniteDamageCooldown = 1f;
    private float igniteDamageTimer;
    private int igniteDamage;


    //shocked
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;


    public int currentHealth;

    public System.Action onHealthChanged;

    public bool isDead { get; private set; }

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        frozenTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;
        if (frozenTimer < 0)
            isFrozen = false;
        if (shockedTimer < 0)
            isShocked = false;

        if (isIgnited)
            ApplyIgniteDamage();
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCritDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        //DoMagicDamage(_targetStats);
        _targetStats.TakeDamage(totalDamage);
    }

    #region Magical Damage and Effects

    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);

        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0) return;

        AttemptToApplyEffects(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private void AttemptToApplyEffects(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyFreeze = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        float randomValue = Random.value;
        while (!canApplyIgnite && !canApplyFreeze && !canApplyShock)
        {
            if (_fireDamage > 0 && randomValue < 0.33f)
            {
                canApplyIgnite = true;
                _targetStats.ApplyEffects(canApplyIgnite, canApplyFreeze, canApplyShock);
                return;
            }
            if (_iceDamage > 0 && randomValue >= 0.33f && randomValue < 0.66f)
            {
                canApplyFreeze = true;
                _targetStats.ApplyEffects(canApplyIgnite, canApplyFreeze, canApplyShock);
                return;
            }
            if (_lightningDamage > 0 && randomValue >= 0.66f)
            {
                canApplyShock = true;
                _targetStats.ApplyEffects(canApplyIgnite, canApplyFreeze, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f));

        if (canApplyShock)
            _targetStats.SetupShockDamage(Mathf.RoundToInt(_lightningDamage * 0.2f));

        _targetStats.ApplyEffects(canApplyIgnite, canApplyFreeze, canApplyShock);
    }

    public void ApplyEffects(bool _isIgnited, bool _isFrozen, bool _isShocked)
    {
        bool canApplyIgnite = !isFrozen && !isIgnited && !isShocked;
        bool canApplyFreeze = !isFrozen && !isIgnited && !isShocked;
        bool canApplyShock = !isFrozen && !isIgnited;

        if (_isIgnited && canApplyIgnite)
        {
            isIgnited = _isIgnited;
            ignitedTimer = effectDuration;
            fx.IgniteFxFor(effectDuration);
        }
        if (_isFrozen && canApplyFreeze)
        {
            isFrozen = _isFrozen;
            frozenTimer = effectDuration;

            float slowPercentage = 0.2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, effectDuration);
            fx.FrozenFxFor(effectDuration);
        }
        if (_isShocked && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_isShocked);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;
                HitTargetsWithThunderStrike();
            }
        }
    }

    #region shock
    public void ApplyShock(bool _isShocked)
    {
        if (isShocked)
            return;
        shockedTimer = effectDuration;
        isShocked = _isShocked;

        fx.ShockFxFor(effectDuration);
    }

    private void HitTargetsWithThunderStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 30); // Aumente o raio para 30 unidades ou ajuste conforme necessário

        // Encontrar o inimigo mais próximo
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        // Ataque o inimigo mais próximo, se encontrado
        if (closestEnemy != null)
        {
            GameObject shockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            shockStrike.GetComponent<ThunderStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }

        // Iterar novamente para atacar todos os outros inimigos dentro de uma distância maior
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && hit.transform != closestEnemy)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy <= 30.0f) // Ajuste a distância conforme necessário
                {
                    // Ataque o inimigo
                    GameObject shockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
                    shockStrike.GetComponent<ThunderStrike_Controller>().Setup(shockDamage, hit.GetComponent<CharacterStats>());
                }
            }
        }
    }

    public void SetupShockDamage(int _damage) => shockDamage = _damage;
    #endregion

    #region ignite
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            Debug.Log("Take burn damage " + igniteDamage);

            DecreaseHealthBy(igniteDamage);

            if (currentHealth <= 0 && !isDead)
                Die();


            igniteDamageTimer = igniteDamageCooldown;
        }
    }
    #endregion

    #endregion

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);
        Debug.Log(transform.name + " takes " + _damage + " damage.");
        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");
        if (currentHealth <= 0 && !isDead)
            Die();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;
        onHealthChanged?.Invoke();
    }

    protected virtual void Die()
    {
        isDead = true;
        Debug.Log(transform.name + " died.");
    }

    #region Stats Calculations

    //chance of the target avoiding the attack
    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("Attack voided");
            return true;
        }
        return false;
    }

    //check if the target has armor and reduce the damage taken
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isFrozen)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = agility.GetValue() + critChance.GetValue();

        if (Random.Range(0, 100) < totalCriticalChance)
        {
            return true;
        }
        return false;
    }

    private int CalculateCritDamage(int totalDamage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;
        float critDamage = totalDamage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    #endregion
}
