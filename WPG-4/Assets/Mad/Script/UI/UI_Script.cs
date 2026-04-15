using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Script : MonoBehaviour
{
    public static UI_Script Instance;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.2f;

    [Header("Shake")]
    public Transform cameraTransform;
    public float shakeDuration = 0.4f;
    public float shakeMagnitude = 0.2f;

    [Header("QTE Timer")]
    public GameObject timerUI;
    public Image timerFill;

    [Header("incorrect effect")]
    public Animator incorrectEffect;

    [Header("Day Success Panel")]
    public GameObject daySuccessPanel;
    public Button dayHomeButton;
    public Button dayNextButton;
    public TMP_Text daySuccessText;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public Button gameOverHomeButton;
    public Button gameOverRestartButton;

    [Header("Close All Ads UI")]
    public GameObject closeAllAdsUI;
    public Animator closeAllAdsAnimator;
    public float closeAllAdsOutDelay = 0.35f;

    // anti double trigger
    bool isProcessingDayNext = false;
    bool isProcessingDayHome = false;
    bool isProcessingGameOverHome = false;
    bool isProcessingGameOverRestart = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (closeAllAdsUI != null) closeAllAdsUI.SetActive(false);
        if (daySuccessPanel != null) daySuccessPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (dayHomeButton != null)
        {
            dayHomeButton.onClick.RemoveListener(OnClickDayHome);
            dayHomeButton.onClick.AddListener(OnClickDayHome);
        }

        if (dayNextButton != null)
        {
            dayNextButton.onClick.RemoveListener(OnClickDayNext);
            dayNextButton.onClick.AddListener(OnClickDayNext);
        }

        if (gameOverHomeButton != null)
        {
            gameOverHomeButton.onClick.RemoveListener(OnClickGameOverHome);
            gameOverHomeButton.onClick.AddListener(OnClickGameOverHome);
        }

        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.RemoveListener(OnClickGameOverRestart);
            gameOverRestartButton.onClick.AddListener(OnClickGameOverRestart);
        }
    }

    public IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;

            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = Mathf.Lerp(from, to, t);

            yield return null;
        }

        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = to;
    }

    public IEnumerator Shake()
    {
        if (cameraTransform == null) yield break;

        Vector3 originalPos = cameraTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            cameraTransform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalPos;
    }

    public void StartTimer(float maxTime)
    {
        if (timerUI != null) timerUI.SetActive(true);
        if (timerFill != null) timerFill.fillAmount = 1f;
    }

    public void UpdateTimer(float current, float max)
    {
        if (timerFill != null)
            timerFill.fillAmount = Mathf.Clamp01(1 - current / max);
    }

    public void StopTimer()
    {
        if (timerUI != null) timerUI.SetActive(false);
    }

    public void ShowDaySuccess(int day)
    {
        isProcessingDayNext = false;
        isProcessingDayHome = false;

        if (daySuccessPanel != null) daySuccessPanel.SetActive(true);
        if (daySuccessText != null) daySuccessText.text = "Day " + day + " Success";

        if (dayNextButton != null) dayNextButton.interactable = true;
        if (dayHomeButton != null) dayHomeButton.interactable = true;

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;
    }
    public void PlayWrongEffect()
    {
        if (incorrectEffect != null)
            incorrectEffect.SetTrigger("Play");
        // optional: camera shake (small)
        if (cameraTransform != null)
            StartCoroutine(Shake());
    }
    public void HideDaySuccess()
    {
        if (daySuccessPanel != null) daySuccessPanel.SetActive(false);

        if (dayNextButton != null) dayNextButton.interactable = true;
        if (dayHomeButton != null) dayHomeButton.interactable = true;

        isProcessingDayNext = false;
        isProcessingDayHome = false;
    }

    public void ShowGameOver()
    {
        isProcessingGameOverHome = false;
        isProcessingGameOverRestart = false;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (gameOverHomeButton != null) gameOverHomeButton.interactable = true;
        if (gameOverRestartButton != null) gameOverRestartButton.interactable = true;

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (gameOverHomeButton != null) gameOverHomeButton.interactable = true;
        if (gameOverRestartButton != null) gameOverRestartButton.interactable = true;

        isProcessingGameOverHome = false;
        isProcessingGameOverRestart = false;
    }

    void OnClickDayHome()
    {
        if (isProcessingDayHome) return;
        isProcessingDayHome = true;

        if (dayHomeButton != null)
            dayHomeButton.interactable = false;

        if (dayNextButton != null)
            dayNextButton.interactable = false;

        M_AudioManager.Instance?.PlayCursorClick();
        DayManager.Instance?.GoHome();
    }

    void OnClickDayNext()
    {
        if (isProcessingDayNext) return;
        isProcessingDayNext = true;

        if (dayNextButton != null)
            dayNextButton.interactable = false;

        if (dayHomeButton != null)
            dayHomeButton.interactable = false;

        M_AudioManager.Instance?.PlayCursorClick();
        HideDaySuccess();
        DayManager.Instance?.NextDay();
    }

    void OnClickGameOverHome()
    {
        if (isProcessingGameOverHome) return;
        isProcessingGameOverHome = true;

        if (gameOverHomeButton != null)
            gameOverHomeButton.interactable = false;

        if (gameOverRestartButton != null)
            gameOverRestartButton.interactable = false;

        M_AudioManager.Instance?.PlayCursorClick();
        DayManager.Instance?.GoHome();
    }

    void OnClickGameOverRestart()
    {
        if (isProcessingGameOverRestart) return;
        isProcessingGameOverRestart = true;

        if (gameOverRestartButton != null)
            gameOverRestartButton.interactable = false;

        if (gameOverHomeButton != null)
            gameOverHomeButton.interactable = false;

        M_AudioManager.Instance?.PlayCursorClick();
        HideGameOver();
        DayManager.Instance?.RestartGame();
    }

    public void ShowCloseAllAds()
    {
        if (closeAllAdsUI != null)
            closeAllAdsUI.SetActive(true);

        if (closeAllAdsAnimator != null)
        {
            closeAllAdsAnimator.ResetTrigger("out");
            closeAllAdsAnimator.ResetTrigger("idle");
            closeAllAdsAnimator.ResetTrigger("in");
            closeAllAdsAnimator.SetTrigger("in");
        }
    }

    public IEnumerator HideCloseAllAdsRoutine()
    {
        if (closeAllAdsAnimator != null)
        {
            closeAllAdsAnimator.ResetTrigger("in");
            closeAllAdsAnimator.ResetTrigger("idle");
            closeAllAdsAnimator.ResetTrigger("out");
            closeAllAdsAnimator.SetTrigger("out");

            yield return new WaitForSecondsRealtime(closeAllAdsOutDelay);
        }

        if (closeAllAdsUI != null)
            closeAllAdsUI.SetActive(false);
    }

    public void HideCloseAllAdsInstant()
    {
        if (closeAllAdsUI != null)
            closeAllAdsUI.SetActive(false);
    }
}