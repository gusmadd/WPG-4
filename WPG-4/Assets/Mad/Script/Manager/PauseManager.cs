using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI")]
    public GameObject pausePanel;
    public Animator pauseAnimator;
    public GameObject pauseButton;

    [Header("Day Info")]
    public TMP_Text dayText;

    private bool isPaused = false;
    private bool isTransitioning = false;

    private M_GameManager.GameState stateBeforePause;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        RefreshPauseButton();
    }

    void Update()
    {
        RefreshPauseButton();
    }

    void RefreshPauseButton()
    {
        if (pauseButton == null) return;

        bool shouldShow = true;

        if (M_GameManager.Instance != null)
        {
            var state = M_GameManager.Instance.currentState;

            if (state == M_GameManager.GameState.QTE ||
                state == M_GameManager.GameState.TaskOverlay)
            {
                shouldShow = false;
            }
        }

        if (isPaused)
            shouldShow = false;

        if (pauseButton.activeSelf != shouldShow)
            pauseButton.SetActive(shouldShow);
    }

    bool CanPause()
    {
        if (M_GameManager.Instance == null) return true;

        var state = M_GameManager.Instance.currentState;

        if (state == M_GameManager.GameState.QTE) return false;
        if (state == M_GameManager.GameState.TaskOverlay) return false;

        return true;
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (isPaused || isTransitioning) return;
        if (!CanPause()) return;

        isPaused = true;
        isTransitioning = true;

        if (dayText != null && DayManager.Instance != null)
            dayText.text = "" + DayManager.Instance.GetCurrentDay();

        if (M_GameManager.Instance != null)
            stateBeforePause = M_GameManager.Instance.currentState;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (pauseAnimator != null)
        {
            pauseAnimator.ResetTrigger("Out");
            pauseAnimator.SetTrigger("In");
        }

        Time.timeScale = 0f;
        TaskManager.Instance?.PauseTimer();

        RefreshPauseButton();
        StartCoroutine(FinishTransition());
    }

    public void Resume()
    {
        if (!isPaused || isTransitioning) return;
        StartCoroutine(ResumeRoutine());
    }

    IEnumerator ResumeRoutine()
    {
        isTransitioning = true;

        if (pauseAnimator != null)
        {
            pauseAnimator.ResetTrigger("In");
            pauseAnimator.SetTrigger("Out");
        }

        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        TaskManager.Instance?.ResumeTimer();

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = stateBeforePause;

        isPaused = false;
        isTransitioning = false;

        RefreshPauseButton();
    }

    public void Restart()
    {
        if (!isPaused || isTransitioning) return;
        StartCoroutine(RestartRoutine());
    }

    IEnumerator RestartRoutine()
    {
        isTransitioning = true;

        if (pauseAnimator != null)
        {
            pauseAnimator.ResetTrigger("In");
            pauseAnimator.SetTrigger("Out");
        }

        yield return new WaitForSecondsRealtime(0.45f);

        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        isPaused = false;
        isTransitioning = false;

        DayManager.Instance?.RestartGame();

        RefreshPauseButton();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Week");
    }

    IEnumerator FinishTransition()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        isTransitioning = false;
    }
}