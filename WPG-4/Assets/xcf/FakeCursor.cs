using UnityEngine;

public class FakeCursor : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 offset = new Vector2(-20f, 20f);

    [Header("Pivot (0-1)")]
    [Range(0f, 1f)] public float pivotX = 0f;
    [Range(0f, 1f)] public float pivotY = 1f;

    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        ApplyCursorPivotAndOffset();
    }

    void ApplyCursorPivotAndOffset()
    {
        if (rt == null) return;

        rt.pivot = new Vector2(pivotX, pivotY);
    }

    void Update()
    {
        transform.position = (Vector2)Input.mousePosition + offset;
    }
}