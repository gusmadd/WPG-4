using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_NoiseSystem : MonoBehaviour
{
    public static M_NoiseSystem Instance;

    [Header("Noise Value")]
    public float currentNoise = 0f;
    public float maxNoise = 100f;
    [SerializeField] private float currentDecayRate;

    [Header("Noise Amounts")]
    public float clickNoise = 2f;
    public float keyboardNoise = 4f;
    public float spaceNoise = 6f;
    public float paymentNoise = 8f;

    [Header("Ads Noise")]
    public float adsNoisePerSecond = 2f;
    bool adsNoiseActive = false;

    [Header("Stage Threshold")]
    public float stage2Threshold = 30f;
    public float stage3Threshold = 70f;
    public float stageBuffer = 2f;

    [Header("Decay Per Stage")]
    public float stage1Decay = 15f;
    public float stage2Decay = 8f;
    public float stage3Decay = 3f;

    [Header("Animator")]
    public Animator ownerAnimator;

    [Header("BigSis Visual")]
    public List<SpriteRenderer> ownerRenderers = new List<SpriteRenderer>();
    public float stageFadeOutTime = 0.03f;
    public float stageInvisibleDelay = 0.03f;
    public float stageFadeInTime = 0.05f;

    [HideInInspector] public bool isQTEActive = false;

    public event Action OnNoiseFull;

    private int currentStage = 0;
    private bool noiseTriggered = false;
    private bool freezeNoise = false;

    private bool isStageSwitching = false;
    private Coroutine switchRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Akses melalui M_AudioManager.Instance
        if (M_AudioManager.Instance.sfxSource != null && M_AudioManager.Instance.footstepSfx != null)
        {
            M_AudioManager.Instance.sfxSource.clip = M_AudioManager.Instance.footstepSfx;  // Set footstep SFX ke AudioSource
            M_AudioManager.Instance.sfxSource.loop = true;  // Aktifkan loop untuk footstep
            M_AudioManager.Instance.sfxSource.volume = M_AudioManager.Instance.miscVolume;  // Set volume sesuai dengan volume yang diinginkan
            M_AudioManager.Instance.sfxSource.Play();  // Memainkan footstep
        }
    }


    void Update()
    {
        HandleDecay();
        HandleAdsNoise();
        UpdateOwnerState();
    }

    bool IsDayResolved()
    {
        return TaskManager.Instance != null && TaskManager.Instance.IsDayResolved();
    }

    bool IsNoiseBlocked()
    {
        if (freezeNoise) return true;
        if (isQTEActive) return true;
        if (IsDayResolved()) return true;

        if (M_GameManager.Instance == null) return false;

        // noise boleh berjalan saat gameplay biasa dan ads overlay
        return M_GameManager.Instance.currentState == M_GameManager.GameState.Boot
            || M_GameManager.Instance.currentState == M_GameManager.GameState.TaskOverlay
            || M_GameManager.Instance.currentState == M_GameManager.GameState.QTE;
    }

    void HandleDecay()
    {
        if (freezeNoise)
        {
            currentDecayRate = 0f;
            return;
        }

        if (isQTEActive)
        {
            currentDecayRate = 0f;
            return;
        }

        if (adsNoiseActive)
        {
            currentDecayRate = 0f;
            return;
        }

        if (IsDayResolved())
        {
            currentDecayRate = 0f;
            return;
        }

        if (currentNoise > 0f)
        {
            float decayAmount = GetCurrentDecay();
            currentDecayRate = decayAmount;

            currentNoise -= decayAmount * Time.deltaTime;
            currentNoise = Mathf.Clamp(currentNoise, 0f, maxNoise);
        }
        else
        {
            currentDecayRate = 0f;
        }
    }

    float GetCurrentDecay()
    {
        if (currentStage == 0) return stage1Decay;
        if (currentStage == 1) return stage2Decay;
        return stage3Decay;
    }

    public void AddNoise(float amount)
    {
        if (amount <= 0f) return;
        if (IsNoiseBlocked()) return;

        currentNoise += amount;
        currentNoise = Mathf.Clamp(currentNoise, 0f, maxNoise);

        TryTriggerNoiseFull();
    }

    void HandleAdsNoise()
    {
        if (!adsNoiseActive) return;
        if (IsNoiseBlocked()) return;

        currentNoise += adsNoisePerSecond * Time.deltaTime;
        currentNoise = Mathf.Clamp(currentNoise, 0f, maxNoise);

        TryTriggerNoiseFull();
    }

    void TryTriggerNoiseFull()
    {
        if (noiseTriggered) return;
        if (maxNoise <= 0f) return;
        if (currentNoise < maxNoise) return;
        if (IsNoiseBlocked()) return;

        currentNoise = maxNoise;
        noiseTriggered = true;
        OnNoiseFull?.Invoke();
    }

    public void StartAdsNoise()
    {
        if (IsDayResolved()) return;
        if (freezeNoise) return;
        if (isQTEActive) return;

        adsNoiseActive = true;
    }

    public void StopAdsNoise()
    {
        adsNoiseActive = false;
    }

    public void ResetNoiseAfterQTE()
    {
        currentNoise = 0f;
        noiseTriggered = false;
        isQTEActive = false;
        adsNoiseActive = false;
    }

    void UpdateOwnerState()
    {
        if (ownerAnimator == null) return;
        if (isStageSwitching) return;

        int newStage = currentStage;

        if (currentStage == 0 && currentNoise >= stage2Threshold)
            newStage = 1;

        if (currentStage == 1 && currentNoise >= stage3Threshold)
            newStage = 2;

        if (currentStage == 2 && currentNoise <= stage3Threshold - stageBuffer)
            newStage = 1;

        if (currentStage == 1 && currentNoise <= stage2Threshold - stageBuffer)
            newStage = 0;

        if (newStage != currentStage)
        {
            if (switchRoutine != null)
                StopCoroutine(switchRoutine);

            switchRoutine = StartCoroutine(SwitchOwnerStageRoutine(newStage));
        }
    }

    IEnumerator SwitchOwnerStageRoutine(int newStage)
    {
        isStageSwitching = true;

        // Fade out untuk transisi visual
        yield return StartCoroutine(FadeOwnerAlpha(1f, 0f, stageFadeOutTime));

        currentStage = newStage;
        ownerAnimator.SetInteger("OwnerState", currentStage);

        // Hentikan footstep jika sedang diputar
        if (M_AudioManager.Instance.sfxSource.isPlaying)
        {
            M_AudioManager.Instance.sfxSource.Stop(); // Menghentikan footstep jika ada di sfxSource
        }

        // Menentukan SFX yang akan diputar berdasarkan stage
        if (newStage == 1) // Stage 2
        {
            M_AudioManager.Instance.PlayBigSisNgintipSfx();
        }
        else if (newStage == 2) // Stage 3
        {
            M_AudioManager.Instance.PlayBigSisStarringSfx();
        }

        // Menunggu sebelum melanjutkan transisi visual
        yield return null;

        if (stageInvisibleDelay > 0f)
            yield return new WaitForSeconds(stageInvisibleDelay);

        // Fade in untuk transisi visual
        yield return StartCoroutine(FadeOwnerAlpha(0f, 1f, stageFadeInTime));

        // Memulai footstep SFX jika berada di stage 1
        if (newStage == 0) // Stage 1
        {
            // Set AudioSource untuk footstep agar looping
            M_AudioManager.Instance.sfxSource.clip = M_AudioManager.Instance.footstepSfx;
            M_AudioManager.Instance.sfxSource.loop = true;  // Aktifkan loop
            M_AudioManager.Instance.sfxSource.Play();       // Mulai footstep dengan loop
        }

        isStageSwitching = false;
        switchRoutine = null;
    }

    IEnumerator FadeOwnerAlpha(float from, float to, float duration)
    {
        if (ownerRenderers == null || ownerRenderers.Count == 0)
            yield break;

        if (duration <= 0f)
        {
            SetOwnerAlpha(to);
            yield break;
        }

        float t = 0f;
        SetOwnerAlpha(from);

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            float a = Mathf.Lerp(from, to, p);
            SetOwnerAlpha(a);
            yield return null;
        }

        SetOwnerAlpha(to);
    }

    void SetOwnerAlpha(float alpha)
    {
        for (int i = 0; i < ownerRenderers.Count; i++)
        {
            if (ownerRenderers[i] == null) continue;

            Color c = ownerRenderers[i].color;
            c.a = alpha;
            ownerRenderers[i].color = c;
        }
    }

    public void ResetNoiseTrigger()
    {
        noiseTriggered = false;
    }

    public void ResetAfterQTE()
    {
        noiseTriggered = false;
        isQTEActive = false;
        adsNoiseActive = false;
    }

    public void FreezeNoise(bool freeze)
    {
        freezeNoise = freeze;
        currentDecayRate = 0f;

        if (freeze)
            adsNoiseActive = false;
    }

    public void ResetForNewDay()
    {
        currentNoise = 0f;
        noiseTriggered = false;
        currentStage = 0;
        adsNoiseActive = false;
        freezeNoise = false;
        isQTEActive = false;
        currentDecayRate = 0f;

        if (switchRoutine != null)
        {
            StopCoroutine(switchRoutine);
            switchRoutine = null;
        }

        isStageSwitching = false;
        SetOwnerAlpha(1f);

        if (ownerAnimator != null)
            ownerAnimator.SetInteger("OwnerState", 0);
    }
}