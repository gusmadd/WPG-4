using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_GameManager : MonoBehaviour
{
    public enum GameState
    {
        Gameplay,
        QTE
    }

    public GameState currentState = GameState.Gameplay;

    public static M_GameManager Instance;

    [Header("QTE")]
    public GameObject qtePrefab;

    [Header("Effects")]
    public GameObject fastFadeUI;
    public Transform cameraTransform;

    [Header("Shake Settings")]
    public float shakeDuration = 0.4f;
    public float shakeMagnitude = 0.2f;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.2f;

    [Header("Animators")]
    public Animator catAnimator;

    private bool isSequenceRunning = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        M_NoiseSystem.Instance.OnNoiseFull += HandleNoiseFull;
    }

    void HandleNoiseFull()
    {
        if (!isSequenceRunning)
            StartCoroutine(NoiseFullSequence());
    }

    IEnumerator NoiseFullSequence()
    {
        isSequenceRunning = true;
        currentState = GameState.QTE;

        // 🛑 Freeze Gameplay
        Time.timeScale = 0f;
        M_NoiseSystem.Instance.isQTEActive = true;

        // 📳 Shake
        yield return StartCoroutine(ShakeCamera());

        // ⚫ Quick Fade
        yield return StartCoroutine(Fade(0f, 1f));
        yield return StartCoroutine(Fade(1f, 0f));

        yield return new WaitForSecondsRealtime(0.2f);
        
        if (catAnimator != null)
            catAnimator.SetTrigger("OnNoiseFull");

        if (fastFadeUI != null)
            fastFadeUI.SetActive(false);

        // ⏳ Delay cinematic
        yield return new WaitForSecondsRealtime(1f);

        // 🎮 Spawn QTE
        Instantiate(qtePrefab);

        // Unfreeze
        Time.timeScale = 1f;

        isSequenceRunning = false;
    }

    IEnumerator ShakeCamera()
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
    IEnumerator Fade(float from, float to)
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
}
