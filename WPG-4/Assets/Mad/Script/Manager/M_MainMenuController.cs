using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Buttons")]
    public Button resetDataButton;
    public Button volumeButton;

    [Header("Trigger Names")]
    public string inTrigger = "In";
    public string outTrigger = "Out";

    [Header("Reset Data Panel")]
    public GameObject resetDataPanel;
    public Animator resetDataPanelAnimator;
    public float resetPanelOutDelay = 0.3f;
    public bool reloadCurrentSceneAfterReset = true;
    public string mainMenuSceneName = "MainMenu";

    [Header("Volume Panel")]
    public GameObject volumeSliderPanel;
    public Animator volumeSliderAnimator;
    public RectTransform volumeButtonRect;
    public RectTransform volumeSliderRect;
    public float volumePanelOutDelay = 0.25f;

    private bool isVolumePanelOpen = false;

    // ==========================================
    // BARU: Scene Transition (Tanpa Animator)
    // ==========================================
    [Header("Scene Transition (Script Based)")]
    public RectTransform transitionImageRect; // RectTransform dari Image Slide
    public float transitionDuration = 0.5f;   // Berapa lama slide berlangsung
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Opsional: Biar gerakannya smooth

    private Vector2 screenHiddenPosition; // Posisi gambar saat tidak terlihat
    private Vector2 screenCenterPosition; // Posisi gambar saat menutupi layar

    private string currentOpenButton = "";
    private bool isBusy = false;
    private bool isResetPanelOpen = false;

    private void Start()
    {
        if (volumeSliderPanel != null)
            volumeSliderPanel.SetActive(false);
        if (resetDataPanel != null)
            resetDataPanel.SetActive(false);

        // Setup posisi transisi
        SetupTransitionPositions();
    }

    // Menghitung posisi awal dan akhir slide berdasarkan ukuran layar
    private void SetupTransitionPositions()
    {
        if (transitionImageRect == null) return;

        // Ambil ukuran canvas dari parent rect image tersebut
        RectTransform canvasRect = transitionImageRect.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float screenWidth = canvasRect.rect.width;

        // Atur Anchor ke tengah layar agar perhitungan Vector2.zero tepat di tengah
        transitionImageRect.anchorMin = new Vector2(0.5f, 0.5f);
        transitionImageRect.anchorMax = new Vector2(0.5f, 0.5f);
        transitionImageRect.pivot = new Vector2(0.5f, 0.5f);

        // Tentukan posisi
        screenCenterPosition = Vector2.zero;
        screenHiddenPosition = new Vector2(-screenWidth, 0); // Di kanan luar layar

        // Set posisi awal dan matikan agar tidak menghalangi klik
        transitionImageRect.anchoredPosition = screenHiddenPosition;
        transitionImageRect.gameObject.SetActive(false);
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
                if (isVolumePanelOpen)
            StartCoroutine(CloseVolumePanelRoutine());
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
                if (isVolumePanelOpen)
            StartCoroutine(CloseVolumePanelRoutine());
    }

    // =========================
    // CLICK
    // =========================
    public void ClickStart()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (isBusy || isResetPanelOpen) return;
        StartCoroutine(StartRoutine());
    }

    public void ClickExit()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (isBusy || isResetPanelOpen) return;
        StartCoroutine(ExitRoutine());
    }

    public void ClickResetData()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (isBusy || isResetPanelOpen) return;
        StartCoroutine(OpenResetPanelRoutine());
    }

    public void ClickCancelReset()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (isBusy || !isResetPanelOpen) return;
        StartCoroutine(CloseResetPanelRoutine());
    }

    public void ClickConfirmReset()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (isBusy || !isResetPanelOpen) return;
        StartCoroutine(ConfirmResetRoutine());
    }

    // =========================
    // ROUTINES
    // =========================
    IEnumerator StartRoutine()
    {
        isBusy = true;

        // 1. Tunggu tombol animasi "Out"
        yield return StartCoroutine(CloseOpenButtonIfAny());

        // 2. Jalankan Slide Transisi via Script
        if (transitionImageRect != null)
        {
            transitionImageRect.gameObject.SetActive(true);
            // Mulai Coroutine untuk menggerakkan gambar
            yield return StartCoroutine(SildInTransition());
        }

        // 3. Pindah Scene
        SceneManager.LoadScene(gameSceneName);
        TelemetryManager.Instance?.SendSessionStart();
    }

    // COROUTINE UNTUK MENGGERAKKAN SLIDE (LERP)
    IEnumerator SildInTransition()
    {
        float elapsedTime = 0;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            // Hitung persentase progres (0 sampai 1)
            float t = elapsedTime / transitionDuration;

            // Gunakan Curve agar gerakan lebih smooth (slow start, slow end)
            float curveT = transitionCurve.Evaluate(t);

            // Gerakkan posisi dari tersembunyi ke tengah
            transitionImageRect.anchoredPosition = Vector2.Lerp(screenHiddenPosition, screenCenterPosition, curveT);

            yield return null; // Tunggu frame berikutnya
        }

        // Pastikan posisi akhirnya tepat di tengah
        transitionImageRect.anchoredPosition = screenCenterPosition;
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

        M_ProgressManager.ResetProgress(); // Commented out karena script eksternal

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

    // Helper opsional untuk mencari Canvas jika script tidak ditaruh di Canvas
    private static class GameObjectExtension
    {
        public static Canvas FindActionableCanvas()
        {
            Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
            foreach (Canvas c in canvases)
            {
                if (c.renderMode != RenderMode.WorldSpace) return c;
            }
            return canvases[0];
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
        M_AudioManager.Instance?.PlayShowKeyboard();
    }

    void HideStart()
    {
        M_AudioManager.Instance?.PlayHideKeyboard();
        if (startAnimator == null) return;
        startAnimator.ResetTrigger(inTrigger);
        startAnimator.SetTrigger(outTrigger);
    }

    void ShowExit()
    {
        if (exitAnimator == null) return;
        exitAnimator.ResetTrigger(outTrigger);
        exitAnimator.SetTrigger(inTrigger);
        M_AudioManager.Instance?.PlayShowKeyboard();
    }

    void HideExit()
    {
        M_AudioManager.Instance?.PlayHideKeyboard();
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
            if (isVolumePanelOpen)
            {
                bool clickOnVolumeButton = volumeButtonRect != null &&
                    RectTransformUtility.RectangleContainsScreenPoint(volumeButtonRect, Input.mousePosition);

                bool clickInsideVolumePanel = volumeSliderRect != null &&
                    RectTransformUtility.RectangleContainsScreenPoint(volumeSliderRect, Input.mousePosition);

                if (!clickOnVolumeButton && !clickInsideVolumePanel)
                {
                    StartCoroutine(CloseVolumePanelRoutine());
                    return;
                }
            }

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
    public void ClickVolume()
    {
        M_AudioManager.Instance?.PlayRandomUi();

        if (isBusy || isResetPanelOpen) return;

        if (isVolumePanelOpen)
            StartCoroutine(CloseVolumePanelRoutine());
        else
            StartCoroutine(OpenVolumePanelRoutine());
    }

    IEnumerator OpenVolumePanelRoutine()
    {
        isBusy = true;

        yield return StartCoroutine(CloseOpenButtonIfAny());

        if (volumeSliderPanel != null)
            volumeSliderPanel.SetActive(true);

        if (volumeSliderAnimator != null)
        {
            volumeSliderAnimator.ResetTrigger(outTrigger);
            volumeSliderAnimator.ResetTrigger(inTrigger);
            volumeSliderAnimator.SetTrigger(inTrigger);
        }

        isVolumePanelOpen = true;
        SetVolumePanelButtonLock(false);
        isBusy = false;
    }

    IEnumerator CloseVolumePanelRoutine()
    {
        isBusy = true;

        if (volumeSliderAnimator != null)
        {
            volumeSliderAnimator.ResetTrigger(inTrigger);
            volumeSliderAnimator.ResetTrigger(outTrigger);
            volumeSliderAnimator.SetTrigger(outTrigger);
        }

        yield return new WaitForSecondsRealtime(volumePanelOutDelay);

        if (volumeSliderPanel != null)
            volumeSliderPanel.SetActive(false);

        isVolumePanelOpen = false;
        SetVolumePanelButtonLock(true);
        isBusy = false;
    }
    void SetVolumePanelButtonLock(bool canInteract)
    {
        if (resetDataButton != null)
            resetDataButton.interactable = canInteract;

        if (volumeButton != null)
            volumeButton.interactable = canInteract;
    }
}