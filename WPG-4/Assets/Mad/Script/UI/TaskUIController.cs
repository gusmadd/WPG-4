using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskUIController : MonoBehaviour
{
    public static TaskUIController Instance;

    [Header("UI")]
    public GameObject taskPanel;
    public Animator taskAnimator;
    public CanvasGroup taskCanvasGroup;
    public List<Image> itemIcons = new List<Image>();
    public Text timerText;
    public TMP_Text dayText;

    [Header("Colors")]
    public Color notDoneColor = Color.white;
    public Color doneColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Flow")]
    public float outAnimDelay = 0.35f;
    public float overlayBlockTime = 1f;

    enum OverlayMode
    {
        None,
        StartDay,
        Reminder
    }

    bool overlayActive = false;
    bool isClosing = false;
    bool canClick = false;
    float overlayTimer = 0f;
    int currentShownDay = 1;

    OverlayMode currentMode = OverlayMode.None;
    Coroutine overlayRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (taskAnimator != null)
        {
            taskAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            taskAnimator.keepAnimatorStateOnDisable = false;
        }

        if (taskPanel != null)
            taskPanel.SetActive(true);

        HideCanvasInstant();
    }

    void Update()
    {
        if (TaskManager.Instance != null && timerText != null && timerText.gameObject.activeSelf)
            timerText.text = TaskManager.FormatTime(TaskManager.Instance.GetTimeLeft());

        if (!overlayActive) return;
        if (isClosing) return;

        if (overlayTimer > 0f)
        {
            overlayTimer -= Time.unscaledDeltaTime;
            return;
        }

        if (!canClick)
        {
            if (!Input.GetMouseButton(0))
                canClick = true;

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentMode == OverlayMode.StartDay)
                StartCoroutine(HideStartDayRoutine());
            else if (currentMode == OverlayMode.Reminder)
                StartCoroutine(HideReminderRoutine());
        }
    }

    public void ShowStartDayOverlay(int day)
    {
        currentShownDay = day;
        currentMode = OverlayMode.StartDay;
        RestartOverlayRoutine();
    }

    public void ShowReminderOverlay(int day)
    {
        currentShownDay = day;
        currentMode = OverlayMode.Reminder;
        RestartOverlayRoutine();
    }

    void RestartOverlayRoutine()
    {
        if (overlayRoutine != null)
        {
            StopCoroutine(overlayRoutine);
            overlayRoutine = null;
        }

        overlayRoutine = StartCoroutine(ShowOverlayRoutine());
    }

    IEnumerator ShowOverlayRoutine()
    {
        isClosing = false;
        overlayActive = true;
        canClick = false;
        overlayTimer = overlayBlockTime;

        if (dayText != null)
            dayText.text = "Day " + currentShownDay;

        UpdateIconsProgress();

        PlayInAnimation();
        ShowCanvasInstant();

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;

        overlayRoutine = null;
        yield break;
    }

    void PlayInAnimation()
    {
        if (taskAnimator == null) return;

        taskAnimator.ResetTrigger("Out");
        taskAnimator.ResetTrigger("In");
        taskAnimator.Play("Base Layer.In", 0, 0f);
    }

    IEnumerator HideStartDayRoutine()
    {
        yield return StartCoroutine(HideOverlayCore());

        if (TaskManager.Instance != null)
            TaskManager.Instance.StartTimer();
    }

    IEnumerator HideReminderRoutine()
    {
        yield return StartCoroutine(HideOverlayCore());
    }

    IEnumerator HideOverlayCore()
    {
        if (isClosing) yield break;
        isClosing = true;

        if (taskAnimator != null)
        {
            taskAnimator.ResetTrigger("In");
            taskAnimator.SetTrigger("Out");
        }

        yield return new WaitForSecondsRealtime(outAnimDelay);

        HideCanvasInstant();

        overlayActive = false;
        isClosing = false;
        currentMode = OverlayMode.None;

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;
    }

    void ShowCanvasInstant()
    {
        if (taskCanvasGroup != null)
        {
            taskCanvasGroup.alpha = 1f;
            taskCanvasGroup.interactable = true;
            taskCanvasGroup.blocksRaycasts = true;
        }
    }

    void HideCanvasInstant()
    {
        if (taskCanvasGroup != null)
        {
            taskCanvasGroup.alpha = 0f;
            taskCanvasGroup.interactable = false;
            taskCanvasGroup.blocksRaycasts = false;
        }
    }

    void UpdateIconsProgress()
    {
        if (TaskManager.Instance == null) return;
        if (ItemDatabase.Instance == null) return;

        var targets = TaskManager.Instance.targetItemIds;

        for (int i = 0; i < itemIcons.Count; i++)
        {
            if (itemIcons[i] == null) continue;

            if (i >= targets.Count)
            {
                itemIcons[i].enabled = false;
                continue;
            }

            itemIcons[i].enabled = true;

            string id = targets[i];
            ItemData data = ItemDatabase.Instance.GetById(id);

            if (data != null)
                itemIcons[i].sprite = data.icon;

            bool done = TaskManager.Instance.IsPurchasedAtIndex(i);
            itemIcons[i].color = done ? doneColor : notDoneColor;
        }
    }

    public void ResetForNewDay(int day)
    {
        if (overlayRoutine != null)
        {
            StopCoroutine(overlayRoutine);
            overlayRoutine = null;
        }

        StopAllCoroutines();

        overlayActive = false;
        isClosing = false;
        canClick = false;
        overlayTimer = 0f;
        currentShownDay = day;
        currentMode = OverlayMode.None;

        HideCanvasInstant();

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        if (dayText != null)
            dayText.text = "Day " + day;

        if (taskAnimator != null)
        {
            taskAnimator.ResetTrigger("In");
            taskAnimator.ResetTrigger("Out");
            taskAnimator.Play("Base Layer.Out", 0, 1f);
        }
    }

    public void HideTaskInstant()
    {
        if (overlayRoutine != null)
        {
            StopCoroutine(overlayRoutine);
            overlayRoutine = null;
        }

        StopAllCoroutines();

        overlayActive = false;
        isClosing = false;
        canClick = false;
        overlayTimer = 0f;
        currentMode = OverlayMode.None;

        HideCanvasInstant();

        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    public void ShowTaskAfterQTE()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(true);

        ShowReminderOverlay(currentShownDay);
    }
}