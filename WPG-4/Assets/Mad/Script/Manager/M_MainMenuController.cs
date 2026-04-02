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

    private string currentOpenButton = "";
    private bool isBusy = false;

    // =========================
    // HOVER
    // =========================
    public void HoverStart()
    {
        if (isBusy) return;

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
        if (isBusy) return;

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
        if (isBusy) return;
        StartCoroutine(StartRoutine());
    }

    public void ClickExit()
    {
        if (isBusy) return;
        StartCoroutine(ExitRoutine());
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
#else
        Application.Quit();
#endif
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
        if (isBusy) return;

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