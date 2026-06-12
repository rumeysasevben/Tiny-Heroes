using UnityEngine;
using System.Collections;

public class SpawnTelegraph : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    
    [Header("Alpha Pulse")]
    [SerializeField] private float pulseSpeed = 10f;
    [SerializeField] private float minAlpha = 0.4f;
    [SerializeField] private float maxAlpha = 1f;
    
    [Header("Scale Pulse")]
    [SerializeField] private float scalePulseAmount = 0.15f;  // ±15% pulse
    
    [Header("Wind-up")]
    [SerializeField] private float startScaleMultiplier = 0.7f; // başta %70 boyutta
    [SerializeField] private float endScaleMultiplier = 1.1f;   // sonda %110
    
    [Header("Color")]
    [SerializeField] private Color telegraphColor = new Color(1f, 0.2f, 0.2f, 1f);

    private Vector3 baseScale;

    public void Play(float duration)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.color = telegraphColor;
        baseScale = transform.localScale;
        StartCoroutine(PulseRoutine(duration));
    }

    private IEnumerator PulseRoutine(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            float sinValue = (Mathf.Sin(t * pulseSpeed) + 1f) * 0.5f; // 0-1 arası

            // Alpha pulse
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, sinValue);
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;

            // Scale: wind-up (genel büyüme) + pulse
            float windUp = Mathf.Lerp(startScaleMultiplier, endScaleMultiplier, p);
            float pulse = 1f + (sinValue - 0.5f) * 2f * scalePulseAmount;
            transform.localScale = baseScale * windUp * pulse;

            yield return null;
        }
        Destroy(gameObject);
    }
}