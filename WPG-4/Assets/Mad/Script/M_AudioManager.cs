using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_AudioManager : MonoBehaviour
{
    public static M_AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Cursor")]
    public AudioClip cursorClick;

    [Header("Keyboard")]
    public AudioClip[] keyboardClicks;   // isi 5 variasi
    public AudioClip spacebarClick;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // CURSOR CLICK
    public void PlayCursorClick()
    {
        sfxSource.PlayOneShot(cursorClick);
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.AddNoise(M_NoiseSystem.Instance.clickNoise); // Tambah noise saat klik
    }

    // KEYBOARD RANDOM CLICK
    public void PlayKeyboardClick()
    {
        if (keyboardClicks.Length == 0) return;

        int randomIndex = Random.Range(0, keyboardClicks.Length);

        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(keyboardClicks[randomIndex]);
        sfxSource.pitch = 1f;

        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.AddNoise(M_NoiseSystem.Instance.keyboardNoise); // Tambah noise saat ketik    
    }

    // SPACEBAR CLICK
    public void PlaySpacebar()
    {
        sfxSource.PlayOneShot(spacebarClick);
        if (M_NoiseSystem.Instance != null)
            M_NoiseSystem.Instance.AddNoise(M_NoiseSystem.Instance.spaceNoise); // Tambah noise saat spacebar
    }
}
