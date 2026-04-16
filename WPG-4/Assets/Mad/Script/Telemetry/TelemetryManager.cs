using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance;

    private bool isReady = false;
    private string runId;
    private float sessionStartTime;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeTelemetry();
    }

    private async Task InitializeTelemetry()
    {
        try
        {
            await UnityServices.InitializeAsync();

            // Untuk prototype / tugas
            // ini menyalakan pengumpulan data setelah init
            AnalyticsService.Instance.StartDataCollection();

            isReady = true;
            Debug.Log("Telemetry ready");
        }
        catch (Exception e)
        {
            Debug.LogError("Telemetry init failed: " + e.Message);
        }
    }

    public void SendSessionStart()
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        runId = Guid.NewGuid().ToString();
        sessionStartTime = Time.time;

        var ev = new CustomEvent("session_start")
        {
            { "run_id", runId }
        };

        AnalyticsService.Instance.RecordEvent(ev);
        AnalyticsService.Instance.Flush();

        Debug.Log("session_start sent, run_id = " + runId);
    }

    public string GetRunId()
    {
        return runId;
    }
    public void SendDayStart(int dayNumber, int totalTasksInDay)
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        if (string.IsNullOrEmpty(runId))
        {
            Debug.LogWarning("run_id belum ada, session_start belum dikirim");
            return;
        }

        var ev = new CustomEvent("day_start")
    {
        { "run_id", runId },
        { "day_number", dayNumber },
        { "total_tasks_in_day", totalTasksInDay }
    };

        AnalyticsService.Instance.RecordEvent(ev);

        Debug.Log($"day_start sent, day = {dayNumber}, tasks = {totalTasksInDay}");
    }

    public void SendTaskStart(string taskId, int dayNumber)
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        if (string.IsNullOrEmpty(runId))
        {
            Debug.LogWarning("run_id belum ada, session_start belum dikirim");
            return;
        }

        var ev = new CustomEvent("task_start")
    {
        { "run_id", runId },
        { "task_id", taskId },
        { "day_number", dayNumber }
    };

        AnalyticsService.Instance.RecordEvent(ev);

        Debug.Log($"task_start sent, task = {taskId}, day = {dayNumber}");
    }

    public void SendTaskComplete(string taskId, float durationSeconds, int dayNumber)
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        if (string.IsNullOrEmpty(runId))
        {
            Debug.LogWarning("run_id belum ada, session_start belum dikirim");
            return;
        }

        var ev = new CustomEvent("task_complete")
    {
        { "run_id", runId },
        { "task_id", taskId },
        { "duration_seconds", durationSeconds },
        { "day_number", dayNumber }
    };

        AnalyticsService.Instance.RecordEvent(ev);

        Debug.Log($"task_complete sent, task = {taskId}, duration = {durationSeconds}, day = {dayNumber}");
    }

    public void SendDayCompleted(int dayNumber)
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        if (string.IsNullOrEmpty(runId))
        {
            Debug.LogWarning("run_id belum ada, session_start belum dikirim");
            return;
        }

        var ev = new CustomEvent("day_completed")
    {
        { "run_id", runId },
        { "day_number", dayNumber }
    };

        AnalyticsService.Instance.RecordEvent(ev);

        Debug.Log($"day_completed sent, day = {dayNumber}");
    }

    public void SendPlayerFail(string cause, int dayNumber, int weekNumber)
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        if (string.IsNullOrEmpty(runId))
        {
            Debug.LogWarning("run_id belum ada, session_start belum dikirim");
            return;
        }

        var ev = new CustomEvent("player_fail")
    {
        { "run_id", runId },
        { "cause", cause },
        { "day_number", dayNumber },
        { "week_number", weekNumber }
    };

        AnalyticsService.Instance.RecordEvent(ev);

        Debug.Log($"player_fail sent, cause = {cause}, day = {dayNumber}, week = {weekNumber}");
    }
    public void SendSessionEnd()
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        if (string.IsNullOrEmpty(runId))
        {
            Debug.LogWarning("run_id belum ada, session_start belum dikirim");
            return;
        }

        float totalTimeSeconds = Time.time - sessionStartTime;

        var ev = new CustomEvent("session_end")
    {
        { "run_id", runId },
        { "total_time_seconds", totalTimeSeconds }
    };

        AnalyticsService.Instance.RecordEvent(ev);
        AnalyticsService.Instance.Flush();

        Debug.Log($"session_end sent, total_time_seconds = {totalTimeSeconds}");
    }

    public void SendNoiseIncrease(float amount, string source, float noiseValue, int dayNumber, int weekNumber)
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        if (string.IsNullOrEmpty(runId))
        {
            Debug.LogWarning("run_id belum ada, session_start belum dikirim");
            return;
        }

        var ev = new CustomEvent("noise_increase")
    {
        { "amount", amount },
        { "source", source },
        { "noise_value", noiseValue },
        { "day_number", dayNumber },
        { "week_number", weekNumber }
    };

        AnalyticsService.Instance.RecordEvent(ev);
        Debug.Log($"noise_increase sent, amount = {amount}, source = {source}, noise = {noiseValue}, day = {dayNumber}, week = {weekNumber}");
    }

    public void SendNoiseStageChanged(int oldStage, int newStage, float noiseValue, int dayNumber, int weekNumber)
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        if (string.IsNullOrEmpty(runId))
        {
            Debug.LogWarning("run_id belum ada, session_start belum dikirim");
            return;
        }

        var ev = new CustomEvent("noise_stage_changed")
    {
        { "old_stage", oldStage },
        { "new_stage", newStage },
        { "noise_value", noiseValue },
        { "day_number", dayNumber },
        { "week_number", weekNumber }
    };

        AnalyticsService.Instance.RecordEvent(ev);
        Debug.Log($"noise_stage_changed sent, {oldStage} -> {newStage}, noise = {noiseValue}, day = {dayNumber}, week = {weekNumber}");
    }

    public void SendStageLevel(int stageLevel, int dayNumber, int weekNumber)
    {
        if (!isReady)
        {
            Debug.LogWarning("Telemetry belum siap");
            return;
        }

        if (string.IsNullOrEmpty(runId))
        {
            Debug.LogWarning("run_id belum ada, session_start belum dikirim");
            return;
        }

        var ev = new CustomEvent("stage_level")
    {
        { "stage_level", stageLevel },
        { "day_number", dayNumber },
        { "week_number", weekNumber }
    };

        AnalyticsService.Instance.RecordEvent(ev);
        Debug.Log($"stage_level sent, stage = {stageLevel}, day = {dayNumber}, week = {weekNumber}");
    }
}