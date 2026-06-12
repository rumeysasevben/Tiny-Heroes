using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float popScale = 1.4f;

    private float timer;
    private Color startColor;
    private Vector3 baseScale;

    void Start()
    {
        timer = lifetime;
        startColor = text.color;
        baseScale = transform.localScale;
        transform.position += new Vector3(Random.Range(-0.3f, 0.3f), 0.2f, 0);
    }

    void Update()
    {
        float t = 1f - (timer / lifetime); // 0 → 1

        // Yukarı uç
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Pop: önce büyü, sonra küçül
        float scale = t < 0.2f
            ? Mathf.Lerp(0.5f, popScale, t / 0.2f)        // 0-20%: 0.5 → 1.4
            : Mathf.Lerp(popScale, 1f, (t - 0.2f) / 0.8f); // 20-100%: 1.4 → 1
        transform.localScale = baseScale * scale;

        // Fade
        timer -= Time.deltaTime;
        float alpha = timer / lifetime;
        text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timer <= 0f)
            Destroy(gameObject);
    }

    public void SetDamage(int amount, Color color)
    {
        text.text = "-" + amount.ToString();
        text.color = color;
    }

    public void SetText(string customText, Color color)
    {
        text.text = customText;
        text.color = color;
    }
}