using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    protected override bool IsDDOL => false;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    public void CreatePool(GameObject prefab, int size)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                poolDictionary[key].Enqueue(obj);
            }
        }
    }
    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
        {
            CreatePool(prefab, 1);
        }

        GameObject obj = poolDictionary[key].Count > 0 ? poolDictionary[key].Dequeue() : Instantiate(prefab);

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }

    public void ReturnToPool(GameObject prefab, GameObject obj)
    {
        string key = prefab.name;

        obj.SetActive(false);
        poolDictionary[key].Enqueue(obj);
    }

}
