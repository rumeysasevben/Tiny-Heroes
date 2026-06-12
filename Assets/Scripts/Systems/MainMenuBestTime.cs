using UnityEngine;
using TMPro;

public class MainMenuBestTime : MonoBehaviour
{
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private string prefix = "Best Time: ";

    private const string BEST_TIME_KEY = "BestTime";

    private void Start()
    {
        float best = PlayerPrefs.GetFloat(BEST_TIME_KEY, 0f);

        if (best <= 0f)
        {
            bestTimeText.text = prefix + "--:--";
        }
        else
        {
            int minutes = Mathf.FloorToInt(best / 60f);
            int seconds = Mathf.FloorToInt(best % 60f);
            bestTimeText.text = prefix + $"{minutes:00}:{seconds:00}";
        }
    }
}