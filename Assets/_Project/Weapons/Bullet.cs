using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private ObjectPool pool;

    private Vector2 direction;
    private float damage = 10f;
    private bool isCrit = false;

    public void Initialize(Vector2 dir, float dmg, bool crit = false)
    {
        direction = dir.normalized;
        damage = dmg;
        isCrit = crit;
    }

    private void OnEnable()
    {
        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void ReturnToPool()
    {
        if (pool != null) pool.Return(gameObject);
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null) enemyHealth.TakeDamage(damage, isCrit);

            var em = other.GetComponent<EnemyMovement>();
            if (em != null) em.ApplyKnockback(direction, 1f);

            AudioManager.Instance?.PlayHit();
            ReturnToPool();
        }
    }

    public void SetPool(ObjectPool p) { pool = p; }
}