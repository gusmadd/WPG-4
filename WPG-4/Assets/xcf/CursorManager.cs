using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public GameObject customCursor;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

        if (customCursor != null)
            customCursor.SetActive(true);
    }
}