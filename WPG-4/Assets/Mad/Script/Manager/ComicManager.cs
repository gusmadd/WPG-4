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
    public Button nextButton;

    [Header("Animation Settings")]
    public float sceneInDuration = 3f;
    public float sceneOutDuration = 1f;
    public int totalScenes = 4;

    [Header("Skip Settings")]
    public float skipShowDelay = 1f;
    public float skipHideBeforeInEnds = 1f;

    [Header("Animator State Name")]
    public string finalOutStateName = "Scene4Out";

    [Header("Next Scene")]
    public string nextSceneName = "WeekScene";

    bool skipRequested = false;
    bool nextRequested = false;
    bool skipWasUsedOrExpired = false;

    void Start()
    {
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
            skipButton.onClick.AddListener(OnSkipPressed);
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(OnNextPressed);
        }

        StartCoroutine(PlayComicScenes());
    }

    void OnDestroy()
    {
        if (skipButton != null)
            skipButton.onClick.RemoveListener(OnSkipPressed);

        if (nextButton != null)
            nextButton.onClick.RemoveListener(OnNextPressed);
    }

    void OnSkipPressed()
    {
        skipRequested = true;
        HideAllButtons();
    }

    void OnNextPressed()
    {
        nextRequested = true;

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
    }

    IEnumerator PlayComicScenes()
    {
        for (int i = 1; i <= totalScenes; i++)
        {
            yield return StartCoroutine(PlayScene(i));

            if (skipRequested)
            {
                yield return StartCoroutine(ForceSkipToLastOut());
                yield break;
            }
        }

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator PlayScene(int sceneIndex)
    {
        nextRequested = false;

        string inTrigger = $"Scene{sceneIndex}In";
        string outTrigger = $"Scene{sceneIndex}Out";

        ResetAllSceneTriggers();

        animator.SetTrigger(inTrigger);

        float timer = 0f;
        bool isFirstScene = sceneIndex == 1;
        float skipHideTime = sceneInDuration - skipHideBeforeInEnds;

        while (timer < sceneInDuration)
        {
            if (skipRequested)
                yield break;

            timer += Time.deltaTime;

            if (isFirstScene && !skipWasUsedOrExpired && skipButton != null)
            {
                bool shouldShowSkip =
                    timer >= skipShowDelay &&
                    timer < skipHideTime;

                if (skipButton.gameObject.activeSelf != shouldShowSkip)
                    skipButton.gameObject.SetActive(shouldShowSkip);
            }

            yield return null;
        }

        if (isFirstScene)
        {
            skipWasUsedOrExpired = true;

            if (skipButton != null)
                skipButton.gameObject.SetActive(false);
        }

        if (skipRequested)
            yield break;

        if (nextButton != null)
            nextButton.gameObject.SetActive(true);

        while (!nextRequested)
        {
            if (skipRequested)
                yield break;

            yield return null;
        }

        ResetAllSceneTriggers();
        animator.SetTrigger(outTrigger);

        yield return new WaitForSeconds(sceneOutDuration);
    }

    IEnumerator ForceSkipToLastOut()
    {
        HideAllButtons();
        ResetAllSceneTriggers();

        // paksa langsung lompat ke state Scene4Out
        animator.Play(finalOutStateName, 0, 0f);

        yield return new WaitForSeconds(sceneOutDuration);

        SceneManager.LoadScene(nextSceneName);
    }

    void HideAllButtons()
    {
        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
    }

    void ResetAllSceneTriggers()
    {
        if (animator == null) return;

        for (int i = 1; i <= totalScenes; i++)
        {
            animator.ResetTrigger($"Scene{i}In");
            animator.ResetTrigger($"Scene{i}Out");
        }
    }
}