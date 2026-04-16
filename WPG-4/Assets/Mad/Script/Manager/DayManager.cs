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

    [Header("Ads Chance per day")]
    public float day1AdsChance = 0.05f;
    public float day2AdsChance = 0.1f;
    public float day3AdsChance = 0.15f;
    public float adsChanceAfterDay3 = 0.2f;

    [Header("Week End")]
    public int maxDay = 5;
    public GameObject weekCompletePanel;

    public GameObject[] dayCalendarObjects; // Array berisi 5 GameObject (Day 1 - Day 5)
    public float calendarDisplayDuration = 5f; // Durasi tampil kalender
    int currentDay;

    public string mainMenuSceneName = "MainMenu";
    public string weekChoiceSceneName = "WeekChoice";
    public string nextWeekSceneName = "NextWeekScene";


    bool isAdvancingDay = false;
    bool isEndingDay = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Max Day: " + maxDay + ", Current Day: " + currentDay);

        currentDay = startDay;

        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.OnDaySuccess -= HandleDaySuccess;
            TaskManager.Instance.OnDaySuccess += HandleDaySuccess;

            TaskManager.Instance.OnDayFailed -= HandleDayFailed;
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

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.OnDaySuccess -= HandleDaySuccess;
            TaskManager.Instance.OnDayFailed -= HandleDayFailed;
        }
    }

    IEnumerator StartDayRoutine(int day)
    {
        Debug.Log("Starting Day: " + day);  // Debug untuk memeriksa apakah StartDayRoutine dipanggil

        int tasks = startTasks + (day - 1) * tasksIncreasePerDay;
        TelemetryManager.Instance?.SendDayStart(day, tasks);

        isAdvancingDay = false;
        isEndingDay = false;

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
        if (isEndingDay) return;
        isEndingDay = true;

        TelemetryManager.Instance?.SendDayCompleted(currentDay);

        M_GameManager.Instance?.ForceEndQTEState();
        StartCoroutine(SuccessRoutine());
    }

    IEnumerator SuccessRoutine()
    {
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.FreezeNoise(true);

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;

        yield return new WaitForSecondsRealtime(successDelay);


        // --- LOGIKA KALENDER BARU ---
        // Cek apakah index kalender valid (currentDay biasanya 1-5, jadi index -1)
        int calendarIndex = currentDay - 1;
        if (dayCalendarObjects != null && calendarIndex < dayCalendarObjects.Length)
        {
            // Aktifkan object kalender yang sesuai hari ini
            if (dayCalendarObjects[calendarIndex] != null)
            {
                dayCalendarObjects[calendarIndex].SetActive(true);

                // Jika ada script animasi spesifik, jalankan di sini
                // calendarAnimation?.PlayAnimation(); 

                // Tunggu selama durasi yang ditentukan
                yield return new WaitForSecondsRealtime(calendarDisplayDuration);

                // Matikan kembali object kalender
                dayCalendarObjects[calendarIndex].SetActive(false);
            }
        }
        // ----------------------------

        // Logika setelah kalender selesai tampil
        if (currentDay >= maxDay)
        {
            M_ProgressManager.CompleteWeek(currentWeek);

            if (currentWeek >= 4)
            {
                TelemetryManager.Instance?.SendSessionEnd();
                Debug.Log("Game tamat di week 4, session_end dikirim");
            }

            if (weekCompletePanel != null)
            {
                M_AudioManager.Instance?.PlayDaySuccesSfx();
                weekCompletePanel.SetActive(true);
            }
            yield break;
        }

        M_AudioManager.Instance?.PlayDaySuccesSfx();
        UI_Script.Instance?.ShowDaySuccess(currentDay);
    }

    void HandleDayFailed()
    {
        if (isEndingDay) return;
        isEndingDay = true;

        TelemetryManager.Instance?.SendPlayerFail("timer_ran_out", currentDay, currentWeek);
        TelemetryManager.Instance?.SendSessionEnd();

        M_GameManager.Instance?.ForceEndQTEState();

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.FreezeNoise(true);

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;

        UI_Script.Instance?.ShowGameOver();
    }

    public void NextDay()
    {
        Debug.Log("Current Day before increment: " + currentDay);  // Tambahkan debug

        if (currentDay >= maxDay)
        {
            if (weekCompletePanel != null)
                weekCompletePanel.SetActive(true);

            isAdvancingDay = false;
            return;
        }

        currentDay++;  // Increment currentDay
        Debug.Log("Current Day after increment: " + currentDay);  // Tambahkan debug untuk memeriksa increment

        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();
        HideWeekComplete();

        ResetPagesToDesktop();

        if (TaskManager.Instance != null)
            TaskManager.Instance.StopTimer();

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ResetForNewDay(currentDay);

        StopAllCoroutines();  // Pastikan tidak ada coroutine yang berjalan

        if (monitorManager != null)
            monitorManager.ResetToOff();

        // Panggil StartDayRoutine untuk memulai hari baru
        StartCoroutine(StartDayRoutine(currentDay));
    }

    public void RestartGame()
    {
        if (M_AudioManager.Instance.sfxSource != null && M_AudioManager.Instance.footstepSfx != null)
        {
            M_AudioManager.Instance.sfxSource.clip = M_AudioManager.Instance.footstepSfx;  // Set footstep SFX ke AudioSource
            M_AudioManager.Instance.sfxSource.loop = true;  // Aktifkan loop untuk footstep
            M_AudioManager.Instance.sfxSource.volume = M_AudioManager.Instance.miscVolume;  // Set volume sesuai dengan volume yang diinginkan
            M_AudioManager.Instance.sfxSource.Play();  // Memainkan footstep
        }
        UI_Script.Instance?.HideDaySuccess();
        UI_Script.Instance?.HideGameOver();

        if (TaskManager.Instance != null)
            TaskManager.Instance.StopTimer();

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ResetForNewDay(startDay);

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.ResetForNewDay();

        ResetPagesToDesktop();
        M_PlayerController.Instance?.BackToIdle();

        currentDay = startDay;
        isAdvancingDay = false;
        isEndingDay = false;

        if (monitorManager != null)
            monitorManager.ResetToOff();

        StopAllCoroutines();
        StartCoroutine(StartDayRoutine(currentDay));
    }

    public void GoHome()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithTransition(mainMenuSceneName);
    }

    public void GoToWeekChoice()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithTransition(weekChoiceSceneName);
    }

    public void GoToNextWeek()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithTransition(nextWeekSceneName);
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
        isAdvancingDay = false;
        isEndingDay = false;

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

        StartCoroutine(StartDayRoutine(currentDay));
    }

    public int GetCurrentWeek()
    {
        return currentWeek;
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }
}