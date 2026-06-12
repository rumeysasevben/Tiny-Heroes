using System.Collections.Generic;
using UnityEngine;

public class LightningController : MonoBehaviour
{
    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private PlayerStats stats;

    private List<GameObject> orbs = new List<GameObject>();
    private float orbitAngle;

    private void Awake()
    {
        if (stats == null) stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (stats == null || !stats.HasLightning)
        {
            if (orbs.Count > 0) ClearOrbs();
            return;
        }

        int desiredCount = stats.GetLightningCount();
        if (orbs.Count != desiredCount)
            RebuildOrbs(desiredCount);

        orbitAngle += stats.GetLightningOrbitSpeed() * Time.deltaTime;
        float radius = stats.GetLightningRadius();
        float damage = stats.GetLightningDamage() * stats.DamageMultiplier;

        for (int i = 0; i < orbs.Count; i++)
        {
            float angle = orbitAngle + (360f / orbs.Count) * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
            orbs[i].transform.localPosition = offset;

            // Sprite dışarıya baksın (player'dan dışa doğru)
            orbs[i].transform.localRotation = Quaternion.Euler(0, 0, angle  + 180f);

            var orb = orbs[i].GetComponent<LightningOrb>();
            if (orb != null) orb.SetDamage(damage);
        }
    }

    private void RebuildOrbs(int count)
    {
        ClearOrbs();
        for (int i = 0; i < count; i++)
        {
            GameObject o = Instantiate(orbPrefab, transform);
            o.transform.localPosition = Vector3.zero;
            orbs.Add(o);
        }
    }

    private void ClearOrbs()
    {
        foreach (var o in orbs)
            if (o != null) Destroy(o);
        orbs.Clear();
    }
}