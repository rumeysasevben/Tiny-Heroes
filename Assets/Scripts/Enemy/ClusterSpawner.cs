using UnityEngine;

public class ClusterSpawner : MonoBehaviour
{
    [SerializeField] private int companionCount = 7;
    [SerializeField] private float clusterRadius = 0.1f;
    [SerializeField] private string companionPoolName = "SwarmPool";

    private ObjectPool companionPool;
    private static bool isSpawningCluster = false;

    // Play başlangıcında static flag'i sıfırla (Unity Play oturumları arası static persist eder!)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        isSpawningCluster = false;
    }

    private void Awake()
    {
        GameObject poolGO = GameObject.Find(companionPoolName);
        if (poolGO != null)
            companionPool = poolGO.GetComponent<ObjectPool>();

        if (companionPool == null)
            Debug.LogWarning($"[Cluster] '{companionPoolName}' pool bulunamadı!");
    }

    private void OnEnable()
    {
        if (isSpawningCluster) return;
        if (companionPool == null) return;

        isSpawningCluster = true;
        try
        {
            for (int i = 0; i < companionCount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * clusterRadius;
                GameObject companion = companionPool.Get();
                if (companion != null)
                {
                    companion.transform.position = transform.position + (Vector3)offset;
                    if (WaveManager.Instance != null)
                        WaveManager.Instance.ApplyWaveScaling(companion);
                }
            }
        }
        finally
        {
            // İstisna olsa bile flag mutlaka sıfırlanır
            isSpawningCluster = false;
        }
    }
}