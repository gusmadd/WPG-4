using System.Collections;
using UnityEngine;

public class M_AdsPopup : MonoBehaviour
{
    [Header("Close Button")]
    public Collider2D closeCollider;

    [Header("Animator")]
    public Animator adsAnimator;
    public string inTriggerName = "AdsIn";
    public string outTriggerName = "AdsOut";
    public float outAnimDelay = 0.25f;

    bool isOpen = false;
    bool isClosing = false;

    private M_GameManager.GameState previousState = M_GameManager.GameState.Gameplay;
    private bool hasStoredPreviousState = false;

    Vector3 initialLocalScale;
    Vector3 initialLocalPosition;
    Quaternion initialLocalRotation;

    RectTransform rectTransform;
    Vector2 initialAnchoredPosition;
    Vector2 initialSizeDelta;

    void Awake()
    {
        initialLocalScale = transform.localScale;
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;

        rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            initialAnchoredPosition = rectTransform.anchoredPosition;
            initialSizeDelta = rectTransform.sizeDelta;
        }
    }

    void OnEnable()
    {
        StopAllCoroutines();

        if (adsAnimator != null)
            adsAnimator.enabled = true;   // nyalakan lagi animator

        ResetVisualState();
        ShowAds();
    }

    void OnDisable()
    {
        StopAllCoroutines();

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.StopAdsNoise();

        isOpen = false;
        isClosing = false;
        hasStoredPreviousState = false;

        ResetVisualState();
    }

    void Update()
    {
        if (!isOpen || isClosing) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main == null) return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (closeCollider != null && closeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                CloseAds();
            }
        }
    }

    void ResetVisualState()
    {
        transform.localScale = initialLocalScale;
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = initialAnchoredPosition;
            rectTransform.sizeDelta = initialSizeDelta;
            rectTransform.localScale = initialLocalScale;
            rectTransform.localRotation = initialLocalRotation;
        }

        if (adsAnimator != null && gameObject.activeSelf)
        {
            adsAnimator.Rebind();
            adsAnimator.Update(0f);
            adsAnimator.ResetTrigger(inTriggerName);
            adsAnimator.ResetTrigger(outTriggerName);
        }
    }

    public void ShowAds()
    {
        if (M_GameManager.Instance == null) return;

        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
        {
            gameObject.SetActive(false);
            return;
        }

        ResetVisualState();

        isOpen = true;
        isClosing = false;

        previousState = M_GameManager.Instance.currentState;
        hasStoredPreviousState = true;

        M_GameManager.Instance.currentState = M_GameManager.GameState.AdsOverlay;

        M_AudioManager.Instance?.PlayAdsSfx();
        M_NoiseSystem.Instance?.StartAdsNoise();

        // kalau pakai trigger
        adsAnimator.ResetTrigger(outTriggerName);
        adsAnimator.SetTrigger(inTriggerName);
    }

    public void CloseAds()
    {
        if (!isOpen || isClosing) return;
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        isClosing = true;

        M_NoiseSystem.Instance?.StopAdsNoise();

        if (adsAnimator != null && !string.IsNullOrEmpty(outTriggerName))
        {
            adsAnimator.ResetTrigger(inTriggerName);
            adsAnimator.SetTrigger(outTriggerName);
        }

        yield return new WaitForSecondsRealtime(outAnimDelay);

        RestoreGameState();

        isOpen = false;
        isClosing = false;
        hasStoredPreviousState = false;

        gameObject.SetActive(false);
    }

    public void ForceCloseAdsInstant()
    {
        StopAllCoroutines();

        M_NoiseSystem.Instance?.StopAdsNoise();

        // kembalikan state game
        if (M_GameManager.Instance != null &&
            M_GameManager.Instance.currentState == M_GameManager.GameState.AdsOverlay)
        {
            if (hasStoredPreviousState)
                M_GameManager.Instance.currentState = previousState;
            else
                M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;
        }

        isOpen = false;
        isClosing = false;
        hasStoredPreviousState = false;

        // jangan biarkan animator mati permanen
        if (adsAnimator != null)
        {
            adsAnimator.Rebind();
            adsAnimator.Update(0f);
        }

        gameObject.SetActive(false);
    }

    void RestoreGameState()
    {
        if (M_GameManager.Instance != null &&
            M_GameManager.Instance.currentState == M_GameManager.GameState.AdsOverlay)
        {
            if (hasStoredPreviousState)
                M_GameManager.Instance.currentState = previousState;
            else
                M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;
        }
    }

}