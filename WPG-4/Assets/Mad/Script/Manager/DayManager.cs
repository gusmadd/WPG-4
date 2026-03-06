using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

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

    int currentDay;

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
        }

        StopAllCoroutines();
        StartCoroutine(StartDayRoutine(currentDay)); // DAY MULAI SAAT PLAY
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

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ResetForNewDay();

        // saat boot, player belum boleh klik apa apa
        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Boot;

        yield return new WaitForSecondsRealtime(bootDelay);

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ShowTaskOverlay(true);

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
        currentDay++;

        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();

        ResetPagesToDesktop();

        StopAllCoroutines();
        StartCoroutine(StartDayRoutine(currentDay));
    }

    public void RestartGame()
    {
        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();

        if (TaskManager.Instance != null)
            TaskManager.Instance.StopTimer();

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ResetForNewDay();

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.ResetForNewDay();

        ResetPagesToDesktop();

        currentDay = startDay;

        if (monitorManager != null)
            monitorManager.ResetToOff();

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;

        StopAllCoroutines();
        StartCoroutine(StartDayRoutine(currentDay)); // balik ke flow awal lagi
    }
    public void GoHome()
    {
        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();

        if (monitorManager != null)
            monitorManager.ResetToOff();

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;
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
}
