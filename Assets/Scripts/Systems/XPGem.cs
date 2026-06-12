using UnityEngine;

public class XPGem : MonoBehaviour
{
    [SerializeField] private float xpAmount = 1f;
    [SerializeField] private float magnetSpeed = 8f;
    [SerializeField] private float pickupDistance = 0.3f;

    private Transform player;
    private bool isAttracted = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null) return;

        if (isAttracted)
        {
            // Player'a doğru hızla git
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                magnetSpeed * Time.deltaTime
            );

            // Yeterince yaklaşınca topla
            if (Vector3.Distance(transform.position, player.position) < pickupDistance)
            {
                PlayerLevel level = player.GetComponent<PlayerLevel>();
                if (level != null) level.AddXP(xpAmount);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isAttracted = true;
        }
        AudioManager.Instance?.PlayXpPickup();
    }
}