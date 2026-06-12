using UnityEngine;
using TMPro;
using System.Collections;

public class GameOverStats : MonoBehaviour
{
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text recordText;   // yeni: ayrı bir text
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private WaveManager waveManager;

    [Header("New Record Animation")]
    [SerializeField] private Color newRecordColor = new Color(1f, 0.85f, 0.2f); // altın sarısı
    [SerializeField] private Color normalColor = new Color(0.9f, 0.9f, 0.9f);   // gri-beyaz
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseScale = 1.1f;

    private Coroutine pulseRoutine;

    void OnEnable()
    {
        UpdateStats();
    }

    void OnDisable()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }
    }

    private void UpdateStats()
    {
        if (statsText == null) return;

        string time = ScoreManager.Instance != null ? ScoreManager.Instance.GetFormattedTime() : "00:00";
        int kills = ScoreManager.Instance != null ? ScoreManager.Instance.KillCount : 0;
        int level = playerLevel != null ? playerLevel.CurrentLevel : 1;
        int wave  = waveManager != null ? waveManager.CurrentWaveNumber : 1;

        statsText.text =
            $"Time: {time}\n" +
            $"Kills: {kills}\n" +
            $"Wave: {wave}\n" +
            $"Level: {level}";

        // Best Time kontrolü
        if (recordText != null && ScoreManager.Instance != null)
        {
            bool isNewRecord = ScoreManager.Instance.TrySaveBestTime();
            string bestTimeStr = ScoreManager.Instance.GetFormattedBestTime();

            if (isNewRecord)
            {
                recordText.text = $"★ NEW RECORD! ★\nBest Time: {bestTimeStr}";
                recordText.color = newRecordColor;
                pulseRoutine = StartCoroutine(PulseRecord());
            }
            else
            {
                recordText.text = $"Best Time: {bestTimeStr}";
                recordText.color = normalColor;
                recordText.transform.localScale = Vector3.one;
            }
        }
    }

    private IEnumerator PulseRecord()
    {
        Vector3 baseScale = Vector3.one;
        while (true)
        {
            float t = Time.unscaledTime * pulseSpeed;
            float scale = 1f + Mathf.Sin(t) * (pulseScale - 1f);
            recordText.transform.localScale = baseScale * scale;
            yield return null;
        }
    }
}