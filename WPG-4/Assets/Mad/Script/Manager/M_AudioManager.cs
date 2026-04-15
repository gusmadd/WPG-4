using UnityEngine;

public class M_AudioManager : MonoBehaviour
{
    public static M_AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource ambienceSource;
    public AudioSource backgroundMusicSource;
    public AudioSource holdBuySource;
    public AudioSource ownerStageSource;

    [Header("Volume Multipliers")]
    [Range(0f, 1f)] public float uiVolume = 1f;
    [Range(0f, 1f)] public float keyboardVolume = 1f;
    [Range(0f, 1f)] public float adsVolume = 1f;
    [Range(0f, 1f)] public float bigSisVolume = 1f;
    [Range(0f, 1f)] public float pcVolume = 1f;
    [Range(0f, 1f)] public float ambienceVolume = 1f;
    [Range(0f, 1f)] public float miscVolume = 1f;

    [Header("Cursor / General UI Click")]
    public AudioClip[] cursorClicks;

    [Header("Keyboard")]
    public AudioClip[] keyboardClicks;
    public AudioClip spacebarClick;
    public bool randomizeKeyboardPitch = true;
    public float keyboardPitchMin = 0.95f;
    public float keyboardPitchMax = 1.05f;

    [Header("UI Open / Close")]
    public AudioClip showKeyboardSfx;
    public AudioClip hideKeyboardSfx;

    [Header("Purchase / Wrong")]
    public AudioClip paymentSfx;
    public AudioClip wrongSfx;
    public AudioClip daySuccessSfx;

    [Header("Ads")]
    public AudioClip[] adsSfx;

    [Header("Big Sis - Presence")]
    public AudioClip bigSisNgintipSfx;
    public AudioClip bigSisStarringSfx;

    [Header("PC")]
    public AudioClip pcPowerOnSfx;
    public AudioClip pcButtonSfx;

    [Header("Bubble")]
    public AudioClip bubbleAppearSfx;

    [Header("Calendar")]
    public AudioClip calendarInSfx;
    public AudioClip calendarOutSfx;

    [Header("Footstep")]
    public AudioClip footstepSfx;

    [Header("Extra UI")]
    public AudioClip[] uiSfx;

    [Header("Hold Buy")]
    public AudioClip holdBuySfx;

    [Header("Misc Gameplay")]
    public AudioClip[] miscSfx;

    [Header("BGM")]
    public AudioClip mainMenuBgm;
    public AudioClip gameplayBgm;
    public bool playMainMenuOnStart = false;
    public bool playGameplayOnStart = false;

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
        if (playMainMenuOnStart)
            PlayMainMenuMusic();
        else if (playGameplayOnStart)
            PlayGameplayMusic();
    }

    bool HasSfxSource() => sfxSource != null;
    bool IsValid(AudioClip clip) => clip != null;
    bool IsValidArray(AudioClip[] clips) => clips != null && clips.Length > 0;

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

    public void PlayCursorClick()
    {
        if (!IsValidArray(cursorClicks)) return;

        int i = Random.Range(0, cursorClicks.Length);
        PlayClip(cursorClicks[i], uiVolume);

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.clickNoise);
    }

    public void PlayRandomUi() => PlayRandom(uiSfx, uiVolume);

    public void PlayUiByIndex(int index)
    {
        if (!IsValidArray(uiSfx)) return;
        if (index < 0 || index >= uiSfx.Length) return;
        PlayClip(uiSfx[index], uiVolume);
    }

    public void PlayWrongSfx() => PlayClip(wrongSfx, uiVolume);
    public void PlayDaySuccesSfx() => PlayClip(daySuccessSfx, uiVolume);

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

    public void PlayShowKeyboard() => PlayClip(showKeyboardSfx, uiVolume);
    public void PlayHideKeyboard() => PlayClip(hideKeyboardSfx, uiVolume);

    public void PlayHoldBuyLoop()
    {
        if (holdBuySource == null || !IsValid(holdBuySfx)) return;
        if (holdBuySource.isPlaying && holdBuySource.clip == holdBuySfx) return;

        holdBuySource.clip = holdBuySfx;
        holdBuySource.loop = true;
        holdBuySource.volume = uiVolume;
        holdBuySource.Play();
    }

    public void StopHoldBuyLoop()
    {
        if (holdBuySource == null) return;
        if (holdBuySource.isPlaying) holdBuySource.Stop();
        holdBuySource.clip = null;
        holdBuySource.loop = false;
    }

    // khusus stage / Big Sis loop
    public void PlayOwnerStageLoop(AudioClip clip)
    {
        if (ownerStageSource == null || !IsValid(clip)) return;
        if (ownerStageSource.isPlaying && ownerStageSource.clip == clip) return;

        ownerStageSource.Stop();
        ownerStageSource.clip = clip;
        ownerStageSource.loop = false;
        ownerStageSource.volume = bigSisVolume;
        ownerStageSource.Play();
    }

    public void StopOwnerStageLoop()
    {
        if (ownerStageSource == null) return;
        if (ownerStageSource.isPlaying) ownerStageSource.Stop();
        ownerStageSource.clip = null;
        ownerStageSource.loop = false;
    }

    public void PlayPayment()
    {
        PlayClip(paymentSfx, uiVolume);

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.paymentNoise);
    }

    public void PlayAdsSfx()
    {
        PlayRandom(adsSfx, adsVolume);

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.clickNoise);
    }

    public void PlayBigSisNgintipSfx() => PlayClip(bigSisNgintipSfx, bigSisVolume);
    public void PlayBigSisStarringSfx() => PlayClip(bigSisStarringSfx, bigSisVolume);
    public void PlayFootstepSfx() => PlayClip(footstepSfx, bigSisVolume);

    public void PlayPcPowerOn() => PlayClip(pcPowerOnSfx, pcVolume);

    public void PlayPcButton()
    {
        PlayClip(pcButtonSfx, pcVolume);

        if (M_NoiseSystem.Instance != null)
            AddNoise(M_NoiseSystem.Instance.clickNoise);
    }

    public void PlayBubbleAppear() => PlayClip(bubbleAppearSfx, uiVolume);
    public void PlayInCalendar() => PlayClip(calendarInSfx, uiVolume);
    public void PlayOutCalendar() => PlayClip(calendarOutSfx, uiVolume);
    public void PlayRandomMisc() => PlayRandom(miscSfx, miscVolume);

    public void PlayMiscByIndex(int index)
    {
        if (!IsValidArray(miscSfx)) return;
        if (index < 0 || index >= miscSfx.Length) return;
        PlayClip(miscSfx[index], miscVolume);
    }

    public void PlayMainMenuMusic() => PlayBgm(mainMenuBgm);
    public void PlayGameplayMusic() => PlayBgm(gameplayBgm);

    void PlayBgm(AudioClip clip)
    {
        if (backgroundMusicSource == null || !IsValid(clip)) return;
        if (backgroundMusicSource.clip == clip && backgroundMusicSource.isPlaying) return;

        backgroundMusicSource.Stop();
        backgroundMusicSource.clip = clip;
        backgroundMusicSource.loop = true;
        backgroundMusicSource.volume = ambienceVolume;
        backgroundMusicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource == null) return;
        backgroundMusicSource.Stop();
    }

    public void PauseBackgroundMusic()
    {
        if (backgroundMusicSource == null || !backgroundMusicSource.isPlaying) return;
        backgroundMusicSource.Pause();
    }

    public void ResumeBackgroundMusic()
    {
        if (backgroundMusicSource == null) return;
        backgroundMusicSource.UnPause();
    }
}