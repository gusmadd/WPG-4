using System.Collections;
using System.Collections.Generic;
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

    [Header("Wrong Purchase Effect")]
    public Animator wrongPurchaseAnimator;
    public bool useShakeOnWrongPurchase = true;
    public float wrongShakeDuration = 0.15f;
    public float wrongShakeMagnitude = 0.08f;

    [Header("QTE Timer")]
    public GameObject timerUI;
    public Image timerFill;

    [Header("Day Success Panel")]
    public GameObject daySuccessPanel;
    public Button dayHomeButton;
    public Button dayNextButton;
    public TMP_Text daySuccessText;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public Button gameOverHomeButton;
    public Button gameOverRestartButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (daySuccessPanel != null) daySuccessPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (dayHomeButton != null) dayHomeButton.onClick.AddListener(OnClickDayHome);
        if (dayNextButton != null) dayNextButton.onClick.AddListener(OnClickDayNext);

        if (gameOverHomeButton != null) gameOverHomeButton.onClick.AddListener(OnClickGameOverHome);
        if (gameOverRestartButton != null) gameOverRestartButton.onClick.AddListener(OnClickGameOverRestart);
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

    public void PlayWrongEffect()
    {
        if (wrongPurchaseAnimator != null)
        {
            wrongPurchaseAnimator.ResetTrigger("Play");
            wrongPurchaseAnimator.SetTrigger("Play");
        }

        if (useShakeOnWrongPurchase)
            StartCoroutine(ShakeWrongPurchase());
    }

    IEnumerator ShakeWrongPurchase()
    {
        if (cameraTransform == null) yield break;

        Vector3 originalPos = cameraTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < wrongShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * wrongShakeMagnitude;
            float y = Random.Range(-1f, 1f) * wrongShakeMagnitude;

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
            timerFill.fillAmount = Mathf.Clamp01(current / max);
    }

    public void StopTimer()
    {
        if (timerUI != null) timerUI.SetActive(false);
    }

    public void ShowDaySuccess(int day)
    {
        if (daySuccessPanel != null) daySuccessPanel.SetActive(true);
        if (daySuccessText != null) daySuccessText.text = "Day " + day + " Success";

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;
    }

    public void HideDaySuccess()
    {
        if (daySuccessPanel != null) daySuccessPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void OnClickDayHome()
    {
        M_AudioManager.Instance?.PlayCursorClick();
        DayManager.Instance?.GoHome();
    }

    void OnClickDayNext()
    {
        M_AudioManager.Instance?.PlayCursorClick();
        HideDaySuccess();
        DayManager.Instance?.NextDay();
    }

    void OnClickGameOverHome()
    {
        M_AudioManager.Instance?.PlayCursorClick();
        DayManager.Instance?.GoHome();
    }

    void OnClickGameOverRestart()
    {
        M_AudioManager.Instance?.PlayCursorClick();
        HideGameOver();
        DayManager.Instance?.RestartGame();
    }
}