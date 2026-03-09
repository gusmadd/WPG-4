using UnityEngine;
using System.Collections;

public class PCScreenController : MonoBehaviour
{
    [Header("Main Screen")]
    public SpriteRenderer screenSprite;   // layar PC utama
    public GameState activeState = GameState.DESKTOP;

    [Header("Browser Layer")]
    public SpriteRenderer browserLayer;   // layer browser
    public float browserDelay = 2f;       // delay setelah nyala PC

    private bool isOn = false;
    private bool browserShown = false;

    void Start()
    {
        if (screenSprite != null)
            screenSprite.enabled = false;

        if (browserLayer != null)
            browserLayer.enabled = false;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        if (!isOn && GameManager.Instance.CurrentState == activeState)
        {
            TurnOnScreen();
        }
    }

    void TurnOnScreen()
    {
        isOn = true;

        if (screenSprite != null)
            screenSprite.enabled = true;

        Debug.Log("SCREEN ON 🖥️✨");

        // mulai delay browser
        StartCoroutine(ShowBrowserAfterDelay());
    }

    IEnumerator ShowBrowserAfterDelay()
    {
        yield return new WaitForSeconds(browserDelay);

        if (browserLayer != null && !browserShown)
        {
            browserLayer.enabled = true;
            browserShown = true;

            Debug.Log("BROWSER LAYER ON 🌐🔥");
        }
    }
}
