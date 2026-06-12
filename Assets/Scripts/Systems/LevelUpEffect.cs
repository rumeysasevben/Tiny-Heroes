using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUpEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image shockwaveImage;   // Canvas'a koyacağın halka Image
    
    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float startScale = 0.1f;
    [SerializeField] private float endScale = 4f;
    [SerializeField] private Color shockwaveColor = new Color(1f, 0.9f, 0.3f, 1f); // sarımsı

    private void Awake()
    {
        if (shockwaveImage != null)
            shockwaveImage.gameObject.SetActive(false);
    }

    public void PlayShockwave()
    {
        if (shockwaveImage == null) return;
        StopAllCoroutines();
        StartCoroutine(ShockwaveRoutine());
    }

    private IEnumerator ShockwaveRoutine()
    {
        shockwaveImage.gameObject.SetActive(true);
        RectTransform rect = shockwaveImage.rectTransform;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);

            // Scale: küçükten büyüğe (ease out)
            float ease = 1f - Mathf.Pow(1f - p, 2f);
            float scale = Mathf.Lerp(startScale, endScale, ease);
            rect.localScale = Vector3.one * scale;

            // Alpha: 1 → 0 (kaybolarak büyür)
            Color c = shockwaveColor;
            c.a = Mathf.Lerp(1f, 0f, p);
            shockwaveImage.color = c;

            yield return null;
        }

        shockwaveImage.gameObject.SetActive(false);
    }
}