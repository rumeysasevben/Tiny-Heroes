using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [Header("XP Gem (default drop)")]
    [SerializeField] private GameObject xpGemPrefab;
    [SerializeField] private int dropCount = 1;

    [Header("Heart Drop")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField, Range(0f, 1f)] private float heartDropChance = 0.05f;

    [Header("Chance Box Drop")]
    [SerializeField] private GameObject[] chanceBoxPrefabs;   // Red, Green, Blue
    [SerializeField, Range(0f, 1f)] private float chanceBoxDropChance = 0.02f;
    [SerializeField] private int chanceBoxDropCount = 1;
    [SerializeField] private float chanceBoxSpreadRadius = 0.8f;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (health != null) health.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        if (health != null) health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        // Öncelik: Chance Box > Heart > XP Gem (sadece biri düşer)

        if (chanceBoxPrefabs != null && chanceBoxPrefabs.Length > 0
            && Random.value < chanceBoxDropChance)
        {
            for (int i = 0; i < chanceBoxDropCount; i++)
            {
                GameObject prefab = chanceBoxPrefabs[Random.Range(0, chanceBoxPrefabs.Length)];
                if (prefab != null)
                {
                    Vector2 offset = Random.insideUnitCircle * chanceBoxSpreadRadius;
                    Vector3 pos = transform.position + (Vector3)offset;
                    Instantiate(prefab, pos, Quaternion.identity);
                }
            }
            return;
        }

        if (heartPrefab != null && Random.value < heartDropChance)
        {
            Instantiate(heartPrefab, transform.position, Quaternion.identity);
            return;
        }

        DropGem();
    }

    private void DropGem()
    {
        if (xpGemPrefab == null) return;

        for (int i = 0; i < dropCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.5f;
            Vector3 pos = transform.position + (Vector3)offset;
            Instantiate(xpGemPrefab, pos, Quaternion.identity);
        }
    }
}