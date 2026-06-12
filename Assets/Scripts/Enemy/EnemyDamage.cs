using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private float damagePerHit = 10f;
    [SerializeField] private float damageCooldown = 0.5f;  // her 0.5s'de bir hasar

    private float baseDamage;
    private float currentDamage;
    private float nextDamageTime;

    private void Awake()
    {
        baseDamage = damagePerHit;
        currentDamage = damagePerHit;
    }

    private void OnEnable()
    {
        currentDamage = baseDamage;
        nextDamageTime = 0f;
    }

    public void SetDamageMultiplier(float mult)
    {
        currentDamage = baseDamage * mult;
    }

    public float BaseDamage => baseDamage;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextDamageTime) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(currentDamage);
                nextDamageTime = Time.time + damageCooldown;
            }
        }
    }
}