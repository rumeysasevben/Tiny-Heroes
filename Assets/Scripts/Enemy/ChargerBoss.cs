using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyMovement))]
public class ChargerBoss : MonoBehaviour
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeInterval = 4f;
    [SerializeField] private float telegraphTime = 0.8f;
    [SerializeField] private float dashDuration = 0.6f;
    [SerializeField] private float dashSpeed = 14f;

    [Header("Visual")]
    [SerializeField] private Color telegraphColor = Color.red;
    [SerializeField] private float scaleUpAmount = 1.25f;
    [SerializeField] private float blinkInterval = 0.1f;

    private Rigidbody2D rb;
    private EnemyMovement movement;
    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 originalScale;
    private Transform player;
    private TrailRenderer trail;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<EnemyMovement>();
        sr = GetComponent<SpriteRenderer>();
        if (sr) originalColor = sr.color;
        originalScale = transform.localScale;
        trail = GetComponent<TrailRenderer>();
        if (trail) trail.emitting = false;
    }

    private void OnEnable()
    {
        StartCoroutine(ChargeLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (sr) sr.color = originalColor;
        transform.localScale = originalScale;
        if (movement) movement.enabled = true;
        if (trail) trail.emitting = false;
    }

    private IEnumerator ChargeLoop()
    {
        while (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
            yield return null;
        }

        while (true)
        {
            yield return new WaitForSeconds(chargeInterval);

            // 1) Telegraph: dur, yan\u0131p s\u00f6n + b\u00fcy\u00fc
            movement.enabled = false;
            rb.linearVelocity = Vector2.zero;

            float t = 0f;
            bool flash = false;
            while (t < telegraphTime)
            {
                flash = !flash;
                if (sr) sr.color = flash ? telegraphColor : originalColor;

                // \u00f6l\u00e7ek anticipation: yava\u015f\u00e7a b\u00fcy\u00fcr
                float progress = t / telegraphTime;
                transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleUpAmount, progress);

                yield return new WaitForSeconds(blinkInterval);
                t += blinkInterval;
            }

            // 2) Dash
            if (sr) sr.color = originalColor;
            if (trail) trail.emitting = true;

            Vector2 dashDir = ((Vector2)player.position - rb.position).normalized;
            float dashT = 0f;
            while (dashT < dashDuration)
            {
                rb.linearVelocity = dashDir * dashSpeed;
                dashT += Time.deltaTime;
                yield return null;
            }

            // 3) Normale d\u00f6n
            if (trail) trail.emitting = false;
            transform.localScale = originalScale;
            movement.enabled = true;
        }
    }
}