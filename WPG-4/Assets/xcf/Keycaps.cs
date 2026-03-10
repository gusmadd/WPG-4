using UnityEngine;

public class KeyboardKey : MonoBehaviour
{
    public string keyValue; // huruf tombol
    public SearchPageController searchController;

    void OnMouseDown()
    {
        if (searchController != null)
        {
            searchController.AddChar(keyValue);
        }
    }
}