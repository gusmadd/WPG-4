using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ComicManager : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("UI")]
    public Button skipButton;

    [Header("Animation Settings")]
    public float sceneDuration = 3f;              // total durasi tiap scene
    public float sceneTransitionDuration = 1f;    // durasi animasi Out
    public int totalScenes = 4;

    [Header("Skip Button Settings")]
    public float skipShowDelay = 1f;              // tombol skip muncul setelah 1 detik

    [Header("Next Scene")]
    public string nextSceneName = "WeekScene";

    private bool skipRequested = false;

    void Start()
    {
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
            skipButton.onClick.AddListener(OnSkipPressed);
        }

        StartCoroutine(PlayComicScenes());
    }

    void OnDestroy()
    {
        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(OnSkipPressed);
        }
    }

    void OnSkipPressed()
    {
        skipRequested = true;

        if (skipButton != null)
            skipButton.gameObject.SetActive(false);
    }

    IEnumerator PlayComicScenes()
    {
        for (int i = 1; i <= totalScenes; i++)
        {
            yield return StartCoroutine(PlayScene(i));
        }

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator PlayScene(int sceneIndex)
    {
        skipRequested = false;

        string inTrigger = $"Scene{sceneIndex}In";
        string outTrigger = $"Scene{sceneIndex}Out";

        // Biar trigger lama tidak nyangkut
        animator.ResetTrigger(inTrigger);
        animator.ResetTrigger(outTrigger);

        // Mainkan animasi masuk
        animator.SetTrigger(inTrigger);

        // Tunggu 1 detik dulu sebelum tombol skip muncul
        float timer = 0f;
        while (timer < skipShowDelay && !skipRequested)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Tampilkan tombol skip kalau scene belum di-skip
        if (!skipRequested && skipButton != null)
        {
            skipButton.gameObject.SetActive(true);
        }

        // Tunggu sisa durasi scene, kecuali kalau user pencet skip
        while (timer < sceneDuration && !skipRequested)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Sembunyikan tombol skip sebelum keluar
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
        }

        // Langsung keluar ke animasi Out
        animator.ResetTrigger(inTrigger);
        animator.ResetTrigger(outTrigger);
        animator.SetTrigger(outTrigger);

        // Tunggu animasi Out selesai
        yield return new WaitForSeconds(sceneTransitionDuration);
    }
}