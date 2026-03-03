using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_DetailFoodPage : MonoBehaviour
{
    [Header("Navigation")]
    public GameObject catFoodPage;
    public GameObject petshopHomePage;
    public GameObject desktopPage;
    public GameObject buySuccessPage;
    public GameObject servicePage;

    [Header("Buttons")]
    public Collider2D backCollider;
    public Collider2D homeCollider;
    public Collider2D closeCollider;
    public Collider2D buyCollider;
    public Collider2D serviceCollider;

    [Header("Hold to Buy")]
    public float holdToBuySeconds = 2f;
    public SpriteRenderer buySprite;          // Sprite tombol BUY
    public Color buyNormalColor = Color.white;
    public Color buyHoldColor = new Color(0.35f, 0.35f, 0.35f, 1f);

    [Header("Navigation")]
    public M_SearchInput homeSearchInput;

    [Header("Item Info")]
    public string itemId;

    bool isHoldingBuy = false;
    float holdTimer = 0f;

    void OnEnable()
    {
        ResetHoldState();
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay) return;

        Vector2 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            // klik tombol lain tetap normal
            if (backCollider != null && backCollider.OverlapPoint(mousePosWorld))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (catFoodPage != null) catFoodPage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            //ke home page
            if (homeCollider != null && homeCollider.OverlapPoint(mousePosWorld))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (petshopHomePage != null) petshopHomePage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            // ke dekstop
            if (closeCollider != null && closeCollider.OverlapPoint(mousePosWorld))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (desktopPage != null) desktopPage.SetActive(true);
                if (homeSearchInput != null) homeSearchInput.ResetToDefault();
                gameObject.SetActive(false);
                return;
            }
            //ke service page
            if (serviceCollider != null && serviceCollider.OverlapPoint(mousePosWorld))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (servicePage != null) servicePage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }

            // mulai hold kalau klik kena BUY
            if (buyCollider != null && buyCollider.OverlapPoint(mousePosWorld))
            {
                StartHoldBuy();
                return;
            }
        }

        // update hold saat tombol mouse masih ditekan
        if (Input.GetMouseButton(0) && isHoldingBuy)
        {
            // kalau cursor keluar dari collider BUY, batal
            if (buyCollider == null || !buyCollider.OverlapPoint(mousePosWorld))
            {
                CancelHoldBuy();
                return;
            }

            holdTimer += Time.deltaTime;
            UpdateBuyDarkening();

            if (holdTimer >= holdToBuySeconds)
            {
                CompleteBuy();
                return;
            }
        }

        // kalau mouse dilepas sebelum selesai, batal
        if (Input.GetMouseButtonUp(0) && isHoldingBuy)
        {
            CancelHoldBuy();
            return;
        }
    }

    void StartHoldBuy()
    {
        isHoldingBuy = true;
        holdTimer = 0f;
        UpdateBuyDarkening();
    }

    void CancelHoldBuy()
    {
        ResetHoldState();
    }

    void CompleteBuy()
    {
        M_AudioManager.Instance?.PlayPayment();

        if (TaskManager.Instance != null)
            TaskManager.Instance.OnItemPurchased(itemId);
        if (buySuccessPage != null)
            buySuccessPage.SetActive(true);

        ResetHoldState();
        gameObject.SetActive(false);
    }

    void UpdateBuyDarkening()
    {
        if (buySprite == null) return;

        float t = Mathf.Clamp01(holdTimer / Mathf.Max(0.01f, holdToBuySeconds));
        buySprite.color = Color.Lerp(buyNormalColor, buyHoldColor, t);
    }

    void ResetHoldState()
    {
        isHoldingBuy = false;
        holdTimer = 0f;

        if (buySprite != null)
            buySprite.color = buyNormalColor;
    }
}