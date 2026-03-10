using System.Collections;
using UnityEngine;

public class MonitorManager : MonoBehaviour
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
    public M_NotFoundController notFoundController;

    [Header("Timing")]
    public float delayBeforeIdle = 0.3f;
    public float loadingDuration = 2f;
    public float delayAfterOut = 0.1f;

    [Header("Colliders")]
    public Collider2D powerCollider;

    bool booting = false;

    void Start()
    {
        Debug.Log("MonitorManager START");

        CheckReferences();

        SetMonitorOff();
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        SpinCircle();
        HandlePowerClick();
        FollowGameState();
    }

    void SpinCircle()
    {
        if (loadingCircle != null && loadingCircle.activeSelf)
        {
            loadingCircle.transform.Rotate(0, 0, -circleSpinSpeed * Time.deltaTime);
        }
    }

    void HandlePowerClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (Camera.main == null)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (currentState == MonitorState.Off &&
            powerCollider != null &&
            powerCollider.OverlapPoint(mousePos))
        {
            Debug.Log("Power button clicked");
            GameManager.Instance.PowerOn();
        }
    }

    void FollowGameState()
    {
        GameState state = GameManager.Instance.CurrentState;

        switch (state)
        {
            case GameState.FAILED:
                if (currentState != MonitorState.Off)
                    SetMonitorOff();
                break;

            case GameState.BOOTING:
                if (!booting && currentState == MonitorState.Off)
                    PowerOn();
                break;

            case GameState.DESKTOP:
                if (currentState == MonitorState.On)
                    ShowDesktop();
                break;

            case GameState.BROWSER:
                if (currentState == MonitorState.On)
                    ShowBrowser();
                break;
        }
    }

    void SetMonitorOff()
    {
        Debug.Log("Monitor OFF");

        currentState = MonitorState.Off;
        booting = false;

        if (screenOff) screenOff.SetActive(true);
        if (screenOn) screenOn.SetActive(false);

        if (loadingCircle) loadingCircle.SetActive(false);

        if (searchPage) searchPage.SetActive(false);
        if (petshopPage) petshopPage.SetActive(false);
    }

    public void PowerOn()
    {
        if (currentState != MonitorState.Off)
            return;

        Debug.Log("Monitor Power ON");
        StartCoroutine(PowerOnSequence());
    }

    IEnumerator PowerOnSequence()
    {
        booting = true;

        currentState = MonitorState.LoadingIn;
        Debug.Log("State → LoadingIn");

        if (screenOff) screenOff.SetActive(false);
        if (screenOn) screenOn.SetActive(true);

        if (loadingAnimator)
            loadingAnimator.SetTrigger("isIn");

        yield return new WaitForSeconds(delayBeforeIdle);

        currentState = MonitorState.LoadingIdle;
        Debug.Log("State → LoadingIdle");

        if (loadingCircle)
            loadingCircle.SetActive(true);

        yield return new WaitForSeconds(loadingDuration);

        currentState = MonitorState.LoadingOut;
        Debug.Log("State → LoadingOut");

        if (loadingCircle)
            loadingCircle.SetActive(false);

        if (loadingAnimator)
            loadingAnimator.SetTrigger("isOut");

        yield return new WaitForSeconds(delayAfterOut);

        currentState = MonitorState.On;
        booting = false;

        Debug.Log("State → On");

        ShowDesktop();
    }

    void ShowDesktop()
    {
        if (searchPage)
            searchPage.SetActive(true);

        if (petshopPage)
            petshopPage.SetActive(false);
    }

    void ShowBrowser()
    {
        if (searchPage)
            searchPage.SetActive(true);
    }

    void CheckReferences()
    {
        if (loadingAnimator == null) Debug.LogError("loadingAnimator NOT assigned");
        if (loadingCircle == null) Debug.LogError("loadingCircle NOT assigned");

        if (screenOff == null) Debug.LogError("screenOff NOT assigned");
        if (screenOn == null) Debug.LogError("screenOn NOT assigned");

        if (searchPage == null) Debug.LogError("searchPage NOT assigned");
        if (petshopPage == null) Debug.LogError("petshopPage NOT assigned");

        if (notFoundController == null) Debug.LogWarning("notFoundController NOT assigned");

        if (powerCollider == null) Debug.LogError("powerCollider NOT assigned");
    }

    public void HandleSearch(string url)
    {
        string cleanUrl = url.ToLower().Trim();

        Debug.Log("Search URL: " + cleanUrl);

        if (cleanUrl == "www.miawshopp.com")
        {
            OpenPetshop();
        }
        else
        {
            Debug.Log("Website not found");

            if (searchPage)
                searchPage.SetActive(false);

            if (notFoundController)
                notFoundController.Show();
        }
    }

    void OpenPetshop()
    {
        Debug.Log("Opening Petshop Page");

        if (searchPage)
            searchPage.SetActive(false);

        if (petshopPage)
            petshopPage.SetActive(true);
    }
}