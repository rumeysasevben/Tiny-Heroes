using UnityEngine;

public enum ChanceBoxType { FullHeal, DamageBoost, BigXP, ScreenClear }

public class ChanceBox : MonoBehaviour
{
    [Header("Tip")]
    public ChanceBoxType type;

    [Header("Ayarlar")]
    public float damageBoostMultiplier = 2f;
    public float damageBoostDuration = 10f;
    [Range(0.1f, 2f)] public float bigXpPercent = 0.8f;
    public float pickupRadius = 0.6f;

    [Header("Glow")]
    [SerializeField] private SpriteRenderer glowSprite;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseMinAlpha = 0.3f;
    [SerializeField] private float pulseMaxAlpha = 0.9f;


    [Header("Patlama")]
    [SerializeField] private GameObject explosionPrefab;

    private Transform player;
    private Color glowColor;
    private Vector3 baseGlowScale;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        {
            switch (type)
        {
            case ChanceBoxType.FullHeal:    glowColor = new Color(1f, 0.5f, 0.5f); break;   // kırmızı
            case ChanceBoxType.ScreenClear: glowColor = new Color(0.5f, 1f, 0.6f); break;   // yeşil
            case ChanceBoxType.BigXP:       glowColor = new Color(0.5f, 0.7f, 1f); break;   // mavi
            case ChanceBoxType.DamageBoost: glowColor = new Color(1f, 0.7f, 0.4f); break;     // turuncu (ileride spawn olursa)
        }
        }

        if (glowSprite != null)
        {
            baseGlowScale = glowSprite.transform.localScale;
            glowSprite.color = glowColor;
        }

        Destroy(gameObject, 15f);
    }

    void Update()
    {
        if (player == null) return;

        // Pulse animasyonu
        if (glowSprite != null)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // 0-1 arası
            float alpha = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, t);
            glowSprite.color = new Color(glowColor.r, glowColor.g, glowColor.b, alpha);

            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.1f;
            glowSprite.transform.localScale = baseGlowScale * scale;
        }

        if (Vector2.Distance(transform.position, player.position) <= pickupRadius)
            Apply();
    }

    void Apply()
    {
        AudioManager.Instance?.PlayChanceBox();
        

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            var ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = glowColor;
            }
            Destroy(explosion, 2f);
        }
        switch (type)
        {
            case ChanceBoxType.FullHeal:
                var hp = player.GetComponent<Health>();
                if (hp) hp.Heal(9999);
                break;

            case ChanceBoxType.DamageBoost:
                var stats = player.GetComponent<PlayerStats>();
                if (stats) stats.ApplyDamageBoost(damageBoostMultiplier, damageBoostDuration);
                break;

            case ChanceBoxType.BigXP:
                var lvl = player.GetComponent<PlayerLevel>();
                if (lvl) lvl.AddXP(lvl.XPToNextLevel * bigXpPercent);
                break;

            case ChanceBoxType.ScreenClear:
                foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    var eh = e.GetComponent<Health>();
                    if (eh) eh.TakeDamage(99999);
                }
                break;
        }
        Destroy(gameObject);
    }
}