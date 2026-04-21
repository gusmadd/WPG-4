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

    [Header("Surprise")]
    public string surpriseTriggerName = "isSuprise";
    public float surpriseCooldown = 0.25f;
    private float surpriseTimer = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        UpdateTypingCooldown();
        UpdateSurpriseCooldown();
    }

    void UpdateTypingCooldown()
    {
        if (typingTimer > 0f)
            typingTimer -= Time.unscaledDeltaTime;
    }

    void UpdateSurpriseCooldown()
    {
        if (surpriseTimer > 0f)
            surpriseTimer -= Time.unscaledDeltaTime;
    }

    bool CanPlayTyping()
    {
        if (M_GameManager.Instance == null) return true;

        M_GameManager.GameState state = M_GameManager.Instance.currentState;

        if (state != M_GameManager.GameState.Gameplay)
            return false;

        if (M_NoiseSystem.Instance != null && M_NoiseSystem.Instance.isQTEActive)
            return false;

        return true;
    }

    public void PlayNoiseFull()
    {
        if (playerAnimator == null) return;

        playerAnimator.ResetTrigger("OnBackToIdle");
        playerAnimator.ResetTrigger("Typing");
        playerAnimator.ResetTrigger(surpriseTriggerName);
        playerAnimator.SetTrigger("OnNoiseFull");
    }

    public void BackToIdle()
    {
        if (playerAnimator == null) return;

        playerAnimator.ResetTrigger("OnNoiseFull");
        playerAnimator.ResetTrigger("Typing");
        playerAnimator.ResetTrigger(surpriseTriggerName);
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

    public void PlaySurprise()
    {
        if (playerAnimator == null) return;
        if (surpriseTimer > 0f) return;

        playerAnimator.ResetTrigger("Typing");
        playerAnimator.ResetTrigger("OnBackToIdle");
        playerAnimator.ResetTrigger("OnNoiseFull");
        playerAnimator.ResetTrigger(surpriseTriggerName);
        playerAnimator.SetTrigger(surpriseTriggerName);

        surpriseTimer = surpriseCooldown;
    }
}