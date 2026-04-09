using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_PlayerController : MonoBehaviour
{
    public static M_PlayerController Instance;

    [Header("Animator")]
    public Animator playerAnimator;

    [Header("Typing")]
    public float typingCooldown = 0.12f;

    private float typingTimer = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        UpdateTypingCooldown();
    }

    void UpdateTypingCooldown()
    {
        if (typingTimer > 0f)
            typingTimer -= Time.unscaledDeltaTime;
    }

    bool CanPlayTyping()
    {
        if (M_GameManager.Instance == null) return true;

        M_GameManager.GameState state = M_GameManager.Instance.currentState;

        // typing hanya saat gameplay biasa
        if (state != M_GameManager.GameState.Gameplay)
            return false;

        // jangan typing saat QTE aktif
        if (M_NoiseSystem.Instance != null && M_NoiseSystem.Instance.isQTEActive)
            return false;

        return true;
    }

    public void PlayNoiseFull()
    {
        if (playerAnimator == null) return;

        playerAnimator.ResetTrigger("OnBackToIdle");
        playerAnimator.ResetTrigger("Typing");
        playerAnimator.SetTrigger("OnNoiseFull");
    }

    public void BackToIdle()
    {
        if (playerAnimator == null) return;

        playerAnimator.ResetTrigger("OnNoiseFull");
        playerAnimator.ResetTrigger("Typing");
        playerAnimator.SetTrigger("OnBackToIdle");
    }

    public void PlayTyping()
    {
        if (playerAnimator == null) return;
        if (!CanPlayTyping()) return;

        playerAnimator.ResetTrigger("Typing");
        playerAnimator.SetTrigger("Typing");
        typingTimer = typingCooldown;
    }
}
