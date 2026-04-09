using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "Week";
    public float actionDelay = 0.35f;

    [Header("Animators")]
    public Animator startAnimator;
    public Animator exitAnimator;

    [Header("Button Areas")]
    public RectTransform startButtonRect;
    public RectTransform exitButtonRect;

    [Header("Trigger Names")]
    public string inTrigger = "In";
    public string outTrigger = "Out";

    [Header("Reset Data Panel")]
    public GameObject resetDataPanel;
    public Animator resetDataPanelAnimator;
    public float resetPanelOutDelay = 0.3f;
    public bool reloadCurrentSceneAfterReset = true;
    public string mainMenuSceneName = "MainMenu";

    private string currentOpenButton = "";
    private bool isBusy = false;
    private bool isResetPanelOpen = false;

    private void Start()
    {
        if (resetDataPanel != null)
            resetDataPanel.SetActive(false);
    }

    // =========================
    // HOVER
    // =========================
    public void HoverStart()
    {
        if (isBusy || isResetPanelOpen) return;

        if (currentOpenButton == "Exit")
            HideExit();

        if (currentOpenButton != "Start")
        {
            ShowStart();
            currentOpenButton = "Start";
        }
    }

    public void HoverExit()
    {
        if (isBusy || isResetPanelOpen) return;

        if (currentOpenButton == "Start")
            HideStart();

        if (currentOpenButton != "Exit")
        {
            ShowExit();
            currentOpenButton = "Exit";
        }
    }

    // =========================
    // CLICK
    // =========================
    public void ClickStart()
    {
        if (isBusy || isResetPanelOpen) return;
        StartCoroutine(StartRoutine());
    }

    public void ClickExit()
    {
        if (isBusy || isResetPanelOpen) return;
        StartCoroutine(ExitRoutine());
    }

    public void ClickResetData()
    {
        if (isBusy || isResetPanelOpen) return;
        StartCoroutine(OpenResetPanelRoutine());
    }

    public void ClickCancelReset()
    {
        if (isBusy || !isResetPanelOpen) return;
        StartCoroutine(CloseResetPanelRoutine());
    }

    public void ClickConfirmReset()
    {
        if (isBusy || !isResetPanelOpen) return;
        StartCoroutine(ConfirmResetRoutine());
    }

    // =========================
    // ROUTINES
    // =========================
    IEnumerator StartRoutine()
    {
        isBusy = true;

        yield return StartCoroutine(CloseOpenButtonIfAny());

        SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator ExitRoutine()
    {
        isBusy = true;

        yield return StartCoroutine(CloseOpenButtonIfAny());

#if UNITY_EDITOR
        Debug.Log("Quit Game");
        isBusy = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator OpenResetPanelRoutine()
    {
        isBusy = true;

        yield return StartCoroutine(CloseOpenButtonIfAny());

        if (resetDataPanel != null)
            resetDataPanel.SetActive(true);

        if (resetDataPanelAnimator != null)
        {
            resetDataPanelAnimator.ResetTrigger(outTrigger);
            resetDataPanelAnimator.SetTrigger(inTrigger);
        }

        isResetPanelOpen = true;
        isBusy = false;
    }

    IEnumerator CloseResetPanelRoutine()
    {
        isBusy = true;

        if (resetDataPanelAnimator != null)
        {
            resetDataPanelAnimator.ResetTrigger(inTrigger);
            resetDataPanelAnimator.SetTrigger(outTrigger);
        }

        yield return new WaitForSecondsRealtime(resetPanelOutDelay);

        if (resetDataPanel != null)
            resetDataPanel.SetActive(false);

        isResetPanelOpen = false;
        isBusy = false;
    }

    IEnumerator ConfirmResetRoutine()
    {
        isBusy = true;

        if (resetDataPanelAnimator != null)
        {
            resetDataPanelAnimator.ResetTrigger(inTrigger);
            resetDataPanelAnimator.SetTrigger(outTrigger);
        }

        yield return new WaitForSecondsRealtime(resetPanelOutDelay);

        M_ProgressManager.ResetProgress();

        if (reloadCurrentSceneAfterReset)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(mainMenuSceneName);
    }

    // =========================
    // HELPER
    // =========================
    IEnumerator CloseOpenButtonIfAny()
    {
        if (currentOpenButton == "Start")
        {
            HideStart();
            currentOpenButton = "";
            yield return new WaitForSeconds(actionDelay);
        }
        else if (currentOpenButton == "Exit")
        {
            HideExit();
            currentOpenButton = "";
            yield return new WaitForSeconds(actionDelay);
        }
    }

    // =========================
    // ANIMATION CONTROL
    // =========================
    void ShowStart()
    {
        if (startAnimator == null) return;
        startAnimator.ResetTrigger(outTrigger);
        startAnimator.SetTrigger(inTrigger);
    }

    void HideStart()
    {
        if (startAnimator == null) return;
        startAnimator.ResetTrigger(inTrigger);
        startAnimator.SetTrigger(outTrigger);
    }

    void ShowExit()
    {
        if (exitAnimator == null) return;
        exitAnimator.ResetTrigger(outTrigger);
        exitAnimator.SetTrigger(inTrigger);
    }

    void HideExit()
    {
        if (exitAnimator == null) return;
        exitAnimator.ResetTrigger(inTrigger);
        exitAnimator.SetTrigger(outTrigger);
    }

    // =========================
    // CLICK OUTSIDE CLOSE
    // =========================
    void Update()
    {
        if (isBusy || isResetPanelOpen) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentOpenButton == "Start")
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(startButtonRect, Input.mousePosition))
                {
                    CloseCurrentButton();
                }
            }
            else if (currentOpenButton == "Exit")
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(exitButtonRect, Input.mousePosition))
                {
                    CloseCurrentButton();
                }
            }
        }
    }

    public void CloseCurrentButton()
    {
        if (currentOpenButton == "Start")
            HideStart();
        else if (currentOpenButton == "Exit")
            HideExit();

        currentOpenButton = "";
    }
}