using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI hpText;

    private void Start()
    {
        if (targetHealth == null || slider == null) return;

        slider.maxValue = targetHealth.MaxHealth;
        slider.value = targetHealth.CurrentHealth;

        UpdateBar(targetHealth.CurrentHealth, targetHealth.MaxHealth);
        
        targetHealth.OnHealthChanged += UpdateBar;
    }

    private void OnDestroy()
    {
        if (targetHealth != null)
            targetHealth.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(float current, float max)
    {
        slider.value = current;
        if (hpText != null)
        {
            hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }
}