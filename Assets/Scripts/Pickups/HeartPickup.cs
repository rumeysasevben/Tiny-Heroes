using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    [Header("Heal Settings")]
    [SerializeField] private float healAmount = 20f;

    [Header("Magnet Settings")]
    [SerializeField] private float magnetRange = 2f;
    [SerializeField] private float moveSpeed = 8f;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= magnetRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                AudioManager.Instance?.PlayHeartPickup();
                Destroy(gameObject);
            }
        }
    }
}