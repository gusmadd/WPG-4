using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class M_ProgressManager
{
    const string HighestUnlockedWeekKey = "HighestUnlockedWeek";
    const string TutorialCompletedKey = "TutorialCompleted";

    // Week:
    // 0 = Tutorial
    // 1 = Week 1
    // 2 = Week 2
    // 3 = Week 3
    // 4 = Week 4

    public static void ResetProgress()
    {
        PlayerPrefs.SetInt(TutorialCompletedKey, 0);
        PlayerPrefs.SetInt(HighestUnlockedWeekKey, 0);
        PlayerPrefs.Save();
    }

    public static bool IsTutorialCompleted()
    {
        return PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1;
    }

    public static void CompleteTutorial()
    {
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);

        // setelah tutorial selesai, minimal week 1 terbuka
        int highest = Mathf.Max(1, GetHighestUnlockedWeek());
        PlayerPrefs.SetInt(HighestUnlockedWeekKey, highest);
        PlayerPrefs.Save();
    }

    public static int GetHighestUnlockedWeek()
    {
        return PlayerPrefs.GetInt(HighestUnlockedWeekKey, 0);
    }

    public static bool IsWeekUnlocked(int week)
    {
        if (week <= 0) return true; // tutorial selalu bisa dibuka
        return week <= GetHighestUnlockedWeek();
    }

    public static void CompleteWeek(int completedWeek)
    {
        int nextWeekToUnlock = completedWeek + 1;
        int highest = GetHighestUnlockedWeek();

        if (nextWeekToUnlock > highest)
        {
            PlayerPrefs.SetInt(HighestUnlockedWeekKey, Mathf.Clamp(nextWeekToUnlock, 0, 4));
            PlayerPrefs.Save();
        }
    }
}
