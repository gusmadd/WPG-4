using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldPowerButton : MonoBehaviour, 
    IPointerDownHandler, IPointerUpHandler
{
    public float holdTimeRequired = 2f;   // detik
    public Image progressFill;            // UI fill (optional)

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool activated = false;

    void Update()
    {
        if (isHolding && !activated)
        {
            holdTimer += Time.deltaTime;

            if (progressFill != null)
                progressFill.fillAmount = holdTimer / holdTimeRequired;

            if (holdTimer >= holdTimeRequired)
            {
                ActivatePower();
            }
        }
    }

    // ================= UI INPUT =================
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("UI POINTER DOWN 🔥");
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("UI POINTER UP ❄️");
        ResetHold();
    }

    // ================= WORLD INPUT FALLBACK =================
    void OnMouseDown()
    {
        Debug.Log("WORLD MOUSE DOWN 🔥");
        isHolding = true;
    }

    void OnMouseUp()
    {
        Debug.Log("WORLD MOUSE UP ❄️");
        ResetHold();
    }

    // ================= LOGIC =================
    void ActivatePower()
    {
        if (activated) return;

        activated = true;
        isHolding = false;

        Debug.Log("PC POWER ON 🔥💻");

        if (GameManager.Instance != null)
            GameManager.Instance.PowerOn();
        else
            Debug.LogError("GameManager Instance NOT FOUND ❌");
    }

    void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;

        if (progressFill != null)
            progressFill.fillAmount = 0f;
    }
}
