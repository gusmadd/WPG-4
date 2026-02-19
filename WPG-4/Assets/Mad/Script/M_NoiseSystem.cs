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

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        HandleDecay();
        UpdateOwnerState();
    }

    void HandleDecay()
    {
        if (isQTEActive)
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
}

