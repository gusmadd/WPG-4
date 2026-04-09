using UnityEngine;

public class M_AudioManager : MonoBehaviour
{
    public static M_AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource ambienceSource;

    [Header("Volume Multipliers")]
    [Range(0f, 1f)] public float uiVolume = 1f;
    [Range(0f, 1f)] public float keyboardVolume = 1f;
    [Range(0f, 1f)] public float adsVolume = 1f;
    [Range(0f, 1f)] public float bigSisVolume = 1f;
    [Range(0f, 1f)] public float pcVolume = 1f;
    [Range(0f, 1f)] public float ambienceVolume = 1f;
    [Range(0f, 1f)] public float miscVolume = 1f;

    [Header("Cursor / General UI Click")]
    [Tooltip("Contoh isi: UI 1, UI 2, mouse click")]
    public AudioClip[] cursorClicks;

    [Header("Keyboard")]
    [Tooltip("Contoh isi: keyboard baru, keyboard baru 2, keyboard baru 3, keyboard baru 4")]
    public AudioClip[] keyboardClicks;
    [Tooltip("Isi dengan suara keyboard yang paling cocok buat spasi")]
    public AudioClip spacebarClick;
    public bool randomizeKeyboardPitch = true;
    public float keyboardPitchMin = 0.95f;
    public float keyboardPitchMax = 1.05f;

    [Header("UI Open / Close")]
    [Tooltip("Contoh isi: UI 3")]
    public AudioClip showKeyboardSfx;
    [Tooltip("Contoh isi: UI 2")]
    public AudioClip hideKeyboardSfx;

    [Header("Purchase / Wrong")]
    public AudioClip paymentSfx;
    [Tooltip("Contoh isi: emilianodleon-button-ui-sound-effect")]
    public AudioClip wrongSfx;

    [Header("Ads")]
    [Tooltip("UI popup / iklan muncul")]
    public AudioClip[] adsSfx;

    [Header("Big Sis - Presence")]
    [Tooltip("bigsis ngintip, bigsis starring")]
    public AudioClip[] bigSisSfx;

    [Header("PC")]
    [Tooltip("Contoh isi: pc nyala")]
    public AudioClip pcPowerOnSfx;
    [Tooltip("Contoh isi: tombol pc")]
    public AudioClip pcButtonSfx;

    [Header("Bubble")]
    [Tooltip("Contoh isi: bubble muncul")]
    public AudioClip bubbleAppearSfx;

    [Header("Calendar")]
    [Tooltip("Contoh isi: suara kalender, suara kalender 2")]
    public AudioClip[] calendarSfx;

    [Header("Footstep")]
    [Tooltip("Contoh isi: footstep bigsis")]
    public AudioClip[] footstepSfx;

    [Header("Extra UI")]
    [Tooltip("Contoh isi: UI 1, UI 2, UI 3, UI 4")]
    public AudioClip[] uiSfx;

    [Header("Misc Gameplay")]
    [Tooltip("Contoh isi: file SFX lain yang tidak cocok ke kategori atas")]
    public AudioClip[] miscSfx;

    [Header("Ambience")]
    [Tooltip("Contoh isi: in game ambience")]
    public AudioClip inGameAmbience;
    public bool playAmbienceOnStart = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (playAmbienceOnStart)
            PlayInGameAmbience();
    }

    // =========================
    // CORE HELPERS
    // =========================
    bool HasSfxSource()
    {
        return sfxSource != null;
    }

    bool IsValid(AudioClip clip)
    {
        return clip != null;
    }

    bool IsValidArray(AudioClip[] clips)
    {
        return clips != null && clips.Length > 0;
    }

    void PlayClip(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (!HasSfxSource()) return;
        if (!IsValid(clip)) return;

        sfxSource.PlayOneShot(clip, volumeMultiplier);
    }

    void PlayRandom(AudioClip[] clips, float volumeMultiplier = 1f)
    {
        if (!HasSfxSource()) return;
        if (!IsValidArray(clips)) return;

        int i = Random.Range(0, clips.Length);
        if (clips[i] == null) return;

        sfxSource.PlayOneShot(clips[i], volumeMultiplier);
    }

    void AddNoise(float amount)
    {
        if (M_NoiseSystem.Instance == null) return;
        M_NoiseSystem.Instance.AddNoise(amount);
    }

    // =========================
    // CURSOR / GENERAL UI
    // =========================
    public void PlayCursorClick()
    {
        if (!IsValidArray(cursorClicks)) return;

        int i = Random.Range(0, cursorClicks.Length);
        PlayClip(cursorClicks[i], uiVolume);

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.clickNoise);
    }

    public void PlayRandomUi()
    {
        PlayRandom(uiSfx, uiVolume);
    }

    public void PlayUiByIndex(int index)
    {
        if (!IsValidArray(uiSfx)) return;
        if (index < 0 || index >= uiSfx.Length) return;

        PlayClip(uiSfx[index], uiVolume);
    }

    public void PlayWrongSfx()
    {
        PlayClip(wrongSfx, uiVolume);
    }

    // =========================
    // KEYBOARD
    // =========================
    public void PlayKeyboardClick()
    {
        if (!HasSfxSource()) return;
        if (!IsValidArray(keyboardClicks)) return;

        int randomIndex = Random.Range(0, keyboardClicks.Length);

        float originalPitch = sfxSource.pitch;

        if (randomizeKeyboardPitch)
            sfxSource.pitch = Random.Range(keyboardPitchMin, keyboardPitchMax);

        sfxSource.PlayOneShot(keyboardClicks[randomIndex], keyboardVolume);
        sfxSource.pitch = originalPitch;

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.keyboardNoise);
    }

    public void PlaySpacebar()
    {
        PlayClip(spacebarClick, keyboardVolume);

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.spaceNoise);
    }

    public void PlayShowKeyboard()
    {
        PlayClip(showKeyboardSfx, uiVolume);
    }

    public void PlayHideKeyboard()
    {
        PlayClip(hideKeyboardSfx, uiVolume);
    }

    // =========================
    // PURCHASE / SHOP
    // =========================
    public void PlayPayment()
    {
        PlayClip(paymentSfx, uiVolume);

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.paymentNoise);
    }

    // =========================
    // ADS
    // =========================
    public void PlayAdsSfx()
    {
        PlayRandom(adsSfx, adsVolume);

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.clickNoise);
    }

    // =========================
    // BIG SIS
    // =========================
    public void PlayBigSisSfx()
    {
        PlayRandom(bigSisSfx, bigSisVolume);
    }

    public void PlayBigSisByIndex(int index)
    {
        if (!IsValidArray(bigSisSfx)) return;
        if (index < 0 || index >= bigSisSfx.Length) return;

        PlayClip(bigSisSfx[index], bigSisVolume);
    }

    public void PlayRandomFootstep()
    {
        PlayRandom(footstepSfx, miscVolume);
    }

    // =========================
    // PC
    // =========================
    public void PlayPcPowerOn()
    {
        PlayClip(pcPowerOnSfx, pcVolume);
    }

    public void PlayPcButton()
    {
        PlayClip(pcButtonSfx, pcVolume);

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.clickNoise);
    }

    // =========================
    // BUBBLE / CALENDAR
    // =========================
    public void PlayBubbleAppear()
    {
        PlayClip(bubbleAppearSfx, uiVolume);
    }

    public void PlayRandomCalendar()
    {
        PlayRandom(calendarSfx, uiVolume);
    }

    public void PlayCalendarByIndex(int index)
    {
        if (!IsValidArray(calendarSfx)) return;
        if (index < 0 || index >= calendarSfx.Length) return;

        PlayClip(calendarSfx[index], uiVolume);
    }

    // =========================
    // EXTRA / MISC
    // =========================
    public void PlayRandomMisc()
    {
        PlayRandom(miscSfx, miscVolume);
    }

    public void PlayMiscByIndex(int index)
    {
        if (!IsValidArray(miscSfx)) return;
        if (index < 0 || index >= miscSfx.Length) return;

        PlayClip(miscSfx[index], miscVolume);
    }

    // =========================
    // AMBIENCE
    // =========================
    public void PlayInGameAmbience()
    {
        if (ambienceSource == null) return;
        if (inGameAmbience == null) return;

        if (ambienceSource.clip == inGameAmbience && ambienceSource.isPlaying)
            return;

        ambienceSource.clip = inGameAmbience;
        ambienceSource.loop = true;
        ambienceSource.volume = ambienceVolume;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        if (ambienceSource == null) return;
        ambienceSource.Stop();
    }

    public void PauseAmbience()
    {
        if (ambienceSource == null) return;
        if (!ambienceSource.isPlaying) return;

        ambienceSource.Pause();
    }

    public void ResumeAmbience()
    {
        if (ambienceSource == null) return;
        ambienceSource.UnPause();
    }

    public void SetAmbienceVolume(float value)
    {
        ambienceVolume = Mathf.Clamp01(value);

        if (ambienceSource != null)
            ambienceSource.volume = ambienceVolume;
    }

    // =========================
    // DEBUG / TEST
    // =========================
    [ContextMenu("Test Cursor Click")]
    public void TestCursorClick()
    {
        PlayCursorClick();
    }

    [ContextMenu("Test Keyboard Click")]
    public void TestKeyboardClick()
    {
        PlayKeyboardClick();
    }

    [ContextMenu("Test PC Power On")]
    public void TestPcPowerOn()
    {
        PlayPcPowerOn();
    }

    [ContextMenu("Test Ads Sfx")]
    public void TestAdsSfx()
    {
        PlayAdsSfx();
    }

    [ContextMenu("Test Big Sis Sfx")]
    public void TestBigSisSfx()
    {
        PlayBigSisSfx();
    }

    [ContextMenu("Test Bubble")]
    public void TestBubble()
    {
        PlayBubbleAppear();
    }

    [ContextMenu("Test Ambience")]
    public void TestAmbience()
    {
        PlayInGameAmbience();
    }
}