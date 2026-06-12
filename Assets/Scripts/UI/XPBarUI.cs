using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    [SerializeField] private PlayerLevel targetLevel;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI xpText;

    private void Start()
    {
        if (targetLevel == null || slider == null) return;

        slider.maxValue = targetLevel.XPToNextLevel;
        slider.value = targetLevel.CurrentXP;

        UpdateBar(targetLevel.CurrentXP, targetLevel.XPToNextLevel);

        targetLevel.OnXPChanged += UpdateBar;
        targetLevel.OnLevelUp += OnLevelUp;
    }

    private void OnDestroy()
    {
        if (targetLevel != null)
        {
            targetLevel.OnXPChanged -= UpdateBar;
            targetLevel.OnLevelUp -= OnLevelUp;
        }
    }

    private void UpdateBar(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
        xpText.text = $"XP: {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    private void OnLevelUp(int newLevel)
    {
        slider.maxValue = targetLevel.XPToNextLevel;
        slider.value = targetLevel.CurrentXP;
    }
}