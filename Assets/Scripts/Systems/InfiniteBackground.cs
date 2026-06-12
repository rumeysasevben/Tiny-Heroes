using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [Tooltip("Genelde Main Camera")]
    public Transform target;

    [Tooltip("floor.png'nin world unit cinsinden boyutu (PPU=8 ise 2)")]
    public float tileSize = 2f;

    void LateUpdate()
    {
        if (target == null) return;

        float x = Mathf.Round(target.position.x / tileSize) * tileSize;
        float y = Mathf.Round(target.position.y / tileSize) * tileSize;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}