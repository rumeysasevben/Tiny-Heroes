using UnityEngine;

public class BoomerangThrower : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private string boomerangPoolName = "BoomerangPool";

    private ObjectPool pool;
    private float timer;

    private void Awake()
    {
        if (stats == null) stats = GetComponent<PlayerStats>();

        GameObject poolGO = GameObject.Find(boomerangPoolName);
        if (poolGO != null) pool = poolGO.GetComponent<ObjectPool>();

        if (pool == null)
            Debug.LogWarning($"[BoomerangThrower] '{boomerangPoolName}' bulunamadı!");
    }

    private void Update()
    {
        if (stats == null || !stats.HasBoomerang) return;
        if (pool == null) return;

        timer += Time.deltaTime;
        float cooldown = stats.GetBoomerangCooldown();

        if (timer >= cooldown)
        {
            timer = 0f;
            Throw();
        }
    }

    private void Throw()
    {
        // En yakın düşmanı bul
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (var e in enemies)
        {
            float d = Vector2.SqrMagnitude((Vector2)(e.transform.position - transform.position));
            if (d < minDist) { minDist = d; nearest = e.transform; }
        }
        if (nearest == null) return;

        Vector2 baseDir = ((Vector2)(nearest.position - transform.position)).normalized;

        int count = stats.GetBoomerangCount();
        float dmg = stats.GetBoomerangDamage() * stats.DamageMultiplier;
        float spreadDegrees = 20f;

        for (int i = 0; i < count; i++)
        {
            float angleOffset = (i - (count - 1) / 2f) * spreadDegrees;
            float rad = angleOffset * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(
                baseDir.x * Mathf.Cos(rad) - baseDir.y * Mathf.Sin(rad),
                baseDir.x * Mathf.Sin(rad) + baseDir.y * Mathf.Cos(rad)
            );

            GameObject b = pool.Get();
            b.transform.position = transform.position;

            Boomerang bm = b.GetComponent<Boomerang>();
            if (bm != null)
            {
                bm.SetPool(pool);
                bm.Initialize(transform, dir, dmg);
            }
        }
    }
}