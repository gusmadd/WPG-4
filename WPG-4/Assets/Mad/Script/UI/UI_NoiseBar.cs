using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NoiseBar : MonoBehaviour
{
    public Image fillImage;
    public M_NoiseSystem noiseSystem;

    [Header("Stage 3 Alert")]
    public GameObject alertImage;

    void Start()
    {
        if (noiseSystem == null)
            noiseSystem = M_NoiseSystem.Instance;

        if (alertImage != null)
            alertImage.SetActive(false);
    }

    void Update()
    {
        if (noiseSystem == null) return;

        float normalized = noiseSystem.currentNoise / noiseSystem.maxNoise;

        if (fillImage != null)
            fillImage.fillAmount = normalized;

        if (alertImage != null)
            alertImage.SetActive(noiseSystem.currentNoise >= noiseSystem.stage3Threshold);
    }

}
