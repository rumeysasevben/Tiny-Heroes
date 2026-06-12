using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    [Header("Spawn Animation")]
    [SerializeField] private float spawnDuration = 0.3f;
    [SerializeField] private float startScaleMultiplier = 0.3f;
    [SerializeField] private float overshootMultiplier = 1.15f; // hafif bounce

    private void Start()
    {
        StartCoroutine(SpawnAnimation());
    }

    private IEnumerator SpawnAnimation()
    {
        Vector3 targetScale = transform.localScale;
        Vector3 startScale = targetScale * startScaleMultiplier;
        Vector3 peakScale = targetScale * overshootMultiplier;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color baseColor = sr != null ? sr.color : Color.white;

        // Collider'ı animasyon bitene kadar kapat (oyuncu yarı şeffaf taşa çarpmasın)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float t = 0f;
        while (t < spawnDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / spawnDuration);

            // Scale: 0.3 → 1.15 → 1.0 (overshoot bounce)
            if (p < 0.7f)
            {
                float sp = p / 0.7f;
                float ease = 1f - Mathf.Pow(1f - sp, 2f); // ease out
                transform.localScale = Vector3.LerpUnclamped(startScale, peakScale, ease);
            }
            else
            {
                float sp = (p - 0.7f) / 0.3f;
                transform.localScale = Vector3.LerpUnclamped(peakScale, targetScale, sp);
            }

            // Alpha: 0 → 1
            if (sr != null)
            {
                Color c = baseColor;
                c.a = Mathf.Lerp(0f, baseColor.a, p);
                sr.color = c;
            }

            yield return null;
        }

        transform.localScale = targetScale;
        if (sr != null) sr.color = baseColor;
        if (col != null) col.enabled = true;
    }
}