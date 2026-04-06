using System.Collections;
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
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.OnNoiseFull += HandleNoiseFull;

        if (mainCamera != null)
            normalPosition = mainCamera.transform.position;
    }

    void HandleNoiseFull()
    {
        if (TaskManager.Instance != null && TaskManager.Instance.IsDayResolved())
            return;

        if (!isSequenceRunning)
            StartCoroutine(NoiseFullSequence());
    }

    public void ForceEndQTEState()
    {
        StopAllCoroutines();

        Time.timeScale = 1f;
        isSequenceRunning = false;

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.isQTEActive = false;

        if (mainCamera != null)
        {
            mainCamera.orthographicSize = normalSize;
            mainCamera.transform.position = normalPosition;
        }

        if (catAnimator != null)
            catAnimator.SetTrigger("OnBackToIdle");

        currentState = GameState.TaskOverlay;
    }

    IEnumerator NoiseFullSequence()
    {
        if (TaskManager.Instance != null && TaskManager.Instance.IsDayResolved())
            yield break;

        isSequenceRunning = true;
        currentState = GameState.QTE;

        M_MonitorManager monitor = FindObjectOfType<M_MonitorManager>();
        if (monitor != null)

        if (keyboard != null)
            keyboard.HideKeyboard();

        TaskManager.Instance?.PauseTimer();
        TaskUIController.Instance?.HideTaskInstant();
        qteCount++;

        Time.timeScale = 0f;

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.isQTEActive = true;

        if (UI_Script.Instance != null)
            yield return StartCoroutine(UI_Script.Instance.Shake());

        yield return StartCoroutine(FadeAndZoom(true));

        if (TaskManager.Instance != null && TaskManager.Instance.IsDayResolved())
        {
            ForceEndQTEState();
            yield break;
        }

        yield return new WaitForSecondsRealtime(0.2f);

        if (catAnimator != null)
            catAnimator.SetTrigger("OnNoiseFull");

        yield return new WaitForSecondsRealtime(1f);

        if (TaskManager.Instance != null && TaskManager.Instance.IsDayResolved())
        {
            ForceEndQTEState();
            yield break;
        }

        if (qtePrefab != null)
            Instantiate(qtePrefab);

        Time.timeScale = 1f;
        isSequenceRunning = false;
    }

    public IEnumerator QTESuccess()
    {
        if (TaskManager.Instance != null && TaskManager.Instance.IsDayResolved())
        {
            ForceEndQTEState();
            yield break;
        }

        yield return new WaitForSecondsRealtime(1f);

        if (TaskManager.Instance != null && TaskManager.Instance.IsDayResolved())
        {
            ForceEndQTEState();
            yield break;
        }

        yield return StartCoroutine(FadeAndZoom(false));

        yield return new WaitForSecondsRealtime(0.5f);

        yield return StartCoroutine(ReduceNoiseSmoothly(31f));

        if (TaskManager.Instance != null && TaskManager.Instance.IsDayResolved())
        {
            ForceEndQTEState();
            yield break;
        }

        // keluar dulu dari QTE state
        currentState = GameState.Gameplay;

        if (M_NoiseSystem.Instance != null)
        {
            M_NoiseSystem.Instance.isQTEActive = false;
            M_NoiseSystem.Instance.ResetAfterQTE();
        }

        // baru task popup/reminder boleh muncul
        TaskUIController.Instance?.ShowTaskAfterQTE();
        TaskManager.Instance?.ResumeTimer();

        yield return new WaitForSecondsRealtime(1f);

        M_DetailFoodPage.CompletePendingBuyIfAny();

        if (catAnimator != null)
            catAnimator.SetTrigger("OnBackToIdle");
    }

    IEnumerator ReduceNoiseSmoothly(float targetValue)
    {
        if (M_NoiseSystem.Instance == null)
            yield break;

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
        if (mainCamera == null)
            yield break;

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

        if (UI_Script.Instance != null)
            yield return StartCoroutine(UI_Script.Instance.Fade(0f, 1f));

        yield return StartCoroutine(ZoomCamera(fromSize, toSize, fromPos, toPos));

        if (UI_Script.Instance != null)
            yield return StartCoroutine(UI_Script.Instance.Fade(1f, 0f));
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");

        Time.timeScale = 1f;

        TaskManager.Instance?.StopTimer();
        TaskUIController.Instance?.HideTaskInstant();

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.FreezeNoise(true);

        currentState = GameState.TaskOverlay;

        M_DetailFoodPage.ClearPendingBuy();

        UI_Script.Instance?.ShowGameOver();
    }

    public IEnumerator QTEFail()
    {
        if (TaskManager.Instance != null && TaskManager.Instance.IsDayResolved())
        {
            ForceEndQTEState();
            yield break;
        }

        currentState = GameState.QTE;

        TaskManager.Instance?.StopTimer();
        TaskUIController.Instance?.HideTaskInstant();

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.isQTEActive = false;

        if (UI_Script.Instance != null)
            yield return StartCoroutine(UI_Script.Instance.Fade(0f, 1f));

        yield return StartCoroutine(
            ZoomCamera(qteSize, normalSize, qtePosition, normalPosition)
        );

        if (UI_Script.Instance != null)
            yield return StartCoroutine(UI_Script.Instance.Fade(1f, 0f));

        currentState = GameState.TaskOverlay;

        GameOver();
    }
}