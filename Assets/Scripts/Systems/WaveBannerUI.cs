using System.Collections;
using UnityEngine;
using TMPro;

public class WaveBannerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bannerText;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float holdDuration = 1.2f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float startYOffset = 200f;

    private RectTransform rt;
    private Vector2 finalPos;
    private Color baseColor;

    void Awake()
    {
        rt = bannerText.GetComponent<RectTransform>();
        finalPos = rt.anchoredPosition;
        baseColor = bannerText.color; // inspector'daki rengi kaydet
        bannerText.gameObject.SetActive(false);
    }

    void Start()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveBanner += ShowBanner;
    }

    void OnDestroy()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveBanner -= ShowBanner;
    }

    public void ShowBanner(string text)
    {
        StopAllCoroutines();
        StartCoroutine(BannerRoutine(text));
    }

    IEnumerator BannerRoutine(string text)
    {
        bannerText.text = text;
        bannerText.gameObject.SetActive(true);

        Vector2 startPos = finalPos + Vector2.up * startYOffset;
        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            float p = t / slideDuration;
            p = 1f - Mathf.Pow(1f - p, 3f);
            rt.anchoredPosition = Vector2.Lerp(startPos, finalPos, p);
            bannerText.color = new Color(baseColor.r, baseColor.g, baseColor.b, p);
            yield return null;
        }
        rt.anchoredPosition = finalPos;
        bannerText.color = baseColor;

        yield return new WaitForSeconds(holdDuration);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeDuration;
            bannerText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f - p);
            yield return null;
        }

        bannerText.gameObject.SetActive(false);
    }
}