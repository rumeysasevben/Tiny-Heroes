using System.Collections;
using UnityEngine;

public class HitPause : MonoBehaviour
{
    public static HitPause Instance { get; private set; }

    private Coroutine current;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Freeze(float duration)
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(FreezeRoutine(duration));
    }

    IEnumerator FreezeRoutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        current = null;
    }
}