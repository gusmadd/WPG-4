using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("Day Settings")]
    public float dayDurationSeconds = 120f;

    [Header("Runtime")]
    public int itemsPerTask = 3;
    public List<string> targetItemIds = new List<string>();
    public List<string> purchasedItemIds = new List<string>();
    public bool completed;

    float timer;
    bool timerRunning = false;
    bool dayResolved = false;

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
        if (dayResolved) return;

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
        purchasedItemIds.Clear();
        completed = false;
        dayResolved = false;

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

        List<ItemCategory> allowedCategories = GetAllowedCategoriesForCurrentWeek();
        List<ItemData> allowedItems = ItemDatabase.Instance.GetItemsByCategories(allowedCategories);

        if (allowedItems.Count == 0)
        {
            Debug.LogError("Tidak ada item yang cocok untuk week ini.");
            return;
        }

        List<ItemData> shuffledPool = new List<ItemData>(allowedItems);
        Shuffle(shuffledPool);

        int uniqueTargetCount = Mathf.Min(itemsPerTask, shuffledPool.Count);

        for (int i = 0; i < uniqueTargetCount; i++)
            targetItemIds.Add(shuffledPool[i].id);

        while (targetItemIds.Count < itemsPerTask)
        {
            ItemData duplicatePick = allowedItems[UnityEngine.Random.Range(0, allowedItems.Count)];
            targetItemIds.Add(duplicatePick.id);
        }

        Debug.Log("Week " + GetCurrentWeek() + " task list: " + string.Join(",", targetItemIds));
    }

    List<ItemCategory> GetAllowedCategoriesForCurrentWeek()
    {
        int week = GetCurrentWeek();
        List<ItemCategory> result = new List<ItemCategory>();

        switch (week)
        {
            case 1:
                result.Add(ItemCategory.Food);
                break;
            case 2:
                result.Add(ItemCategory.Toys);
                break;
            case 3:
            case 4:
                result.Add(ItemCategory.Food);
                result.Add(ItemCategory.Toys);
                break;
            default:
                result.Add(ItemCategory.Food);
                result.Add(ItemCategory.Toys);
                break;
        }

        return result;
    }

    int GetCurrentWeek()
    {
        if (DayManager.Instance == null)
            return 1;

        return DayManager.Instance.GetCurrentWeek();
    }

    void Shuffle(List<ItemData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            ItemData temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void StartTimer()
    {
        if (dayResolved) return;
        if (completed) return;

        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void PauseTimer()
    {
        timerRunning = false;
    }

    public void ResumeTimer()
    {
        if (dayResolved) return;
        if (completed) return;
        if (timer <= 0f) return;

        timerRunning = true;
    }

    void FailDay()
    {
        if (completed) return;
        if (dayResolved) return;

        dayResolved = true;
        timerRunning = false;
        OnDayFailed?.Invoke();
    }

    public void OnItemPurchased(string itemId)
    {
        if (completed) return;
        if (dayResolved) return;

        int targetCount = GetTargetCount(itemId);
        int purchasedCount = GetPurchasedCount(itemId);

        if (targetCount <= 0) return;
        if (purchasedCount >= targetCount) return;

        purchasedItemIds.Add(itemId);

        if (purchasedItemIds.Count >= targetItemIds.Count)
        {
            completed = true;
            dayResolved = true;
            timerRunning = false;
            OnDaySuccess?.Invoke();
            return;
        }

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ShowReminderOverlay(
                DayManager.Instance != null ? DayManager.Instance.GetCurrentDay() : 1
            );
    }

    int GetTargetCount(string itemId)
    {
        int count = 0;
        for (int i = 0; i < targetItemIds.Count; i++)
            if (targetItemIds[i] == itemId)
                count++;
        return count;
    }

    int GetPurchasedCount(string itemId)
    {
        int count = 0;
        for (int i = 0; i < purchasedItemIds.Count; i++)
            if (purchasedItemIds[i] == itemId)
                count++;
        return count;
    }

    public bool IsPurchasedAtIndex(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= targetItemIds.Count)
            return false;

        string targetId = targetItemIds[targetIndex];
        int requiredCopiesUpToIndex = 0;

        for (int i = 0; i <= targetIndex; i++)
            if (targetItemIds[i] == targetId)
                requiredCopiesUpToIndex++;

        return GetPurchasedCount(targetId) >= requiredCopiesUpToIndex;
    }

    public float GetTimeLeft() => Mathf.Max(0f, timer);

    public bool IsDayResolved()
    {
        return dayResolved;
    }

    public static string FormatTime(float t)
    {
        int sec = Mathf.CeilToInt(t);
        int min = sec / 60;
        sec %= 60;
        return min.ToString("00") + ":" + sec.ToString("00");
    }
}