using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIController : MonoBehaviour
{
    public static TaskUIController Instance;

    [Header("UI")]
    public GameObject taskPanel;
    public Animator taskAnimator;
    public List<Image> itemIcons = new List<Image>();
    public Text timerText;

    [Header("Colors")]
    public Color notDoneColor = Color.white;
    public Color doneColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Flow")]
    public float outAnimDelay = 0.2f;

    bool overlayActive = false;
    bool firstStartDone = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (TaskManager.Instance != null && timerText != null)
            timerText.text = TaskManager.FormatTime(TaskManager.Instance.GetTimeLeft());
        if (TaskManager.Instance != null && timerText != null && timerText.gameObject.activeSelf)
            timerText.text = TaskManager.FormatTime(TaskManager.Instance.GetTimeLeft());

        if (!overlayActive) return;

        if (Input.GetMouseButtonDown(0))
            StartCoroutine(HideOverlay());
    }

    public void ShowTaskOverlay(bool isBoot)
    {
        if (overlayActive) return;

        overlayActive = true;

        if (taskPanel != null) taskPanel.SetActive(true);
        UpdateIconsProgress();

        if (taskAnimator != null) taskAnimator.SetTrigger("In");

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.TaskOverlay;
    }

    IEnumerator HideOverlay()
    {
        if (taskAnimator != null) taskAnimator.SetTrigger("Out");

        yield return new WaitForSecondsRealtime(outAnimDelay);

        if (taskPanel != null) taskPanel.SetActive(false);

        overlayActive = false;

        if (M_GameManager.Instance != null)
            M_GameManager.Instance.currentState = M_GameManager.GameState.Gameplay;

        if (!firstStartDone)
        {
            if (TaskManager.Instance == null) yield break;

            // guard: task belum siap
            if (TaskManager.Instance.targetItemIds == null || TaskManager.Instance.targetItemIds.Count == 0)
            {
                Debug.LogError("Task belum siap. targetItemIds kosong.");
                overlayActive = true;
                if (taskPanel != null) taskPanel.SetActive(true);
                yield break;
            }

            // guard: timer masih 0
            if (TaskManager.Instance.GetTimeLeft() <= 0f)
            {
                Debug.LogError("Timer masih 0. Cek dayDurationSeconds.");
                overlayActive = true;
                if (taskPanel != null) taskPanel.SetActive(true);
                yield break;
            }

            firstStartDone = true;
            TaskManager.Instance.StartTimer();
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
            if (data != null) itemIcons[i].sprite = data.icon;

            bool done = TaskManager.Instance.IsPurchased(id);
            itemIcons[i].color = done ? doneColor : notDoneColor;
        }
    }

    public void ResetForNewDay()
    {
        firstStartDone = false;
        overlayActive = false;
        if (taskPanel != null) taskPanel.SetActive(false);
        if (timerText != null)
            timerText.gameObject.SetActive(true);
    }
    public void HideTaskInstant()
    {
        overlayActive = false;

        if (taskPanel != null)
            taskPanel.SetActive(false);

        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    public void ShowTaskAfterQTE()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(true);

        ShowTaskOverlay(false);
    }
}
