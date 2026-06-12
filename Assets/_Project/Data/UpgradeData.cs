using UnityEngine;

public enum UpgradeType
{
    MoveSpeed,
    FireRate,
    BulletDamage,
    DetectionRange,
    MaxHealth,
    Boomerang,
    Lightning
}

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public UpgradeType type;
    public float value;
    public bool isPercentage = true;

    [Tooltip("Bu upgrade en az hangi player level'dan itibaren listede gözüksün")]
    public int minPlayerLevel = 1;

    public void Apply(PlayerStats stats)
    {
        // Boomerang ve Lightning kendi level sistemine sahip (max 5), soft cap uygulanmaz
        if (type == UpgradeType.Boomerang)
        {
            stats.boomerangLevel = Mathf.Min(stats.maxBoomerangLevel, stats.boomerangLevel + 1);
            return;
        }
        if (type == UpgradeType.Lightning)
        {
            stats.lightningLevel = Mathf.Min(stats.maxLightningLevel, stats.lightningLevel + 1);
            return;
        }

        // Soft cap uygulanmış değer
        float effectiveValue = stats.GetEffectiveValue(type, value);

        switch (type)
        {
            case UpgradeType.MoveSpeed:
                stats.moveSpeed = isPercentage
                    ? stats.moveSpeed * (1f + effectiveValue)
                    : stats.moveSpeed + effectiveValue;
                break;
            case UpgradeType.FireRate:
                stats.fireRate = isPercentage
                    ? stats.fireRate * (1f - effectiveValue)
                    : Mathf.Max(0.05f, stats.fireRate - effectiveValue);
                break;
            case UpgradeType.BulletDamage:
                stats.bulletDamage = isPercentage
                    ? stats.bulletDamage * (1f + effectiveValue)
                    : stats.bulletDamage + effectiveValue;
                break;
            case UpgradeType.DetectionRange:
                stats.detectionRange = isPercentage
                    ? stats.detectionRange * (1f + effectiveValue)
                    : stats.detectionRange + effectiveValue;
                break;
            case UpgradeType.MaxHealth:
                float oldMax = stats.maxHealth;
                stats.maxHealth = isPercentage
                    ? stats.maxHealth * (1f + effectiveValue)
                    : stats.maxHealth + effectiveValue;

                float increase = stats.maxHealth - oldMax;

                // Health component'ini de güncelle + artan kadar can yenile
                Health playerHealth = stats.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(stats.maxHealth);
                    playerHealth.Heal(increase);
                }
                break;
        }

        stats.IncrementStack(type);
    }
}