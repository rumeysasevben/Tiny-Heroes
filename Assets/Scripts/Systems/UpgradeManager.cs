using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeData> allUpgrades;
    [SerializeField] private UpgradeUI upgradeUI;
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private PlayerStats playerStats;

    public bool IsSelecting { get; private set; }

    private void Start()
    {
        if (playerLevel != null)
            playerLevel.OnLevelUp += HandleLevelUp;
    }

    private void OnDestroy()
    {
        if (playerLevel != null)
            playerLevel.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        List<UpgradeData> choices = GetRandomUpgrades(3, newLevel);
        upgradeUI.Show(choices, OnUpgradeSelected);
        Time.timeScale = 0f;
        IsSelecting = true;
    }

    private List<UpgradeData> GetRandomUpgrades(int count, int currentLevel)
    {
        // Sadece player'ın levelina uygun olanları al
        return allUpgrades
            .Where(u => u.minPlayerLevel <= currentLevel)
            .OrderBy(x => Random.value)
            .Take(count)
            .ToList();
    }

    private void OnUpgradeSelected(UpgradeData selected)
    {
        selected.Apply(playerStats);
        upgradeUI.Hide();
        Time.timeScale = 1f;
        IsSelecting = false;
    }
}