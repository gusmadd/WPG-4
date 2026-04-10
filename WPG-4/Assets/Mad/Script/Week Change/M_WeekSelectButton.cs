using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class M_WeekSelectButton : MonoBehaviour
{
    [Header("Week Info")]
    [Range(0, 4)] public int weekNumber = 0;
    public string sceneName;

    [Header("UI")]
    public Button button;
    public GameObject lockIcon;
    public Image buttonImage;

    [Header("Colors")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(0.35f, 0.35f, 0.35f, 1f);

    void Start()
    {
        M_ProgressManager.ResetProgress();
        Refresh();
    }

    public void Refresh()
    {
        bool unlocked = M_ProgressManager.IsWeekUnlocked(weekNumber);

        if (button != null)
            button.interactable = unlocked;

        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);

        if (buttonImage != null)
            buttonImage.color = unlocked ? unlockedColor : lockedColor;
    }

    public void ClickOpen()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (!M_ProgressManager.IsWeekUnlocked(weekNumber))
            return;

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithTransition(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
