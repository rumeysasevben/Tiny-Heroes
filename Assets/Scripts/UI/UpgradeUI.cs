using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] cardButtons;
    [SerializeField] private TMP_Text[] cardNames;
    [SerializeField] private TMP_Text[] cardDescriptions;
    [SerializeField] private PlayerStats playerStats;

    [Header("Bounce Animation")]
    [SerializeField] private float bounceDuration = 0.35f;
    [SerializeField] private float delayBetweenCards = 0.1f;
    [SerializeField] private float startYOffset = -300f;
    [SerializeField] private float overshootScale = 1.15f;
    [SerializeField] private LevelUpEffect levelUpEffect;

    [Header("Keyboard")]
    [SerializeField] private float highlightScaleMultiplier = 1.08f;
    [SerializeField] private Color highlightColor = new Color(1f, 0.9f, 0.4f);

    private Action<UpgradeData> onSelectCallback;
    private List<UpgradeData> currentChoices;
    private Vector2[] originalPositions;
    private Vector3[] originalScales;
    private RectTransform[] cardRects;

    private int highlightedIndex = 0;
    private bool keyboardActive = false;
    private Color[] originalCardColors;

    private void Awake()
    {
        panel.SetActive(false);

        cardRects = new RectTransform[cardButtons.Length];
        originalPositions = new Vector2[cardButtons.Length];
        originalScales = new Vector3[cardButtons.Length];
        originalCardColors = new Color[cardButtons.Length];

        for (int i = 0; i < cardButtons.Length; i++)
        {
            cardRects[i] = cardButtons[i].GetComponent<RectTransform>();
            originalPositions[i] = cardRects[i].anchoredPosition;
            originalScales[i] = cardRects[i].localScale;

            Image img = cardButtons[i].GetComponent<Image>();
            if (img != null) originalCardColors[i] = img.color;

            int index = i;
            cardButtons[i].onClick.AddListener(() => SelectCard(index));
        }
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        // Sol/Sağ ok ile gezin
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            keyboardActive = true;
            int count = GetActiveCardCount();
            highlightedIndex = (highlightedIndex - 1 + count) % count;
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            keyboardActive = true;
            int count = GetActiveCardCount();
            highlightedIndex = (highlightedIndex + 1) % count;
            UpdateHighlight();
        }
        // Enter/Space ile vurgulanan kartı seç
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (keyboardActive
                && highlightedIndex < cardButtons.Length
                && cardButtons[highlightedIndex].interactable)
            {
                SelectCard(highlightedIndex);
            }
        }
        // 1/2/3 ile direkt seç
        else if (Input.GetKeyDown(KeyCode.Alpha1) && cardButtons.Length > 0 && cardButtons[0].interactable)
        {
            SelectCard(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && cardButtons.Length > 1 && cardButtons[1].interactable)
        {
            SelectCard(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && cardButtons.Length > 2 && cardButtons[2].interactable)
        {
            SelectCard(2);
        }
    }

    public void Show(List<UpgradeData> choices, Action<UpgradeData> callback)
    {
        currentChoices = choices;
        onSelectCallback = callback;

        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                cardButtons[i].gameObject.SetActive(true);

                string baseName = choices[i].upgradeName;
                string suffix = GetStackSuffix(choices[i]);
                cardNames[i].text = string.IsNullOrEmpty(suffix) ? baseName : $"{baseName} {suffix}";

                cardDescriptions[i].text = choices[i].description;
            }
            else
            {
                cardButtons[i].gameObject.SetActive(false);
            }
        }

        panel.SetActive(true);

        if (levelUpEffect != null)
            levelUpEffect.PlayShockwave();

        // Klavye state sıfırla
        highlightedIndex = 0;
        keyboardActive = false;

        StartCoroutine(AnimateCardsIn());
    }

    private IEnumerator AnimateCardsIn()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (!cardButtons[i].gameObject.activeSelf) continue;
            cardRects[i].anchoredPosition = originalPositions[i] + new Vector2(0, startYOffset);
            cardRects[i].localScale = originalScales[i] * 0.5f;
            cardButtons[i].interactable = false;
        }

        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (!cardButtons[i].gameObject.activeSelf) continue;
            StartCoroutine(BounceCard(i));
            yield return new WaitForSecondsRealtime(delayBetweenCards);
        }
    }

    private IEnumerator BounceCard(int index)
    {
        RectTransform rect = cardRects[index];
        Vector2 startPos = originalPositions[index] + new Vector2(0, startYOffset);
        Vector2 endPos = originalPositions[index];
        Vector3 startScale = originalScales[index] * 0.5f;
        Vector3 peakScale = originalScales[index] * overshootScale;
        Vector3 endScale = originalScales[index];

        float t = 0f;
        while (t < bounceDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / bounceDuration);

            float ease = 1f - Mathf.Pow(1f - p, 3f);
            rect.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, ease);

            if (p < 0.7f)
            {
                float sp = p / 0.7f;
                rect.localScale = Vector3.LerpUnclamped(startScale, peakScale, sp);
            }
            else
            {
                float sp = (p - 0.7f) / 0.3f;
                rect.localScale = Vector3.LerpUnclamped(peakScale, endScale, sp);
            }

            yield return null;
        }

        rect.anchoredPosition = endPos;
        rect.localScale = endScale;
        cardButtons[index].interactable = true;
    }

    private int GetActiveCardCount()
    {
        int count = 0;
        foreach (var btn in cardButtons)
            if (btn.gameObject.activeSelf) count++;
        return Mathf.Max(1, count);
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (!cardButtons[i].gameObject.activeSelf) continue;

            if (keyboardActive && i == highlightedIndex)
            {
                cardRects[i].localScale = originalScales[i] * highlightScaleMultiplier;
                Image img = cardButtons[i].GetComponent<Image>();
                if (img != null) img.color = highlightColor;
            }
            else
            {
                cardRects[i].localScale = originalScales[i];
                Image img = cardButtons[i].GetComponent<Image>();
                if (img != null && originalCardColors != null) img.color = originalCardColors[i];
            }
        }
    }

    private string GetStackSuffix(UpgradeData upgrade)
    {
        if (playerStats == null) return "";

        if (upgrade.type == UpgradeType.Boomerang)
            return $"(Lv {playerStats.boomerangLevel}/{playerStats.maxBoomerangLevel})";

        if (upgrade.type == UpgradeType.Lightning)
            return $"(Lv {playerStats.lightningLevel}/{playerStats.maxLightningLevel})";

        int stack = playerStats.GetStackCount(upgrade.type);
        if (stack == 0) return "(New!)";

        if (stack >= playerStats.softCapThreshold)
            return $"(×{stack} - Decreasing Effect!)";

        return $"(×{stack})";
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private void SelectCard(int index)
    {
        if (index < currentChoices.Count && onSelectCallback != null)
        {
            onSelectCallback(currentChoices[index]);
        }
    }
}