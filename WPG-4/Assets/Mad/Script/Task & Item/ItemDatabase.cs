using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    public List<ItemData> items = new List<ItemData>();
    void Awake()
    {
        Instance = this;
    }

    public ItemData GetById(string id)
    {
        for (int i = 0; i < items.Count; i++)
            if (items[i].id == id) return items[i];
        return null;
    }

    public ItemData GetRandom()
    {
        if (items == null || items.Count == 0) return null;
        return items[Random.Range(0, items.Count)];
    }
}
