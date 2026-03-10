using UnityEngine;

public class HoldBuyTutorial : MonoBehaviour
{
    public GameObject congratsPopup;
    public float holdTime = 2f;

    private float holdTimer = 0f;
    private bool holding = false;
    private bool completed = false;
    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();

        // popup disembunyikan saat awal
        if (congratsPopup != null)
            congratsPopup.SetActive(false);
    }

    void Update()
    {
        if (completed) return;

        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col != null && col.OverlapPoint(mousePos))
                holding = true;
        }
        else
        {
            holding = false;
            holdTimer = 0f;
        }

        if (holding)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdTime)
                CompleteTutorial();
        }
    }

    void CompleteTutorial()
    {
        completed = true;

        if (congratsPopup != null)
            congratsPopup.SetActive(true);

        // 🔴 STOP GAME
        Time.timeScale = 0f;

        Debug.Log("Tutorial Completed!");
    }
}