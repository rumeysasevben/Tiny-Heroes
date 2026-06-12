using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int size = 20;

    private Queue<GameObject> pool;
    private Transform holder;

    private void Awake()
    {
        pool = new Queue<GameObject>();

        // Disabled holder: çocukları aktive etmeden saklayalım (OnEnable fire etmesin)
        GameObject holderGO = new GameObject(prefab.name + "_PoolHolder");
        holderGO.transform.SetParent(transform);
        holderGO.SetActive(false);
        holder = holderGO.transform;

        for (int i = 0; i < size; i++)
        {
            CreateAndEnqueue();
        }
    }

    private void CreateAndEnqueue()
    {
        GameObject obj = Instantiate(prefab, holder);
        pool.Enqueue(obj);
    }

    public GameObject Get()
    {
        // Pool tükendiyse otomatik genişle (hata yerine yeni objesi oluştur)
        if (pool.Count == 0)
        {
            CreateAndEnqueue();
        }

        GameObject obj = pool.Dequeue();
        obj.transform.SetParent(null);   // disabled holder'dan çıksın
        obj.SetActive(true);
        pool.Enqueue(obj);                // rotating queue
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
    }
}