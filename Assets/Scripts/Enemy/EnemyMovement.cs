using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float knockbackDuration = 0.15f;

    private float baseMoveSpeed;
    private Rigidbody2D rb;
    private Transform player;
    private float knockbackTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        moveSpeed = baseMoveSpeed;
        knockbackTimer = 0f;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void FixedUpdate()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return; // knockback s\u0131ras\u0131nda chase yapma
        }

        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    public void SetSpeedBoost(float extraSpeed, float maxSpeed)
    {
        moveSpeed = Mathf.Min(baseMoveSpeed + extraSpeed, maxSpeed);
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (rb == null) return;
        rb.linearVelocity = direction.normalized * force;
        knockbackTimer = knockbackDuration;
    }
}