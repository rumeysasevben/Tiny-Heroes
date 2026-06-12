using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
         CameraFollow.Instance?.Shake(0.15f, 0.2f);
        Time.timeScale = 0f;  // Oyunu durdur
    }
}