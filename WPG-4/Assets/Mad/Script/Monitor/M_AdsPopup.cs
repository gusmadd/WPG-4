using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_AdsPopup : MonoBehaviour
{
    [Header("Close Button")]
    public Collider2D closeCollider;

    [Header("Animator")]
    public Animator adsAnimator;
    public string inStateName = "in";
    public string outTriggerName = "AdsOut";
    public float outAnimDelay = 0.25f;

    bool isOpen = false;
    bool isClosing = false;

    private M_GameManager.GameState previousState = M_GameManager.GameState.Gameplay;
    private bool hasStoredPreviousState = false;

    void OnEnable()
    {
        ShowAds();
    }

    void OnDisable()
    {
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.StopAdsNoise();

        isOpen = false;
        isClosing = false;
        hasStoredPreviousState = false;
    }

    void Update()
    {
        if (!isOpen) return;
        if (isClosing) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (closeCollider != null && closeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                CloseAds();
            }
        }
    }

    public void ShowAds()
    {
        // Ads hanya boleh muncul saat gameplay
        if (M_GameManager.Instance == null) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
        {
            gameObject.SetActive(false);
            return;
        }

        isOpen = true;
        isClosing = false;

        previousState = M_GameManager.Instance.currentState;
        hasStoredPreviousState = true;

        M_GameManager.Instance.currentState = M_GameManager.GameState.AdsOverlay;

        M_AudioManager.Instance?.PlayAdsSfx();

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.StartAdsNoise();

        if (adsAnimator != null && !string.IsNullOrEmpty(inStateName))
            adsAnimator.Play(inStateName, 0, 0f);
    }

    public void CloseAds()
    {
        if (!isOpen) return;
        if (isClosing) return;

        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        isClosing = true;

        if (adsAnimator != null && !string.IsNullOrEmpty(outTriggerName))
            adsAnimator.SetTrigger(outTriggerName);

        yield return new WaitForSecondsRealtime(outAnimDelay);

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.StopAdsNoise();

        if (M_GameManager.Instance != null)
        {
            if (hasStoredPreviousState)
                M_GameManager.Instance.currentState = previousState;
            else
                M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;
        }

        isOpen = false;
        isClosing = false;
        hasStoredPreviousState = false;

        gameObject.SetActive(false);
    }
}