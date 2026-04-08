using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Transition Object")]
    public GameObject transitionObject;

    [Header("Animator")]
    public Animator transitionAnimator;

    [Header("Trigger Names")]
    public string closeTrigger = "Close";
    public string openTrigger = "Open";

    [Header("Timing")]
    public float closeDuration = 3f;       // anim Close kamu
    public float openDelayAfterLoad = 0.05f;
    public float openFinishDelay = 1.5f;   // anim Open kamu

    private bool isTransitioning = false;

    void Awake()
    {
        if (transform.parent != null)
        {
            Debug.LogError("SceneTransitionManager must be placed on a ROOT GameObject.");
            return;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (transitionObject != null)
            transitionObject.SetActive(false);
    }

    public void LoadSceneWithTransition(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        isTransitioning = true;

        if (transitionObject != null)
            transitionObject.SetActive(true);

        if (transitionAnimator != null)
        {
            transitionAnimator.ResetTrigger(openTrigger);
            transitionAnimator.ResetTrigger(closeTrigger);
            transitionAnimator.Play("Idle", 0, 0f); // optional, biar selalu mulai dari awal
            transitionAnimator.SetTrigger(closeTrigger);
        }

        yield return new WaitForSecondsRealtime(closeDuration);

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return new WaitForSecondsRealtime(openDelayAfterLoad);

        if (transitionAnimator != null)
        {
            transitionAnimator.ResetTrigger(closeTrigger);
            transitionAnimator.ResetTrigger(openTrigger);
            transitionAnimator.SetTrigger(openTrigger);
        }

        yield return new WaitForSecondsRealtime(openFinishDelay);

        if (transitionObject != null)
            transitionObject.SetActive(false);

        isTransitioning = false;
    }
}
