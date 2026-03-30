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
    public SpriteRenderer buySprite;

    [Header("Navigation")]
    public M_SearchInput homeSearchInput;

    [Header("Item Info")]
    public string itemId;

    bool isHoldingBuy = false;
    float holdTimer = 0f;

    Material runtimeBuyMat;
    static readonly int FillAmountID = Shader.PropertyToID("_FillAmount");

    void Awake()
    {
        if (buySprite != null && buySprite.sharedMaterial != null)
        {
            runtimeBuyMat = new Material(buySprite.sharedMaterial);
            buySprite.material = runtimeBuyMat;
        }
    }

    void OnEnable()
    {
        ResetHoldState();
    }

    void OnDestroy()
    {
        if (runtimeBuyMat != null)
            Destroy(runtimeBuyMat);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay) return;

        Vector2 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (backCollider != null && backCollider.OverlapPoint(mousePosWorld))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                if (catFoodPage != null) catFoodPage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }

            if (homeCollider != null && homeCollider.OverlapPoint(mousePosWorld))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                if (petshopHomePage != null) petshopHomePage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }

            if (closeCollider != null && closeCollider.OverlapPoint(mousePosWorld))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (desktopPage != null) desktopPage.SetActive(true);
                if (homeSearchInput != null) homeSearchInput.ResetToDefault();
                gameObject.SetActive(false);
                return;
            }

            if (serviceCollider != null && serviceCollider.OverlapPoint(mousePosWorld))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                if (servicePage != null) servicePage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }

            if (buyCollider != null && buyCollider.OverlapPoint(mousePosWorld))
            {
                StartHoldBuy();
                return;
            }
        }

        if (Input.GetMouseButton(0) && isHoldingBuy)
        {
            if (buyCollider == null || !buyCollider.OverlapPoint(mousePosWorld))
            {
                CancelHoldBuy();
                return;
            }

            holdTimer += Time.deltaTime;
            UpdateBuyFill();

            if (holdTimer >= holdToBuySeconds)
            {
                CompleteBuy();
                return;
            }
        }

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
        UpdateBuyFill();
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

    void UpdateBuyFill()
    {
        if (runtimeBuyMat == null) return;

        float t = Mathf.Clamp01(holdTimer / Mathf.Max(0.01f, holdToBuySeconds));
        runtimeBuyMat.SetFloat(FillAmountID, t);
    }

    void ResetHoldState()
    {
        isHoldingBuy = false;
        holdTimer = 0f;

        if (runtimeBuyMat != null)
            runtimeBuyMat.SetFloat(FillAmountID, 0f);
    }
}