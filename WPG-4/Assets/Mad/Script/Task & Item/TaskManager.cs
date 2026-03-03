using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("Day Settings")]
    public float dayDurationSeconds = 120f;
    public int itemsPerTask = 3;

    [Header("Runtime")]
    public List<string> targetItemIds = new List<string>();
    public HashSet<string> purchasedSet = new HashSet<string>();
    public bool completed;

    float timer;
    bool timerRunning = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ResetTaskData();
        CreateNewTask();
        timerRunning = false;
    }

    void Update()
    {
        if (!timerRunning) return;
        if (completed) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            FailAndNewTask();
            return;
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    void CreateNewTask()
    {
        targetItemIds.Clear();
        purchasedSet.Clear();
        completed = false;
        timer = dayDurationSeconds;

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

        Debug.Log("Task baru: " + string.Join(",", targetItemIds));
    }

    void ResetTaskData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void OnItemPurchased(string itemId)
    {
        if (completed) return;
        if (!targetItemIds.Contains(itemId)) return;

        purchasedSet.Add(itemId);

        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ShowTaskOverlay(false);

        if (purchasedSet.Count >= targetItemIds.Count)
        {
            completed = true;
            Debug.Log("Task selesai.");
            if (TaskUIController.Instance != null)
                TaskUIController.Instance.ShowTaskOverlay(false);
        }
    }

    void FailAndNewTask()
    {
        CreateNewTask();
        if (TaskUIController.Instance != null)
            TaskUIController.Instance.ShowTaskOverlay(false);
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
}
