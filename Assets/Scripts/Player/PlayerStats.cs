using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat")]
    [Range(0f, 1f)] public float critChance = 0.15f;
    public float critMultiplier = 2f;
    public float fireRate = 0.3f;
    public float bulletDamage = 10f;
    public float detectionRange = 7f;

    [Header("Survival")]
    public float maxHealth = 100f;

    [Header("Soft Cap (Diminishing Returns)")]
    [Tooltip("Bu sayıya kadar tam etki, sonra azalmaya başlar")]
    public int softCapThreshold = 5;
    [Tooltip("Threshold sonrası her seçimde etki bununla çarpılır (0.7 = %30 azalma)")]
    [Range(0.1f, 1f)] public float softCapDecay = 0.7f;

    [Header("Boomerang")]
    public int boomerangLevel = 0;
    public int maxBoomerangLevel = 5;
    public bool HasBoomerang => boomerangLevel >= 1;

    public float GetBoomerangDamage()
    {
        float[] damages = { 20f, 30f, 45f, 65f, 90f };
        return damages[Mathf.Clamp(boomerangLevel - 1, 0, damages.Length - 1)];
    }
    public float GetBoomerangCooldown()
    {
        float[] cd = { 2.0f, 1.7f, 1.5f, 1.3f, 1.1f };
        return cd[Mathf.Clamp(boomerangLevel - 1, 0, cd.Length - 1)];
    }
    public int GetBoomerangCount()
    {
        int[] c = { 1, 1, 2, 2, 3 };
        return c[Mathf.Clamp(boomerangLevel - 1, 0, c.Length - 1)];
    }

    [Header("Lightning")]
    public int lightningLevel = 0;
    public int maxLightningLevel = 5;
    public bool HasLightning => lightningLevel >= 1;

    public int GetLightningCount()
    {
        int[] c = { 2, 2, 3, 3, 4 };
        return c[Mathf.Clamp(lightningLevel - 1, 0, c.Length - 1)];
    }
    public float GetLightningDamage()
    {
        float[] d = { 15f, 22f, 30f, 42f, 60f };
        return d[Mathf.Clamp(lightningLevel - 1, 0, d.Length - 1)];
    }
    public float GetLightningRadius()
    {
        float[] r = { 1f, 1.2f, 1.4f, 1.6f, 1.8f };
        return r[Mathf.Clamp(lightningLevel - 1, 0, r.Length - 1)];
    }
    public float GetLightningOrbitSpeed()
    {
        float[] s = { 80f, 95f, 110f, 125f, 140f };
        return s[Mathf.Clamp(lightningLevel - 1, 0, s.Length - 1)];
    }

    // --- Upgrade stack tracking ---
    private Dictionary<UpgradeType, int> upgradeStacks = new Dictionary<UpgradeType, int>();

    public int GetStackCount(UpgradeType type)
    {
        return upgradeStacks.TryGetValue(type, out int count) ? count : 0;
    }

    public void IncrementStack(UpgradeType type)
    {
        if (!upgradeStacks.ContainsKey(type))
            upgradeStacks[type] = 0;
        upgradeStacks[type]++;
    }

    /// <summary>
    /// Threshold'a kadar tam değer, sonrasında her seçimde decay ile azalır.
    /// </summary>
    public float GetEffectiveValue(UpgradeType type, float baseValue)
    {
        int stack = GetStackCount(type);
        if (stack < softCapThreshold) return baseValue;
        return baseValue * Mathf.Pow(softCapDecay, stack - softCapThreshold + 1);
    }

    // --- Damage Boost (ChanceBox) ---
    private float damageMultiplier = 1f;
    public float DamageMultiplier => damageMultiplier;

    public void ApplyDamageBoost(float multiplier, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DamageBoostRoutine(multiplier, duration));
    }

    private IEnumerator DamageBoostRoutine(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
    }
}