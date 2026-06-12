using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    [SerializeField] private float speed = 9f;
    [SerializeField] private float outDuration = 0.7f;
    [SerializeField] private float maxLifetime = 4f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float returnDistance = 0.5f;

    private Transform player;
    private Vector2 throwDirection;
    private float damage;
    private float stateTimer;
    private float lifetimeTimer;
    private bool returning;
    private ObjectPool pool;
    private readonly HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    public void SetPool(ObjectPool p) { pool = p; }

    public void Initialize(Transform playerTransform, Vector2 direction, float dmg)
    {
        player = playerTransform;
        throwDirection = direction.normalized;
        damage = dmg;
        stateTimer = 0f;
        lifetimeTimer = 0f;
        returning = false;
        hitEnemies.Clear();
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;
        lifetimeTimer += Time.deltaTime;

        if (lifetimeTimer >= maxLifetime)
        {
            ReturnToPool();
            return;
        }

        Vector2 currentPos = transform.position;
        Vector2 moveDir;

        if (!returning)
        {
            moveDir = throwDirection;
            if (stateTimer >= outDuration)
                returning = true;
        }
        else
        {
            if (player == null) { ReturnToPool(); return; }
            Vector2 toPlayer = (Vector2)player.position - currentPos;
            if (toPlayer.magnitude < returnDistance)
            {
                ReturnToPool();
                return;
            }
            moveDir = toPlayer.normalized;
        }

        transform.position = currentPos + moveDir * speed * Time.deltaTime;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (hitEnemies.Contains(other.gameObject)) return;

        hitEnemies.Add(other.gameObject);
        Health hp = other.GetComponent<Health>();
        if (hp != null) hp.TakeDamage(damage);

        AudioManager.Instance?.PlayHit();
    }

    private void ReturnToPool()
    {
        if (pool != null) pool.Return(gameObject);
        else gameObject.SetActive(false);
    }
}