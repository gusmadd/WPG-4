using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NoiseBar : MonoBehaviour
{
 public Image fillImage;
    public M_NoiseSystem noiseSystem;

    void Update()
    {
        float normalized = noiseSystem.currentNoise / noiseSystem.maxNoise;
        fillImage.fillAmount = normalized;
    }
}
