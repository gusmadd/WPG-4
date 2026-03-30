using UnityEngine;

public class FakeCursor : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 offset; // adjust click point if needed

    void Update()
    {
        transform.position = (Vector2)Input.mousePosition + offset;
    }
}