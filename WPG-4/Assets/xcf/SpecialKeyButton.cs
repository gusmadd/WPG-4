using UnityEngine;

public class SpecialKeyButton : MonoBehaviour
{
    public enum KeyType { Backspace, Clear, Enter }
    public KeyType keyType;

    void OnMouseDown()
    {
        if (KeyboardManager.Instance == null) return;

        switch (keyType)
        {
            case KeyType.Backspace:
                KeyboardManager.Instance.Backspace();
                break;

            case KeyType.Clear:
                KeyboardManager.Instance.Clear();
                break;

            case KeyType.Enter:
               Debug.Log("ENTER PRESSED 🌐");

              if (SearchEnterUnlock.Instance != null)
               SearchEnterUnlock.Instance.OnEnterPressed();

               break;

        }
    }
}
