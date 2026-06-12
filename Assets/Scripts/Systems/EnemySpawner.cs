using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Distance")]
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float nearBandMax = 12f;
    [SerializeField] private float farBandMax = 18f;
    [SerializeField, Range(0f, 1f)] private float nearSpawnChance = 0.8f;

    [Header("Spawn Telegraph")]
    [SerializeField] private SpawnTelegraph telegraphPrefab;
    [SerializeField] private float telegraphDuration = 0.35f;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float interval = WaveManager.Instance != null
                ? WaveManager.Instance.CurrentWave.spawnInterval
                : 1f;

            yield return new WaitForSeconds(interval);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (player == null || WaveManager.Instance == null) return;

        ObjectPool pool = WaveManager.Instance.GetRandomPool();
        if (pool == null) return;

        // Konum hesabı
        float distance;
        if (Random.value < nearSpawnChance)
            distance = Random.Range(minDistance, nearBandMax);
        else
            distance = Random.Range(nearBandMax, farBandMax);

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(randomDir * distance);

        // Telegraph + gecikmeli spawn
        StartCoroutine(TelegraphThenSpawn(pool, spawnPos));
    }

    private IEnumerator TelegraphThenSpawn(ObjectPool pool, Vector3 spawnPos)
    {
        // Telegraph göster
        if (telegraphPrefab != null)
        {
            SpawnTelegraph tg = Instantiate(telegraphPrefab, spawnPos, Quaternion.identity);
            tg.Play(telegraphDuration);
        }

        yield return new WaitForSeconds(telegraphDuration);

        // Düşmanı çıkar
        GameObject enemy = pool.Get();
        enemy.transform.position = spawnPos;
        WaveManager.Instance.ApplyWaveScaling(enemy);
    }
}