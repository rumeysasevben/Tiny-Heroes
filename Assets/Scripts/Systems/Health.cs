using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Color damageNumberColor = Color.yellow;
    [SerializeField] private Color healColor = new Color(0.3f, 1f, 0.3f); // yeşil
    [SerializeField] private Color critColor = new Color(0.5f, 0.8f, 0.1f);
    [Header("Damage Popup")]
    [SerializeField] private GameObject damageNumberPrefab;
    private float currentHealth;

    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private float baseMaxHealth = -1f;

    public float BaseMaxHealth
    {
        get
        {
            if (baseMaxHealth < 0f) baseMaxHealth = maxHealth;
            return baseMaxHealth;
        }
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"{gameObject.name} spawned HP:{currentHealth}/{maxHealth} tag:{gameObject.tag} layer:{LayerMask.LayerToName(gameObject.layer)}");
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount) => TakeDamage(amount, false);

    public void TakeDamage(float amount, bool isCrit)
    {
        if (currentHealth <= 0f) return;
        CameraFollow.Instance?.Shake(isCrit ? 0.15f : 0.08f, isCrit ? 0.12f : 0.01f);
        if (isCrit) HitPause.Instance?.Freeze(0.01f);
        currentHealth -= amount;

        if (damageNumberPrefab != null)
        {
            GameObject popup = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
            var dn = popup.GetComponent<DamageNumber>();
            if (dn != null) dn.SetDamage(Mathf.RoundToInt(amount), isCrit ? critColor : damageNumberColor);
            if (isCrit) popup.transform.localScale *= 1.6f;
        }

        var flash = GetComponent<HitFlash>();
        if (flash != null) flash.Flash();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0f) return;
        float actualHeal = Mathf.Min(amount, maxHealth - currentHealth);
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (damageNumberPrefab != null && actualHeal > 0f)
        {
            GameObject popup = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
            var dn = popup.GetComponent<DamageNumber>();
            if (dn != null) dn.SetText("+" + Mathf.RoundToInt(actualHeal) + " HP", healColor);
        }
    }

    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
        currentHealth = newMax;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}