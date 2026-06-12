using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int KillCount { get; private set; }
    public float ElapsedTime { get; private set; }

    public event Action<int> OnKillCountChanged;

    private const string BEST_TIME_KEY = "BestTime";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        ElapsedTime += Time.deltaTime;
    }

    public void AddKill()
    {
        KillCount++;
        OnKillCountChanged?.Invoke(KillCount);
    }

    public void ResetScore()
    {
        KillCount = 0;
        ElapsedTime = 0f;
        OnKillCountChanged?.Invoke(KillCount);
    }

    public string GetFormattedTime()
    {
        return FormatTime(ElapsedTime);
    }

    // ===== BEST TIME SYSTEM =====

    public float GetBestTime()
    {
        return PlayerPrefs.GetFloat(BEST_TIME_KEY, 0f);
    }

    public string GetFormattedBestTime()
    {
        return FormatTime(GetBestTime());
    }

    /// <summary>Game Over anında çağrılır. Yeni rekorsa kaydeder, true döndürür.</summary>
    public bool TrySaveBestTime()
    {
        float best = GetBestTime();
        if (ElapsedTime > best)
        {
            PlayerPrefs.SetFloat(BEST_TIME_KEY, ElapsedTime);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    private string FormatTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}