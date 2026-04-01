using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("Day Settings")]
    public float dayDurationSeconds = 120f;

    [Header("Runtime")]
    public int itemsPerTask = 3;
    public List<string> targetItemIds = new List<string>();
    public HashSet<string> purchasedSet = new HashSet<string>();
    public bool completed;

    float timer;
    bool timerRunning = false;

    public event Action OnDaySuccess;
    public event Action OnDayFailed;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!timerRunning) return;
        if (completed) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = 0f;
            FailDay();
        }
    }

    public void SetupNewDay(int newItemsPerTask, float durationSeconds)
    {
        StopTimer();

        itemsPerTask = newItemsPerTask;
        dayDurationSeconds = durationSeconds;

        targetItemIds.Clear();
        purchasedSet.Clear();
        completed = false;

        timer = dayDurationSeconds;

        Debug.Log("SetupNewDay done. itemsPerTask=" + itemsPerTask + " timer=" + GetTimeLeft());

        CreateTaskList();
    }

    void CreateTaskList()
    {
        if (ItemDatabase.Instance == null || ItemDatabase.Instance.items.Count == 0)
        {
            Debug.LogError("ItemDatabase kosong.");
            return;
        }

        int safety = 999;
        while (targetItemIds.Count < itemsPerTask && safety-- > 0)
        {
            ItemData pick = ItemDatabase.Instance.GetRandom();
            if (pick == null) break;

            if (!targetItemIds.Contains(pick.id))
                targetItemIds.Add(pick.id);
        }

        Debug.Log("Task list: " + string.Join(",", targetItemIds));
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    void FailDay()
    {
        if (completed) return;
        timerRunning = false;
        OnDayFailed?.Invoke();
    }

    public void OnItemPurchased(string itemId)
    {
        if (completed) return;
        if (!targetItemIds.Contains(itemId)) return;

        purchasedSet.Add(itemId);

        if (purchasedSet.Count >= targetItemIds.Count)
        {
            completed = true;
            timerRunning = false;
            OnDaySuccess?.Invoke();
            return;
        }

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ShowReminderOverlay(
                DayManager.Instance != null ? DayManager.Instance.GetCurrentDay() : 1
            );
    }

    public bool IsPurchased(string itemId) => purchasedSet.Contains(itemId);
    public float GetTimeLeft() => Mathf.Max(0f, timer);

    public static string FormatTime(float t)
    {
        int sec = Mathf.CeilToInt(t);
        int m = sec / 60;
        int r = sec % 60;
        return m.ToString("00") + ":" + r.ToString("00");
    }

    public void PauseTimer()
    {
        StopTimer();
    }

    public void ResumeTimer()
    {
        StartTimer();
    }
}