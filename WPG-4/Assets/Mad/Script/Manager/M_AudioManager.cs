using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_AudioManager : MonoBehaviour
{
    public static M_AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Cursor")]
    public AudioClip[] cursorClicks; // isi 2 variasi

    [Header("Keyboard")]
    public AudioClip[] keyboardClicks;   // isi variasi
    public AudioClip spacebarClick;

    [Header("UI / Payment")]
    public AudioClip paymentSfx;
    public AudioClip showKeyboardSfx;
    public AudioClip hideKeyboardSfx;

    [Header("Ads")]
    public AudioClip[] adsSfx; // isi 2 variasi

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // CURSOR CLICK (Random dari 2 SFX)
    public void PlayCursorClick()
    {
        if (sfxSource == null) return;
        if (cursorClicks == null || cursorClicks.Length == 0) return;

        int i = Random.Range(0, cursorClicks.Length);
        sfxSource.PlayOneShot(cursorClicks[i]);

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.AddNoise(M_NoiseSystem.Instance.clickNoise);
    }

    // KEYBOARD RANDOM CLICK
    public void PlayKeyboardClick()
    {
        if (sfxSource == null) return;
        if (keyboardClicks == null || keyboardClicks.Length == 0) return;

        int randomIndex = Random.Range(0, keyboardClicks.Length);

        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(keyboardClicks[randomIndex]);
        sfxSource.pitch = 1f;

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.AddNoise(M_NoiseSystem.Instance.keyboardNoise);
    }

    // SPACEBAR CLICK
    public void PlaySpacebar()
    {
        if (sfxSource == null) return;
        if (spacebarClick == null) return;

        sfxSource.PlayOneShot(spacebarClick);

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.AddNoise(M_NoiseSystem.Instance.spaceNoise);
    }

    // PAYMENT
    public void PlayPayment()
    {
        if (sfxSource == null) return;
        if (paymentSfx == null) return;

        sfxSource.PlayOneShot(paymentSfx);

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.AddNoise(M_NoiseSystem.Instance.paymentNoise);
    }

    // ADS (Random dari 2 SFX)
    public void PlayAdsSfx()
    {
        if (sfxSource == null) return;
        if (adsSfx == null || adsSfx.Length == 0) return;

        int i = Random.Range(0, adsSfx.Length);
        sfxSource.PlayOneShot(adsSfx[i]);

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.AddNoise(M_NoiseSystem.Instance.clickNoise);
    }

    // SHOW KEYBOARD
    public void PlayShowKeyboard()
    {
        if (sfxSource == null) return;
        if (showKeyboardSfx == null) return;

        sfxSource.PlayOneShot(showKeyboardSfx);
    }

    // HIDE KEYBOARD
    public void PlayHideKeyboard()
    {
        if (sfxSource == null) return;
        if (hideKeyboardSfx == null) return;

        sfxSource.PlayOneShot(hideKeyboardSfx);
    }
}
