using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_GameManager : MonoBehaviour
{
    public enum GameState
    {
        Boot,
        Gameplay,
        TaskOverlay,
        AdsOverlay,
        QTE
    }

    public static M_GameManager Instance;

    [Header("QTE")]
    public GameObject qtePrefab;

    [Header("Animators")]
    public Animator catAnimator;

    [Header("Camera Zoom")]
    public Camera mainCamera;

    public M_KeyboardController keyboard;

    public float normalSize = 5f;
    public float qteSize = 2f;

    public Vector3 normalPosition;
    public Vector3 qtePosition;

    public float zoomDuration = 0.3f;

    private bool isSequenceRunning = false;

    public GameState currentState = GameState.Boot;

    public int qteCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        M_NoiseSystem.Instance.OnNoiseFull += HandleNoiseFull;
        normalPosition = mainCamera.transform.position;
    }

    void HandleNoiseFull()
    {
        if (!isSequenceRunning)
            StartCoroutine(NoiseFullSequence());
    }

    IEnumerator NoiseFullSequence()
    {
        isSequenceRunning = true;
        currentState = GameState.QTE;
        keyboard.HideKeyboard();
        // pause task saat QTE
        TaskManager.Instance?.PauseTimer();
        TaskUIController.Instance?.HideTaskInstant();
        qteCount++;

        Time.timeScale = 0f;
        M_NoiseSystem.Instance.isQTEActive = true;

        // Shake (UI_Script)
        yield return StartCoroutine(UI_Script.Instance.Shake());

        // Fade (UI_Script)
        yield return StartCoroutine(FadeAndZoom(true));

        yield return new WaitForSecondsRealtime(0.2f);

        if (catAnimator != null)
            catAnimator.SetTrigger("OnNoiseFull");

        yield return new WaitForSecondsRealtime(1f);

        Instantiate(qtePrefab);

        Time.timeScale = 1f;

        isSequenceRunning = false;
    }

    public IEnumerator QTESuccess()
    {
        yield return new WaitForSecondsRealtime(1f);

        yield return StartCoroutine(FadeAndZoom(false));

        yield return new WaitForSecondsRealtime(0.5f);

        yield return StartCoroutine(ReduceNoiseSmoothly(31f));
        // setelah kamera normal, tampilkan task lagi dan lanjut timer
        TaskUIController.Instance?.ShowTaskAfterQTE();
        TaskManager.Instance?.ResumeTimer();

        yield return new WaitForSecondsRealtime(1f);

        M_NoiseSystem.Instance.isQTEActive = false;
        M_NoiseSystem.Instance.ResetAfterQTE();

        currentState = GameState.Gameplay;

        if (catAnimator != null)
            catAnimator.SetTrigger("OnBackToIdle");
    }

    IEnumerator ReduceNoiseSmoothly(float targetValue)
    {
        float speed = 40f;

        while (M_NoiseSystem.Instance.currentNoise > targetValue)
        {
            M_NoiseSystem.Instance.currentNoise -= speed * Time.unscaledDeltaTime;
            yield return null;
        }

        M_NoiseSystem.Instance.currentNoise = targetValue;
    }

    IEnumerator ZoomCamera(float fromSize, float toSize, Vector3 fromPos, Vector3 toPos)
    {
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / zoomDuration;

            mainCamera.orthographicSize = Mathf.Lerp(fromSize, toSize, t);
            mainCamera.transform.position = Vector3.Lerp(fromPos, toPos, t);

            yield return null;
        }

        mainCamera.orthographicSize = toSize;
        mainCamera.transform.position = toPos;
    }

    IEnumerator FadeAndZoom(bool toQTE)
    {
        float fromSize = toQTE ? normalSize : qteSize;
        float toSize = toQTE ? qteSize : normalSize;

        Vector3 fromPos = toQTE ? normalPosition : qtePosition;
        Vector3 toPos = toQTE ? qtePosition : normalPosition;

        // 1️⃣ Fade to black
        yield return StartCoroutine(UI_Script.Instance.Fade(0f, 1f));

        // 2️⃣ Zoom saat layar hitam
        yield return StartCoroutine(
            ZoomCamera(fromSize, toSize, fromPos, toPos)
        );

        // 3️⃣ Fade back (lebih smooth)
        yield return StartCoroutine(UI_Script.Instance.Fade(1f, 0f));
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");

        Time.timeScale = 1f;

        // stop task + hide task UI
        TaskManager.Instance?.StopTimer();
        TaskUIController.Instance?.HideTaskInstant();

        // pastikan noise tidak jalan lagi
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.FreezeNoise(true);

        // kunci input
        currentState = GameState.TaskOverlay;

        // munculin panel game over
        UI_Script.Instance?.ShowGameOver();
    }
    public IEnumerator QTEFail()
    {
        // kunci state
        currentState = GameState.QTE;

        // stop task + hide task ui
        TaskManager.Instance?.StopTimer();
        TaskUIController.Instance?.HideTaskInstant();

        // pastikan QTE flag mati biar decay/noise normal jika perlu
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.isQTEActive = false;

        // Fade to black
        yield return StartCoroutine(UI_Script.Instance.Fade(0f, 1f));

        // Balikin kamera ke normal saat layar hitam
        yield return StartCoroutine(
            ZoomCamera(qteSize, normalSize, qtePosition, normalPosition)
        );

        // Fade balik
        yield return StartCoroutine(UI_Script.Instance.Fade(1f, 0f));

        // set state overlay supaya input berhenti
        currentState = GameState.TaskOverlay;

        // Freeze noise supaya tidak berubah saat game over
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.FreezeNoise(true);

        // munculin panel game over
        UI_Script.Instance?.ShowGameOver();
    }
}
