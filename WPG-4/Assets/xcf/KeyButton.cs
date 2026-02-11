using UnityEngine;

public class KeyButton : MonoBehaviour
{
    public string keyValue; // contoh: "a", "b", "1", ".", "www", "com"

    void OnMouseDown()
    {
        if (KeyboardManager.Instance != null)
            KeyboardManager.Instance.AddChar(keyValue);
    }
}
