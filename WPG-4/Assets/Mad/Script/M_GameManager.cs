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

        Time.timeScale = 0f;
        M_NoiseSystem.Instance.isQTEActive = true;

        // Shake (UI_Script)
        yield return StartCoroutine(UI_Script.Instance.Shake());

        // Fade (UI_Script)
        yield return StartCoroutine(UI_Script.Instance.Fade(0f, 1f));
        yield return StartCoroutine(UI_Script.Instance.Fade(1f, 0f));

        yield return new WaitForSecondsRealtime(0.2f);

        if (catAnimator != null)
            catAnimator.SetTrigger("OnNoiseFull");

        yield return new WaitForSecondsRealtime(1f);

        Instantiate(qtePrefab);

        Time.timeScale = 1f;

        isSequenceRunning = false;
    }

    public IEnumerator QTESuccess()
    {
        yield return new WaitForSecondsRealtime(1f);

        yield return StartCoroutine(ReduceNoiseSmoothly(31f));

        yield return new WaitForSecondsRealtime(0.5f);

        yield return StartCoroutine(UI_Script.Instance.Fade(0f, 1f));
        yield return StartCoroutine(UI_Script.Instance.Fade(1f, 0f));

        yield return new WaitForSecondsRealtime(1f);

        M_NoiseSystem.Instance.isQTEActive = false;
        M_NoiseSystem.Instance.ResetAfterQTE();

        currentState = GameState.Gameplay;

        if (catAnimator != null)
            catAnimator.SetTrigger("OnBackToIdle");
    }

    IEnumerator ReduceNoiseSmoothly(float targetValue)
    {
        float speed = 40f;

        while (M_NoiseSystem.Instance.currentNoise > targetValue)
        {
            M_NoiseSystem.Instance.currentNoise -= speed * Time.unscaledDeltaTime;
            yield return null;
        }

        M_NoiseSystem.Instance.currentNoise = targetValue;
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 1f;
    }
}
