using UnityEngine;

[RequireComponent(typeof(Health))]
public class BossDeathHandler : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandleBossDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleBossDeath;
    }

    private void HandleBossDeath()
    {
        // Boss öldü, WaveManager'a haber ver
        WaveManager.Instance?.OnBossDefeated();
    }
}