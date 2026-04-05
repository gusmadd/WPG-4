using UnityEngine;

public class M_QTEPopUp : MonoBehaviour
{
    private M_QTEAddClean manager;
    private bool isClosed = false;

    [Header("Hit Area")]
    [SerializeField] private Collider2D hitCollider;

    public void Init(M_QTEAddClean qteManager)
    {
        manager = qteManager;

        if (hitCollider == null)
            hitCollider = GetComponent<Collider2D>();
    }

    void Awake()
    {
        if (hitCollider == null)
            hitCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isClosed) return;
        if (M_NoiseSystem.Instance == null) return;
        if (!M_NoiseSystem.Instance.isQTEActive) return;
        if (hitCollider == null) return;
        if (Camera.main == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (hitCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                CloseAd();
            }
        }
    }

    void CloseAd()
    {
        if (isClosed) return;

        isClosed = true;

        if (manager != null)
            manager.AdClosed(this);

        Destroy(gameObject);
    }
}