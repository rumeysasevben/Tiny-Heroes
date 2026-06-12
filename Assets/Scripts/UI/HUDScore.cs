using UnityEngine;
using TMPro;

public class HUDScore : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private PlayerLevel playerLevel;

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnKillCountChanged += UpdateKills;
            UpdateKills(ScoreManager.Instance.KillCount);
        }

        if (playerLevel != null)
        {
            playerLevel.OnLevelUp += UpdateLevel;
            UpdateLevel(playerLevel.CurrentLevel);
        }
    }

    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnKillCountChanged -= UpdateKills;
        if (playerLevel != null)
            playerLevel.OnLevelUp -= UpdateLevel;
    }

    void Update()
    {
        if (waveManager != null && waveText != null)
            waveText.text = $"Wave: {waveManager.CurrentWaveNumber}";

        if (timeText != null && ScoreManager.Instance != null)
            timeText.text = ScoreManager.Instance.GetFormattedTime();
    }

    void UpdateKills(int kills)
    {
        if (killText != null) killText.text = $"Kills: {kills}";
    }

    void UpdateLevel(int level)
    {
        if (levelText != null) levelText.text = $"Level: {level}";
    }
}