using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] obstaclePrefabs;  // taş/kaya prefab varyasyonları
    
    [Header("Spawn Settings")]
    [SerializeField] private int targetObstacleCount = 25;       // aktif engel hedefi
    [SerializeField] private float minSpawnDistance = 6f;        // oyuncudan en az bu kadar uzakta spawn
    [SerializeField] private float maxSpawnDistance = 20f;       // bu kadarın ötesi spawn olmaz
    [SerializeField] private float despawnDistance = 28f;        // bu kadar uzaktakileri sil
    [SerializeField] private float minDistanceBetweenObstacles = 2.5f; // engeller arası min mesafe
    [SerializeField] private float spawnCheckInterval = 2f;      // her 2s'de bir kontrol et
    
    private Transform player;
    private List<Transform> activeObstacles = new List<Transform>();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // İlk başta hızlıca doldur
        for (int i = 0; i < targetObstacleCount; i++)
        {
            TrySpawnObstacle();
        }

        while (true)
        {
            yield return new WaitForSeconds(spawnCheckInterval);
            CleanupFarObstacles();
            
            int needed = targetObstacleCount - activeObstacles.Count;
            for (int i = 0; i < needed; i++)
            {
                TrySpawnObstacle();
            }
        }
    }

    private void TrySpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        if (player == null) return;

        // 10 deneme yap, uygun pozisyon bulamazsa pas geç
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 spawnPos = player.position + (Vector3)(dir * dist);

            // Diğer engellerle çakışıyor mu?
            bool tooClose = false;
            foreach (var obs in activeObstacles)
            {
                if (obs == null) continue;
                if (Vector3.Distance(obs.position, spawnPos) < minDistanceBetweenObstacles)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;

            // Spawn
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            GameObject obs2 = Instantiate(prefab, spawnPos, Quaternion.identity);
            activeObstacles.Add(obs2.transform);
            return;
        }
    }

    private void CleanupFarObstacles()
    {
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] == null)
            {
                activeObstacles.RemoveAt(i);
                continue;
            }

            if (Vector3.Distance(activeObstacles[i].position, player.position) > despawnDistance)
            {
                Destroy(activeObstacles[i].gameObject);
                activeObstacles.RemoveAt(i);
            }
        }
    }
}