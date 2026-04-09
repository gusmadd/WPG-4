using UnityEngine;

public class FakeCursor : MonoBehaviour
{
    [Header("Cursor Position")]
    public Vector2 offset = new Vector2(-20f, 20f);

    [Header("Cursor Pivot")]
    [Range(0f, 1f)] public float pivotX = 0f;
    [Range(0f, 1f)] public float pivotY = 1f;

    [Header("Noise Reference")]
    public M_NoiseSystem noiseSystem;

    [Header("Mouse Weight / Follow Speed")]
    public float normalFollowSpeed = 25f;
    public float stage1FollowSpeed = 14f;
    public float stage2FollowSpeed = 8f;
    public float stage3FollowSpeed = 4f;

    [Header("Continuous Shake Per Stage")]
    public float stage0ShakeAmount = 0f;
    public float stage1ShakeAmount = 6f;
    public float stage2ShakeAmount = 12f;
    public float stage3ShakeAmount = 20f;

    [Header("Shake Speed Per Stage")]
    public float stage1ShakeSpeed = 35f;
    public float stage2ShakeSpeed = 55f;
    public float stage3ShakeSpeed = 80f;

    [Header("Extra Burst On Stage Change")]
    public float burstDuration = 0.15f;
    public float stage1BurstAmount = 10f;
    public float stage2BurstAmount = 18f;
    public float stage3BurstAmount = 28f;

    private RectTransform rt;
    private Vector2 smoothedPosition;
    private int lastState = 0;
    private float burstTimeLeft = 0f;
    private float currentBurstAmount = 0f;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (rt != null)
            rt.pivot = new Vector2(pivotX, pivotY);

        if (noiseSystem == null)
            noiseSystem = M_NoiseSystem.Instance;

        smoothedPosition = (Vector2)Input.mousePosition + offset;

        if (noiseSystem != null)
            lastState = GetNoiseState();
    }

    void Update()
    {
        Vector2 targetPos = (Vector2)Input.mousePosition + offset;

        CheckStateTransition();

        float followSpeed = GetCurrentFollowSpeed();
        smoothedPosition = Vector2.Lerp(
            smoothedPosition,
            targetPos,
            1f - Mathf.Exp(-followSpeed * Time.unscaledDeltaTime)
        );

        Vector2 finalPos = smoothedPosition;

        if (CanShakeCursor())
        {
            finalPos += GetContinuousShake();
            finalPos += GetBurstShake();
        }
        else
        {
            // matikan burst kalau UI prioritas kebuka
            burstTimeLeft = 0f;
        }

        transform.position = finalPos;
    }

    bool CanShakeCursor()
    {
        if (M_GameManager.Instance == null) return false;

        // shake cuma saat gameplay normal
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
            return false;

        // pause panel kebuka
        if (PauseManager.Instance != null)
        {
            if (PauseManager.Instance.pausePanel != null &&
                PauseManager.Instance.pausePanel.activeInHierarchy)
                return false;
        }

        // game over / day success kebuka
        if (UI_Script.Instance != null)
        {
            if (UI_Script.Instance.gameOverPanel != null &&
                UI_Script.Instance.gameOverPanel.activeInHierarchy)
                return false;

            if (UI_Script.Instance.daySuccessPanel != null &&
                UI_Script.Instance.daySuccessPanel.activeInHierarchy)
                return false;
        }

        return true;
    }

    void CheckStateTransition()
    {
        if (noiseSystem == null) return;

        int currentState = GetNoiseState();

        if (currentState != lastState)
        {
            if (CanShakeCursor())
            {
                burstTimeLeft = burstDuration;
                currentBurstAmount = GetBurstAmountForState(currentState);
            }

            lastState = currentState;
        }
    }

    int GetNoiseState()
    {
        if (noiseSystem == null) return 0;

        if (noiseSystem.currentNoise >= noiseSystem.maxNoise * 0.9f)
            return 3;

        if (noiseSystem.currentNoise >= noiseSystem.stage3Threshold)
            return 2;

        if (noiseSystem.currentNoise >= noiseSystem.stage2Threshold)
            return 1;

        return 0;
    }

    float GetCurrentFollowSpeed()
    {
        if (!CanShakeCursor())
            return normalFollowSpeed;

        switch (GetNoiseState())
        {
            case 1: return stage1FollowSpeed;
            case 2: return stage2FollowSpeed;
            case 3: return stage3FollowSpeed;
            default: return normalFollowSpeed;
        }
    }

    Vector2 GetContinuousShake()
    {
        int state = GetNoiseState();
        float shakeAmount = 0f;
        float shakeSpeed = 0f;

        switch (state)
        {
            case 1:
                shakeAmount = stage1ShakeAmount;
                shakeSpeed = stage1ShakeSpeed;
                break;
            case 2:
                shakeAmount = stage2ShakeAmount;
                shakeSpeed = stage2ShakeSpeed;
                break;
            case 3:
                shakeAmount = stage3ShakeAmount;
                shakeSpeed = stage3ShakeSpeed;
                break;
            default:
                shakeAmount = stage0ShakeAmount;
                shakeSpeed = 0f;
                break;
        }

        if (shakeAmount <= 0f) return Vector2.zero;

        float t = Time.unscaledTime * shakeSpeed;

        float x = Mathf.PerlinNoise(t, 0f) - 0.5f;
        float y = Mathf.PerlinNoise(0f, t) - 0.5f;

        return new Vector2(x, y) * 2f * shakeAmount;
    }

    Vector2 GetBurstShake()
    {
        if (burstTimeLeft <= 0f) return Vector2.zero;

        burstTimeLeft -= Time.unscaledDeltaTime;

        float normalized = burstTimeLeft / Mathf.Max(0.0001f, burstDuration);
        float strength = currentBurstAmount * normalized;

        return new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ) * strength;
    }

    float GetBurstAmountForState(int state)
    {
        switch (state)
        {
            case 1: return stage1BurstAmount;
            case 2: return stage2BurstAmount;
            case 3: return stage3BurstAmount;
            default: return 0f;
        }
    }
}