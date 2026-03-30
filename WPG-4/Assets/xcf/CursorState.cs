using UnityEngine;
using UnityEngine.UI;

public class CursorState : MonoBehaviour
{
    [Header("Cursor Sprites")]
    public Sprite normalCursor;
    public Sprite clickCursor;

    private Image cursorImage;

    void Start()
    {
        cursorImage = GetComponent<Image>();
        cursorImage.sprite = normalCursor;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            cursorImage.sprite = clickCursor;
        }
        else
        {
            cursorImage.sprite = normalCursor;
        }
    }
}