using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_MasterVolumeSlider : MonoBehaviour
{
 [Header("Slider")]
    public Slider volumeSlider;

    bool isLoading = false;

    void Awake()
    {
        if (volumeSlider == null)
            volumeSlider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        if (volumeSlider == null) return;
        if (M_AudioManager.Instance == null) return;

        volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
        volumeSlider.onValueChanged.AddListener(OnSliderChanged);

        isLoading = true;
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.wholeNumbers = false;
        volumeSlider.value = M_AudioManager.Instance.GetMasterVolume();
        isLoading = false;
    }

    void OnDisable()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    public void OnSliderChanged(float value)
    {
        if (isLoading) return;
        M_AudioManager.Instance?.SetMasterVolume(value);
    }
}
