using UnityEngine;

public enum ItemCategory
{
    Food,
    Toys
}

[System.Serializable]
public class ItemData
{
    public string id;
    public string displayName;
    public Sprite icon;
    public ItemCategory category = ItemCategory.Food;
}
