using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_MonitorManager : MonoBehaviour
{
    public enum MonitorState
    {
        Off,
        LoadingIn,
        LoadingIdle,
        LoadingOut,
        On
    }

    [Header("State")]
    public MonitorState currentState = MonitorState.Off;

    [Header("References")]
    public Animator loadingAnimator;

    [Header("Circle Spin")]
    public float circleSpinSpeed = 180f;
    public GameObject loadingCircle;

    public GameObject screenOff;
    public GameObject screenOn;

    [Header("Pages")]
    public GameObject searchPage;
    public GameObject petshopPage;
    public M_NotFoundController notFoundController; // 🔥 pakai controller

    [Header("Timing")]
    public float delayBeforeIdle = 0.3f;
    public float loadingDuration = 2f;
    public float delayAfterOut = 0.1f;

    [Header("Colliders")]
    public Collider2D powerCollider;

    void Start()
    {
        currentState = MonitorState.Off;

        screenOff.SetActive(true);
        screenOn.SetActive(false);
        loadingCircle.SetActive(false);

        searchPage.SetActive(false);
        petshopPage.SetActive(false);
    }

    void Update()
    {
        if (loadingCircle.activeSelf)
        {
            loadingCircle.transform.Rotate(0, 0, -circleSpinSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
                return;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (currentState == MonitorState.Off &&
                powerCollider.OverlapPoint(mousePos))
            {
                PowerOn();
            }
        }
    }

    public void PowerOn()
    {
        if (currentState != MonitorState.Off) return;
        StartCoroutine(PowerOnSequence());
    }

    IEnumerator PowerOnSequence()
    {
        currentState = MonitorState.LoadingIn;
        loadingAnimator.SetTrigger("isIn");

        yield return new WaitUntil(() =>
            loadingAnimator.GetCurrentAnimatorStateInfo(0).IsName("in") &&
            loadingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        yield return new WaitForSeconds(0.1f);

        currentState = MonitorState.LoadingIdle;
        loadingCircle.SetActive(true);

        yield return new WaitForSeconds(loadingDuration);

        currentState = MonitorState.LoadingOut;
        loadingCircle.SetActive(false);
        loadingAnimator.SetTrigger("isOut");

        yield return new WaitForSeconds(delayAfterOut);

        currentState = MonitorState.On;

        screenOff.SetActive(false);
        screenOn.SetActive(true);

        searchPage.SetActive(true); // 🔥 masuk ke search setelah nyala
    }

    public void HandleSearch(string url)
    {
        string cleanUrl = url.ToLower().Trim();

        if (cleanUrl == "www.miawshopp.com")
        {
            OpenPetshop();
        }
        else
        {
            searchPage.SetActive(false);
            notFoundController.Show();
        }
    }

    void OpenPetshop()
    {
        searchPage.SetActive(false);
        petshopPage.SetActive(true);
    }
}
