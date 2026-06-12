using UnityEngine;
using TMPro;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text statsText;

    private void OnEnable()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log($"Player bulundu mu: {player != null} | StatsText atandı mı: {statsText != null}");
        if (player == null || statsText == null) return;

        var stats  = player.GetComponent<PlayerStats>();
        var level  = player.GetComponent<PlayerLevel>();
        var health = player.GetComponent<Health>();

        float currentDamage = stats.bulletDamage * stats.DamageMultiplier;
        bool hasBuff = stats.DamageMultiplier > 1f;

        statsText.text =
            $"<b>STATS</b>\n\n" +
            $"Level: {level.CurrentLevel}\n" +
            $"XP: {level.CurrentXP:F0} / {level.XPToNextLevel:F0}\n" +
            $"HP: {health.CurrentHealth:F0} / {health.MaxHealth:F0}\n" +
            $"Damage: {currentDamage:F1}" + (hasBuff ? " <color=#ffeb3b>(BUFF!)</color>" : "") + "\n" +
            $"Fire Rate: {(1f/stats.fireRate):F1}/sn\n" +
            $"Crit Chance: %{(stats.critChance*100):F0}\n" +
            $"Crit Multipler: x{stats.critMultiplier:F1}\n" +
            $"Speed: {stats.moveSpeed:F1}\n" +
            $"Range: {stats.detectionRange:F1}";
    }
}