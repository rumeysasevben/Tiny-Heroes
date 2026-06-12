using UnityEngine;
using UnityEngine.UI;

public class SoundToggleUI : MonoBehaviour
{
    [SerializeField] private Sprite soundOnIcon;
    [SerializeField] private Sprite soundOffIcon;
    [SerializeField] private Image iconImage;

    void OnEnable()
    {
        UpdateIcon();
    }

    public void OnClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
            UpdateIcon();
        }
        else
        {
            Debug.LogWarning("AudioManager bulunamadı.");
        }
    }

    void UpdateIcon()
    {
        if (iconImage != null && AudioManager.Instance != null)
            iconImage.sprite = AudioManager.Instance.IsMuted() ? soundOffIcon : soundOnIcon;
    }
}