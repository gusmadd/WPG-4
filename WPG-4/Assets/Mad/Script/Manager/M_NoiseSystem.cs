using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;  // 🔥 TAMBAH INI

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
    public float adsNoisePerSecond = 2f; // 2 per detik
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

    [HideInInspector] public bool isQTEActive = false;

    public event Action OnNoiseFull;

    private int currentStage = 0;
    private bool noiseTriggered = false;
    bool freezeNoise = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        HandleDecay();
        HandleAdsNoise();
        UpdateOwnerState();
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

        // kalau ads lagi tampil, jangan decay
        if (adsNoiseActive)
        {
            currentDecayRate = 0f;
            return;
        }

        if (currentNoise > 0)
        {
            float decayAmount = GetCurrentDecay();
            currentDecayRate = decayAmount;

            currentNoise -= decayAmount * Time.deltaTime;
            currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);
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
        if (freezeNoise) return;
        if (isQTEActive) return;

        currentNoise += amount;
        currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);

        if (!noiseTriggered && currentNoise >= maxNoise)
        {
            currentNoise = maxNoise;
            noiseTriggered = true;
            OnNoiseFull?.Invoke();
        }
        if (!noiseTriggered && currentNoise >= maxNoise - 0.1f)
        {
            noiseTriggered = true;
        }
    }
    void HandleAdsNoise()
    {
        if (freezeNoise) return;
        if (!adsNoiseActive) return;
        if (isQTEActive) return;

        currentNoise += adsNoisePerSecond * Time.deltaTime;
        currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);

        if (!noiseTriggered && currentNoise >= maxNoise)
        {
            currentNoise = maxNoise;
            noiseTriggered = true;
            OnNoiseFull?.Invoke();
        }
        if (!noiseTriggered && currentNoise >= maxNoise - 0.1f)
        {
            noiseTriggered = true;
        }
    }

    public void StartAdsNoise()
    {
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
    }

    void UpdateOwnerState()
    {
        if (ownerAnimator == null) return;

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
            currentStage = newStage;
            ownerAnimator.SetInteger("OwnerState", currentStage);
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
    }
    public void FreezeNoise(bool freeze)
    {
        freezeNoise = freeze;
        currentDecayRate = 0f;
    }

    public void ResetForNewDay()
    {
        currentNoise = 0f;
        noiseTriggered = false;
        currentStage = 0;
        adsNoiseActive = false;
        freezeNoise = false;

        if (ownerAnimator != null)
            ownerAnimator.SetInteger("OwnerState", 0);
    }
}

