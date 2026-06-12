using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public float smoothTime = 0.15f;
    public float speedFollowFactor = 1.5f;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0, 0, -10);

    private Vector3 velocity = Vector3.zero;
    private Rigidbody2D targetRb;
    private Vector3 shakeOffset = Vector3.zero;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (target != null)
            targetRb = target.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        float currentSmoothTime = smoothTime;
        if (targetRb != null)
        {
            float playerSpeed = targetRb.linearVelocity.magnitude;
            currentSmoothTime = smoothTime / (1f + playerSpeed * speedFollowFactor * 0.1f);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            currentSmoothTime
        ) + shakeOffset;
    }

    public void Shake(float duration = 0.5f, float magnitude = 0.01f)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            shakeOffset = (Vector3)(Random.insideUnitCircle * magnitude);
            elapsed += Time.unscaledDeltaTime;  // <-- scaled değil
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }
}