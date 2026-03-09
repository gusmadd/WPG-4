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

    void OnEnable()
    {
        ShowAds();
    }

    void OnDisable()
    {
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.StopAdsNoise();

        isOpen = false;
    }

    void Update()
    {
        if (!isOpen) return;

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
        isOpen = true;

        M_AudioManager.Instance?.PlayAdsSfx();

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.StartAdsNoise();

        if (adsAnimator != null && !string.IsNullOrEmpty(inStateName))
            adsAnimator.Play(inStateName, 0, 0f);

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.AdsOverlay;
    }

    public void CloseAds()
    {
        if (!isOpen) return;
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        if (adsAnimator != null && !string.IsNullOrEmpty(outTriggerName))
            adsAnimator.SetTrigger(outTriggerName);

        yield return new WaitForSecondsRealtime(outAnimDelay);

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.StopAdsNoise();

        isOpen = false;
        gameObject.SetActive(false);
    }
}
