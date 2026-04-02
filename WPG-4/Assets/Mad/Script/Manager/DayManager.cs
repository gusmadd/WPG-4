using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    [Header("Week Setup")]
    [Range(1, 4)] public int currentWeek = 1;

    public int startDay = 1;
    public int startTasks = 3;
    public int tasksIncreasePerDay = 1;
    public float dayDurationSeconds = 120f;

    public float bootDelay = 1.5f;
    public float successDelay = 0.7f;

    [Header("Pages Reset")]
    public GameObject desktopPage;
    public GameObject[] pagesToDisable;
    public M_MonitorManager monitorManager;

    [Header("Search Reset")]
    public M_SearchInput homeSearchInput;
    public M_SearchInput resultSearchInput;

    [Header("Ads Changce per day")]
    public float day1AdsChance = 0.05f;
    public float day2AdsChance = 0.1f;
    public float day3AdsChance = 0.15f;
    public float adsChanceAfterDay3 = 0.2f;

    [Header("Week End")]
    public int maxDay = 5;
    public GameObject weekCompletePanel;

    int currentDay;
    public string mainMenuSceneName = "MainMenu";
    public string weekChoiceSceneName = "WeekChoice";
    public string nextWeekSceneName = "NextWeekScene";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentDay = startDay;

        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.OnDaySuccess += HandleDaySuccess;
            TaskManager.Instance.OnDayFailed += HandleDayFailed;
            TaskManager.Instance.StopTimer();
        }

        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();
        HideWeekComplete();

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ResetForNewDay(currentDay);

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.ResetForNewDay();

        ResetPagesToDesktop();

        if (monitorManager != null)
            monitorManager.ResetToOff();

        StopAllCoroutines();
        StartCoroutine(StartDayRoutine(currentDay));
    }

    IEnumerator StartDayRoutine(int day)
    {
        int tasks = startTasks + (day - 1) * tasksIncreasePerDay;

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.ResetForNewDay();

        if (TaskManager.Instance != null)
            TaskManager.Instance.SetupNewDay(tasks, dayDurationSeconds);

        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();
        HideWeekComplete();

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ResetForNewDay(day);

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Boot;

        yield return new WaitForSecondsRealtime(bootDelay);

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ShowStartDayOverlay(day);

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;
    }

    void HandleDaySuccess()
    {
        StartCoroutine(SuccessRoutine());
    }

    IEnumerator SuccessRoutine()
    {
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.FreezeNoise(true);

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;

        yield return new WaitForSecondsRealtime(successDelay);

        if (currentDay >= maxDay)
        {
            M_ProgressManager.CompleteWeek(currentWeek);
            if (weekCompletePanel != null)
                weekCompletePanel.SetActive(true);
            yield break;
        }

        UI_Script.Instance?.ShowDaySuccess(currentDay);
    }

    void HandleDayFailed()
    {
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.FreezeNoise(true);

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;

        UI_Script.Instance?.ShowGameOver();
    }

    public void NextDay()
    {
        if (currentDay >= maxDay)
        {
            if (weekCompletePanel != null)
                weekCompletePanel.SetActive(true);
            return;
        }

        currentDay++;

        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();
        HideWeekComplete();

        ResetPagesToDesktop();

        if (TaskManager.Instance != null)
            TaskManager.Instance.StopTimer();

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ResetForNewDay(currentDay);

        StopAllCoroutines();

        if (monitorManager != null)
            monitorManager.ResetToOff();

        StartCoroutine(StartDayRoutine(currentDay));
    }

    public void RestartGame()
    {
        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();

        if (TaskManager.Instance != null)
            TaskManager.Instance.StopTimer();

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ResetForNewDay(startDay);

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.ResetForNewDay();

        ResetPagesToDesktop();
        M_GameManager.Instance?.catAnimator?.SetTrigger("OnBackToIdle");

        currentDay = startDay;

        if (monitorManager != null)
            monitorManager.ResetToOff();

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;

        StopAllCoroutines();
        StartCoroutine(StartDayRoutine(currentDay));
    }

    public void GoHome()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void GoToWeekChoice()
    {
        SceneManager.LoadScene(weekChoiceSceneName);
    }

    public void GoToNextWeek()
    {
        SceneManager.LoadScene(nextWeekSceneName);
    }

    void ResetPagesToDesktop()
    {
        if (pagesToDisable != null)
        {
            for (int i = 0; i < pagesToDisable.Length; i++)
                if (pagesToDisable[i] != null)
                    pagesToDisable[i].SetActive(false);
        }

        if (desktopPage != null)
            desktopPage.SetActive(true);

        if (homeSearchInput != null)
            homeSearchInput.ResetToDefault();

        if (resultSearchInput != null)
            resultSearchInput.ResetToDefault();
    }

    public float GetAdsChanceForCurrentDay()
    {
        if (currentDay <= 1) return Mathf.Clamp01(day1AdsChance);
        if (currentDay == 2) return Mathf.Clamp01(day2AdsChance);
        if (currentDay == 3) return Mathf.Clamp01(day3AdsChance);
        return Mathf.Clamp01(adsChanceAfterDay3);
    }

    public bool TrySpawnAdsOnPawshoppClick()
    {
        float chance = GetAdsChanceForCurrentDay();
        return Random.value < chance;
    }

    public void TryShowAdsFromPawshoppClick()
    {
        if (monitorManager == null) return;

        if (TrySpawnAdsOnPawshoppClick())
            monitorManager.ShowRandomAdsFromExternal();
    }

    public void HideWeekComplete()
    {
        if (weekCompletePanel != null)
            weekCompletePanel.SetActive(false);
    }

    public void StartNewGameFromMenu()
    {
        StopAllCoroutines();

        currentDay = startDay;

        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();
        HideWeekComplete();

        if (TaskManager.Instance != null)
            TaskManager.Instance.StopTimer();

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ResetForNewDay(currentDay);

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.ResetForNewDay();

        ResetPagesToDesktop();

        if (monitorManager != null)
            monitorManager.ResetToOff();

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Boot;

        StartCoroutine(StartDayRoutine(currentDay));
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    public int GetCurrentWeek()
    {
        return currentWeek;
    }
}
