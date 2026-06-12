using UnityEngine;
using System;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentXP = 0f;
    [SerializeField] private float xpToNextLevel = 5f;
    [SerializeField] private float xpGrowthFactor = 1.30f;   // 1.5 → 1.30
    [SerializeField] private float xpGrowthFactorLate = 1.20f; // geç oyun daha da yumuşak
    [SerializeField] private int lateGameStartsAt = 10;

    public event Action<int> OnLevelUp;
    public event Action<float, float> OnXPChanged;

    public int CurrentLevel => currentLevel;
    public float CurrentXP => currentXP;
    public float XPToNextLevel => xpToNextLevel;

    public void AddXP(float amount)
    {
        currentXP += amount;
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);

        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    #if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddXP(1000f);
            Debug.Log("[CHEAT] +1000 XP");
        }
    }
    #endif

    public void ForceLevelUp()
    {
        LevelUp();
    }

    private void LevelUp()
    {
        currentXP = Mathf.Max(0f, currentXP - xpToNextLevel);
        currentLevel++;

        float factor = (currentLevel >= lateGameStartsAt) ? xpGrowthFactorLate : xpGrowthFactor;
        xpToNextLevel *= factor;

        Debug.Log($"LEVEL UP! Level {currentLevel} | Next: {xpToNextLevel:F1} XP");
        OnLevelUp?.Invoke(currentLevel);
        AudioManager.Instance?.PlayLevelUp();
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }
}