using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void Awake()
    {
        Instance = this;
    }

    // ===================== FADE =====================

    public IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        fadeCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
    }

    // ===================== SHAKE =====================

    public IEnumerator Shake()
    {
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

    // ===================== TIMER =====================

    public void StartTimer(float maxTime)
    {
        if (timerUI != null)
            timerUI.SetActive(true);

        if (timerFill != null)
            timerFill.fillAmount = 1f;
    }

    public void UpdateTimer(float current, float max)
    {
        if (timerFill != null)
            timerFill.fillAmount = Mathf.Clamp01(current / max);
    }

    public void StopTimer()
    {
        if (timerUI != null)
            timerUI.SetActive(false);
    }

}
