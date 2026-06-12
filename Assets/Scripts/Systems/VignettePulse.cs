using UnityEngine;
using UnityEngine.UI;

public class VignettePulse : MonoBehaviour
{
    [Header("References")]
    public Image redVignette;
    public Health playerHealth;

    [Header("Settings")]
    [Range(0f, 1f)] public float lowHpThreshold = 0.3f;
    public float pulseSpeed = 3f;
    public float maxAlpha = 0.75f;
    public float fadeSpeed = 5f;

    void Update()
    {
        if (playerHealth == null || redVignette == null) return;

        float hpRatio = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
        Color c = redVignette.color;
        Debug.Log($"HP Ratio: {hpRatio} | Alpha: {c.a}");

        if (hpRatio < lowHpThreshold && hpRatio > 0f)
        {
            // Can ne kadar azsa pulse o kadar güçlü
            float intensity = 1f - (hpRatio / lowHpThreshold);
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float targetAlpha = pulse * maxAlpha * intensity;

            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        }
        else
        {
            // Can yeterli ya da öldü → kırmızıyı gizle
            c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * fadeSpeed);
        }

        redVignette.color = c;
    }
}