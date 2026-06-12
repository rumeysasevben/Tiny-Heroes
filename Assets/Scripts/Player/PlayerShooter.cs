using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private ObjectPool bulletPool;

    private float fireTimer;
    [SerializeField] private PlayerStats stats;

    private void Awake()
    {
        if (stats == null) stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            Transform nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                Shoot(nearestEnemy.position);
                fireTimer = stats.fireRate;
            }
        }
    }

    private Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float minDist = stats.detectionRange;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }

    private void Shoot(Vector3 targetPos)
    {
        Vector2 direction = (targetPos - transform.position).normalized;
        GameObject bullet = bulletPool.Get();
        bullet.transform.position = transform.position;

        float damage = stats.bulletDamage * stats.DamageMultiplier;
        bool isCrit = Random.value < stats.critChance;
        if (isCrit) damage *= stats.critMultiplier;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetPool(bulletPool);
        bulletScript.Initialize(direction, damage, isCrit);

        AudioManager.Instance?.PlayShoot();
    }
}