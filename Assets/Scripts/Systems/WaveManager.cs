using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public ObjectPool pool;
        [Range(0, 100)] public int weight = 50;
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        public float duration = 30f;
        public float spawnInterval = 1f;
        public List<EnemySpawnEntry> enemies;
    }

    public static WaveManager Instance;
    public event Action<string> OnWaveBanner;

    [SerializeField] private List<Wave> waves;

    [Header("Boss")]
    [SerializeField] private ObjectPool bossPool;
    [SerializeField] private ObjectPool chargerBossPool;
    [SerializeField] private int chargerStartsAtBossWave = 1;

    [Header("Boss Warning")]
    [SerializeField] private float bossWarningTime = 5f;
    [SerializeField] private string bossWarningText = "FINAL WAVE: BOSS INCOMING!";

    [Header("Boss Respawn")]
    [SerializeField] private float initialBossDelay = 90f;
    [SerializeField] private float delayReductionPerWave = 10f;
    [SerializeField] private float minBossDelay = 60f;
    [SerializeField] private int initialBossCount = 1;

    [Header("Boss Spawn Distance")]
    [SerializeField] private float initialSpawnDistance = 8f;
    [SerializeField] private float spawnDistanceReduction = 0.3f;
    [SerializeField] private float minSpawnDistance = 6f;

    [Header("Wave-based Scaling (linear)")]
    [SerializeField] private float enemyHPMultiplierPerWave = 0.15f;
    [SerializeField] private float enemySpeedIncreasePerWave = 0.1f;
    [SerializeField] private float maxEnemySpeed = 3.5f;

    [Header("Boss Speed Scaling")]
    [SerializeField] private float bossSpeedIncreasePerWave = 0.4f;
    [SerializeField] private float maxBossSpeed = 4f;

    [Header("Time-based Scaling (exponential)")]
    [Tooltip("Bu süreden önce time scaling devreye girmez (saniye)")]
    [SerializeField] private float scalingStartTime = 60f;
    [Tooltip("Bu süre sonunda HP 2x olur (saniye). Küçük = daha hızlı zorlaşma")]
    [SerializeField] private float hpDoubleTime = 45f;
    [Tooltip("Bu süre sonunda hasar 2x olur (saniye)")]
    [SerializeField] private float damageDoubleTime = 75f;
    [Tooltip("Maksimum HP çarpanı (oyun çıldırmasın)")]
    [SerializeField] private float maxHPMultiplier = 1000000f;
    [Tooltip("Maksimum hasar çarpanı")]
    [SerializeField] private float maxDamageMultiplier = 1000f;

    [Header("Failsafe")]
    [SerializeField] private float bossWaveTimeout = 120f;

    private int currentWaveIndex = 0;
    private float waveTimer = 0f;
    private bool firstBossSpawned = false;
    private bool firstBossWarningShown = false;

    private int bossWaveNumber = 1;
    private int aliveBossCount = 0;
    private bool waitingForNextBossWave = false;
    private float bossWaveStartTime = 0f;

    private float gameTime = 0f;

    public Wave CurrentWave => waves[currentWaveIndex];
    public int CurrentWaveNumber => currentWaveIndex + 1;
    public float GameTime => gameTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        OnWaveBanner?.Invoke("WAVE 1");
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        if (!firstBossSpawned)
        {
            waveTimer += Time.deltaTime;

            // Son wave'deyken boss'a yaklaşıyorsak uyarı göster
            if (currentWaveIndex == waves.Count - 1 && !firstBossWarningShown)
            {
                float timeLeft = waves[currentWaveIndex].duration - waveTimer;
                if (timeLeft <= bossWarningTime)
                {
                    OnWaveBanner?.Invoke(bossWarningText);
                    firstBossWarningShown = true;
                }
            }

            if (waveTimer >= waves[currentWaveIndex].duration)
            {
                waveTimer = 0f;
                if (currentWaveIndex < waves.Count - 1)
                {
                    currentWaveIndex++;
                    Debug.Log("Wave changed to: " + waves[currentWaveIndex].waveName);
                    OnWaveBanner?.Invoke("WAVE " + (currentWaveIndex + 1));
                }
                else
                {
                    SpawnBossWave();
                    firstBossSpawned = true;
                }
            }
        }
        else
        {
            if (aliveBossCount > 0 && !waitingForNextBossWave)
            {
                if (Time.time - bossWaveStartTime > bossWaveTimeout)
                {
                    Debug.Log($"[Failsafe] Boss dalgası timeout.");
                    aliveBossCount = 0;
                    StartCoroutine(WaitAndSpawnNextBossWave());
                }
            }
        }
    }

    public float GetHPTimeMultiplier()
    {
        if (gameTime < scalingStartTime) return 1f;
        float effective = gameTime - scalingStartTime;
        float mult = Mathf.Pow(2f, effective / hpDoubleTime);
        return Mathf.Min(maxHPMultiplier, mult);
    }

    public float GetDamageTimeMultiplier()
    {
        if (gameTime < scalingStartTime) return 1f;
        float effective = gameTime - scalingStartTime;
        float mult = Mathf.Pow(2f, effective / damageDoubleTime);
        return Mathf.Min(maxDamageMultiplier, mult);
    }

    private void SpawnBossWave()
    {
        if (bossPool == null) { Debug.LogWarning("BossPool atanmamış!"); return; }

        int bossCount = initialBossCount + (bossWaveNumber - 1);
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        float spawnDistance = Mathf.Max(
            minSpawnDistance,
            initialSpawnDistance - (spawnDistanceReduction * (bossWaveNumber - 1))
        );

        float extraSpeed = bossSpeedIncreasePerWave * (bossWaveNumber - 1);
        bool spawnCharger = (chargerBossPool != null && bossWaveNumber >= chargerStartsAtBossWave);

        float startAngleOffset = UnityEngine.Random.Range(0f, 360f);

        for (int i = 0; i < bossCount; i++)
        {
            float baseAngle = (360f / bossCount) * i + startAngleOffset;
            float jitter = UnityEngine.Random.Range(-15f, 15f);
            float angle = (baseAngle + jitter) * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector3 spawnPos = player.position + (Vector3)(dir * spawnDistance);

            ObjectPool poolToUse = (spawnCharger && i == 0) ? chargerBossPool : bossPool;

            GameObject boss = poolToUse.Get();
            boss.transform.position = spawnPos;

            var movement = boss.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SetSpeedBoost(extraSpeed, maxBossSpeed);
            }

            ApplyTimeScaling(boss);

            aliveBossCount++;
        }

        bossWaveStartTime = Time.time;
        Debug.Log($"BOSS DALGASI #{bossWaveNumber}! ({bossCount} boss, charger: {spawnCharger}) | HPx{GetHPTimeMultiplier():F1} | DMGx{GetDamageTimeMultiplier():F1}");
        OnWaveBanner?.Invoke("BOSS WAVE " + bossWaveNumber);
        bossWaveNumber++;
    }

    public void OnBossDefeated()
    {
        aliveBossCount = Mathf.Max(0, aliveBossCount - 1);
        Debug.Log($"Boss defeated. Kalan boss: {aliveBossCount}");

        if (aliveBossCount == 0 && !waitingForNextBossWave)
        {
            StartCoroutine(WaitAndSpawnNextBossWave());
        }
    }

    private IEnumerator WaitAndSpawnNextBossWave()
    {
        waitingForNextBossWave = true;

        float delay = Mathf.Max(
            minBossDelay,
            initialBossDelay - (delayReductionPerWave * (bossWaveNumber - 1))
        );

        Debug.Log($"Sonraki boss dalgası {delay} saniye sonra...");

        // Uyarı süresi kadar önce banner göster
        float waitBeforeWarning = Mathf.Max(0f, delay - bossWarningTime);
        yield return new WaitForSeconds(waitBeforeWarning);

        OnWaveBanner?.Invoke(bossWarningText);
        yield return new WaitForSeconds(bossWarningTime);

        SpawnBossWave();
        waitingForNextBossWave = false;
    }

    public ObjectPool GetRandomPool()
    {
        var wave = CurrentWave;
        if (wave.enemies == null || wave.enemies.Count == 0) return null;

        int totalWeight = 0;
        foreach (var e in wave.enemies) totalWeight += e.weight;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (var e in wave.enemies)
        {
            cumulative += e.weight;
            if (roll < cumulative) return e.pool;
        }
        return wave.enemies[0].pool;
    }

    public void ApplyWaveScaling(GameObject enemy)
    {
        int waveNum = CurrentWaveNumber - 1;
        float hpTimeMult = GetHPTimeMultiplier();

        var health = enemy.GetComponent<Health>();
        if (health != null)
        {
            float scaledHP = health.BaseMaxHealth
                             * (1f + enemyHPMultiplierPerWave * waveNum)
                             * hpTimeMult;
            health.SetMaxHealth(scaledHP);
        }

        var movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            float extraSpeed = enemySpeedIncreasePerWave * waveNum;
            movement.SetSpeedBoost(extraSpeed, maxEnemySpeed);
        }

        var damage = enemy.GetComponent<EnemyDamage>();
        if (damage != null)
        {
            damage.SetDamageMultiplier(GetDamageTimeMultiplier());
        }
    }

    private void ApplyTimeScaling(GameObject enemy)
    {
        float hpTimeMult = GetHPTimeMultiplier();

        var health = enemy.GetComponent<Health>();
        if (health != null)
        {
            float scaledHP = health.BaseMaxHealth * hpTimeMult;
            health.SetMaxHealth(scaledHP);
        }

        var damage = enemy.GetComponent<EnemyDamage>();
        if (damage != null)
        {
            damage.SetDamageMultiplier(GetDamageTimeMultiplier());
        }
    }
}