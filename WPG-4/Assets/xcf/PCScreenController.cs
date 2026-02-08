using UnityEngine;

public class PCScreenController : MonoBehaviour
{
    public SpriteRenderer screenSprite;   // sprite layar pc
    public GameState activeState = GameState.DESKTOP; // state saat layar nyala

    private bool isOn = false;

    void Start()
    {
        if (screenSprite != null)
            screenSprite.enabled = false; // awal mati
    }

void Update()
{
    if (GameManager.Instance == null)
    {
        Debug.Log("GM NULL ❌");
        return;
    }

    if (!isOn && GameManager.Instance.CurrentState == activeState)
    {
        TurnOnScreen();
    }
}


    void TurnOnScreen()
    {
        isOn = true;

        if (screenSprite != null)
        {
            screenSprite.enabled = true;
            screenSprite.color = Color.white;

            Debug.Log("SCREEN ON 🖥️✨");
        }
        else
        {
            Debug.Log("SCREEN ga nyala ❌");
        }
    }
}