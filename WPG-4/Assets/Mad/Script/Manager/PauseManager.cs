using System.Collections;
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

        M_AudioManager.Instance?.PlayRandomUi();

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
        StartCoroutine(FinishPauseAudioAndTransition());
    }

    IEnumerator FinishPauseAudioAndTransition()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        AudioListener.pause = true;
        isTransitioning = false;
    }

    public void Resume()
    {
        AudioListener.pause = false;
        M_AudioManager.Instance?.PlayRandomUi();
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
        AudioListener.pause = false;
        M_AudioManager.Instance?.PlayRandomUi();
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

        yield return new WaitForSecondsRealtime(1f);

        AudioListener.pause = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (M_GameManager.Instance != null && M_GameManager.Instance.keyboard != null)
            M_GameManager.Instance.keyboard.HideKeyboard();

        isPaused = false;
        isTransitioning = false;

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithTransition(currentSceneName);
        else
            SceneManager.LoadScene(currentSceneName);
    }

    public void BackToMenu()
    {
        AudioListener.pause = false;
        M_AudioManager.Instance?.PlayRandomUi();
        Time.timeScale = 1f;

        M_MonitorManager monitor = FindObjectOfType<M_MonitorManager>();
        if (monitor != null)
            monitor.ResetToOff();

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithTransition("Week");
    }

    IEnumerator FinishTransition()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        isTransitioning = false;
    }
}