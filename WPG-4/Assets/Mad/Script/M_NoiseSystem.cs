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

    [Header("Stage Threshold")]
    public float stage2Threshold = 30f;
    public float stage3Threshold = 70f;

    [Header("Decay Per Stage")]
    public float stage1Decay = 15f;
    public float stage2Decay = 8f;
    public float stage3Decay = 3f;

    [Header("Noise Amounts")]
    public float clickNoise = 2f;
    public float keyboardNoise = 4f;
    public float spaceNoise = 6f;

    [Header("Owner")]
    public Animator ownerAnimator;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (currentNoise > 0)
        {
            float decayAmount = GetCurrentDecay();
            currentDecayRate = decayAmount; // DEBUG VIEW

            currentNoise -= decayAmount * Time.deltaTime;
            currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);
        }
        else
        {
            currentDecayRate = 0f;
        }

        UpdateOwnerState();
    }

    float GetCurrentDecay()
    {
        if (currentNoise < stage2Threshold)
            return stage1Decay;
        else if (currentNoise < stage3Threshold)
            return stage2Decay;
        else
            return stage3Decay;
    }

    public void AddNoise(float amount)
    {
        currentNoise += amount;
        currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);
    }

    void UpdateOwnerState()
    {
        if (ownerAnimator == null) return;

        if (currentNoise < stage2Threshold)
            ownerAnimator.SetInteger("OwnerState", 0);
        else if (currentNoise < stage3Threshold)
            ownerAnimator.SetInteger("OwnerState", 1);
        else
            ownerAnimator.SetInteger("OwnerState", 2);
    }
}
