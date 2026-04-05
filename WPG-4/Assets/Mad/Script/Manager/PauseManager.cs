using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI")]
    public GameObject pausePanel;
    public Animator pauseAnimator;

    private bool isPaused = false;
    private bool isTransitioning = false;

    void Awake()
    {
        Instance = this;
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

        // ❗ block kondisi tertentu
        if (M_GameManager.Instance != null &&
            M_GameManager.Instance.currentState == M_GameManager.GameState.QTE)
            return;

        isPaused = true;
        isTransitioning = true;

        pausePanel.SetActive(true);
        pauseAnimator.SetTrigger("In");

        Time.timeScale = 0f;
        TaskManager.Instance?.PauseTimer();

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;

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

        pauseAnimator.SetTrigger("Out");

        yield return new WaitForSecondsRealtime(0.45f);

        pausePanel.SetActive(false);

        Time.timeScale = 1f;
        TaskManager.Instance?.ResumeTimer();

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;

        isPaused = false;
        isTransitioning = false;
    }

    public void Restart()
    {
        if (!isPaused || isTransitioning) return;

        StartCoroutine(RestartRoutine());
    }

    IEnumerator RestartRoutine()
    {
        isTransitioning = true;

        pauseAnimator.SetTrigger("Out");

        yield return new WaitForSecondsRealtime(0.45f);

        Time.timeScale = 1f;

        pausePanel.SetActive(false);

        isPaused = false;
        isTransitioning = false;

        DayManager.Instance.RestartGame();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator FinishTransition()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        isTransitioning = false;
    }
}
