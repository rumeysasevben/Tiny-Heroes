using System.Collections.Generic;
using UnityEngine;

public class LightningOrb : MonoBehaviour
{
    [SerializeField] private float hitInterval = 0.5f;
    [SerializeField] private float selfRotateSpeed = 0f;  // kendi ekseni etrafında dönüş
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseAmount = 0.05f;     // %5 büyüyüp küçülür

    private float damage;
    private Vector3 baseScale;
    private readonly Dictionary<GameObject, float> hitCooldowns = new Dictionary<GameObject, float>();

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        // Kendi ekseni etrafında dönsün
        transform.Rotate(0f, 0f, selfRotateSpeed * Time.deltaTime);

        // Pulse (büyüyüp küçülme)
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * pulse;
    }

    public void SetDamage(float dmg) => damage = dmg;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        GameObject e = other.gameObject;
        float now = Time.time;

        if (hitCooldowns.TryGetValue(e, out float lastHit))
            if (now - lastHit < hitInterval) return;

        hitCooldowns[e] = now;
        Health hp = e.GetComponent<Health>();
        if (hp != null) hp.TakeDamage(damage);

        AudioManager.Instance?.PlayHit();
    }
}