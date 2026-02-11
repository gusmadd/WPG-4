using UnityEngine;

public class BrowserBarButton : MonoBehaviour
{
    [Header("Layers")]
    public SpriteRenderer browserBarLayer;   // address bar
    public SpriteRenderer keyboardRenderer;  // keyboard sprite renderer

    private bool isActive = false;

    void Start()
    {
        // pastiin keyboard ngumpet pas awal
        if (keyboardRenderer != null)
            keyboardRenderer.enabled = false;

        if (browserBarLayer != null)
            browserBarLayer.enabled = false;
    }

    void OnMouseDown()
    {
        ActivateBrowserBar();
    }

    void ActivateBrowserBar()
    {
        if (isActive) return;
        isActive = true;

        if (browserBarLayer != null)
            browserBarLayer.enabled = true;

        if (keyboardRenderer != null)
            keyboardRenderer.enabled = true;

        Debug.Log("BROWSER BAR ON 🌐🦈 + KEYBOARD POP ⌨️");
    }
}
